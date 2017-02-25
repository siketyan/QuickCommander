using System.Windows;
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
            Deactivated += (sender, e) => PlayCloseAnimation();
        }

        public void PlayShowAnimation()
        {
            Storyboard sb = FindResource("ShowAnimation") as Storyboard;
            sb.Begin();
        }

        public void PlayCloseAnimation()
        {
            Storyboard sb = FindResource("CloseAnimation") as Storyboard;
            sb.Begin();
        }
    }
}