using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Widget;
using Java.Interop;

namespace Merseyrail.Fragments;

public class TimePickerFragment : Android.Support.V4.App.DialogFragment, TimePickerDialog.IOnTimeSetListener, IJavaObject, IDisposable, IJavaPeerable
{
	public int HourOfDay;

	public int Minute;

	public event EventHandler<TimePickerDialog.TimeSetEventArgs> OnTimeSetEvent;

	public override Dialog OnCreateDialog(Bundle savedInstanceState)
	{
		DateTime now = DateTime.Now;
		return new TimePickerDialog(base.Activity, this, now.Hour, now.Minute, is24HourView: false);
	}

	public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
	{
		HourOfDay = hourOfDay;
		Minute = minute;
		if (this.OnTimeSetEvent != null)
		{
			this.OnTimeSetEvent.BeginInvoke(this, new TimePickerDialog.TimeSetEventArgs(hourOfDay, minute), null, null);
		}
	}
}
