using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Common.Services;
using Java.Lang;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class DateTimeDialogFragment : DialogFragment
{
	public delegate void DialogClosedEventHandler(object sender, DateTimeDialogClosedEventArgs args);

	private DateTime? selectedDateTime;

	private View _view { get; set; }

	private Button closeButton { get; set; }

	private DatePicker datePicker { get; set; }

	private TimePicker timePicker { get; set; }

	public DateTime? SelectedDateTime
	{
		get
		{
			return selectedDateTime;
		}
		set
		{
			selectedDateTime = value;
		}
	}

	private bool returnJourney { get; set; }

	public event DialogClosedEventHandler DialogClosed;

	private void UpdateControls()
	{
		if (selectedDateTime.HasValue)
		{
			datePicker.UpdateDate(selectedDateTime.Value.Year, selectedDateTime.Value.Month - 1, selectedDateTime.Value.Day);
			timePicker.CurrentHour = (Integer)(Java.Lang.Object)selectedDateTime.Value.Hour;
			timePicker.CurrentMinute = (Integer)(Java.Lang.Object)selectedDateTime.Value.Minute;
		}
	}

	public DateTimeDialogFragment()
	{
	}

	public DateTimeDialogFragment(DateTime? datetime, bool _returnJourney = false)
	{
		returnJourney = _returnJourney;
		SelectedDateTime = datetime;
	}

	public override void OnResume()
	{
		base.OnResume();
		UpdateControls();
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361838, null, attachToRoot: false);
			closeButton = (Button)_view.FindViewById(2131230878);
			closeButton.Click += delegate
			{
				Dialog.Cancel();
				FireDialogClosed();
			};
			datePicker = (DatePicker)_view.FindViewById(2131230879);
			timePicker = (TimePicker)_view.FindViewById(2131230880);
		}
		return _view;
	}

	private void FireDialogClosed()
	{
		SelectedDateTime = new DateTime(datePicker.Year, datePicker.Month + 1, datePicker.DayOfMonth, (int)timePicker.CurrentHour, (int)timePicker.CurrentMinute, 0);
		SharedServices.IoC.Resolve<JourneyPlannerService>().GetStationClosureNotifications(SelectedDateTime.Value, delegate(ClosureNotification closureNotification)
		{
			if (closureNotification.IsClosed)
			{
				((Main)base.ParentFragment.Activity).ShowAlertDialogue(closureNotification.Message, closureNotification.Title ?? "Important Information", closureNotification.ButtonText, closureNotification.ButtonUrl);
			}
			else
			{
				if (SelectedDateTime < DateTime.Now)
				{
					SelectedDateTime = DateTime.Now;
				}
				DateTimeDialogClosedEventArgs args = new DateTimeDialogClosedEventArgs
				{
					DialogDateTime = SelectedDateTime.Value
				};
				if (this.DialogClosed != null)
				{
					this.DialogClosed(this, args);
				}
			}
		});
	}
}
