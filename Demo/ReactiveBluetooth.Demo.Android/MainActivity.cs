﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Demo;
using Microsoft.Practices.Unity;
using Prism.Unity;
using ReactiveBluetooth.Android.Central;
using ReactiveBluetooth.Android.Peripheral;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;

namespace ReactiveBluetooth.Demo.Android
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);            
            var app = new App(new PlatformInitializer());
            LoadApplication(app);
        }
    }

    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IPeripheralManager, PeripheralManager>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<ICentralManager, CentralManager>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
        }
    }
}

