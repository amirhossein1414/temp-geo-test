﻿using PerfoTest.Model;
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
using GeoJSON.Net;

namespace PerfoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var reqMaker = new RequestMaker();
            //reqMaker.CreateFakeSqlRequest();

            //PostgresGeoBusiness.ReadLayer();

            //GisApiRequestMaker.SendParallel();
            //GisApiRequestMaker.SendPostRequest();

            //PostgresGeoBusiness.InsertBulk();
            //GisApiRequestMaker.InsertLayer();
            //**GisLayerBusiness.InsertBulk(GeoJSONObjectType.Point);


            //GisLayerBusiness.AddLayerRequest();
            //GisLayerBusiness.ReadLayerRequest();
            //**GisLayerBusiness.ReadLayerBenchmark();
            //*GisLayerBusiness.NetworkBenchmark();

            Console.ReadLine();
        }
    }
}
