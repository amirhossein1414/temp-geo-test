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
            var points = new List<PostgresStringPoint>()
            {
                new PostgresStringPoint(){FirstPart = "51.3361930847168" , SecondPart = "35.705959231097545"},
                new PostgresStringPoint(){FirstPart = "51.30821228027344" , SecondPart = "35.622698214535184"},
                new PostgresStringPoint(){FirstPart = "51.44073486328125" , SecondPart = "35.62381451392674"},
                new PostgresStringPoint(){FirstPart = "51.433353424072266" , SecondPart = "35.70777131894265"},
                new PostgresStringPoint(){FirstPart = "51.3361930847168" , SecondPart = "35.705959231097545"}
            };

            var polygonString = QueryMaker.GetPolygonQueryString(points);
            var queryString = QueryMaker.GetSearchWithinPolygonQueryString("geotable", "geodata", polygonString,"1");

            var sql = "SELECT id,content from geotable limit 10";
            using (var cmd = new NpgsqlCommand(queryString, sqlConn))
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
