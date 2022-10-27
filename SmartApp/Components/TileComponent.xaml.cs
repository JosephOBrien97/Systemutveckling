using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using SmartApp.Helpers;
using SmartApp.Models;
using SmartApp.Services;
using SmartApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SmartApp.Components
{
    public partial class TileComponent : UserControl, INotifyPropertyChanged
    {
        public TileComponent()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static readonly DependencyProperty DeviceNameProperty = DependencyProperty.Register("DeviceName", typeof(string), typeof(TileComponent));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceTypeProperty = DependencyProperty.Register("DeviceType", typeof(string), typeof(TileComponent));
        public string DeviceType
        {
            get { return (string)GetValue(DeviceTypeProperty); }
            set { SetValue(DeviceTypeProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(TileComponent));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty DeviceStateProperty = DependencyProperty.Register("DeviceState", typeof(bool), typeof(TileComponent));

        private bool _deviceState;

        public bool DeviceState
        {
            get { return _deviceState; }
            set
            {
                _deviceState = value;
                OnPropertyChanged();
            }
        }

        private async void DeviceTile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var deviceItem = (DeviceItem)button!.DataContext;
                deviceItem.DeviceState = !deviceItem.DeviceState;
                IsChecked = deviceItem.DeviceState;

                using ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=kyh-funapp-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=H21+QXAX6wXddKm3DdQnAqxkupEmaMGXuacb5q0SDBI=");

                var directMethod = new CloudToDeviceMethod("OnOff");
                directMethod.SetPayloadJson(JsonConvert.SerializeObject(new { deviceState = IsChecked }));
                var result = await serviceClient.InvokeDeviceMethodAsync(deviceItem.DeviceId, directMethod);
            }
            catch { }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=kyh-funapp-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=H21+QXAX6wXddKm3DdQnAqxkupEmaMGXuacb5q0SDBI=");

            var button = sender as Button;
            var deviceItem = (DeviceItem)button!.DataContext;
            tileComponent.Visibility = Visibility.Collapsed;
            var result = await registryManager.GetDeviceAsync(deviceItem.DeviceId);

            await registryManager.RemoveDeviceAsync(result).ConfigureAwait(false);
        }
    }
}
