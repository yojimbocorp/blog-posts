using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BusyIndicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer _timer = null;
        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.IsEnabled = false;

            _timer.Tick += new System.EventHandler(RunTask);
        }

        private void StartProcess(object sender, RoutedEventArgs e)
        {
            _busyIndicator.IsBusy = true;
            _timer.Interval = new TimeSpan(0,0,0,0,100);
            _timer.Start();
            // go back to the user thread, giving the busy indicator a chance to come up before the next timer tick event
        }

        void RunTask(object sender, System.EventArgs e)
        {
            _timer.IsEnabled = false;

            var T = new Task(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    System.Threading.Thread.Sleep(50);
                }
            });

            T.ContinueWith(antecedent => _busyIndicator.IsBusy = false,
                                     TaskScheduler.FromCurrentSynchronizationContext()
                );

            T.Start();
        }
    }
}
