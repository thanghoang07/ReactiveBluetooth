﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using System.Reactive.Linq;
using System.Threading;
using Prism.Services;
using ReactiveBluetooth.Core.Types;
using SampleApp.Common.Behaviors;
using Xamarin.Forms;

namespace SampleApp.ViewModels.Central
{
    public class CharacteristicViewModel : BindableBase, INavigationAware, IPageAppearingAware, INavigationBackAware
    {
        private readonly IPageDialogService _pageDialogService;
        private ICharacteristic _characteristic;
        private string _value;
        private IDisposable _connectionDisp;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _connected;
        private string _writeValue;

        public CharacteristicViewModel(IPageDialogService pageDialogService)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _pageDialogService = pageDialogService;

            ReadValueCommand = DelegateCommand.FromAsyncHandler(Read)
                .ObservesCanExecute(o => Connected);
            WriteValueCommand = DelegateCommand.FromAsyncHandler(Write)
                .ObservesCanExecute(o => Connected);
        }

        public CharacteristicViewModel()
        {
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
        public DelegateCommand ReadValueCommand { get; }
        public DelegateCommand WriteValueCommand { get; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(nameof(ICharacteristic)))
            {
                ICharacteristic characteristic = (ICharacteristic) parameters[nameof(ICharacteristic)];
                Characteristic = characteristic;
            }
            if (parameters.ContainsKey(nameof(IDevice)))
            {
                IDevice device = (IDevice) parameters[nameof(IDevice)];
                Device = device;
            }
            if (parameters.ContainsKey("ConnectionObservable"))
            {
                IObservable<ConnectionState> connectionObservable = (IObservable<ConnectionState>) parameters["ConnectionObservable"];
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

        public void OnAppearing(Page page)
        {
        }

        public void OnDisappearing(Page page)
        {
        }

        public void PagePopped()
        {
            _cancellationTokenSource?.Cancel();
            _connectionDisp.Dispose();
        }

        private void Update()
        {
            OnPropertyChanged(() => Uuid);
            OnPropertyChanged(() => CanRead);
            OnPropertyChanged(() => CanWrite);
            OnPropertyChanged(() => CanNotify);
            OnPropertyChanged(() => Properties);
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
            }
        }
    }
}