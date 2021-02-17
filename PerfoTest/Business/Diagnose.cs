using System.Diagnostics;

namespace PerfoTest.Business
{
    public static class AppTimer
    {
        private static Stopwatch StopWatch { get; set; }
        public static void CreateAndStart()
        {
            StopWatch = new Stopwatch();
            StopWatch.Start();
        }
        public static float StopAndGetTime()
        {
            StopWatch.Stop();
            var ellapsed = (float)StopWatch.ElapsedMilliseconds / 1000;
            return ellapsed;
        }

    }
}
