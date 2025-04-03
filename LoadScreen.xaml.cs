using System.Windows;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MindScribe
{
    public partial class LoadScreen : Window
    {
        public LoadScreen()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Animate progress bar
            var animation = new DoubleAnimation
            {
                To = 300,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            progressFill.BeginAnimation(Border.WidthProperty, animation);

            await Task.Delay(2500);

            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}