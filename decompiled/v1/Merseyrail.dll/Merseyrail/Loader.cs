using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Common.Services;
using Firebase.Iid;
using Merseyrail.Helpers;
using Merseyrail.Services;
using Merseyrail.Shared;
using Microsoft.AppCenter.Crashes;

namespace Merseyrail;

[Activity(NoHistory = true, Label = "Merseyrail", Theme = "@style/Theme.Splash", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
public class Loader : Activity
{
	private const float RequiredDatabaseVersion = 3f;

	protected override async void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		try
		{
			SetupFirebase();
			InitDatabases();
			if (Intent != null)
			{
				SetPendingMessageIdFromIntent(Intent);
			}
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
	}

	private void SetupFirebase()
	{
		if (!SharedSettings.HasConnection(this))
		{
			return;
		}
		string token = FirebaseInstanceId.Instance.Token;
		string firebaseToken = SharedSettings.FirebaseToken;
		bool allowIncidents = NotificationHelper.IsNotificationChannelEnabled(this, "Incidents");
		bool allowReminders = NotificationHelper.IsNotificationChannelEnabled(this, "Reminders");
		bool allowCalamities = NotificationHelper.IsNotificationChannelEnabled(this, "Calamities");
		string currentAppVersion = Android.App.Application.Context.PackageManager!.GetPackageInfo(Android.App.Application.Context.PackageName, (PackageInfoFlags)0)!.VersionName;
		if (!NotificationHelper.ShouldUpdateServer(token, firebaseToken, allowIncidents, allowReminders, allowCalamities, currentAppVersion))
		{
			return;
		}
		SharedServices.IoC.Resolve<UserService>().UpdateNotificationTokenAndPermissions(SharedSettings.DeviceInstanceId, token, DeviceType.Android, allowIncidents, allowReminders, allowCalamities, currentAppVersion, delegate(string result)
		{
			if (!result.Contains("ERROR"))
			{
				SharedSettings.FirebaseToken = result;
				SharedSettings.IncidentsAllowed = allowIncidents;
				SharedSettings.RemindersAllowed = allowReminders;
				SharedSettings.CalamitiesAllowed = allowCalamities;
				SharedSettings.AppVersion = currentAppVersion;
			}
		});
	}

	protected override void OnNewIntent(Intent intent)
	{
		base.OnNewIntent(intent);
		SetPendingMessageIdFromIntent(intent);
	}

	private void SetPendingMessageIdFromIntent(Intent intent)
	{
		if (intent.Extras != null && intent.Action == "glow.merseyrail.widget.STATION_SELECTED_NAME" && intent.Extras!.KeySet()!.Contains("glow.merseyrail.widget.STATION_SELECTED_NAME"))
		{
			string stringExtra = intent.GetStringExtra("glow.merseyrail.widget.STATION_SELECTED_NAME");
			string stringExtra2 = intent.GetStringExtra("glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID");
			SharedSettings.SettingsService.Initialise(this);
			SharedSettings.SettingsService.SaveSetting("Widget_Selected_Station", stringExtra);
			SharedSettings.SettingsService.SaveSetting("Widget_Selected_Train_Id", stringExtra2);
		}
	}

	private void InitDatabases()
	{
		ISettingsService settingsService = SharedServices.IoC.Resolve<ISettingsService>();
		if (settingsService.GetSetting("DB_CURRENT_VERSION", 0f) < 3f)
		{
			settingsService.SaveSetting("DB_FILE_SCHEDULE", string.Empty);
			settingsService.SaveSetting("DB_LASTMODIFIED_FILE_SCHEDULE", DateTime.MinValue.ToString());
			settingsService.SaveSetting("DB_FILE_STATIONS", string.Empty);
			settingsService.SaveSetting("DB_LASTMODIFIED_FILE_STATIONS", DateTime.MinValue.ToString());
			settingsService.SaveSetting("DB_CURRENT_VERSION", 3f);
		}
		CreateScheduleDB();
		CreateStationDB();
		if (SharedSettings.HasConnection(this))
		{
			DatabaseFileService databaseFileService = SharedServices.IoC.Resolve<DatabaseFileService>();
			databaseFileService.OnScheduleDbUpdateComplete += delegate
			{
				StartActivity(typeof(Main));
			};
			databaseFileService.UpdateScheduleDatabase();
		}
		else
		{
			StartActivity(typeof(Main));
		}
	}

	private string CreateScheduleDB()
	{
		ISettingsService settingsService = SharedServices.IoC.Resolve<ISettingsService>();
		string text = SharedSettings.SettingsService.Prefs.GetString("DB_FILE_SCHEDULE", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			text = CreateDBFileFromAsset("schedule.db");
			settingsService.SaveSetting("DB_FILE_SCHEDULE", text);
		}
		return text;
	}

	private string CreateStationDB()
	{
		ISettingsService settingsService = SharedServices.IoC.Resolve<ISettingsService>();
		string text = SharedSettings.SettingsService.Prefs.GetString("DB_FILE_STATIONS", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			text = CreateDBFileFromAsset("stations.db");
			settingsService.SaveSetting("DB_FILE_STATIONS", text);
		}
		return text;
	}

	private string CreateDBFileFromAsset(string assetName)
	{
		string text = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), assetName);
		File.Delete(text);
		if (!File.Exists(text))
		{
			using (Stream stream = Assets!.Open(assetName))
			{
				using FileStream destination = File.Create(text);
				stream.CopyTo(destination);
				return text;
			}
		}
		return text;
	}

	private void NotifyUser(string msg)
	{
		Notification notification = new NotificationCompat.Builder(this, GetChannel()).SetContentTitle("Merseyrail Data Updates").SetContentText(msg).SetSmallIcon(2131165320)
			.Build();
		NotificationManager manager = GetManager();
		int id = new Random().Next();
		manager.Notify(id, notification);
	}

	private string GetChannel()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			NotificationChannel channel = new NotificationChannel("data_updates", "data_updates", NotificationImportance.Low)
			{
				Description = "Merseyrail Data Updates"
			};
			GetManager().CreateNotificationChannel(channel);
		}
		return "data_updates";
	}

	private NotificationManager GetManager()
	{
		return NotificationManager.FromContext(this);
	}
}
