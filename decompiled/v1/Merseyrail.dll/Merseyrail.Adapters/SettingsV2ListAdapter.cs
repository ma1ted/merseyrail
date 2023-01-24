using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Shared;

namespace Merseyrail.Adapters;

public class SettingsV2ListAdapter : BaseAdapter<Alert>
{
	private List<Alert> items;

	private Activity context;

	public override int Count => items.Count;

	public override Alert this[int index] => items[index];

	public SettingsV2ListAdapter(Activity context, List<Alert> alertList)
	{
		items = alertList;
		this.context = context;
	}

	public override long GetItemId(int position)
	{
		return items[position].Id;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		Alert alert = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361913, null);
		}
		if (alert.StartStation == null || alert.EndStation == null)
		{
			StationService stationService = SharedServices.IoC.Resolve<StationService>();
			alert.StartStation = stationService.GetStationByCrs(alert.StartCRS, withDescription: false);
			alert.EndStation = stationService.GetStationByCrs(alert.EndCRS, withDescription: false);
		}
		view.FindViewById<TextView>(2131231191).Text = string.Format("{0} - {1}", alert.StartTime.ToString("hh\\:mm"), alert.EndTime.ToString("hh\\:mm"));
		if (alert.StartStation != null)
		{
			view.FindViewById<TextView>(2131231190).Text = $"{alert.StartStation.Name} to {alert.EndStation.Name}";
		}
		view.FindViewById<TextView>(2131231189).Text = $"{alert.DayNames()}";
		return view;
	}
}
