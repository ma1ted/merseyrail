using Android.Net;

namespace Merseyrail.Notifications.Models;

public abstract class BaseAlert
{
	public string Title { get; set; }

	public string Message { get; set; }

	public abstract NotificationType GetNotificationType();

	public abstract Uri GetSound();
}
