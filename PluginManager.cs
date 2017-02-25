using QuickCommander.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QuickCommander
{
    public static class PluginManager
    {
        public static T FindPlugins<T>() where T : IList<Plugin>, new()
        {
            var plugins = new T();
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                               + @"\plugins";

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
                return plugins;
            }

            foreach (string dll in Directory.GetFiles(location, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.IsClass && t.IsPublic && !t.IsAbstract &&
                            t.GetInterface(typeof(IPlugin).FullName) != null)
                        {
                            plugins.Add(new Plugin(dll, t.FullName));
                        }
                    }
                }
                catch {}
            }

            return plugins;
        }

        public static void DisablePlugins(IList<Plugin> plugins)
        {
            foreach (Plugin p in plugins)
            {
                p.Disable();
            }
        }
    }

    public class Plugin
    {
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }
        public string Location { get; private set; }
        public string ClassName { get; private set; }

        private IPlugin instance;

        public Plugin(string path, string className)
        {
            Location = path;
            ClassName = className;

            try
            {
                Assembly asm = Assembly.LoadFrom(Location);
                instance = (IPlugin)asm.CreateInstance(ClassName);
            }
            catch
            {
                throw new Exception("プラグイン " + Location + " が正しく読み込まれませんでした。");
            }

            Name = instance.Name;
            Author = instance.Author;
            Version = instance.Version;

            instance.OnEnable();
        }

        public void Disable()
        {
            instance.OnDisable();
        }
    }
}