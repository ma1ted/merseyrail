using System;
using Android.App;
using Android.Content;
using Android.Locations;

namespace Merseyrail.BroadcastReceivers;

[BroadcastReceiver]
[IntentFilter(new string[] { "glow.merseyrail.LocationUpdated" })]
public class LocationUpdatedReceiver : BroadcastReceiver
{
	public event EventHandler<Location> OnLocationUpdated;

	public override void OnReceive(Context context, Intent intent)
	{
		Location e = (Location)intent.GetParcelableExtra("location");
		if (this.OnLocationUpdated != null)
		{
			this.OnLocationUpdated(this, e);
		}
	}
}
