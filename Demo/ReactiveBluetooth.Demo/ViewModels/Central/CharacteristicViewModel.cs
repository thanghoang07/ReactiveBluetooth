﻿using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common.Behaviors;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;
using Xamarin.Forms;

namespace Demo.ViewModels.Central
{
    public class CharacteristicViewModel : BindableBase, INavigationAware, IPageAppearingAware, INavigationBackAware, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IPageDialogService _pageDialogService;
        private ICharacteristic _characteristic;
        private bool _connected;
        private IDisposable _connectionDisp;
		private IDisposable _notifyDisposable;
        private string _value;
        private string _writeValue;

        public CharacteristicViewModel(IPageDialogService pageDialogService) : this()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _pageDialogService = pageDialogService;

            ReadValueCommand = new DelegateCommand(() =>
            {
                var read = Read();
            }).ObservesCanExecute(o => Connected);
            WriteValueCommand = new DelegateCommand(() =>
            {
                var write = Write();
            }).ObservesCanExecute(o => Connected);
            ToggleNotificationsCommands = new DelegateCommand(() =>
            {
                var notify = ToggleNotifications();
            });
        }

        public CharacteristicViewModel()
        {
            Descriptors = new ObservableCollection<DescriptorViewModel>();
        }

        public IDevice Device { get; set; }

        public ICharacteristic Characteristic
        {
            get { return _characteristic; }
            set
            {
                _characteristic = value;
                Update();
            }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string WriteValue
        {
            get { return _writeValue; }
            set { SetProperty(ref _writeValue, value); }
        }

        public bool Connected
        {
            get { return _connected; }
            set { SetProperty(ref _connected, value); }
        }

        public Guid Uuid => _characteristic?.Uuid ?? Guid.Empty;
        public bool CanRead => _characteristic?.Properties.HasFlag(CharacteristicProperty.Read) ?? false;

        public bool CanWrite
        {
            get
            {
                var write = _characteristic?.Properties.HasFlag(CharacteristicProperty.Write) ?? false;
                var writeWithouResponse = _characteristic?.Properties.HasFlag(CharacteristicProperty.WriteWithoutResponse) ?? false;
                return write || writeWithouResponse;
            }
        }

        public bool CanNotify => (_characteristic?.Properties.HasFlag(CharacteristicProperty.Notify) ?? false) || (_characteristic?.Properties.HasFlag(CharacteristicProperty.Indicate) ?? false);
        public CharacteristicProperty Properties => _characteristic?.Properties ?? 0;
        public ObservableCollection<DescriptorViewModel> Descriptors { get; }
        public DelegateCommand ReadValueCommand { get; }
        public DelegateCommand WriteValueCommand { get; }
        public DelegateCommand ToggleNotificationsCommands { get; }

		public void OnNavigatingTo(NavigationParameters parameters)
		{

		}

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(nameof(ICharacteristic)))
            {
                var characteristic = (ICharacteristic) parameters[nameof(ICharacteristic)];
                Characteristic = characteristic;
            }
            if (parameters.ContainsKey(nameof(IDevice)))
            {
                var device = (IDevice) parameters[nameof(IDevice)];
                Device = device;
            }
            if (parameters.ContainsKey("ConnectionObservable"))
            {
                var connectionObservable = (IObservable<ConnectionState>) parameters["ConnectionObservable"];
                _connectionDisp = connectionObservable.Subscribe(state =>
                {
                    if (state == ConnectionState.Disconnecting || state == ConnectionState.Disconnected)
                    {
                        Connected = false;
                        _cancellationTokenSource.Cancel();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { _pageDialogService.DisplayAlertAsync("Disconnected", "Device disconnected", "Ok"); });
                    }
                });
            }
            Connected = true;
        }

        public void PagePopped()
        {
            _cancellationTokenSource?.Cancel();
            _connectionDisp.Dispose();
        }

        public void OnAppearing(Page page)
        {
        }

        public void OnDisappearing(Page page)
        {
        }

        public async Task ToggleNotifications()
        {
        	if (_notifyDisposable != null)
	        {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
	            bool stop = await Device.StopNotifiations(Characteristic, cancellationTokenSource.Token);
        		_notifyDisposable?.Dispose();
				_notifyDisposable = null;
			} else {
			    try
			    {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000);
			        try
			        {
			            bool start = await Device.StartNotifications(Characteristic, cancellationTokenSource.Token);
			            if (!start)
			            {
			                await _pageDialogService.DisplayAlertAsync("Error", "Failed to enable notifications", "OK");
			                return;
			            }
			        }
			        catch (Exception e)
			        {
                        await _pageDialogService.DisplayAlertAsync("Error", "Failed to enable notifications", "OK");
                        return;
                    }
                    
                    _notifyDisposable = Device.Notifications(Characteristic)
			            .Subscribe(bytes =>
			            {
			                if (bytes != null)
			                {
			                    Value = BitConverter.ToString(bytes);
			                }
			                else
			                {
			                    Value = "(null)";
			                }
			            }, async exception =>
			            {
                            await _pageDialogService.DisplayAlertAsync("Error", "Failed to enable notifications", "OK");
                        });
			    }
			    catch (Exception e)
			    {
                    await _pageDialogService.DisplayAlertAsync("Error", "Failed to enable notifications", "OK");
                }
				
			}
        }

        private void Update()
        {
            OnPropertyChanged(() => Uuid);
            OnPropertyChanged(() => CanRead);
            OnPropertyChanged(() => CanWrite);
            OnPropertyChanged(() => CanNotify);
            OnPropertyChanged(() => Properties);

            Descriptors.Clear();
            if (_characteristic.Descriptors == null)
                return;

            foreach (var descriptor in _characteristic.Descriptors)
            {
                Descriptors.Add(new DescriptorViewModel(descriptor));
            }
        }

        private async Task Read()
        {
            if (Device == null)
                return;

            try
            {
                var result = await Device.ReadValue(Characteristic, _cancellationTokenSource.Token);

                if (result != null)
                    Value = BitConverter.ToString(result);
            }
            catch (TaskCanceledException)
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Failed to write characteristic", "OK");
            }
        }

        private async Task Write()
        {
            if (Device == null)
                return;

            var writeType = Properties.HasFlag(CharacteristicProperty.WriteWithoutResponse) ? WriteType.WithoutRespoonse : WriteType.WithResponse;

            try
            {
                var result = await Device.WriteValue(Characteristic, Encoding.UTF8.GetBytes(WriteValue), writeType, _cancellationTokenSource.Token);
                if (result)
                {
                    await Read();
                }
            }
            catch (TaskCanceledException)
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Failed to write characteristic", "OK");
            }
        }

		

		
		public void Dispose() {
			_notifyDisposable?.Dispose();
		}
	}
}