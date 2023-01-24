using System;
using Android.App;
using Android.Content;
using Android.Net;
using Common.Services;
using Merseyrail.Services;

namespace Merseyrail.Shared;

public static class SharedSettings
{
	public const string WIDGET_SELECTED_STATION = "Widget_Selected_Station";

	public const string WIDGET_SELECTED_TRAIN_ID = "Widget_Selected_Train_Id";

	public static SettingsService SettingsService { get; set; }

	public static bool InstanceIdRegistered
	{
		get
		{
			return !string.IsNullOrEmpty(SettingsService.Prefs.GetString("DEVICE_INSTANCE_ID", string.Empty));
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutBoolean("DEVICE_INSTANCE_ID", value)!.Commit();
		}
	}

	public static string DeviceInstanceId
	{
		get
		{
			if (!InstanceIdRegistered)
			{
				string text = Guid.NewGuid().ToString();
				InstanceIdRegistered = true;
				DeviceInstanceId = text;
				return text;
			}
			string text2 = SettingsService.Prefs.GetString("DEVICE_INSTANCE_ID", string.Empty);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Guid.NewGuid().ToString();
				InstanceIdRegistered = true;
				DeviceInstanceId = text2;
			}
			return text2;
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutString("DEVICE_INSTANCE_ID", value)!.Commit();
		}
	}

	public static bool IsRegistered => SettingsService.Prefs.GetBoolean("DEVICE_FIREBASE_REGISTERED", defValue: false);

	public static string FirebaseToken
	{
		get
		{
			return SettingsService.Prefs.GetString("DEVICE_FIREBASE_TOKEN", string.Empty);
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutString("DEVICE_FIREBASE_TOKEN", value)!.Commit();
			SettingsService.Prefs.Edit()!.PutBoolean("DEVICE_FIREBASE_REGISTERED", value: true)!.Commit();
		}
	}

	public static string DefaultName => SettingsService.Prefs.GetString("DEFAULT_NAME", string.Empty);

	public static string DefaultEmail => SettingsService.Prefs.GetString("DEFAULT_EMAIL", string.Empty);

	public static bool IncidentsAllowed
	{
		get
		{
			return SettingsService.Prefs.GetBoolean("DEVICE_INCIDENTS_PERMISSION", defValue: true);
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutBoolean("DEVICE_INCIDENTS_PERMISSION", value)!.Commit();
		}
	}

	public static bool RemindersAllowed
	{
		get
		{
			return SettingsService.Prefs.GetBoolean("DEVICE_REMINDERS_PERMISSION", defValue: true);
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutBoolean("DEVICE_REMINDERS_PERMISSION", value)!.Commit();
		}
	}

	public static bool CalamitiesAllowed
	{
		get
		{
			return SettingsService.Prefs.GetBoolean("DEVICE_CALAMITY_PERMISSION", defValue: true);
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutBoolean("DEVICE_CALAMITY_PERMISSION", value)!.Commit();
		}
	}

	public static string AppVersion
	{
		get
		{
			return SettingsService.Prefs.GetString("APP_VERSION", "");
		}
		set
		{
			SettingsService.Prefs.Edit()!.PutString("APP_VERSION", value)!.Commit();
		}
	}

	static SharedSettings()
	{
		if (SettingsService == null)
		{
			SettingsService = (SettingsService)SharedServices.IoC.Resolve<ISettingsService>();
		}
	}

	public static bool HasViewedHelpItem(string helpTag)
	{
		return SettingsService.Prefs.GetBoolean(helpTag, defValue: false);
	}

	public static bool HelpItemViewed(string helpTag)
	{
		ISharedPreferencesEditor? sharedPreferencesEditor = SettingsService.Prefs.Edit();
		sharedPreferencesEditor!.PutBoolean(helpTag, value: true);
		sharedPreferencesEditor!.Commit();
		return true;
	}

	public static bool HasConnection(Context context)
	{
		if (!HasMobileDataConnection(context))
		{
			return HasWifiDataConnection(context);
		}
		return true;
	}

	public static bool HasMobileDataConnection(Context context)
	{
		if (context != null)
		{
			NetworkInfo activeNetworkInfo = ((ConnectivityManager)context.GetSystemService("connectivity")).ActiveNetworkInfo;
			if (activeNetworkInfo != null)
			{
				return activeNetworkInfo.IsConnected;
			}
		}
		else
		{
			NetworkInfo activeNetworkInfo2 = ((ConnectivityManager)Application.Context.GetSystemService("connectivity")).ActiveNetworkInfo;
			if (activeNetworkInfo2 != null)
			{
				return activeNetworkInfo2.IsConnected;
			}
		}
		return false;
	}

	public static bool HasWifiDataConnection(Context context)
	{
		if (((ConnectivityManager)context.GetSystemService("connectivity")).GetNetworkInfo(ConnectivityType.Wifi)!.GetState() == NetworkInfo.State.Connected)
		{
			return true;
		}
		return false;
	}
}
