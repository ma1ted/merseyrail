using System;
using Android.App;
using Android.Content;

namespace Merseyrail.BroadcastReceivers;

[BroadcastReceiver]
[IntentFilter(new string[] { "glow.merseyrail.WidgetStationSelected" })]
public class WidgetStationSelectedReceiver : BroadcastReceiver
{
	public event EventHandler<WidgetStationSelectedEventArgs> OnWidgetStationSelected;

	public override void OnReceive(Context context, Intent intent)
	{
		WidgetStationSelectedEventArgs widgetStationSelectedEventArgs = new WidgetStationSelectedEventArgs();
		widgetStationSelectedEventArgs.TrainId = intent.GetStringExtra("glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID");
		if (this.OnWidgetStationSelected != null)
		{
			this.OnWidgetStationSelected(this, widgetStationSelectedEventArgs);
		}
	}
}
