﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Exceptions
{
    public class DiscoverDeviceException : Exception
    {
        public DiscoverDeviceException(string message) : base(message)
        {
        }
    }
}
