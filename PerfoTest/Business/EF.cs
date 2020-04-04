using PerfoTest.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Business
{
    public class EF
    {

        public void CreateDataBase()
        {
            using (var ctx = new GeoContext())
            {
                var list = new List<SuperMarket>();
                for (int i = 1; i < 10; i++)
                {
                    //var lat = float.Parse($"0.{i}");
                    //var lng = float.Parse($"0.{i}");
                    //var location = DbGeography.PointFromText($"Point({lng} {lat})", 4326);
                    //var super = new SuperMarket() { Content = "" + i, GeoData = location };
                    var super = new SuperMarket();
                    list.Add(super);
                }

                ctx.SuperMarkets.AddRange(list);
                ctx.SaveChanges();
            }
        }
    }
}
