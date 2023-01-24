using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using Newtonsoft.Json;

namespace Merseyrail.AppWidgets;

internal class StackRemoteViewsFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory, IJavaObject, IDisposable, IJavaPeerable
{
	private int AppwidgetId;

	private List<WidgetDepartureItem> WidgetDepartureItems;

	private Context context;

	public int Count => WidgetDepartureItems.Count;

	public RemoteViews LoadingView => null;

	public int ViewTypeCount => 1;

	public bool HasStableIds => false;

	public StackRemoteViewsFactory(Context context, Intent intent)
	{
		this.context = context;
		AppwidgetId = intent.GetIntExtra("appWidgetId", 0);
		string stringExtra = intent.GetStringExtra("widgetDepartureItemListJson");
		WidgetDepartureItems = JsonConvert.DeserializeObject<List<WidgetDepartureItem>>(stringExtra).Take(9).ToList();
	}

	public void OnCreate()
	{
	}

	public void OnDestroy()
	{
	}

	public RemoteViews GetViewAt(int position)
	{
		if (position > WidgetDepartureItems.Count)
		{
			return null;
		}
		RemoteViews remoteViews = new RemoteViews(context.PackageName, 2131361836);
		WidgetDepartureItem widgetDepartureItem = WidgetDepartureItems[position];
		remoteViews.SetTextViewText(2131231259, widgetDepartureItem.StationName);
		remoteViews.SetTextViewText(2131231258, $"{widgetDepartureItem.Platform}");
		remoteViews.SetTextViewText(2131231257, $"{GetMinutesWait(widgetDepartureItem)} {widgetDepartureItem.DepartureActualTime.Hours:00}:{widgetDepartureItem.DepartureActualTime.Minutes:00}");
		Bundle bundle = new Bundle();
		bundle.PutString("SelectedStation", widgetDepartureItem.StationName);
		bundle.PutString("TrainId", widgetDepartureItem.TrainId);
		Intent intent = new Intent();
		intent.PutExtras(bundle);
		remoteViews.SetOnClickFillInIntent(2131230961, intent);
		return remoteViews;
	}

	private string GetMinutesWait(WidgetDepartureItem widgetItem)
	{
		TimeSpan timeSpan = widgetItem.DepartureActualTime.Subtract(DateTime.Now.TimeOfDay);
		double num = timeSpan.TotalMinutes + 1.0;
		if (timeSpan.TotalMinutes < 0.0)
		{
			return $"Now";
		}
		if (num > 0.0 && num < 60.0)
		{
			return $"{timeSpan.Minutes + 1} Mins";
		}
		if (num > 1.0 && num < 60.0)
		{
			return $"{timeSpan.Hours:00}H {timeSpan.Minutes:00} Mins";
		}
		return "N/A";
	}

	public long GetItemId(int position)
	{
		return position;
	}

	public void OnDataSetChanged()
	{
	}
}
