using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Merseyrail.Shared;

namespace Merseyrail.Helpers;

public static class NotificationHelper
{
	public static bool IsNotificationChannelEnabled(Context context, string channelName)
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			if (context.GetSystemService("notification") is NotificationManager notificationManager)
			{
				if (!string.IsNullOrEmpty(channelName))
				{
					NotificationChannel notificationChannel = notificationManager.GetNotificationChannel(channelName);
					if (notificationChannel != null)
					{
						if (notificationChannel.Importance != 0)
						{
							return notificationManager.AreNotificationsEnabled();
						}
						return false;
					}
					return true;
				}
				return notificationManager.AreNotificationsEnabled();
			}
			return false;
		}
		return NotificationManagerCompat.From(context).AreNotificationsEnabled();
	}

	public static bool ShouldUpdateServer(string latestToken, string currentToken, bool allowIncidents, bool allowReminders, bool allowCalamities, string currentAppVersion)
	{
		if (string.IsNullOrEmpty(latestToken) || !(currentToken != latestToken))
		{
			if (allowIncidents == SharedSettings.IncidentsAllowed && allowReminders == SharedSettings.RemindersAllowed && allowCalamities == SharedSettings.CalamitiesAllowed)
			{
				return currentAppVersion != SharedSettings.AppVersion;
			}
			return true;
		}
		return true;
	}
}
