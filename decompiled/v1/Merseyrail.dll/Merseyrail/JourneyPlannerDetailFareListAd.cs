using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail;

internal class JourneyPlannerDetailFareListAdapter : BaseAdapter<JourneyFare>
{
	public List<JourneyFare> items;

	private Activity context;

	public string fareType { get; set; }

	public string farePrice { get; set; }

	public override JourneyFare this[int index] => items[index];

	public override int Count => items.Count;

	public JourneyPlannerDetailFareListAdapter(Journey journey, Activity context)
	{
		this.context = context;
		items = journey.Fares;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		JourneyFare journeyFare = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361845, null);
		}
		view.FindViewById<TextView>(2131230867).Text = journeyFare.Description;
		view.FindViewById<TextView>(2131230865).Text = journeyFare.Amount.ToString("C");
		view.FindViewById<TextView>(2131230866).Visibility = ((!(journeyFare.Saving > 0.0)) ? ViewStates.Gone : ViewStates.Visible);
		view.FindViewById<TextView>(2131230866).Text = string.Format("Save {0}", journeyFare.Saving.ToString("C"));
		return view;
	}
}
