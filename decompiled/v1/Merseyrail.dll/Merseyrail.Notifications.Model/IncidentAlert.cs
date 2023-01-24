using Android.Net;

namespace Merseyrail.Notifications.Models;

public class IncidentAlert : BaseAlert
{
	private string PackageName { get; set; }

	public string RouteCrs { get; set; }

	public IncidentAlert(string packageName, string routeCrs, string title, string message)
	{
		PackageName = packageName;
		RouteCrs = routeCrs;
		base.Title = title;
		base.Message = message;
	}

	public override NotificationType GetNotificationType()
	{
		return NotificationType.Incident;
	}

	public override Uri GetSound()
	{
		return Uri.Parse("android.resource://" + PackageName + "/" + 2131427328);
	}
}
