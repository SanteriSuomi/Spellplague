using System;
using System.Diagnostics;

namespace Spellplague.Utility
{
    #pragma warning disable CA1063, S3881 // Disable warnings about dispose patterns, 
    // as dispose is only used for using keyword syntactic sugar for easy dispose.
    /// <summary>
    /// TimerLogger class is a timer that logs time elapsed and a potential message after it's dispose is called.
    /// Ex. using (new TimerLogger()) { }
    /// </summary>
    public class TimerLogger : IDisposable
    {
        private readonly Stopwatch stopWatch;
        private readonly string logMessage;

        public TimerLogger()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        public TimerLogger(string logMessage)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            this.logMessage = logMessage;
        }

        public void Dispose()
        {
            stopWatch.Stop();
            if (logMessage == null)
            {
                UnityEngine.Debug.Log($"Time elapsed: {stopWatch.Elapsed.Ticks}.");
            }
            else
            {
                UnityEngine.Debug.Log($"{logMessage}, time elapsed: {stopWatch.Elapsed.Ticks}.");
            }
        }
    }
}