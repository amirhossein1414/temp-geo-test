using Microsoft.SqlServer.Types;
using PerfoTest.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PerfoTest.Business
{
    public class RequestMaker
    {
        private readonly object timesLock = new object();
        private Random random = new Random();
        public void CreateFakeRequests()
        {
            var times = new List<DateTime>();
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                var newTask = Task.Run(async () =>
                {
                    var time = await FakeRequest();
                    lock (timesLock)
                    {
                        times.Add(time);
                    }
                });

                tasks.Add(newTask);
            }

            Console.WriteLine("Created 100 long Running Tasks");
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("All 100 long Running Tasks Finished");
            Console.ReadLine();
        }

        public void CreateFakeSqlRequest()
        {
            var count = 10;
            var totalTime = new Stopwatch();

            var ellapsedTimes = new List<long>();

            using (SqlConnection sqlConn = new SqlConnection(GeoContext.ConnectionString))
            {
                sqlConn.Open();

                var tasks = new List<Task>();
                totalTime.Start();
                for (int i = 0; i < count; i++)
                {
                    var newTask = Task.Run(() =>
                   {
                       var stopWatch = new Stopwatch();
                       stopWatch.Start();
                       GetPointsInsidePolygon(sqlConn);
                       stopWatch.Stop();
                       lock (timesLock)
                       {
                           ellapsedTimes.Add(stopWatch.ElapsedMilliseconds);
                       }
                   });

                    tasks.Add(newTask);
                }

                Console.WriteLine($"Created {count} long Running Tasks");
                Task.WaitAll(tasks.ToArray());
                totalTime.Stop();
                Console.WriteLine($"All {count} long Running Tasks Finished");

                var totalEllapsedTime = totalTime.ElapsedMilliseconds; ;
                Console.WriteLine("Ellapsed {0} miliseconds or {1} seconds.", totalEllapsedTime, (float)totalEllapsedTime / 1000);

                foreach (var time in ellapsedTimes)
                {
                    Console.WriteLine($"one item Ellapsed : {time}");
                }

                Console.ReadLine();
            }
        }

        private void GetPointsInsidePolygon(SqlConnection sqlConn)
        {
            //var newId = random.Next(1000000, 9000000);
            //var sqlQuery = "select * from supermarkets" +
            //    " WITH(INDEX(\"PK_dbo.SuperMarkets\"))" +
            //    $" where id = {newId}";

            var sqlDeclaration = "DECLARE @tehranZone geometry;" +
                "SET @tehranZone = geometry::STPolyFromText('POLYGON ((51.35833740234375 35.73703779932528," +
                "51.359710693359375 35.67626300279665," +
                "51.40571594238281 35.646137228802424," +
                "51.44554138183594 35.68351380631503," +
                "51.44279479980469 35.73982452242507," +
                "51.402969360351555 35.762114795721," +
                "51.35833740234375 35.73703779932528))', 0); ";

            var sqlSelect = "select top 1 * from SuperMarkets s " +
                            "WITH(INDEX(\"SpatialIndex-20200405-013341\"))" +
                            "where @tehranZone.STIntersects(s.GeoData) = 1 ";

            var sqlQuery = sqlDeclaration + sqlSelect;

            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConn);
            using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var superMarker = new SuperMarket();
                    superMarker.Id = int.Parse(dataReader["Id"].ToString());
                    superMarker.Content = dataReader["Content"]?.ToString();

                    var myGeoData = new SqlGeometry();
                    myGeoData.Read(new BinaryReader(dataReader.GetSqlBytes(2).Stream));
                    var data = myGeoData?.ToString();
                }
            }
        }

        private async Task<DateTime> FakeRequest()
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return DateTime.Now;
        }
    }
}
