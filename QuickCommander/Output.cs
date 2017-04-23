using System;
using System.Threading;
using System.Windows;

namespace QuickCommander
{
    public class Output
    {
        public event EventHandler OutputTimeout;

        public string Message { get; set; }

        private Timer timer;

        public Output(string message, int timeout)
        {
            Message = message;

            if (timeout > 0)
            {
                timer = new Timer(new TimerCallback(OnTimerCallback));
                timer.Change(timeout, Timeout.Infinite);
            }
        }

        private void OnTimerCallback(object state)
        {
            timer.Dispose();
            OutputTimeout?.Invoke(this, null);
        }
    }
}