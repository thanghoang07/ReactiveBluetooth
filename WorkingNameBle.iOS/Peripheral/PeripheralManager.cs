﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.iOS.Peripheral
{
    public class PeripheralManagerDelegate : CBPeripheralManagerDelegate
    {
        public Subject<ManagerState> StateUpdatedObservable;
        
        public PeripheralManagerDelegate()
        {
            StateUpdatedObservable = new Subject<ManagerState>();
        }

        public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError error)
        {
            
        }

        public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            
        }

        public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            
        }

        public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            
        }

        public override void ReadyToUpdateSubscribers(CBPeripheralManager peripheral)
        {
            
        }

        public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError error)
        {
            
        }

        public override void WriteRequestsReceived(CBPeripheralManager peripheral, CBATTRequest[] requests)
        {
            
        }

        public override void StateUpdated(CBPeripheralManager peripheral)
        {
            StateUpdatedObservable.OnNext((ManagerState)peripheral.State);
        }
    }

    public class PeripheralManager : IPeripheralManager
    {
        private CBPeripheralManager _peripheralManager;
        private PeripheralManagerDelegate _peripheralDelegate;

        public ManagerState State
        {
            get
            {
                if (_peripheralManager != null)
                    return (ManagerState) _peripheralManager.State;
                return ManagerState.PoweredOff;
            }
        }
        
        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            _peripheralDelegate = new PeripheralManagerDelegate();
            _peripheralManager = new CBPeripheralManager(_peripheralDelegate, DispatchQueue.MainQueue);

            return _peripheralDelegate.StateUpdatedObservable;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void StartAdvertising(AdvertisingOptions advertisingOptions)
        {
            throw new NotImplementedException();
        }
    }
}
