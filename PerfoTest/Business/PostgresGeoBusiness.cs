using Npgsql;
using NpgsqlTypes;
using PerfoTest.Model;
using PostgreSQLCopyHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PerfoTest.Business
{
    public class PostgresGeoBusiness
    {
        public readonly static string PostgresConnectionString = "Server=127.0.0.1;Port=5432;Database=geo;Uid=postgres;Pwd=123;";
        public void TestConnection()
        {
            using (var sqlConn = new NpgsqlConnection(PostgresConnectionString))
            {
                sqlConn.Open();
            }
        }

        public void InsertBulk(NpgsqlConnection sqlConn)
        {
            var amir = NpgsqlDbType.Geography;
            DataTable dataTable = new DataTable();
            dataTable.Clear();
            //*dataTable.Columns.Add("content", typeof(string));
            //*dataTable.Columns.Add("geodata", typeof(dynamic));

            for (int i = 1; i <= 1000; i++)
            {
                //*var market = GetNewSuperMarket();
                //*var randomPoint = RandomPointCoordinates();
                //*market["geometry"]["coordinates"] = JArray.FromObject(randomPoint);

                //market["properties"]["name"] = "eslam-shahr";

                //*var location = SqlGeometry.Point(randomPoint[1], randomPoint[0], 0);
                //var location = SqlGeometry.Point(randomPoint[1], randomPoint[0], 4326);
                //var location = DbGeometry.PointFromText($"POINT({randomPoint[0]} {randomPoint[1]})", 0);

                DataRow row = dataTable.NewRow();
                //*var marketString = market.ToString();
                //*row["Content"] = marketString;
                //*row["GeoData"] = location;
                dataTable.Rows.Add(row);
            }

            //using (SqlBulkCopy sqlbc = new SqlBulkCopy(null/*sqlConn*/))
            //{
            //    sqlbc.DestinationTableName = "SuperMarkets";
            //    sqlbc.ColumnMappings.Add("Content", "Content");
            //    sqlbc.ColumnMappings.Add("GeoData", "GeoData");
            //    sqlbc.WriteToServer(dataTable);
            //}
        }
        public void InsertBulkToPsql()
        {
            var copyHelper = new PostgreSQLCopyHelper<PostgresItem>("public", "geotable")
                                 .Map("id", x => x.id, NpgsqlDbType.Text);

            using (var sqlConn = new NpgsqlConnection(PostgresConnectionString))
            {
                sqlConn.Open();
                var items = new List<PostgresItem>();
                Enumerable.Range(0, 10000000).ToList().ForEach(x =>
                {
                    items.Add(new PostgresItem()
                    {
                        id = Guid.NewGuid(),
                        title = Guid.NewGuid().ToString() +
                         Guid.NewGuid().ToString() +
                          Guid.NewGuid().ToString() +
                           Guid.NewGuid().ToString() +
                            Guid.NewGuid().ToString() +
                             Guid.NewGuid().ToString() +
                             Guid.NewGuid().ToString() +
                          Guid.NewGuid().ToString() +
                           Guid.NewGuid().ToString() +
                            Guid.NewGuid().ToString() +
                             Guid.NewGuid().ToString() +
                             Guid.NewGuid().ToString() +
                          Guid.NewGuid().ToString() +
                           Guid.NewGuid().ToString() +
                            Guid.NewGuid().ToString() +
                             Guid.NewGuid().ToString()
                    });
                });

                copyHelper.SaveAll(sqlConn, items);
            }
        }
    }
}
