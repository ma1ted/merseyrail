using System;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Merseyrail.Notifications.Models;

namespace Merseyrail.Notifications;

public class IncidentNotification : BaseNotification<IncidentAlert>
{
	private const string ChannelName = "Incidents";

	public override Type[] SupportedTypes => new Type[1] { typeof(IncidentAlert) };

	public IncidentNotification(Context context, IncidentAlert alert)
		: base(context, alert)
	{
	}

	protected override int GetNotificationId()
	{
		return new Random().Next();
	}

	protected override string GetNotificationTitle()
	{
		return base.Data.Title ?? "Merseyrail Incident";
	}

	protected override string GetNotificationBody()
	{
		return base.Data.Message;
	}

	protected override string GetChannelId()
	{
		return "Incidents";
	}

	protected override string GetChannelName()
	{
		return "Incidents";
	}

	protected override string GetChannelDescription()
	{
		return "Sent to communicate minor incidents that may affect you";
	}

	protected override NotificationImportance GetChannelImportance()
	{
		return NotificationImportance.Max;
	}

	protected override NotificationCompat.Builder GetNotification(NotificationCompat.Builder builder)
	{
		Intent intent = new Intent(base.Context, typeof(NotificationViewer));
		intent.PutExtra("Title", GetNotificationTitle());
		intent.PutExtra("Description", GetNotificationBody());
		intent.PutExtra("NotificationType", base.Data.GetNotificationType().ToString());
		intent.PutExtra("Alert", GetNotificationBody());
		intent.PutExtra("RouteCrs", base.Data.RouteCrs);
		intent.PutExtra("fromNotification", value: true);
		PendingIntent activity = PendingIntent.GetActivity(base.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
		builder.SetContentIntent(activity).SetPriority(1).SetTicker(GetNotificationBody())
			.SetOngoing(ongoing: false)
			.SetContentTitle(GetNotificationTitle())
			.SetContentText(GetNotificationBody())
			.SetSound(base.Data.GetSound())
			.SetAutoCancel(autoCancel: true);
		return builder;
	}
}
