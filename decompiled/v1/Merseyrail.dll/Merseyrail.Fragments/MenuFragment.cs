using Android.OS;
using Android.Views;
using Android.Widget;
using Common;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class MenuFragment : BaseFragment
{
	public LinearLayout BtnActiveReminder { get; set; }

	public MenuItem BtnDepartures { get; set; }

	public MenuItem BtnLiveMap { get; set; }

	public MenuItem BtnJourneyPlanner { get; set; }

	public MenuItem BtnFeedback { get; set; }

	public MenuItem BtnSettings { get; set; }

	public MenuItem BtnPlatformUpgrades { get; set; }

	public MenuItem BtnMtogo { get; set; }

	public MenuItem BtnNewTrains { get; set; }

	public MenuItem BtnDeveloperArea { get; set; }

	private View _view { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		_view = inflater.Inflate(2131361858, null, attachToRoot: false);
		BtnActiveReminder = (LinearLayout)_view.FindViewById(2131231019);
		BtnDepartures = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231021);
		BtnLiveMap = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231022);
		BtnJourneyPlanner = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231023);
		BtnFeedback = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231024);
		BtnSettings = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231025);
		BtnPlatformUpgrades = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231026);
		BtnPlatformUpgrades.MenuItemView.Visibility = ((!Utils.DisruptionActive()) ? ViewStates.Gone : ViewStates.Visible);
		BtnMtogo = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231027);
		BtnNewTrains = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231028);
		BtnDeveloperArea = (MenuItem)base.ChildFragmentManager.FindFragmentById(2131231029);
		BtnNewTrains.MenuItemView.Visibility = ViewStates.Gone;
		if (BtnDeveloperArea != null && BtnDeveloperArea.MenuItemView != null)
		{
			BtnDeveloperArea.MenuItemView.Visibility = ViewStates.Gone;
		}
		return _view;
	}

	public override void OnResume()
	{
		bool flag = SharedSettings.HasConnection(base.Activity);
		if (BtnJourneyPlanner != null)
		{
			BtnJourneyPlanner.View.Visibility = ((!flag) ? ViewStates.Gone : ViewStates.Visible);
		}
		_view.Invalidate();
		base.OnResume();
	}
}
