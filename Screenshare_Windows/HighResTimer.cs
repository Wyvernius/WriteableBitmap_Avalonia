using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Timers
{
    public class HighResTimer
    {
        Stopwatch stopwatch;


        public double interval;
        double nextInterval;
        double average;
        long callcounter = 0;

        public Action Callback;

        public bool Running => stopwatch == null ? false : stopwatch.IsRunning;

        public HighResTimer(long interval, Action callback)
        {
            this.interval = interval;
            nextInterval = interval;
            Callback = callback;

            Setup();
            
        }

        public HighResTimer(float interval, Action callback)
        {
            this.interval = interval;
            nextInterval = interval;
            Callback = callback;

            Setup();

        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                long elapsed = 0;
                stopwatch.Start();
                DateTime t = DateTime.Now;
                long ms = stopwatch.ElapsedMilliseconds;
                DateTime d;
                while (stopwatch.IsRunning)
                {
                    Thread.Sleep(1);
                    if (((d = DateTime.Now) - t).TotalMilliseconds >= nextInterval)
                    {
                        t = d;
                        long functionStart = stopwatch.ElapsedMilliseconds;

                        if (stopwatch.IsRunning)
                            Callback?.Invoke();

                        long functionTime = stopwatch.ElapsedMilliseconds - functionStart;

                        //nextInterval = interval - functionTime;
                    }
                }
            });
        }

        public void Stop()
        {
            Callback = null;
            stopwatch.Stop();
        }

        private void Setup()
        {
            stopwatch = new Stopwatch();
        }
    }
}
