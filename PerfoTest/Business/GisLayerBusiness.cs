using GeoJSON.Net;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using PerfoTest.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using WebApplication1.Model;

namespace PerfoTest.Business
{
    public static class GisLayerBusiness
    {
        public static HttpClient client = new HttpClient();
        private readonly static string PostgresConnectionString = "Server=192.168.30.96;Port=5432;Database=Layers;Uid=postgres;Pwd=123;";
        private static NpgsqlConnection sqlConn { get; set; } = new NpgsqlConnection(PostgresConnectionString);

        private static DataInsertionBusiness dataMaker = new DataInsertionBusiness();
        static GisLayerBusiness()
        {
            //sqlConn.Open();
        }
        public static void InsertBulk(GeoJSONObjectType featureType)
        {

            NpgsqlTransaction transaction = sqlConn.BeginTransaction();
            Console.WriteLine("started");
            for (var i = 1; i <= 10000; i++)
            {

                var items = new List<LayerItem>();
                for (var j = 0; j < 1000; j++)
                {
                    var newItem = CreateNewItem(featureType);
                    items.Add(newItem);
                }

                InsertAll(items, transaction);
                Console.WriteLine(i);

                if (i % 10 == 0)
                {
                    transaction.Commit();
                    Console.WriteLine("commited");
                    transaction = sqlConn.BeginTransaction();
                    Console.WriteLine("new tran");
                }
            }
            transaction?.Commit();

        }

        private static string InsertAll(List<LayerItem> layerItems, NpgsqlTransaction transaction)
        {
            try
            {
                layerItems?.ForEach(item =>
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO public.\"Layers\" (id,title,content,area) " +
                        $"VALUES (@id,@title,@content,ST_GeomFromText('Point({item?.Area?.Lng} {item?.Area?.Lat})'))", sqlConn, transaction))
                    {
                        cmd.Parameters.AddWithValue("id", item?.Id);
                        cmd.Parameters.AddWithValue("title", item?.Title);
                        cmd.Parameters.AddWithValue("content", item?.Content);
                        cmd.ExecuteNonQuery();
                    }
                });

                return "ok";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        private static LayerItem CreateNewItem(GeoJSONObjectType featureType)
        {
            var objectType = featureType;
            var superMarket = dataMaker.GetNewSuperMarket();
            var randomPoint = dataMaker.RandomPointCoordinates();

            switch (objectType)
            {
                case GeoJSONObjectType.Point:
                    superMarket["geometry"]["type"] = "Point";
                    superMarket["geometry"]["coordinates"] = JArray.FromObject(randomPoint);
                    break;
                case GeoJSONObjectType.MultiPoint:
                    superMarket["geometry"]["type"] = "MultiPoint";
                    superMarket["geometry"]["coordinates"] = JToken.FromObject(GetMultiPoint());
                    break;
                case GeoJSONObjectType.LineString:
                    superMarket["geometry"]["type"] = "LineString";
                    superMarket["geometry"]["coordinates"] = JToken.FromObject(GetLineString());
                    break;
                case GeoJSONObjectType.MultiLineString:
                    superMarket["geometry"]["type"] = "MultiLineString";
                    superMarket["geometry"]["coordinates"] = JToken.FromObject(GetMultiLineString());
                    break;
                case GeoJSONObjectType.Polygon:
                    superMarket["geometry"]["type"] = "Polygon";
                    superMarket["geometry"]["coordinates"] = JToken.FromObject(GetPolygon());
                    break;
                case GeoJSONObjectType.MultiPolygon:
                    superMarket["geometry"]["type"] = "MultiPolygon";
                    superMarket["geometry"]["coordinates"] = JToken.FromObject(GetMultiPolygon());
                    break;
            }

            var location = new GeoLocation() { Lat = randomPoint[0].ToString(), Lng = randomPoint[1].ToString() };
            var superMarketJsonString = superMarket.ToString();

            return new LayerItem()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Title" + Guid.NewGuid().ToString(),
                Area = location,
                Content = superMarketJsonString
            };
        }

        private static List<double[]> GetLineString()
        {
            var lineString = new List<double[]>();
            for (var i = 0; i < 5; i++)
            {
                var randomPoint = dataMaker.RandomPointCoordinates();
                lineString.Add(randomPoint);
            }
            return lineString;
        }

        private static List<List<double[]>> GetMultiLineString()
        {
            var lineStrings = new List<List<double[]>>();
            for (var i = 0; i < 5; i++)
            {
                var lineString = GetLineString();
                lineStrings.Add(lineString);
            }
            return lineStrings;
        }

        private static List<double[]> GetMultiPoint()
        {
            var points = new List<double[]>();
            for (var i = 0; i < 5; i++)
            {
                var randomPoint = dataMaker.RandomPointCoordinates();
                points.Add(randomPoint);
            }
            return points;
        }

        private static List<List<double[]>> GetPolygon()
        {
            var polygon = new List<List<double[]>>();
            var lineString = new List<double[]>();
            var startingPoint = dataMaker.RandomPointCoordinates();

            lineString.Add(startingPoint);
            for (var i = 0; i < 5; i++)
            {
                var randomPoint = dataMaker.RandomPointCoordinates();
                lineString.Add(randomPoint);
            }

            lineString.Add(startingPoint);
            polygon.Add(lineString);
            return polygon;
        }

        private static List<List<List<double[]>>> GetMultiPolygon()
        {
            var multiPolygon = new List<List<List<double[]>>>();
            for (var i = 0; i < 5; i++)
            {
                var polygon = GetPolygon();
                multiPolygon.Add(polygon);
            }

            return multiPolygon;
        }

        public static void AddLayerRequest()
        {
            var featuresCount = 100000;
            var featureType = GeoJSONObjectType.MultiPolygon;
            var geoJson = GetLayerGeoJson(featuresCount, featureType);
            var layer = new Layer()
            {
                Title = "Layer Title",
                PortalLayerId = Guid.NewGuid().ToString(),
                GeoJson = geoJson
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = client.PostAsJsonAsync("https://localhost:44330/layers/AddLayer", layer);

            result.ContinueWith((response) =>
            {
                stopwatch.Stop();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"ellapsed time for inserting {featuresCount} features : " + totalEllapsedTime + "ms");
                var res = response.Result;
            });
        }

        private static string GetLayerGeoJson(int count, GeoJSONObjectType featureType)
        {
            var items = new List<LayerItem>();
            for (var j = 0; j < count; j++)
            {
                var newItem = CreateNewItem(featureType);
                items.Add(newItem);
            }
            var featureList = items.Select(x => x.Content).ToList();

            var features = string.Join(",", featureList);
            var featureCollection = "{" +
                    "\"type\": \"FeatureCollection\"," +
                    $"\"features\": [{features}]" +
                    "}";

            return featureCollection;
        }



        public static void ReadLayerRequest()
        {
            var layer = new GetLayersRequest()
            {
                LayerIds = new List<string>() { "123", "456" },
                boundingBox = new BoundingBox()
                {
                    Coordinates = new List<GeoPoint>()
                    {
                        new GeoPoint()
                        {
                            Lng="51.3361930847168",
                            Lat="35.705959231097545"
                        },
                        new GeoPoint()
                        {
                            Lng="51.30821228027344",
                            Lat="35.622698214535184"
                        },
                        new GeoPoint()
                        {
                            Lng="51.44073486328125",
                            Lat="35.62381451392674"
                        },
                        new GeoPoint()
                        {
                            Lng="51.433353424072266",
                            Lat="35.70777131894265"
                        },
                        new GeoPoint()
                        {
                            Lng="51.3361930847168",
                            Lat="35.705959231097545"
                        }
                    }
                }
            };

            var result = client.PostAsJsonAsync("https://localhost:44330/layers/GetLayer", layer);

            result.ContinueWith((response) =>
            {
                var featureCollectionString = response.Result.Content.ReadAsStringAsync().Result;
                var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(featureCollectionString);
            });
        }
    }
}
