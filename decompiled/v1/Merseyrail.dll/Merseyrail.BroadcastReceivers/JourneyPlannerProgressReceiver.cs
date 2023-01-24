using System;
using Android.App;
using Android.Content;

namespace Merseyrail.BroadcastReceivers;

[BroadcastReceiver(Enabled = true)]
[IntentFilter(new string[] { "glow.merseyrail.JourneyPlannerProgressReceiver" })]
public class JourneyPlannerProgressReceiver : BroadcastReceiver
{
	public event EventHandler<bool> ShowOnProgressUpdate;

	public override void OnReceive(Context context, Intent intent)
	{
		bool booleanExtra = intent.GetBooleanExtra("departuresUpdating", defaultValue: false);
		bool booleanExtra2 = intent.GetBooleanExtra("returnUpdating", defaultValue: false);
		bool e = booleanExtra || booleanExtra2;
		if (this.ShowOnProgressUpdate != null)
		{
			this.ShowOnProgressUpdate(this, e);
		}
	}
}
