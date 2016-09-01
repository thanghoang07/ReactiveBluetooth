using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class MtuChanged
    {
        public MtuChanged(BluetoothDevice bluetoothDevice, int mtu)
        {
            BluetoothDevice = bluetoothDevice;
            Mtu = mtu;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int Mtu { get; }
    }
}