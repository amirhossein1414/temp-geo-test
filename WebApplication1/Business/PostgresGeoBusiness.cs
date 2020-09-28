using Newtonsoft.Json;
using Npgsql;
using PerfoTest.Business.Postgres;
using PerfoTest.Model;
using System.Collections.Generic;

namespace PerfoTest.Business
{
    public static class PostgresGeoBusiness
    {
        private static NpgsqlConnection sqlConn;
        public readonly static string PostgresConnectionString = "Server=127.0.0.1;Port=5432;Database=geo;Uid=postgres;Pwd=123;";
        //public readonly static string PostgresConnectionString = "Server=192.168.30.96;Port=5432;Database=geo;Uid=postgres;Pwd=123;";

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
