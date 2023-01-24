using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Common;
using Common.Services;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail.Helpers;

public static class CalamityHelpers
{
	public static void CheckForCalamity(Activity activity)
	{
		SharedServices.IoC.Resolve<CalamityService>().GetActiveAlert().ContinueWith(delegate(Task<CalamityAlert> task)
		{
			CalamityAlert calamity = task.Result;
			if (calamity != null)
			{
				activity.RunOnUiThread(delegate
				{
					ShowCalamityAlert(calamity, activity);
				});
			}
		});
	}

	private static void ShowCalamityAlert(CalamityAlert calamity, Activity activity)
	{
		string value = JsonConvert.SerializeObject(calamity);
		Intent intent = new Intent(activity, typeof(CalamityAlertActivity));
		intent.PutExtra("calamityJson", value);
		activity.StartActivity(intent);
	}
}
