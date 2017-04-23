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
        public string Name { get { return Instance.Name; } }
        public string Description { get { return Instance.Description; } }
        public string Author { get { return Instance.Author; } }
        public string Version { get { return Instance.Version; } }
        public string Location { get; private set; }
        public string ClassName { get; private set; }
        public IPlugin Instance { get; private set; }

        public Plugin(string path, string className)
        {
            Location = path;
            ClassName = className;

            try
            {
                Assembly asm = Assembly.LoadFrom(Location);
                Instance = (IPlugin)asm.CreateInstance(ClassName);
            }
            catch
            {
                throw new Exception("プラグイン " + Location + " が正しく読み込まれませんでした。");
            }

            Instance.OnEnable();
        }

        public void Disable()
        {
            Instance.OnDisable();
        }
    }
}