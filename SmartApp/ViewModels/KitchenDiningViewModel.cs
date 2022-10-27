using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using SmartApp.Helpers;
using SmartApp.Models;
using SmartApp.Services;
using SmartApp.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmartApp.ViewModels
{
    internal class KitchenDiningViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly NavigationStore _navigationStore;
        private readonly IWeatherService _weatherService;

        public ICommand NavigateToSettings { get; }

        public KitchenDiningViewModel(IDeviceService deviceService, NavigationStore navigationStore, IWeatherService weatherService)
        {
            _deviceService = deviceService;
            _navigationStore = navigationStore;
            _weatherService = weatherService;

            DeviceItems = new ObservableCollection<DeviceItem>();
            NavigateToSettings = new NavigateCommand<KitchenDiningViewModel>(navigationStore, () => new KitchenDiningViewModel(_deviceService, _navigationStore, _weatherService));

            SetWeatherAsync().ConfigureAwait(false);
            PopulateDeviceItemsAsync().ConfigureAwait(false);
        }

        private ObservableCollection<DeviceItem>? _deviceItems;
        public ObservableCollection<DeviceItem>? DeviceItems
        {
            get => _deviceItems;
            set
            {
                _deviceItems = value;
                OnPropertyChanged();
            }
        }

        private string? _currentHumidity;
        public string CurrentHumidity
        {
            get => _currentHumidity!;
            set
            {
                _currentHumidity = value;
                OnPropertyChanged();
            }
        }

        private string? _currentTemperature;
        public string CurrentTemperature
        {
            get => _currentTemperature!;
            set
            {
                _currentTemperature = value;
                OnPropertyChanged();
            }
        }

        protected override async void second_timer_tick(object? sender, EventArgs e)
        {
            await PopulateDeviceItemsAsync();
            base.second_timer_tick(sender, e);
        }

        private async Task SetWeatherAsync()
        {
            var weather = await _weatherService.GetWeatherDataAsync();
            CurrentTemperature = weather.Temperature.ToString();
            CurrentHumidity = weather.Humidity.ToString();
        }

        private async Task PopulateDeviceItemsAsync()
        {
            var result = await _deviceService.GetDevicesAsync("select * from devices");

            result.ForEach(device =>
            {
                var item = DeviceItems?.FirstOrDefault(x => x.DeviceId == device.DeviceId);
                if (item == null)
                    DeviceItems?.Add(device);
                else
                {
                    var index = _deviceItems!.IndexOf(item);
                    _deviceItems[index] = device;
                }
            });
        }
    }
}
