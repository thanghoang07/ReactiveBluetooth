using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class ServiceAdded
    {
        public ServiceAdded(ProfileState profileState, BluetoothGattService bluetoothGattService)
        {
            ProfileState = profileState;
            BluetoothGattService = bluetoothGattService;
        }

        public ProfileState ProfileState { get; }
        public BluetoothGattService BluetoothGattService { get; }
    }
}