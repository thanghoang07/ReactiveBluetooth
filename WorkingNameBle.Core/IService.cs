﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core
{
    public interface IService
    {
        Guid Id { get; }

        IObservable<IList<ICharacteristic>> DiscoverCharacteristics();
    }
}
