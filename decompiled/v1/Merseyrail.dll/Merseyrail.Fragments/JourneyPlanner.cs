using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Common.Domain;
using Common.Services;
using Merseyrail.BroadcastReceivers;
using Merseyrail.Services;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

[IntentFilter(new string[] { "android.intent.action.MAIN" }, Categories = new string[] { "SectionFragment" })]
public class JourneyPlanner : BaseFragment
{
	private DateTime date;

	private bool _gettingJourneys;

	private const int DATE_DIALOG_ID = 0;

	private bool eventsBound;

	private Button text_from { get; set; }

	private string crscode_from { get; set; }

	private Button text_to { get; set; }

	private string crscode_to { get; set; }

	private LinearLayout journeyPlanner_fragment_loading_panel { get; set; }

	private RadioButton journeyplanner_date_radio_departing { get; set; }

	private RadioButton journeyplanner_date_radio_arriving { get; set; }

	private RadioButton journeyplanner_date_return_radio_departing { get; set; }

	private RadioButton journeyplanner_date_return_radio_arriving { get; set; }

	private JourneyPlannerProgressReceiver journeyPlannerProgressReceiver { get; set; }

	private RadioButton journeyplanner_date_radio { get; set; }

	private bool journeyplanner_date_isdeparture { get; set; }

	private RadioButton journeyplanner_date_return_radio { get; set; }

	private bool journeyplanner_date_return_isdeparture { get; set; }

	private Button text_time { get; set; }

	private Button text_time_return { get; set; }

	private Button btnFindTrains { get; set; }

	private View _view { get; set; }

	private JourneyPlannerService journeyPlannerService { get; set; }

	private DateTimeDialogFragment DateTimePicker { get; set; }

	private DateTimeDialogFragment DateTimePickerReturn { get; set; }

	private DateTime? FromDateTime { get; set; }

	private DateTime? ReturnDateTime { get; set; }

	public Station SelectedFromStation { get; set; }

	public Station SelectedToStation { get; set; }

	private string dateOrReturn { get; set; }

	public JourneyPlanner()
	{
		eventsBound = false;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		date = DateTime.Now;
		if (DateTimePicker == null)
		{
			DateTimePicker = new DateTimeDialogFragment();
			DateTimePicker.DialogClosed += delegate(object sender, DateTimeDialogClosedEventArgs e)
			{
				UpdateDepartureDateTime(sender, e);
			};
		}
	}

	public override void OnViewCreated(View view, Bundle savedInstanceState)
	{
		base.OnViewCreated(view, savedInstanceState);
		CheckForNetworkClosures();
	}

	private void CheckForNetworkClosures()
	{
		SharedServices.IoC.Resolve<JourneyPlannerService>().GetStationClosureNotifications(DateTime.Now, delegate(ClosureNotification closureNotification)
		{
			if (closureNotification.IsClosed)
			{
				base.Activity.RunOnUiThread(delegate
				{
					ShowClosureAlert(closureNotification.Message, closureNotification.Title ?? "Important Information", closureNotification.ButtonText, closureNotification.ButtonUrl);
				});
			}
		});
	}

	private void ShowClosureAlert(string message, string title, string buttonText, string buttonUrl)
	{
		AlertDialog.Builder builder = new AlertDialog.Builder(base.Activity);
		builder.SetTitle(title);
		builder.SetMessage(message);
		builder.SetCancelable(cancelable: false);
		builder.SetPositiveButton(buttonText ?? "OK", delegate
		{
			if (buttonUrl != null)
			{
				Android.Net.Uri uri = Android.Net.Uri.Parse(buttonUrl);
				Intent intent = new Intent("android.intent.action.VIEW", uri);
				StartActivity(intent);
			}
		});
		if (!string.IsNullOrEmpty(buttonUrl))
		{
			builder.SetNegativeButton("OK", delegate
			{
			});
		}
		builder.Show();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361843, null, attachToRoot: false);
		}
		text_from = (Button)_view.FindViewById(2131230926);
		journeyPlanner_fragment_loading_panel = _view.FindViewById<LinearLayout>(2131230917);
		journeyPlanner_fragment_loading_panel.Visibility = ViewStates.Gone;
		journeyplanner_date_radio_departing = (RadioButton)_view.FindViewById(2131230921);
		journeyplanner_date_radio_arriving = (RadioButton)_view.FindViewById(2131230920);
		journeyplanner_date_return_radio_departing = (RadioButton)_view.FindViewById(2131230925);
		journeyplanner_date_return_radio_arriving = (RadioButton)_view.FindViewById(2131230924);
		ImageButton imageButton = (ImageButton)_view.FindViewById(2131230801);
		text_to = (Button)_view.FindViewById(2131230927);
		text_time = (Button)_view.FindViewById(2131230918);
		text_time_return = (Button)_view.FindViewById(2131230922);
		btnFindTrains = (Button)_view.FindViewById(2131230916);
		if (!eventsBound)
		{
			imageButton.Click += delegate
			{
				((Main)base.Activity).ToggleDrawerLayout();
			};
			text_from.Click += delegate
			{
				Intent intent2 = new Intent(base.Activity, typeof(StationSelectorActivity));
				base.Activity.StartActivityForResult(intent2, 311);
			};
			text_to.Click += delegate
			{
				Intent intent = new Intent(base.Activity, typeof(StationSelectorActivity));
				base.Activity.StartActivityForResult(intent, 322);
			};
			text_time.Click += delegate
			{
				dateOrReturn = "d";
				Android.Support.V4.App.FragmentTransaction transaction2 = base.ChildFragmentManager.BeginTransaction();
				DateTimePicker = ((base.ChildFragmentManager.FindFragmentByTag("dtp") != null) ? ((DateTimeDialogFragment)base.ChildFragmentManager.FindFragmentByTag("dtp")) : new DateTimeDialogFragment(FromDateTime));
				DateTimePicker.DialogClosed += delegate(object s, DateTimeDialogClosedEventArgs args)
				{
					base.Activity.RunOnUiThread(delegate
					{
						UpdateDepartureDateTime(s, args);
					});
				};
				DateTimePicker.Show(transaction2, "dialog");
			};
			text_time_return.Click += delegate
			{
				dateOrReturn = "r";
				Android.Support.V4.App.FragmentTransaction transaction = base.ChildFragmentManager.BeginTransaction();
				DateTimePickerReturn = ((base.ChildFragmentManager.FindFragmentByTag("dtpr") != null) ? ((DateTimeDialogFragment)base.ChildFragmentManager.FindFragmentByTag("dtpr")) : new DateTimeDialogFragment(ReturnDateTime, _returnJourney: true));
				DateTimePickerReturn.DialogClosed += delegate(object s, DateTimeDialogClosedEventArgs args)
				{
					base.Activity.RunOnUiThread(delegate
					{
						UpdateReturnDateTime(s, args);
					});
				};
				DateTimePickerReturn.Show(transaction, "dialogreturn");
			};
			Exception ex;
			btnFindTrains.Click += delegate
			{
				if (ValidDestinationSearch() && ValidReturnSearch())
				{
					JourneyPlannerList fragment = null;
					bool @checked = journeyplanner_date_radio_departing.Checked;
					bool return_departing = journeyplanner_date_return_radio_departing.Checked;
					journeyPlannerService = SharedServices.IoC.Resolve<JourneyPlannerService>();
					_gettingJourneys = true;
					UpdateProgressView(_gettingJourneys);
					try
					{
						journeyPlannerService.GetJourneyPlanner(crscode_from, crscode_to, FromDateTime.Value, @checked, delegate(JourneyPlan outPlan)
						{
							if (outPlan.IncludesSwitchOff)
							{
								_gettingJourneys = false;
								base.Activity.RunOnUiThread(delegate
								{
									UpdateProgressView(_gettingJourneys);
									ShowClosureAlert(outPlan.SwitchOffMessage, "Line Closure", null, null);
								});
							}
							else
							{
								journeyPlannerService.GetJourneyPlanner(crscode_to, crscode_from, ReturnDateTime.Value, return_departing, delegate(JourneyPlan returnPlan)
								{
									_gettingJourneys = false;
									if (returnPlan.IncludesSwitchOff)
									{
										base.Activity.RunOnUiThread(delegate
										{
											UpdateProgressView(_gettingJourneys);
											ShowClosureAlert(returnPlan.SwitchOffMessage, "Line Closure", null, null);
										});
									}
									else
									{
										FragmentActivity activity = base.Activity;
										fragment = SharedServices.IoC.Resolve<FragmentService>().InitSection<JourneyPlannerList>("journeyplannerlist");
										activity.RunOnUiThread(delegate
										{
											fragment.UpdateLists(outPlan, FromDateTime.Value, returnPlan, ReturnDateTime.Value);
										});
									}
								});
							}
						});
						return;
					}
					catch (Exception ex2)
					{
						ex = ex2;
						_gettingJourneys = false;
						base.Activity.RunOnUiThread(delegate
						{
							UpdateProgressView(_gettingJourneys);
							Toast.MakeText(base.Activity, "Error accessing planner (" + ex.Message + ")", ToastLength.Short)!.Show();
						});
						return;
					}
				}
				base.Activity.RunOnUiThread(delegate
				{
					if (crscode_from == crscode_to)
					{
						Toast.MakeText(base.Activity, "Your departure and destination station must be different", ToastLength.Short)!.Show();
					}
					else
					{
						Toast.MakeText(base.Activity, "Please select your search stations and times", ToastLength.Short)!.Show();
					}
				});
			};
			eventsBound = true;
		}
		InitForm();
		return _view;
	}

	private void UpdateProgress()
	{
		Intent intent = new Intent("glow.merseyrail.JourneyPlannerProgressReceiver");
		intent.PutExtra("departuresUpdating", _gettingJourneys);
		intent.PutExtra("returnUpdating", _gettingJourneys);
		Application.Context.SendBroadcast(intent);
	}

	public override void OnResume()
	{
		base.OnResume();
		((Main)base.Activity).CurrentFragment = this;
	}

	public override void OnPause()
	{
		base.OnPause();
	}

	public override void OnStart()
	{
		base.OnStart();
		UpdateProgressView(show: false);
	}

	private bool ValidDestinationSearch()
	{
		if (FromDateTime.HasValue && crscode_from != null && crscode_to != null)
		{
			return crscode_from != crscode_to;
		}
		return false;
	}

	private bool ValidReturnSearch()
	{
		if (ReturnDateTime.HasValue && crscode_from != null && crscode_to != null && ReturnDateTime > FromDateTime)
		{
			return crscode_from != crscode_to;
		}
		return false;
	}

	private void InitForm()
	{
		FromDateTime = DateTime.Now;
		text_time.Text = string.Format("{1} on {0}", FromDateTime.Value.ToLongDateString(), FromDateTime.Value.ToShortTimeString());
		ReturnDateTime = DateTime.Now.AddHours(3.0);
		text_time_return.Text = string.Format("{1} on {0}", ReturnDateTime.Value.ToLongDateString(), ReturnDateTime.Value.ToShortTimeString());
	}

	private void UpdateDepartureDateTime(object sender, DateTimeDialogClosedEventArgs e)
	{
		base.Activity.RunOnUiThread(delegate
		{
			FromDateTime = e.DialogDateTime;
			text_time.Text = string.Format("{1} on {0}", FromDateTime.Value.ToLongDateString(), FromDateTime.Value.ToShortTimeString());
			if (FromDateTime > ReturnDateTime)
			{
				ReturnDateTime = FromDateTime.Value.AddHours(1.0);
				text_time_return.Text = string.Format("{1} on {0}", ReturnDateTime.Value.ToLongDateString(), ReturnDateTime.Value.ToShortTimeString());
			}
		});
	}

	private void UpdateReturnDateTime(object sender, DateTimeDialogClosedEventArgs e)
	{
		base.Activity.RunOnUiThread(delegate
		{
			DateTime dialogDateTime = e.DialogDateTime;
			DateTime? fromDateTime = FromDateTime;
			if (dialogDateTime > fromDateTime)
			{
				ReturnDateTime = e.DialogDateTime;
				text_time_return.Text = string.Format("{1} on {0}", ReturnDateTime.Value.ToLongDateString(), ReturnDateTime.Value.ToShortTimeString());
			}
			else
			{
				ReturnDateTime = FromDateTime.Value.AddHours(1.0);
				text_time_return.Text = string.Format("{1} on {0}", ReturnDateTime.Value.ToLongDateString(), ReturnDateTime.Value.ToShortTimeString());
			}
		});
	}

	public void SetSelectedFromStation(string stationName)
	{
		Station station = (from x in SharedServices.IoC.Resolve<StationService>().GetStations(merseyrailOnly: true)
			where x.Name == stationName
			select x).Single();
		if (station != null)
		{
			SelectedFromStation = station;
			text_from.Text = station.Name;
			crscode_from = station.CrsCode;
		}
	}

	public void SetSelectedToStation(string stationName)
	{
		Station station = (from x in SharedServices.IoC.Resolve<StationService>().GetStations(merseyrailOnly: true)
			where x.Name == stationName
			select x).Single();
		if (station != null)
		{
			SelectedToStation = station;
			text_to.Text = station.Name;
			crscode_to = station.CrsCode;
		}
	}

	public override void OnStop()
	{
		UpdateProgressView(show: false);
		base.OnStop();
	}

	private void UpdateProgressView(object sender, bool e)
	{
		UpdateProgressView(e);
	}

	private void UpdateProgressView(bool show)
	{
		base.Activity.RunOnUiThread(delegate
		{
			journeyPlanner_fragment_loading_panel.Visibility = ((!show) ? ViewStates.Gone : ViewStates.Visible);
		});
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}
}
