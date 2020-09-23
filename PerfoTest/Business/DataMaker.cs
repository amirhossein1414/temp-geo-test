using PerfoTest.Model;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace PerfoTest.Business
{
    public class DataMaker
    {
        public void MakeData()
        {
            var ef = new EF();
            //ef.CreateDataBase();

            var helper = new DataInsertionBusiness();
            using (SqlConnection sqlConn = new SqlConnection(GeoContext.ConnectionString))
            {
                sqlConn.Open();
                var sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < 10000; i++)
                {
                    helper.InsertBulk(sqlConn);
                    Console.WriteLine(i);
                }
                sw.Stop();
                File.WriteAllText("file.txt", "" + sw.ElapsedMilliseconds);
            }
        }
    }
}
