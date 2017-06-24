using System;
using System.Collections.Generic;

namespace QuickCommander.API
{
    public static class ConfigManager
    {
        public static event EventHandler<ConfigSettedEventArgs> ConfigSetted;

        private static bool isLoaded;
        private static Dictionary<string, object> configs;

        public static void Load()
        {
            configs = new Dictionary<string, object>();
            isLoaded = true;
        }

        public static void Load(Dictionary<string, object> conf)
        {
            if (isLoaded)
            {
                throw new InvalidOperationException("Loading config from plugins is not allowed.");
            }

            configs = conf;
            isLoaded = true;
        }

        public static Dictionary<string, object> GetAll()
        {
            return configs;
        }

        public static object Get(string key, object defaultValue = null)
        {
            return configs.ContainsKey(key) ? configs[key] : defaultValue;
        }

        public static void Set(string key, object value)
        {
            if (configs.ContainsKey(key))
            {
                configs[key] = value;
            }
            else
            {
                configs.Add(key, value);
            }

            ConfigSetted?.Invoke(
                null,
                new ConfigSettedEventArgs
                {
                    Key = key,
                    Value = value
                }
            );
        }
    }

    public class ConfigSettedEventArgs
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}