using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;

namespace Merseyrail.Helpers;

public static class PermissionHelper
{
	public static void GetLocationPermission(Activity activity, int requestLocationId)
	{
		if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, "android.permission.ACCESS_FINE_LOCATION"))
		{
			activity.RunOnUiThread(delegate
			{
				Toast.MakeText(activity, "Location access is required to show nearby stations.", ToastLength.Long)!.Show();
			});
		}
		else
		{
			ActivityCompat.RequestPermissions(activity, new string[2] { "android.permission.ACCESS_FINE_LOCATION", "android.permission.ACCESS_COARSE_LOCATION" }, requestLocationId);
		}
	}

	public static bool HasLocationPermission(Activity activity)
	{
		if (ContextCompat.CheckSelfPermission(activity, "android.permission.ACCESS_FINE_LOCATION") == Permission.Granted)
		{
			return ContextCompat.CheckSelfPermission(activity, "android.permission.ACCESS_COARSE_LOCATION") == Permission.Granted;
		}
		return false;
	}
}
