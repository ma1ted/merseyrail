using System;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Merseyrail.Notifications.Models;

namespace Merseyrail.Notifications;

public class ReminderNotification : BaseNotification<ReminderAlert>
{
	private const string ChannelName = "Reminders";

	public override Type[] SupportedTypes => new Type[1] { typeof(ReminderAlert) };

	public ReminderNotification(Context context, ReminderAlert alert)
		: base(context, alert)
	{
	}

	protected override int GetNotificationId()
	{
		return new Random().Next();
	}

	protected override string GetNotificationTitle()
	{
		return base.Data.Title ?? "Merseyrail Reminder";
	}

	protected override string GetNotificationBody()
	{
		return base.Data.Message;
	}

	protected override string GetChannelId()
	{
		return "Reminders";
	}

	protected override string GetChannelName()
	{
		return "Reminders";
	}

	protected override string GetChannelDescription()
	{
		return "Sent to remind you about your train";
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
