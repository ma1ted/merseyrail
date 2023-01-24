using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Merseyrail.Adapters;

public class DeveloperListAdapter : BaseAdapter
{
	public List<DeveloperItem> ListItems { get; }

	private Context Context { get; }

	public override int Count => ListItems.Count;

	public DeveloperListAdapter(Context context, List<DeveloperItem> listItems)
	{
		ListItems = listItems;
		Context = context;
	}

	public override Java.Lang.Object GetItem(int position)
	{
		return null;
	}

	public override long GetItemId(int position)
	{
		return 0L;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		View view = convertView;
		try
		{
			if (view == null)
			{
				view = LayoutInflater.From(Context)!.Inflate(2131361876, null, attachToRoot: false);
			}
			view.FindViewById<TextView>(2131230912).Text = ListItems[position].Title;
			TextView textView = view.FindViewById<TextView>(2131230911);
			textView.Text = ListItems[position].Description;
			Button button = view.FindViewById<Button>(2131230910);
			button.Visibility = ViewStates.Gone;
			textView.Visibility = ViewStates.Visible;
			if (ListItems[position].Title == "Force Crash")
			{
				if (false)
				{
					if (button != null)
					{
						button.Visibility = ViewStates.Visible;
						textView.Visibility = ViewStates.Gone;
						button.Click += delegate
						{
						};
						return view;
					}
					return view;
				}
				textView.Text = "Force crash only available in debug mode";
				return view;
			}
			return view;
		}
		catch (System.Exception)
		{
			return view;
		}
	}
}
