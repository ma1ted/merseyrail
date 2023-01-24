using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Java.Lang;

namespace Merseyrail.Notifications;

public abstract class BaseNotification<T> : INotificationHandler
{
	public abstract Type[] SupportedTypes { get; }

	protected Context Context { get; private set; }

	protected Resources Resources => Context.Resources;

	protected T Data { get; private set; }

	protected abstract int GetNotificationId();

	protected abstract string GetNotificationTitle();

	protected abstract string GetNotificationBody();

	protected abstract string GetChannelId();

	protected abstract string GetChannelName();

	protected abstract string GetChannelDescription();

	protected abstract NotificationImportance GetChannelImportance();

	protected abstract NotificationCompat.Builder GetNotification(NotificationCompat.Builder builder);

	protected BaseNotification()
	{
	}

	protected BaseNotification(Context context, T data)
	{
		Context = context;
		Data = data;
	}

	private NotificationManager GetManager()
	{
		return NotificationManager.FromContext(Context);
	}

	private string GetChannel()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			NotificationChannel channel = new NotificationChannel(GetChannelId(), GetChannelName(), GetChannelImportance())
			{
				Description = GetChannelDescription()
			};
			GetManager().CreateNotificationChannel(channel);
		}
		return GetChannelId();
	}

	private NotificationCompat.Builder GetBuilder()
	{
		NotificationCompat.Builder builder = new NotificationCompat.Builder(Context, GetChannel()).SetColor(ContextCompat.GetColor(Context, 2131034228)).SetColorized(colorize: false).SetContentTitle(GetNotificationTitle())
			.SetContentText(GetNotificationBody())
			.SetWhen(JavaSystem.CurrentTimeMillis());
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop && Build.Manufacturer!.ToUpper() != "SAMSUNG")
		{
			builder.SetSmallIcon(2131165323);
			builder.SetColor(ContextCompat.GetColor(Context, 2131034228));
		}
		else
		{
			builder.SetSmallIcon(2131165320);
		}
		return builder;
	}

	public Notification GenerateNotification()
	{
		return GetNotification(GetBuilder()).Build();
	}

	public void DisplayNotification()
	{
		GetManager().Notify(notification: GenerateNotification(), id: GetNotificationId());
	}

	public void Cancel()
	{
		GetManager().Cancel(GetNotificationId());
	}

	public void UpdateData(T data)
	{
		Data = data;
		DisplayNotification();
	}

	public void Process(Context context, object stanza)
	{
		Context = context;
		Data = (T)stanza;
		DisplayNotification();
	}
}
