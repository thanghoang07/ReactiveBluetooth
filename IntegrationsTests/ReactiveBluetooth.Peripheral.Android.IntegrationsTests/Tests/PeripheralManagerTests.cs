using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveBluetooth.Android.Peripheral;
using ReactiveBluetooth.Core.Peripheral;

namespace ReactiveBluetooth.Peripheral.Android.IntegrationsTests.Tests
{
    public class PeripheralManagerTests : Shared.IntegrationsTests.PeripheralManagerTests
    {
        public override IPeripheralManager GetPeripheralManager()
        {
            return new PeripheralManager();
        }
    }
}