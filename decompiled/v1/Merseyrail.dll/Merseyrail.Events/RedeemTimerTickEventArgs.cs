using System;

namespace Merseyrail.Events;

public class RedeemTimerTickEventArgs : EventArgs
{
	public long MillisUntilFinished { get; set; }
}
