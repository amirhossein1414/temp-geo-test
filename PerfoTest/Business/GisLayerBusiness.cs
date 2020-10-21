using Newtonsoft.Json.Linq;
using Npgsql;
using PerfoTest.Model;
using System;
using System.Collections.Generic;
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
        public static void InsertBulk()
        {

            NpgsqlTransaction transaction = sqlConn.BeginTransaction();
            Console.WriteLine("started");
            for (var i = 1; i <= 10000; i++)
            {

                var items = new List<LayerItem>();
                for (var j = 0; j < 1000; j++)
                {
                    var newItem = CreateNewItem();
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

        private static LayerItem CreateNewItem()
        {
            var superMarket = dataMaker.GetNewSuperMarket();
            var randomPoint = dataMaker.RandomPointCoordinates();
            superMarket["geometry"]["coordinates"] = JArray.FromObject(randomPoint);
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

        public static void AddLayerRequest()
        {
            var geoJson = GetLayerGeoJson();
            var layer = new Layer()
            {
                Title = "Layer Title",
                LayerId = "123",
                GeoJson = geoJson
            };

            var result = client.PostAsJsonAsync("https://localhost:44330/layers/AddLayer", layer);

            result.ContinueWith((response) =>
            {
                var res = response.Result;
            });
        }

        private static string GetLayerGeoJson()
        {
            var items = new List<LayerItem>();
            for (var j = 0; j < 3; j++)
            {
                var newItem = CreateNewItem();
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
    }
}
