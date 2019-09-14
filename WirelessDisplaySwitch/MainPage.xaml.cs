using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WirelessDisplaySwitch
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DisplayHandling _displayHandling;

        public MainPage()
        {
            this.InitializeComponent();

            _displayHandling = new DisplayHandling(OnDeviceSelected);

            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  if (_displayHandling.IsDeviceSelected())
                  {
                      tbSelectedDevice.Text = _displayHandling.SelectedDeviceID;

                      var task = StartProjection();
                  }
              });
        }

        private async void OnDeviceSelected(string deviceId)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                tbSelectedDevice.Text = deviceId;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _displayHandling.SelectDevice();
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;
            try
            {
                await StartProjection();

            }
            finally
            {
                // Because we need the context here.
                button2.IsEnabled = true;
            }

        }

        private async Task StartProjection()
        {
            int currentViewId = ApplicationView.GetForCurrentView().Id;

            int newViewId = 0;
       
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await _displayHandling.StartProjectingAsync(newViewId, currentViewId);

                Application.Current.Exit();
            });
        }
    }
}