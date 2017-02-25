using Kennedy.ManagedHooks;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
    }
}