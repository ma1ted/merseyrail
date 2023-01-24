using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Common;
using Common.Domain;

namespace Merseyrail.Adapters;

internal class TrainJourneyListAdapter : BaseAdapter<DepartureBoardItem>
{
	public List<DepartureBoardItem> items;

	private Activity context;

	public override DepartureBoardItem this[int position] => items[position];

	public override int Count => items.Count;

	public TrainJourneyListAdapter(List<DepartureBoardItem> items, Activity context)
	{
		this.context = context;
		this.items = items;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		DepartureBoardItem departureBoardItem = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361873, null);
		}
		TimeSpan timeSpan = Utils.OverNightTime((departureBoardItem.DepartureActual.TotalSeconds == 0.0) ? departureBoardItem.ArrivalActual : departureBoardItem.DepartureActual);
		DateTime now = DateTime.Now;
		double totalMinutes = timeSpan.Subtract(new TimeSpan(now.Hour, now.Minute, now.Second)).TotalMinutes;
		view.FindViewById<TextView>(2131231219).Text = departureBoardItem.DestinationStation.Name;
		view.FindViewById<TextView>(2131231220).Text = "Platform" + departureBoardItem.Platform;
		TextView textView = view.FindViewById<TextView>(2131231217);
		TextView textView2 = view.FindViewById<TextView>(2131231230);
		if (departureBoardItem.HasProblem)
		{
			textView2.Text = "";
			textView2.SetTextColor(Color.Argb(255, 192, 0, 0));
			textView.Text = departureBoardItem.Status;
			textView.SetTextColor(Color.Argb(255, 192, 0, 0));
		}
		else
		{
			if (totalMinutes > 0.0)
			{
				departureBoardItem.DepartureActual.Subtract(departureBoardItem.DepartureSchedule);
				textView2.Text = $"{(int)totalMinutes}min";
				textView2.SetTextColor(Color.Argb(255, 0, 0, 0));
				textView.SetTextColor(Color.Argb(255, 0, 0, 0));
			}
			else
			{
				textView2.Text = "Missed";
				textView2.SetTextColor(Color.Argb(255, 192, 192, 192));
				textView.SetTextColor(Color.Argb(255, 192, 192, 192));
			}
			textView.Text = $"{departureBoardItem.DepartureActual.Hours:00}:{departureBoardItem.DepartureActual.Minutes:00}";
		}
		view.FindViewById<ImageView>(2131231229).SetImageResource(2131165271);
		return view;
	}
}
