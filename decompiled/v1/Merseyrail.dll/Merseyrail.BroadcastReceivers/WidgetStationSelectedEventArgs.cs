using System;

namespace Merseyrail.BroadcastReceivers;

public class WidgetStationSelectedEventArgs : EventArgs
{
	public string StationName { get; set; }

	public string TrainId { get; set; }
}
