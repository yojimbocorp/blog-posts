using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace BusyUI
{
    public partial class MainPage : UserControl
    {
        private volatile bool _hasBusyIndicatorUpdatedLayout = false;
        public MainPage()
        {
            InitializeComponent();
            busyIndicator.LayoutUpdated += BusyIndicatorLayoutUpdated;
        }

        void BusyIndicatorLayoutUpdated(object sender, EventArgs e)
        {
            _hasBusyIndicatorUpdatedLayout = true;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            SetBusy("Exporting data...");

            ThreadPool.QueueUserWorkItem((state) =>
                                             {
                                                 // this is how we check if the busy indicator has 
                                                 // started to display to the user
                                                 while (!_hasBusyIndicatorUpdatedLayout)
                                                 {
                                                     Thread.Sleep(100);
                                                 }

                                                 // now we can perform our busy task
                                                 DoBusyWorkOnUiThread();
                                                 RemoveBusy();
                                             });
        }

        // NOTE: If you can avoid doing work on the UI thread, don't use Dispather.BeginInvoke
        // This situation only arises in special cases. I ran into it when trying to use a third-party grid with a large number of 
        // data rows and I wanted to use the built-in export to Excel function of the grid object. The grid and its methods can only
        // be accessed on the UI thread, which forced me to come up with a work-around to displaying the busy indicator
        private void DoBusyWorkOnUiThread()
        {
            Dispatcher.BeginInvoke(() => Thread.Sleep((3000)));
        }

        private void SetBusy(string message)
        {
            // reset the layout updated state
            _hasBusyIndicatorUpdatedLayout = false;
            // make sure we're running on the UI thread 
            Dispatcher.BeginInvoke(() =>
                                       {
                                           busyIndicator.IsBusy = true;
                                           busyIndicator.BusyContent = message;
                                       });
        }

        private void RemoveBusy()
        {
            Dispatcher.BeginInvoke(() => busyIndicator.IsBusy = false);
        }
    }
}
