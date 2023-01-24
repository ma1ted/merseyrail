using System;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Common.Domain;
using Merseyrail.Shared;
using Merseyrail.Views;

namespace Merseyrail.Adapters;

internal class LiveTrainStationsListAdapter : BaseAdapter<TrainStop>
{
	private float distanceThreshold = 1000f;

	private TrainProgress items;

	private Activity context;

	private Android.Support.V4.App.FragmentManager FragmentManager;

	private string Line => items.Line;

	public override TrainStop this[int position] => items[position];

	public override int Count => items.Count;

	public LiveTrainStationsListAdapter(TrainProgress items, Activity context, Android.Support.V4.App.FragmentManager fragmentManager)
	{
		this.context = context;
		this.items = items;
		FragmentManager = fragmentManager;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	private LiveTrainIcon resetIcon(LiveTrainIcon icon)
	{
		icon.TrainIcon = false;
		icon.HorisBlue = false;
		icon.HorisDark = false;
		icon.HorisGreen = false;
		icon.HorisGrey = false;
		icon.HorisNone = false;
		icon.TopBlue = false;
		icon.TopDark = false;
		icon.TopGreen = false;
		icon.TopGrey = false;
		icon.BottomBlue = false;
		icon.BottomDark = false;
		icon.BottomGreen = false;
		icon.BottomGrey = false;
		icon.CircleBlue = false;
		icon.CircleDark = false;
		icon.CircleGreen = false;
		icon.CircleGrey = false;
		icon.UpdateView();
		return icon;
	}

	public bool IsTrainLate(TrainStop item)
	{
		DateTime now = DateTime.Now;
		return (new TimeSpan(now.Hour, now.Minute, now.Second) - item.ActualDeparture).TotalSeconds > 0.0;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		TrainStop trainStop = items[position];
		TimeSpan timeSpan = Utils.OverNightTime((trainStop.ActualDeparture.TotalSeconds == 0.0) ? trainStop.ActualArrival : trainStop.ActualDeparture);
		_ = timeSpan.Subtract(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)).TotalMinutes;
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361856, null);
		}
		view.FindViewById<TextView>(2131230987).Text = trainStop.Station.Name;
		LiveTrainIcon icon = view.FindViewById<LiveTrainIcon>(2131230986);
		icon = resetIcon(icon);
		if (timeSpan > Utils.OverNightTime())
		{
			icon.TrainIcon = trainStop.CurrentStop;
			switch (Line)
			{
			case "W":
				icon.TopGreen = true;
				icon.CircleGreen = true;
				icon.BottomGreen = true;
				break;
			case "N":
				icon.TopBlue = true;
				icon.CircleBlue = true;
				icon.BottomBlue = true;
				break;
			case "C":
				icon.TopDark = true;
				icon.CircleDark = true;
				icon.BottomDark = true;
				break;
			}
			switch (trainStop.Station.Lines.Replace(Line, ""))
			{
			case "W":
				icon.HorisGreen = true;
				break;
			case "N":
				icon.HorisBlue = true;
				break;
			case "C":
				icon.HorisDark = true;
				break;
			}
		}
		else
		{
			icon.TopGrey = true;
			icon.CircleGrey = true;
			icon.BottomGrey = true;
			switch (trainStop.Station.Lines.Replace(Line, ""))
			{
			case "W":
				icon.HorisGrey = true;
				break;
			case "N":
				icon.HorisGrey = true;
				break;
			case "C":
				icon.HorisGrey = true;
				break;
			}
		}
		TextView textView = view.FindViewById<TextView>(2131230985);
		textView.Visibility = ViewStates.Invisible;
		TextView textView2 = view.FindViewById<TextView>(2131230988);
		DateTime now = DateTime.Now;
		int minutes = trainStop.ScheduledDeparture.Subtract(new TimeSpan(now.Hour, now.Minute, now.Second)).Minutes;
		if (minutes > 0)
		{
			textView.FindViewById<TextView>(2131230985).Text = $"{minutes}min";
			textView.Visibility = ViewStates.Visible;
			textView2.Text = timeSpan.ToString("hh\\:mm");
			textView2.SetTextColor(Color.Argb(255, 0, 0, 0));
		}
		else
		{
			textView2.Text = "Missed";
			textView2.SetTextColor(Color.Argb(255, 192, 192, 192));
		}
		view.FindViewById<ImageView>(2131230989).Visibility = ViewStates.Invisible;
		LinearLayout linearLayout = view.FindViewById<LinearLayout>(2131230984);
		if (trainStop.RiverAfter)
		{
			linearLayout.SetBackgroundResource(2131165406);
		}
		else
		{
			linearLayout.SetBackgroundColor(Color.White);
		}
		icon.UpdateView();
		return view;
	}

	private static void CheckIfWorkHomeStation(TrainStop item, ImageView workhome)
	{
		switch (item.ItemType)
		{
		case DepartureBoardItemType.Home:
			workhome.SetImageResource(2131165315);
			workhome.Visibility = ViewStates.Visible;
			break;
		case DepartureBoardItemType.HomeAndWork:
			workhome.SetImageResource(2131165314);
			workhome.Visibility = ViewStates.Visible;
			break;
		case DepartureBoardItemType.Work:
			workhome.SetImageResource(2131165414);
			workhome.Visibility = ViewStates.Visible;
			break;
		case DepartureBoardItemType.None:
			workhome.Visibility = ViewStates.Invisible;
			break;
		}
	}

	private string GetDistanceToStation(TrainStop item)
	{
		_ = (Main)context;
		Location currentDeviceLocation = SharedValues.CurrentDeviceLocation;
		if (currentDeviceLocation != null)
		{
			Location location = new Location("");
			location.Latitude = item.Station.Lat;
			location.Longitude = item.Station.Lon;
			float num = currentDeviceLocation.DistanceTo(location);
			if (num < distanceThreshold)
			{
				return $"{Math.Round(num)}m";
			}
		}
		return "";
	}
}
