using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Model
{
    public class SuperMarket
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DbGeometry GeoData { get; set; }
    }
}
