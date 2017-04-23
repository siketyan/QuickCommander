using System;

namespace QuickCommander.API
{
    public class IOManager
    {
        public event EventHandler<string> Output;

        private static IOManager instance;

        public IOManager()
        {
            instance = this;
        }

        public static void Out(object sender, string message)
        {
            if (sender == null) return;

            instance.Output(sender, message);
        }
    }
}