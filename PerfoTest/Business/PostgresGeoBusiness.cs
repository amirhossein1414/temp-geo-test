using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using PerfoTest.Business.Postgres;
using PerfoTest.Model;
using PostgreSQLCopyHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Linq;

namespace PerfoTest.Business
{
    public static class PostgresGeoBusiness
    {
        private static NpgsqlConnection sqlConn;
        public readonly static string PostgresConnectionString = "Server=127.0.0.1;Port=5432;Database=geo;Uid=postgres;Pwd=123;";
        //public readonly static string PostgresConnectionString = "Server=192.168.30.96;Port=5432;Database=geo;Uid=postgres;Pwd=123;";
        public static DataInsertionBusiness dataMaker = new DataInsertionBusiness();

        public static void InitConnection()
        {
            sqlConn = new NpgsqlConnection(PostgresConnectionString);
            sqlConn.Open();
        }

        public static void TestConnection()
        {
            using (var sqlConn = new NpgsqlConnection(PostgresConnectionString))
            {
                sqlConn.Open();
            }
        }

        public static void InsertBulk()
        {
            var copyHelper = new PostgreSQLCopyHelper<PostgresItem>("public", "geotable")
                                 .Map("id", x => x.id, NpgsqlDbType.Text)
                                 .Map("title", x => x.title, NpgsqlDbType.Text)
                                 .Map("geodata", x => x.geodata, NpgsqlDbType.Point)
                                 .Map("content", x => x.content, NpgsqlDbType.Text);

            using (var sqlConn = new NpgsqlConnection(PostgresConnectionString))
            {
                sqlConn.Open();

                for (var i = 0; i < 6000; i++)
                {
                    var items = new List<PostgresItem>();
                    for (var j = 0; j < 1000; j++)
                    {
                        var newItem = CreateNewItem();
                        items.Add(newItem);
                    }
                    copyHelper.SaveAll(sqlConn, items);
                    Console.WriteLine(i);
                }
            }
        }

        private static PostgresItem CreateNewItem()
        {
            var superMarket = dataMaker.GetNewSuperMarket();
            var randomPoint = dataMaker.RandomPointCoordinates();
            superMarket["geometry"]["coordinates"] = JArray.FromObject(randomPoint);
            //var location = SqlGeometry.Point(randomPoint[1], randomPoint[0], 0);

            string wkt = String.Format("POINT({0} {1})", randomPoint[1], randomPoint[0]);
            var location = DbGeography.PointFromText(wkt, 4326);
            var postGisLocation = new NpgsqlPoint(randomPoint[1], randomPoint[0]);

            var superMarketJsonString = superMarket.ToString();

            return new PostgresItem()
            {
                id = Guid.NewGuid().ToString(),
                title = "Title" + Guid.NewGuid().ToString(),
                geodata = postGisLocation,
                content = superMarketJsonString
            };
        }

        public static string ReadLayer()
        {
            var items = new List<PostgresItem>();
            var maker = new QueryMaker();
            var polygonString = maker.GetPolygonQueryString();
            //var queryString = maker.GetSearchWithinPolygonQueryString("", "", polygonString);

            var sql = "SELECT id,content from geotable limit 10";
            using (var cmd = new NpgsqlCommand(sql, sqlConn))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        items.Add(new PostgresItem() { content = rdr.GetString(1), id = rdr.GetString(0) });
                        //Console.WriteLine("{0} {1} ", rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }

            var jsonResult = JsonConvert.SerializeObject(items);
            return jsonResult;
        }

    }
}
