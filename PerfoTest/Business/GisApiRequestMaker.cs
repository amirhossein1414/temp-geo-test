using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace PerfoTest.Business
{
    public static class GisApiRequestMaker
    {
        public static HttpClient client = new HttpClient();
        public static List<long> EllapsedTimes = new List<long>();
        public static List<HttpResponseMessage> Responses = new List<HttpResponseMessage>();
        public static Task<HttpResponseMessage> SendGetRequest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = client.GetAsync("http://localhost:6715/api/layer");
            result.ContinueWith((x) =>
            {
                stopwatch.Stop();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                EllapsedTimes.Add(totalEllapsedTime);
                Responses.Add(x.Result);
            });

            return result;
        }
        public static Task<HttpResponseMessage> SendPostRequest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var lostions = new List<MapPointLocation>() { new MapPointLocation() { lng = 1, lat = 2 } };
            var result = client.PostAsJsonAsync("http://localhost:6715/api/GetSuperMarkets", lostions);

            result.ContinueWith((x) =>
            {
                stopwatch.Stop();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                EllapsedTimes.Add(totalEllapsedTime);
                Responses.Add(x.Result);
            });

            return result;
        }

        public async static void SendParallel()
        {
            var count = 30;
            var totalTime = new Stopwatch();
            var tasks = new List<Task>();
            totalTime.Start();

            for (int i = 0; i < count; i++)
            {
                var newTask = SendGetRequest();

                //Console.WriteLine($"Request {i} Sent.");
                tasks.Add(newTask);
            }

            Console.WriteLine($"{count} asynchronous requests sent.");
            Task.WaitAll(tasks.ToArray());
            totalTime.Stop();

            //Console.WriteLine($"All {count} long Running Tasks Finished");

            int j = 1;
            foreach (var time in EllapsedTimes)
            {
                Console.WriteLine($"request {j} took {time} miliSeconds");
                j++;
            }

            //var totalEllapsedTime = totalTime.ElapsedMilliseconds; ;
            //Console.WriteLine("Ellapsed {0} miliseconds or {1} seconds.", totalEllapsedTime, (float)totalEllapsedTime / 1000);



            //foreach (var response in Responses)
            //{
            //    Console.WriteLine($"****Reponse*****");
            //    Console.WriteLine(response);
            //    j++;
            //    Console.WriteLine();
            //}
        }


    }
}
