using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace QuickCommander
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Deactivated += (sender, e) => CloseCommandLine();
        }

        public void ShowCommandLine()
        {
            Storyboard sb = FindResource("ShowAnimation") as Storyboard;
            sb.Completed += (sender, e) => Keyboard.Focus(Command);
            sb.Begin();
        }

        public void CloseCommandLine()
        {
            Storyboard sb = FindResource("CloseAnimation") as Storyboard;
            sb.Completed += (sender, e) => Command.Text = "";
            sb.Begin();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) CloseCommandLine();
            if (e.Key != Key.Enter) return;

            var line = Command.Text.Split(new char[]{' '}, 2);
            var cmd = line[0];
            var args = (line.Length < 2) ? new string[0] : line[1].Split(' ');

            switch (cmd)
            {
                case "close":
                    CloseCommandLine();
                    break;
            }
        }
    }
}