using PerfoTest.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Types;
using PerfoTest.Business;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PerfoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var helper = new Helper();
            using (SqlConnection sqlConn = new SqlConnection(GeoContext.ConnectionString))
            {
                sqlConn.Open();
                var sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < 1; i++)
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
