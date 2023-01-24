using System;

namespace Merseyrail.Fragments;

public class DateTimeDialogClosedEventArgs : EventArgs
{
	public DateTime DialogDateTime { get; set; }
}
