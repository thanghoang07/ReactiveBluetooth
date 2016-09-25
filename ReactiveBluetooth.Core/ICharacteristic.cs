﻿using System;
using System.Security.Cryptography.X509Certificates;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core
{
    public interface ICharacteristic
    {
        Guid Uuid { get; }

        CharacteristicProperty Properties { get; }
    }
}
