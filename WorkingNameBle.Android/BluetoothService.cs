using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Android
{
    public class BluetoothService : IBluetoothService
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;

        public void Init(IScheduler scheduler = null)
        {
            if (!_initialized)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            _initialized = true;
        }

        public void Shutdown()
        {
            _initialized = false;
        }

        public Task<bool> ReadyToDiscover()
        {
            return Task.FromResult(_initialized);
        }

        public IObservable<IDevice> ScanForDevices()
        {
            CheckInitialized();

            if (_discoverObservable == null)
            {
                // Store this and return the same someone else subscribes
                _discoverObservable = Observable.Create<IDevice>(observer =>
                {
                    BleScanCallback bleScanCallback = new BleScanCallback((type, result) =>
                    {
                        var device = new Device(result.Device);
                        observer.OnNext(device);
                    }, failure =>
                    {
                        observer.OnError(new DiscoverDeviceException(failure.ToString()));
                    });

                    _bluetoothAdapter.BluetoothLeScanner.StartScan(bleScanCallback);

                    return Disposable.Create(() =>
                    {
                        _bluetoothAdapter.BluetoothLeScanner.StopScan(bleScanCallback); 
                    });
                });
            }
            return _discoverObservable;
        }

        public Task<bool> ConnectToDevice(IDevice device)
        {
            var nativeDevice = ((Device)device).NativeDevice;
            var context = Application.Context;

            var callback = new BleGattCallback();
            nativeDevice.ConnectGatt(context, true, callback);
        }

        private void CheckInitialized()
        {
            if (!_initialized)
            {
                throw new Exception("Service not initialized");
            }
        }
    }

    public class BleGattCallback : BluetoothGattCallback
    {
        private readonly Action<BluetoothGatt, GattStatus, ProfileState> _connectionStateChangedAction;

        public BleGattCallback(Action<BluetoothGatt, GattStatus, ProfileState> connectionStateChangedAction)
        {
            _connectionStateChangedAction = connectionStateChangedAction;
        }


        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            _connectionStateChangedAction?.Invoke(gatt, status, newState);
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorRead(gatt, descriptor, status);
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
        }

        public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, GattStatus status)
        {
            base.OnReadRemoteRssi(gatt, rssi, status);
        }

        public override void OnReliableWriteCompleted(BluetoothGatt gatt, GattStatus status)
        {
            base.OnReliableWriteCompleted(gatt, status);
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
        }
    }
}