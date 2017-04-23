using System;

namespace QuickCommander.API
{
    public class IOManager
    {
        public event EventHandler<OutputEventArgs> Output;

        private static IOManager instance;

        public IOManager()
        {
            instance = this;
        }

        public static void Out(object sender, string message, int timeout = 2000)
        {
            if (sender == null) return;

            instance.Output(
                sender,
                new OutputEventArgs
                {
                    Message = message,
                    Timeout = timeout
                }
            );
        }
    }

    public class OutputEventArgs
    {
        public string Message { get; set; }
        public int Timeout { get; set; }
    }
}