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
using WorkingNameBle.Android.Central;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Central.Android.IntegrationsTests.Tests
{
    public class CentralManagerTests : Shared.IntegrationsTests.CentralManagerTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}