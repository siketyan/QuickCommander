using QuickCommander.API;
using System.Windows;

namespace QuickCommander.SamplePlugin
{
    public class SamplePlugin : IPlugin
    {
        public string Name { get; } = "SamplePlugin";
        public string Author { get; } = "Siketyan";
        public string Version { get; } = "1.0";
        public string Description { get; } = "QuickCommanderのサンプルプラグインです。";

        public void OnEnable()
        {
            MessageBox.Show("SamplePlugin: OnEnable");
            CommandManager.RegistCommand(
                "sample",
                args => IOManager.Out(
                    this, (string)ConfigManager.Get("SamplePlugin.Message", "Default")
                )
            );
        }

        public void OnDisable()
        {
            MessageBox.Show("SamplePlugin: OnDisable");
        }
    }
}