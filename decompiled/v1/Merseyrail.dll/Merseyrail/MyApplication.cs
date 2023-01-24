using System;
using Android.App;
using Android.Runtime;
using Common.IoC;
using Common.Services;
using Firebase;
using Merseyrail.Helpers;
using Merseyrail.Services;
using Merseyrail.Shared;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Merseyrail;

[Application]
public class MyApplication : Application
{
	public static MyApplication App { get; set; }

	public MyApplication(IntPtr javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
		App = this;
	}

	public override async void OnCreate()
	{
		base.OnCreate();
		AppCenter.Start("15430130-577d-468a-8653-9302c6a4c81d", typeof(Analytics), typeof(Crashes));
		try
		{
			InitIoC();
			FirebaseApp.InitializeApp(this);
		}
		catch (Exception)
		{
		}
		AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
	}

	private void InitIoC()
	{
		IoCUtils.RegisterSingletons();
		AppInitHelpers.AppIoCInit();
		SharedSettings.SettingsService = (SettingsService)SharedServices.IoC.Resolve<ISettingsService>();
		SharedSettings.SettingsService.Initialise(this);
	}

	private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Exception ex = (Exception)e.ExceptionObject;
		if (ex != null)
		{
			Crashes.TrackError(ex);
		}
	}
}
