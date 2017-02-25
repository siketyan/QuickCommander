using Kennedy.ManagedHooks;
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
        private KeyboardHook globalHook;
        private List<Plugin> plugins;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            plugins = PluginManager.FindPlugins<List<Plugin>>();
            foreach (Plugin p in plugins)
            {
                MessageBox.Show(
                    "Name: " + p.Name
                        + "\nDescription: "+ p.Description
                        + "\nAuthor: " + p.Author
                        + "\nVersion: " + p.Version
                );
            }

            var window = new MainWindow();
            var screen = Forms.Screen.AllScreens[0];
            var rect = screen.Bounds;

            window.Top = rect.Top - 2;
            window.Left = rect.Left + SIDE_MARGIN;
            window.Width = rect.Width - (SIDE_MARGIN * 2);

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