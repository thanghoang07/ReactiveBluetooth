﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveBluetooth.Core.Central;

namespace SampleApp.ViewModels.Central
{
    public class ServiceViewModel : BindableBase
    {
        private readonly IService _service;

        public ServiceViewModel(IService service)
        {
            _service = service;
            Uuid = _service.Uuid;
        }

        private Guid _uuid;

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }
    }
}
