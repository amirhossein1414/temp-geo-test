using PerfoTest.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApplication1.WebApi
{
    public class LayerController : ApiController
    { 
        public string Get()
        {
            var result = PostgresGeoBusiness.ReadLayer();
            return result;
        }
    }
}