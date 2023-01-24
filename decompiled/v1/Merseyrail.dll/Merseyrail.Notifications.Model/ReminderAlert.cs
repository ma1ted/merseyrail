using Android.Net;

namespace Merseyrail.Notifications.Models;

public class ReminderAlert : BaseAlert
{
	private string PackageName { get; set; }

	public ReminderAlert(string packageName, string title, string message)
	{
		PackageName = packageName;
		base.Title = title;
		base.Message = message;
	}

	public override NotificationType GetNotificationType()
	{
		return NotificationType.Reminder;
	}

	public override Uri GetSound()
	{
		return Uri.Parse("android.resource://" + PackageName + "/" + 2131427329);
	}
}
