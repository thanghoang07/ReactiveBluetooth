﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.iOS.Central
{
    public class Device : IDevice
    {
        private PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;

        public Device(CBPeripheral peripheral)
        {
            Peripheral = peripheral;
            _cbPeripheralDelegate = new PeripheralDelegate.PeripheralDelegate();
            peripheral.Delegate = _cbPeripheralDelegate;
            Name = peripheral.Name;
        }

        public CBPeripheral Peripheral { get; }
        public string Name { get; }
        public ConnectionState State => (ConnectionState) Peripheral.State;

        public IObservable<IList<IService>> DiscoverServices()
        {
            return Observable.Create<IList<IService>>(observer =>
            {
                var discoverDisp = _cbPeripheralDelegate.DiscoveredServiceSubject.Select(x => x.Peripheral.Services).Subscribe(services =>
                {
                    IList<IService> list = services.Select(x => new Service(x, Peripheral))
                        .Cast<IService>().ToList();
                    observer.OnNext(list);
                });

                Peripheral.DiscoverServices();

                return Disposable.Create(() =>
                {
                    discoverDisp.Dispose();
                });
            });
        }
    }
}