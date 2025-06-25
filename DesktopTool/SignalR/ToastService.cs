using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DesktopTool.SignalR
{
    public static class ToastService
    {
        public static void ShowToast(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new Border
                {
                    Background = Brushes.DimGray,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10),
                    Child = new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.White,
                        FontSize = 14
                    }
                };

                var popup = new Window
                {
                    Width = 300,
                    Height = 50,
                    Topmost = true,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Background = Brushes.Transparent,
                    Content = toast,
                    ShowInTaskbar = false,
                    Left = SystemParameters.WorkArea.Width - 320,
                    Top = SystemParameters.WorkArea.Height - 80
                };

                popup.Show();

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                timer.Tick += (s, e) =>
                {
                    popup.Close();
                    timer.Stop();
                };
                timer.Start();
            });
        }
    }
}
