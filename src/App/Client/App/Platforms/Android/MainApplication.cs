﻿using Android.App;
using Android.Runtime;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.ChangeWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.ManageWifiInterfaces)]
[assembly: UsesPermission(Android.Manifest.Permission.ManageWifiNetworkSelection)]


namespace Functionland.FxBlox.Client.App.Platforms.Android
{
    [Application(
#if DEBUG
    UsesCleartextTraffic = true
#endif
    )]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram
            .CreateMauiAppBuilder()
            .Build();
    }
}