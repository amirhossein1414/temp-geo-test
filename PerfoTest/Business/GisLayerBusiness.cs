using GeoJSON.Net;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using PerfoTest.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using LayersApi.Models.DBModels;
using System.Linq;
using LayersApi.Business;

namespace PerfoTest.Business
{
    public static class GisLayerBusiness
    {
        public static HttpClient client = new HttpClient();
        //private readonly static string PostgresConnectionString = "Server=192.168.30.96;Port=5432;Database=Layers;Uid=postgres;Pwd=123;";
        private readonly static string PostgresConnectionString = "Server=127.0.0.1;Port=5432;Database=LayersDB;Uid=postgres;Pwd=123;";
        private static NpgsqlConnection sqlConn { get; set; } = new NpgsqlConnection(PostgresConnectionString);

        private static DataInsertionBusiness dataMaker = new DataInsertionBusiness();
        private static DataBaseGeometryCreator dataBaseGeometryCreator = new DataBaseGeometryCreator();
        static GisLayerBusiness()
        {
            sqlConn.Open();
        }

        public static void InsertBulk(GeoJSONObjectType featureType)
        {
            NpgsqlTransaction transaction = null;
            Console.WriteLine("started");

            try
            {
                for (var i = 1; i <= 100; i++)
                {
                    var items = Enumerable.Range(0, 10000).ToList().Select(x =>
                    {
                        var newItem = CreateNewItem(featureType);
                        return newItem;
                    }).ToList();

                    var geoJson = CreateGeoJsonFromFeaturesString(items);
                    var newLayer = new LayerDto()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Title",
                        GeoJson = geoJson
                    };

                    transaction = sqlConn.BeginTransaction();
                    Console.WriteLine($"layer {i}");
                    InsertLayerDto(newLayer, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
            }

            Console.WriteLine("all done...");
        }

        private static void InsertLayerDto(LayerDto layerDto, NpgsqlTransaction transaction)
        {
            var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(layerDto.GeoJson);
            var features = FeatureBusiness.CreateFeatures(featureCollection, layerDto.Id);

            var newLayer = new Layer()
            {
                Id = layerDto.Id,
                Title = layerDto.Title,
                Features = features
            };

            InsertLayer(newLayer, transaction);
            InsertFeatures(newLayer, transaction);
        }

        public static void InsertLayer(Layer layer, NpgsqlTransaction transaction)
        {
            //var copyHelper = new PostgreSQLCopyHelper<Layer>("public", "\"Layer\"")
            //                     .Map("\"Id\"", x => x.Id, NpgsqlDbType.Text)
            //                     .Map("\"Title\"", x => x.Title, NpgsqlDbType.Text);
            //copyHelper.SaveAll(sqlConn, layers);

            using (var cmd = new NpgsqlCommand("INSERT INTO public.\"Layer\" (\"Id\",\"Title\")" +
                                   $"VALUES (@Id,@Title)", sqlConn, transaction))
            {
                cmd.Parameters.AddWithValue("Id", layer.Id);
                cmd.Parameters.AddWithValue("Title", layer.Title);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertFeatures(Layer layer, NpgsqlTransaction transaction)
        {
            //var features = layers.SelectMany(x => x.Features).ToList();
            //var copyHelper = new PostgreSQLCopyHelper<DbFeature>("public", "\"Feature\"")
            //                     .Map("\"Id\"", x => x.Id, NpgsqlDbType.Text)
            //                     .Map("\"GeojsonArea\"", x => x.GeojsonArea, NpgsqlDbType.Text)
            //                     .Map("\"GeometryType\"", x => x.GeometryType, NpgsqlDbType.Smallint)
            //                     .Map("\"LayerId\"", x => x.Layer.Id, NpgsqlDbType.Text)
            //                     .Map("\"Area\"", x => x.Area, NpgsqlDbType.Point);
            //copyHelper.SaveAll(sqlConn, features);
            layer.Features.ForEach(feature =>
            {
                var geojsonFeature = JsonConvert.DeserializeObject<GeoJSON.Net.Feature.Feature>(feature.GeojsonArea);
                var area = dataBaseGeometryCreator.GetGeometryString(geojsonFeature);
                using (var cmd = new NpgsqlCommand("INSERT INTO public.\"Feature\" (\"Id\",\"GeojsonArea\",\"Area\",\"GeometryType\",\"LayerId\")" +
                                    $"VALUES (@Id,@GeojsonArea,{area},@GeometryType,@LayerId)", sqlConn, transaction))
                {
                    cmd.Parameters.AddWithValue("Id", feature?.Id);
                    cmd.Parameters.AddWithValue("GeojsonArea", feature?.GeojsonArea);
                    //cmd.Parameters.AddWithValue("Area", area);
                    cmd.Parameters.AddWithValue("GeometryType", feature.GeometryType);
                    cmd.Parameters.AddWithValue("LayerId", feature.Layer.Id);
                    cmd.ExecuteNonQuery();
                }
            });
        }

        private static string CreateNewItem(GeoJSONObjectType featureType)
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

            var superMarketJsonString = superMarket.ToString();
            return superMarketJsonString;
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
            var featuresCount = 5;
            var featureType = GeoJSONObjectType.Point;
            var geoJson = GetLayerGeoJson(featuresCount, featureType);
            var layer = new LayerDto()
            {
                Title = "Layer Title",
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //var result = client.PostAsJsonAsync("https://localhost:44330/layers/AddLayer", layer);
            var result = client.PostAsJsonAsync("https://localhost:5001/layers/AddLayer", layer);

            result.ContinueWith((response) =>
            {
                stopwatch.Stop();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"ellapsed time for inserting {featuresCount} features : " + totalEllapsedTime + "ms");
                var res = response.Result;
                res.EnsureSuccessStatusCode();
            });
        }

        public static string GetLayerGeoJson(int count, GeoJSONObjectType featureType)
        {
            var jsonFeatures = new List<string>();
            for (var j = 0; j < count; j++)
            {
                var newItem = CreateNewItem(featureType);
                jsonFeatures.Add(newItem);
            }

            var featureCollection = CreateGeoJsonFromFeaturesString(jsonFeatures);
            return featureCollection;
        }

        public static string CreateGeoJsonFromFeaturesString(List<string> jsonFeatures)
        {
            var features = string.Join(",", jsonFeatures);
            var featureCollection = "{" +
                    "\"type\": \"FeatureCollection\"," +
                    $"\"features\": [{features}]" +
                    "}";

            return featureCollection;
        }

        public static void ReadLayerBenchmark()
        {
            var request = new GetLayersRequest()
            {
                LayerIds = new List<string>() { /*"006be665-7422-48eb-be2c-cf647c53136a"*/ },
                boundingBox = new BoundingBox()
                {
                    Coordinates = GetTehranCoordinates()
                }
            };

            var boundingBoxString = QueryMaker.GetPolygonQueryString(request.boundingBox);
            var queryString = QueryMaker.GetSearchWithinPolygonQueryString(request, boundingBoxString, "200");
            var jsonFeatures = new List<string>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (NpgsqlCommand command = new NpgsqlCommand(queryString, sqlConn))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    jsonFeatures.Add(reader[0].ToString());
                }
            }

            var featureCollection = CreateFeatureCollection(jsonFeatures);
            stopwatch.Stop();
            var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"ellapsed time : " + totalEllapsedTime + $"ms, {totalEllapsedTime / 1000}sec");
        }

        public static void NetworkBenchmark()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = client.GetAsync("https://localhost:5001/layers/index");

            result.ContinueWith((response) =>
            {
                stopwatch.Stop();
                var res = response.Result;
                res.EnsureSuccessStatusCode();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"ellapsed time : " + totalEllapsedTime + $"ms, {totalEllapsedTime / 1000}sec");
            });
        }

        private static string CreateFeatureCollection(List<string> featuresString)
        {
            var features = string.Join(",", featuresString);
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
                LayerIds = new List<string>() { /*"006be665-7422-48eb-be2c-cf647c53136a"*/ },
                boundingBox = new BoundingBox()
                {
                    Coordinates = GetTehranCoordinates()
                }
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //var result = client.PostAsJsonAsync("https://localhost:44330/layers/GetLayer", layer);
            var result = client.PostAsJsonAsync("https://localhost:5001/layers/GetLayer", layer);

            result.ContinueWith((response) =>
            {
                stopwatch.Stop();

                var featureCollectionString = response.Result.Content.ReadAsStringAsync().Result;
                var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(featureCollectionString);
                var res = response.Result;
                res.EnsureSuccessStatusCode();

                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"ellapsed time : " + totalEllapsedTime + $"ms, {totalEllapsedTime/1000}sec");
            });
        }

        private static List<GeoPoint> GetTehranCoordinates()
        {
            return new List<GeoPoint>()
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
                    };
        }
    }
}
