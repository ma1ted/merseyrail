using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail.Fragments;

[IntentFilter(new string[] { "android.intent.action.MAIN" }, Categories = new string[] { "SectionFragment" })]
public class JourneyPlannerDetail : BaseFragment
{
	private bool eventsBound;

	public Journey SelectedJourney { get; set; }

	private View _view { get; set; }

	private ListView journeyLegList { get; set; }

	private ListView journeyFareList { get; set; }

	public JourneyPlannerDetail()
	{
		eventsBound = false;
		PreviousFragment = typeof(JourneyPlanner);
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override void OnResume()
	{
		base.OnResume();
		((Main)base.Activity).CurrentFragment = this;
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstance)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361844, null, attachToRoot: false);
		}
		journeyLegList = (ListView)_view.FindViewById(2131230928);
		journeyFareList = (ListView)_view.FindViewById(2131230938);
		ImageButton imageButton = (ImageButton)_view.FindViewById(2131231080);
		if (!eventsBound)
		{
			imageButton.Click += delegate
			{
				((Main)base.Activity).OnBackPressed();
			};
			eventsBound = true;
		}
		UpdateView();
		return _view;
	}

	public void UpdateView()
	{
		if (SelectedJourney != null)
		{
			_ = SelectedJourney.Legs[0].DestinationStation;
			JourneyPlannerDetailListAdapter adapter = new JourneyPlannerDetailListAdapter(SelectedJourney, base.Activity);
			journeyLegList.Adapter = adapter;
			JourneyPlannerDetailFareListAdapter adapter2 = new JourneyPlannerDetailFareListAdapter(SelectedJourney, base.Activity);
			journeyFareList.Adapter = adapter2;
		}
	}

	private string GetVia(Journey journey)
	{
		string text = "";
		bool flag = true;
		if (SelectedJourney.Legs.Count > 1)
		{
			for (int i = 0; i < SelectedJourney.Legs.Count - 1; i++)
			{
				text = text + (flag ? "" : ", ") + SelectedJourney.Legs[i].DestinationStation.Name;
				flag = false;
			}
			if (text.Contains(','))
			{
				text.Remove(text.Length - 2, 2);
				text += ".";
			}
		}
		return text;
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}
}
