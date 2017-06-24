using System;

namespace QuickCommander.API
{
    public static class IOManager
    {
        public static event EventHandler<OutputEventArgs> Output;
        
        public static void Out(object sender, string message, int timeout = 2000)
        {
            if (sender == null) return;

            Output?.Invoke(
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