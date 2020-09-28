using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Business
{
    public static class GisApiRequestMaker
    {
        public static HttpClient client = new HttpClient();
        public static List<long> EllapsedTimes = new List<long>();
        public static Task<HttpResponseMessage> SendRequest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = client.GetAsync("http://localhost:82/api/layer");
            result.ContinueWith((x) =>
            {
                stopwatch.Stop();
                var totalEllapsedTime = stopwatch.ElapsedMilliseconds;
                EllapsedTimes.Add(totalEllapsedTime);
            });

            return result;
        }

        public static void SendParallel()
        {
            var count = 30;
            var totalTime = new Stopwatch();
            var tasks = new List<Task>();
            totalTime.Start();

            for (int i = 0; i < count; i++)
            {
                var newTask = SendRequest();

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

            Console.ReadLine();
        }


    }
}
