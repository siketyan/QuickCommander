using Kennedy.ManagedHooks;
using QuickCommander.API;
using System.Collections.Generic;
using System.Windows;
using Forms = System.Windows.Forms;

namespace QuickCommander
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private const int SIDE_MARGIN = 256;

        public List<Plugin> plugins;
        private KeyboardHook globalHook;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var screen = Forms.Screen.AllScreens[1];
            var rect = screen.Bounds;
            var window = new MainWindow(rect.Top)
            {
                Top = rect.Top - 50,
                Left = rect.Left + SIDE_MARGIN,
                Width = rect.Width - (SIDE_MARGIN * 2)
            };

            IOManager.Output += window.OnOutput;
            ConfigManager.ConfigSetted += window.OnConfigSet;

            plugins = PluginManager.FindPlugins<List<Plugin>>();

            globalHook = new KeyboardHook();
            globalHook.KeyboardEvent += window.OnGlobalKeyDown;
            globalHook.InstallHook();

            window.Show();
            window.ShowCommandLine();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            globalHook.UninstallHook();
            PluginManager.DisablePlugins(plugins);
        }
    }
}