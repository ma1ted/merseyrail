using Android.Net;

namespace Merseyrail.Notifications.Models;

public class CalamityFCMAlert : BaseAlert
{
	private string PackageName { get; set; }

	public string CalamityJson { get; set; }

	public string RouteCrs { get; set; }

	public CalamityFCMAlert(string packageName, string calamityJson, string routeCrs, string title, string message)
	{
		PackageName = packageName;
		CalamityJson = calamityJson;
		RouteCrs = routeCrs;
		base.Title = title;
		base.Message = message;
	}

	public override NotificationType GetNotificationType()
	{
		return NotificationType.Calamity;
	}

	public override Uri GetSound()
	{
		return Uri.Parse("android.resource://" + PackageName + "/" + 2131427328);
	}
}
