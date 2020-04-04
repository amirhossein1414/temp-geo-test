void junkCode()
        {
            var sw = new Stopwatch();
            sw.Start();
            //using (var ctx = new GeoContext())
            //{
            //    //var options = new BulkInsertOptions();
            //    //ctx.Configuration.AutoDetectChangesEnabled = false;
            //    //ctx.Configuration.ValidateOnSaveEnabled = false;
            //    var list = new List<SuperMarket>();
            //    for (int i = 1; i < 10; i++)
            //    {
            //        var lat = float.Parse($"0.{i}");
            //        var lng = float.Parse($"0.{i}");
            //        var location = DbGeography.PointFromText($"Point({lng} {lat})", 4326);
            //        var super = new SuperMarket() { Content = "" + i, GeoData = location };
            //        list.Add(super);
            //    }


            //    //ctx.BulkInsert(list);
            //    ctx.SuperMarkets.AddRange(list);
            //    ctx.SaveChanges();
            //    sw.Stop();
            //    Console.WriteLine("Elappsed Time for insertion: " + sw.ElapsedMilliseconds);

            //    sw.Start();
            //    var item = ctx.SuperMarkets.Where(x => x.Content == "5").FirstOrDefault();
            //    sw.Stop();

            //    Console.WriteLine("Elappsed Time for search: " + sw.ElapsedMilliseconds);
            //    Console.WriteLine("lng value is: " + item.GeoData.Longitude);

            //}
        }
		