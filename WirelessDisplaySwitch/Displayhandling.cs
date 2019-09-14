using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace WirelessDisplaySwitch
{
    internal class DisplayHandling
    {
        private const string SelectedDeviceKey = "selectedDevice";
        private readonly DevicePicker _picker = new DevicePicker();
        private DeviceInformation _selectedDeviceInformation;
        private readonly Action<string> _onDeviceSelected;

        public string SelectedDeviceID { get; set; }

        public DisplayHandling(Action<string> onDeviceSelected)
        {
            // Get the device selector for Miracast devices
            _picker.Filter.SupportedDeviceSelectors.Add(ProjectionManager.GetDeviceSelector());

            //Hook up device selected event
            _picker.DeviceSelected += Picker_DeviceSelected;

            //Hook up picker dismissed event
            _picker.DevicePickerDismissed += Picker_DevicePickerDismissed;

            _onDeviceSelected = onDeviceSelected;

            // Hook up the events that are received when projection is stopped
            //pvb.ProjectionStopping += Pvb_ProjectionStopping;

            // load a setting that is local to the device
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            SelectedDeviceID = localSettings.Values[SelectedDeviceKey] as string;
        }

        internal bool IsDeviceSelected()
        {
            return !string.IsNullOrEmpty(SelectedDeviceID);
        }

        public void SelectDevice()
        {

            var rect = new Rect(0, 0, 100, 200);
            _picker.Show(rect);
        }

        private void Picker_DeviceSelected(DevicePicker sender, DeviceSelectedEventArgs args)
        {
            // Getting the selected device
            _selectedDeviceInformation = args.SelectedDevice;

            // Save a setting locally on the device
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[SelectedDeviceKey] = _selectedDeviceInformation.Id;

            _onDeviceSelected(_selectedDeviceInformation.Id);

            // Set status to Connecting
            _picker.SetDisplayStatus(args.SelectedDevice, "", DevicePickerDisplayStatusOptions.None);
        }

        public async Task StartProjectingAsync(int currentViewId, int newViewId)
        {
            if (ProjectionManager.ProjectionDisplayAvailable)
            {
                if (_selectedDeviceInformation == null)
                {
                    _selectedDeviceInformation = await DeviceInformation.CreateFromIdAsync(SelectedDeviceID);
                }


                if (_selectedDeviceInformation != null)
                {
                    bool isFound = false;
                    do
                    {
                        try
                        {
                            await ProjectionManager.StartProjectingAsync(newViewId, currentViewId, _selectedDeviceInformation);

                            isFound = true;
                        }
                        catch (ArgumentException)
                        {
                            // just ignore the exception, the projection is anyway going on
                            isFound = true;
                        }
                        catch (Exception)
                        {
                            // TODO: Unexpected exception handling
                        }
                    } while (!isFound);
                }
            }
        }

        private void Picker_DevicePickerDismissed(DevicePicker sender, object args)
        {
            if (_selectedDeviceInformation != null)
            {
                _picker.SetDisplayStatus(_selectedDeviceInformation, "", DevicePickerDisplayStatusOptions.None);
                _selectedDeviceInformation = null;
            }
        }

  
    }


}