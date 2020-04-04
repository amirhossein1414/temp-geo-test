using Microsoft.SqlServer.Types;
using Newtonsoft.Json.Linq;
using PerfoTest.Model;
using System;
using System.Data;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;

namespace PerfoTest.Business
{
    public class Helper
    {
        Random random = new Random();
        static string content;
        public JObject GetSampleJson()
        {
            if (content == null)
                content = System.IO.File.ReadAllText("json-data/sample.json");
            JObject json = JObject.Parse(content);
            return json;
        }

        public double[] RandomPointCoordinates()
        {
            var lat = random.Next(648720, 771495);
            var lng = random.Next(263377, 508957);

            string latString = "35." + lat;
            string lngString = "51." + lng;

            // eslam shahr location
            //string latString = "35." + "554685";
            //string lngString = "51." + "235813";

            var arr = new double[2];
            arr[0] = double.Parse(latString);
            arr[1] = double.Parse(lngString);

            return arr;
        }

        public JObject GetNewSuperMarket()
        {
            var obj = GetSampleJson();
            return obj;
        }

        public void InsertBulk(SqlConnection sqlConn)
        {
            DataTable dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("GeoData", typeof(SqlGeometry));

            for (int i = 1; i <= 1000; i++)
            {
                var market = GetNewSuperMarket();
                var randomPoint = RandomPointCoordinates();
                market["geometry"]["coordinates"] = JArray.FromObject(randomPoint);

                //market["properties"]["name"] = "eslam-shahr";

                var location = SqlGeometry.Point(randomPoint[1], randomPoint[0], 0);
                //var location = SqlGeometry.Point(randomPoint[1], randomPoint[0], 4326);
                //var location = DbGeometry.PointFromText($"POINT({randomPoint[0]} {randomPoint[1]})", 0);

                DataRow row = dataTable.NewRow();
                var marketString = market.ToString();
                row["Content"] = marketString;
                row["GeoData"] = location;
                dataTable.Rows.Add(row);
            }

            using (SqlBulkCopy sqlbc = new SqlBulkCopy(sqlConn))
            {
                sqlbc.DestinationTableName = "SuperMarkets";
                sqlbc.ColumnMappings.Add("Content", "Content");
                sqlbc.ColumnMappings.Add("GeoData", "GeoData");
                sqlbc.WriteToServer(dataTable);
            }
        }
    }
}
