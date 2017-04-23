using System;
using System.Collections.Generic;

namespace QuickCommander.API
{
    public static class CommandManager
    {
        private static Dictionary<string, Action<string[]>> commands
                            = new Dictionary<string, Action<string[]>>();

        public static void RegistCommand(string cmd, Action<string[]> action)
        {
            if (commands.ContainsKey(cmd))
                throw new CommandAlreadyRegistedException();

            commands.Add(cmd, action);
        }

        public static void Execute(string cmd, string[] args)
        {
            if (!commands.ContainsKey(cmd)) return;

            commands[cmd](args);
        }
    }

    public class CommandAlreadyRegistedException : Exception
    {
        public CommandAlreadyRegistedException() : base("コマンドは既に登録されています。") {}
    }
}