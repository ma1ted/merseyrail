using System;
using Android.App;
using Android.Content.PM;
using Android.Util;
using Common;
using Common.Services;
using Firebase.Messaging;
using Merseyrail.Helpers;
using Merseyrail.Notifications;
using Merseyrail.Notifications.Models;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail.Firebase.Messaging;

[Service]
[IntentFilter(new string[] { "com.google.firebase.MESSAGING_EVENT" })]
public class MerseyrailFirebaseMessagingService : FirebaseMessagingService
{
	public override void OnMessageReceived(RemoteMessage message)
	{
		HandleNotification(message);
	}

	public override void OnNewToken(string p0)
	{
		Log.Info("MERSEYRAIL-FCM", "Token refreshed: " + p0);
		bool allowIncidents = NotificationHelper.IsNotificationChannelEnabled(this, "Incidents");
		bool allowReminders = NotificationHelper.IsNotificationChannelEnabled(this, "Reminders");
		bool allowCalamities = NotificationHelper.IsNotificationChannelEnabled(this, "Calamities");
		string currentAppVersion = Android.App.Application.Context.PackageManager!.GetPackageInfo(Android.App.Application.Context.PackageName, (PackageInfoFlags)0)!.VersionName;
		SharedServices.IoC.Resolve<UserService>().UpdateNotificationTokenAndPermissions(SharedSettings.DeviceInstanceId, p0, DeviceType.Android, allowIncidents, allowReminders, allowCalamities, currentAppVersion, delegate(string result)
		{
			if (result != "ERROR")
			{
				SharedSettings.FirebaseToken = result;
				SharedSettings.IncidentsAllowed = allowIncidents;
				SharedSettings.RemindersAllowed = allowReminders;
				SharedSettings.CalamitiesAllowed = allowCalamities;
				SharedSettings.AppVersion = currentAppVersion;
			}
		});
	}

	private void HandleNotification(RemoteMessage message)
	{
		if (IsReminder(message))
		{
			CreateNotification("Merseyrail Reminder", message.Data["alert"], message);
		}
		else if (IsCalamity(message))
		{
			CreateNotification("Merseyrail", message.Data["Title"], message);
		}
		else if (IsIncident(message))
		{
			CreateNotification("Merseyrail Incident", message.Data["alert"], message);
		}
		else if (IsDirectMessage(message))
		{
			CreateNotification("Direct Message", message.Data["alert"], message);
		}
	}

	private void CreateNotification(string title, string desc, RemoteMessage message)
	{
		if (IsReminder(message))
		{
			ReminderAlert alert = new ReminderAlert(PackageName, title, desc);
			new ReminderNotification(this, alert).DisplayNotification();
		}
		else if (IsIncident(message) || IsDirectMessage(message))
		{
			IncidentAlert alert2 = new IncidentAlert(PackageName, message.Data["routeCrs"], title, desc);
			new IncidentNotification(this, alert2).DisplayNotification();
		}
		else if (IsCalamity(message))
		{
			string text = "";
			if (message.Data.ContainsKey("Id"))
			{
				string s = message.Data["DateFrom"];
				DateTime dateFrom = new DateTime(long.Parse(s));
				string s2 = message.Data["DateTo"];
				DateTime dateTo = new DateTime(long.Parse(s2));
				CalamityAlert calamityAlert = new CalamityAlert
				{
					Id = int.Parse(message.Data["Id"]),
					DateFrom = dateFrom,
					DateTo = dateTo,
					Title = message.Data["Title"],
					Body = message.Data["Body"]
				};
				SharedServices.IoC.Resolve<CalamityService>().SuppressAlert(calamityAlert.Id);
				text = JsonConvert.SerializeObject(calamityAlert);
			}
			else
			{
				text = JsonConvert.SerializeObject(new CalamityAlert
				{
					Title = title,
					Body = message.Data["alert"]
				});
				desc = message.Data["alert"];
			}
			CalamityFCMAlert alert3 = new CalamityFCMAlert(PackageName, text, message.Data["routeCrs"], title, desc);
			new CalamityNotification(this, alert3).DisplayNotification();
		}
	}

	private bool IsReminder(RemoteMessage message)
	{
		return message?.Data.ContainsKey("reminder") ?? false;
	}

	private bool IsIncident(RemoteMessage message)
	{
		return message?.Data.ContainsKey("Incident") ?? false;
	}

	private bool IsCalamity(RemoteMessage message)
	{
		return message?.Data.ContainsKey("Calamity") ?? false;
	}

	private bool IsDirectMessage(RemoteMessage message)
	{
		return message?.Data.ContainsKey("DirectMessage") ?? false;
	}
}
