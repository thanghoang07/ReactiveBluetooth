﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Peripheral
{
    public interface IAttRequest
    {
        int Offset { get; }
        byte[] Value { get; }
    }
}
