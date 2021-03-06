﻿using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;
using ReactiveBluetooth.Core.Central;

namespace Demo.ViewModels.Central
{
    public class ServiceViewModel : BindableBase
    {
        private readonly IService _service;
        private Guid _uuid;
        private bool _displayCharacteristics;

        public ServiceViewModel(IService service)
        {
            _service = service;
            Uuid = _service.Uuid;
            Characteristics = new ObservableCollection<CharacteristicViewModel>();
        }

        public ObservableCollection<CharacteristicViewModel> Characteristics { get; }

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public bool DisplayCharacteristics
        {
            get { return _displayCharacteristics; }
            set { SetProperty(ref _displayCharacteristics, value); }
        }

        public async Task DiscoverCharacteristics(CancellationToken cancellationToken)
        {
            try
            {
                var characteristics = await _service.DiscoverCharacteristics(cancellationToken);
                Characteristics.Clear();

                foreach (var characteristic in characteristics)
                {
                    CharacteristicViewModel characteristicViewModel = new CharacteristicViewModel
                    {
                        Characteristic = characteristic,
                    };
                    Characteristics.Add(characteristicViewModel);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}