using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.BroadcastReceivers;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class JourneyPlannerList : PagedFragmentBase
{
	private JourneyPlan outwardPlan;

	private JourneyPlan returnPlan;

	private JourneyPlannerProgressReceiver journeyPlannerProgressReceiver;

	private bool UpdatingDepartures;

	private bool UpdatingReturn;

	private View _view { get; set; }

	private string fromStation { get; set; }

	private string toStation { get; set; }

	private ListView DeparturesList { get; set; }

	private ListView ReturnsList { get; set; }

	private LinearLayout journeyPlannerList_fragment_loading_panel { get; set; }

	private ScrollView Scroller { get; set; }

	private JourneyPlannerListAdapter departureJourneyPlannerListAdapter { get; set; }

	private JourneyPlannerListAdapter returnJourneyPlannerListAdapter { get; set; }

	public JourneyPlannerList()
	{
		PreviousFragment = typeof(JourneyPlanner);
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		journeyPlannerProgressReceiver = new JourneyPlannerProgressReceiver();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup viewGroup, Bundle savedInstance)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361847, null, attachToRoot: false);
		}
		_view.FindViewById<Button>(2131230955).Click += delegate
		{
			if (outwardPlan.Journeys.Any())
			{
				LoadAnotherPage(outward: true, departingTime: false, outwardPlan.Journeys.First().Legs.Last().ArrivalAct);
			}
		};
		_view.FindViewById<Button>(2131230956).Click += delegate
		{
			if (outwardPlan.Journeys.Any())
			{
				LoadAnotherPage(outward: true, departingTime: true, outwardPlan.Journeys.Last().Legs.First().DepartAct);
			}
		};
		_view.FindViewById<Button>(2131230957).Click += delegate
		{
			if (outwardPlan.Journeys.Any())
			{
				LoadAnotherPage(outward: false, departingTime: false, returnPlan.Journeys.First().Legs.Last().ArrivalAct);
			}
		};
		_view.FindViewById<Button>(2131230958).Click += delegate
		{
			if (outwardPlan.Journeys.Any())
			{
				LoadAnotherPage(outward: false, departingTime: true, returnPlan.Journeys.Last().Legs.First().DepartAct);
			}
		};
		((ImageButton)_view.FindViewById(2131230940)).Click += delegate
		{
			try
			{
				base.Activity.OnBackPressed();
			}
			catch (NullReferenceException)
			{
			}
		};
		journeyPlannerList_fragment_loading_panel = _view.FindViewById<LinearLayout>(2131230915);
		DeparturesList = (ListView)_view.FindViewById(2131230939);
		DeparturesList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
		{
			Journey journey2 = ((JourneyPlannerListAdapter)((ListView)sender).Adapter).items[e.Position];
			((Main)base.Activity).ShowJourneyPlannerDetail(journey2);
		};
		ReturnsList = (ListView)_view.FindViewById(2131230941);
		ReturnsList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
		{
			Journey journey = ((JourneyPlannerListAdapter)((ListView)sender).Adapter).items[e.Position];
			((Main)base.Activity).ShowJourneyPlannerDetail(journey);
		};
		return _view;
	}

	private void LoadReturnJourney(string originCrs, string destinationCrs, DateTime time, bool departing)
	{
		UpdatingReturn = true;
		UpdateProgressView(UpdatingReturn);
		UpdateReturn(originCrs, destinationCrs, departing, time);
	}

	private void LoadOutwardJourney(string originCrs, string destinationCrs, DateTime time, bool departing)
	{
		UpdatingDepartures = true;
		UpdateProgressView(UpdatingDepartures);
		UpdateDepartures(originCrs, destinationCrs, departing, time);
	}

	private void LoadAnotherPage(bool outward, bool departingTime, DateTime time)
	{
		if (outward)
		{
			LoadOutwardJourney(fromStation, toStation, time, departingTime);
		}
		else
		{
			LoadReturnJourney(toStation, fromStation, time, departingTime);
		}
	}

	public override void OnStart()
	{
		base.OnStart();
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
			journeyPlannerList_fragment_loading_panel.Visibility = ((!show) ? ViewStates.Gone : ViewStates.Visible);
		});
	}

	public void UpdateDepartures(string fromCrs, string toCrs, bool isDeparture, DateTime time)
	{
		fromStation = fromCrs;
		toStation = toCrs;
		UpdatingDepartures = true;
		UpdateProgressView(UpdatingDepartures);
		SharedServices.IoC.Resolve<JourneyPlannerService>().GetJourneyPlanner(fromCrs, toCrs, time, isDeparture, delegate(JourneyPlan plan)
		{
			if (!plan.IncludesSwitchOff)
			{
				PopulateDepartures(plan, time);
			}
			else
			{
				UpdatingDepartures = false;
				UpdateProgressView(UpdatingDepartures);
				ShowAlert("Line Closure", plan.SwitchOffMessage, pushBack: true);
			}
		});
	}

	public void UpdateLists(JourneyPlan plan, DateTime time, JourneyPlan returnPlan, DateTime returnTime)
	{
		fromStation = plan.OriginCRS;
		toStation = plan.DestinationCRS;
		PopulateDepartures(plan, time);
		UpdateReturn(returnPlan, returnTime);
	}

	public void UpdateReturn(JourneyPlan plan, DateTime time)
	{
		PopulateReturn(plan, time);
		GetCMSContent();
	}

	public void UpdateReturn(string fromCrs, string toCrs, bool isDeparture, DateTime time)
	{
		SharedServices.IoC.Resolve<JourneyPlannerService>().GetJourneyPlanner(fromCrs, toCrs, time, isDeparture, delegate(JourneyPlan plan)
		{
			if (!plan.IncludesSwitchOff)
			{
				PopulateReturn(plan, time);
			}
			else
			{
				UpdatingReturn = false;
				UpdateProgressView(UpdatingReturn);
				ShowAlert("Line Closure", plan.SwitchOffMessage, pushBack: true);
			}
		});
	}

	public void GetCMSContent()
	{
		SharedServices.IoC.Resolve<JourneyPlannerService>().GetJourneyPlannerContent(delegate(JourneyPlannerContent content)
		{
			if (content != null && !string.IsNullOrEmpty(content.CmsContent))
			{
				base.Activity.RunOnUiThread(delegate
				{
					ShowAlert(content.Title, content.CmsContent);
				});
			}
		});
	}

	private void PopulateDepartures(JourneyPlan journeyPlan, DateTime time)
	{
		outwardPlan = journeyPlan;
		if (journeyPlan.Journeys.Any() && journeyPlan.Journeys.First().Legs.Any() && journeyPlan.Journeys.First().Legs.First().DepartAct.Date > time.Date)
		{
			base.Activity.RunOnUiThread(delegate
			{
				ShowAlert("No journeys on that day", "There are no journeys on the day you selected, we have returned the next available journey which may be on the following day");
			});
		}
		UpdatingDepartures = false;
		departureJourneyPlannerListAdapter = new JourneyPlannerListAdapter(journeyPlan, base.Activity);
		base.Activity.RunOnUiThread(delegate
		{
			DeparturesList.Adapter = departureJourneyPlannerListAdapter;
			DeparturesList.Invalidate();
			DeparturesList.RequestLayout();
		});
		UpdateProgressView(UpdatingDepartures);
	}

	private void PopulateReturn(JourneyPlan journeyPlan, DateTime time)
	{
		returnPlan = journeyPlan;
		if (journeyPlan.Journeys.Any() && journeyPlan.Journeys.First().Legs.Any() && journeyPlan.Journeys.First().Legs.First().DepartAct.Date > time.Date)
		{
			base.Activity.RunOnUiThread(delegate
			{
				ShowAlert("No journeys on that day", "There are no journeys on the day you selected, we have returned the next available journey which may be on the following day");
			});
		}
		UpdatingReturn = false;
		returnJourneyPlannerListAdapter = new JourneyPlannerListAdapter(journeyPlan, base.Activity);
		base.Activity.RunOnUiThread(delegate
		{
			ReturnsList.Adapter = returnJourneyPlannerListAdapter;
			ReturnsList.Invalidate();
			ReturnsList.RequestLayout();
		});
		UpdateProgressView(UpdatingReturn);
	}

	private void ShowAlert(string title, string message, bool pushBack = false)
	{
		AlertDialog.Builder alert = new AlertDialog.Builder(base.Activity);
		alert.SetTitle(title);
		alert.SetMessage(message);
		alert.SetPositiveButton("Ok", delegate
		{
			if (pushBack)
			{
				try
				{
					base.Activity.RunOnUiThread(delegate
					{
						base.Activity.OnBackPressed();
					});
				}
				catch (NullReferenceException)
				{
				}
			}
		});
		base.Activity.RunOnUiThread(delegate
		{
			alert.Show();
		});
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}
}
