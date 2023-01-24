using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace Merseyrail;

internal class MenuItemAdapter : BaseAdapter<MenuItem>
{
	private List<MenuItem> items;

	private Activity context;

	public override MenuItem this[int position] => items[position];

	public override int Count => items.Count;

	public MenuItemAdapter(Activity context, List<MenuItem> items)
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
		MenuItem menuItem = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361886, null);
		}
		view.FindViewById<TextView>(2131231033).Text = menuItem.Destination;
		view.FindViewById<TextView>(2131231034).Text = menuItem.Platform;
		view.FindViewById<ImageView>(2131231040).SetImageResource(menuItem.ImageHomeIconResourceId);
		view.FindViewById<TextView>(2131231043).Text = menuItem.Countdown;
		view.FindViewById<TextView>(2131231042).Text = menuItem.TimeActual;
		view.FindViewById<TextView>(2131231045).Text = menuItem.TimeScheduled;
		view.FindViewById<ImageView>(2131231044).SetImageResource(menuItem.ImageArrowResourceId);
		return view;
	}
}
