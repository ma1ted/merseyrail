using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail;

internal class JourneyPlannerDetailListAdapter : BaseAdapter<JourneyLeg>
{
	private List<JourneyLeg> items;

	private Activity context;

	public string fromStation { get; set; }

	public string toStation { get; set; }

	private Journey SelectedJourney { get; set; }

	public override JourneyLeg this[int position] => items[position];

	public override int Count => items.Count;

	public JourneyPlannerDetailListAdapter(Journey journey, Activity context)
	{
		this.context = context;
		items = journey.Legs;
		SelectedJourney = journey;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		JourneyLeg journeyLeg = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361849, null);
		}
		LinearLayout linearLayout = view.FindViewById<LinearLayout>(2131230943);
		_ = journeyLeg.TrainHeading;
		string name = journeyLeg.OriginStation.Name;
		string name2 = journeyLeg.DestinationStation.Name;
		string text = journeyLeg.DepartAct.ToShortTimeString();
		string text2 = journeyLeg.ArrivalAct.ToShortTimeString();
		_ = journeyLeg.DepartPlatform;
		_ = journeyLeg.ArrivePlatform;
		_ = journeyLeg.Operator;
		int num = items.Count - 1;
		TimeSpan timeSpan = journeyLeg.ArrivalAct.Subtract(journeyLeg.DepartAct);
		view.FindViewById<TextView>(2131230948).Text = name;
		view.FindViewById<TextView>(2131230952).Text = name2;
		view.FindViewById<TextView>(2131230949).Text = text;
		view.FindViewById<TextView>(2131230954).Text = text2;
		view.FindViewById<TextView>(2131230944).Text = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}";
		view.FindViewById<TextView>(2131230953).Text = num.ToString();
		TextView textView = view.FindViewById<TextView>(2131230945);
		if (SelectedJourney.HasProblem)
		{
			textView.Visibility = ViewStates.Visible;
			textView.Text = SelectedJourney.Status.Replace("_", " ").Replace("\"", "");
			linearLayout.SetBackgroundResource(2131034212);
		}
		else
		{
			if (journeyLeg.IsTrainMode)
			{
				textView.Text = $"{journeyLeg.Operator} service to {journeyLeg.TrainHeading}";
			}
			else
			{
				textView.Text = journeyLeg.TransportMode.Replace("_", "").Replace("\"", "");
			}
			linearLayout.SetBackgroundResource(2131034183);
		}
		return view;
	}
}
