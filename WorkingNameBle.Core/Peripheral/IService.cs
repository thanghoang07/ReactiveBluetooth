﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core.Peripheral
{
    public interface IService : Core.IService
    {
        bool AddCharacteristic(ICharacteristic characteristic);
    }
}
