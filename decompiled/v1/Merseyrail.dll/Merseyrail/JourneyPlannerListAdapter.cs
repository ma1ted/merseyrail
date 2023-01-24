using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail;

internal class JourneyPlannerListAdapter : BaseAdapter<Journey>
{
	public List<Journey> items;

	private Activity context;

	public string fromStation { get; set; }

	public string toStation { get; set; }

	private JourneyPlan JourneyPlan { get; set; }

	public override Journey this[int position] => items[position];

	public override int Count => items.Count;

	public JourneyPlannerListAdapter(JourneyPlan plan, Activity context)
	{
		this.context = context;
		items = plan.Journeys;
		JourneyPlan = plan;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		Journey journey = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361850, null);
		}
		LinearLayout linearLayout = view.FindViewById<LinearLayout>(2131230943);
		string name = journey.OriginStation.Name;
		string name2 = journey.DestinationStation.Name;
		DateTime departSch = journey.Legs.First().DepartSch;
		DateTime arrivalSch = journey.Legs.Last().ArrivalSch;
		int num = journey.Legs.Count - 1;
		TimeSpan timeSpan = arrivalSch.Subtract(departSch);
		double cheapestFare = journey.CheapestFare;
		view.FindViewById<TextView>(2131230948).Text = name;
		view.FindViewById<TextView>(2131230952).Text = name2;
		view.FindViewById<TextView>(2131230949).Text = departSch.ToShortTimeString();
		view.FindViewById<TextView>(2131230954).Text = arrivalSch.ToShortTimeString();
		view.FindViewById<TextView>(2131230953).Text = num.ToString();
		view.FindViewById<TextView>(2131230944).Text = timeSpan.ToString();
		view.FindViewById<TextView>(2131230950).Text = cheapestFare.ToString("C");
		TextView textView = view.FindViewById<TextView>(2131230945);
		if (journey.HasProblem)
		{
			textView.Visibility = ViewStates.Visible;
			textView.Text = journey.Status.Replace("_", " ").Replace("\"", "");
			linearLayout.SetBackgroundResource(2131034212);
		}
		else
		{
			textView.Visibility = ViewStates.Invisible;
			textView.Text = string.Empty;
			linearLayout.SetBackgroundResource(2131034183);
		}
		return view;
	}
}
