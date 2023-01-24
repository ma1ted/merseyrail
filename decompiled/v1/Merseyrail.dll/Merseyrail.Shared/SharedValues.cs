using Android.Locations;
using Android.Support.V4.App;

namespace Merseyrail.Shared;

public static class SharedValues
{
	public static string CurrentDatabase { get; set; }

	public static string CurrentUpdateDatabase { get; set; }

	public static Fragment CurrentFragment { get; set; }

	public static Location CurrentDeviceLocation { get; set; }
}
