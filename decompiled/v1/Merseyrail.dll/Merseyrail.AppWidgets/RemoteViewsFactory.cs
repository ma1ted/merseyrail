using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Newtonsoft.Json;

namespace Merseyrail.AppWidgets;

internal class RemoteViewsFactory
{
	internal static RemoteViews ShowProgressLayout(Context context, int appWidgetId, WidgetDataModel widgetDataModel)
	{
		widgetDataModel.IsUpdating = true;
		RemoteViews remoteViews = new RemoteViews(context.PackageName, 2131361833);
		remoteViews.SetViewVisibility(2131231087, ViewStates.Visible);
		widgetDataModel.AppWidgetManager.UpdateAppWidget(widgetDataModel.AppWidgetId, remoteViews);
		return remoteViews;
	}

	internal static RemoteViews HideProgressLayout(Context context, int appWidgetId, WidgetDataModel widgetDataModel)
	{
		widgetDataModel.IsUpdating = false;
		RemoteViews remoteViews = new RemoteViews(context.PackageName, 2131361833);
		remoteViews.SetViewVisibility(2131231087, ViewStates.Invisible);
		remoteViews.SetViewVisibility(2131231056, ViewStates.Invisible);
		widgetDataModel.AppWidgetManager.UpdateAppWidget(widgetDataModel.AppWidgetId, remoteViews);
		return remoteViews;
	}

	internal static RemoteViews DoTimeoutLayout(Context context, int appWidgetId, WidgetDataModel widgetDataModel)
	{
		RemoteViews remoteViews = new RemoteViews(context.PackageName, 2131361833);
		remoteViews.SetViewVisibility(2131231087, ViewStates.Invisible);
		SetClickRefresh(context, remoteViews, appWidgetId);
		SetClickHeaderPrefs(context, remoteViews, appWidgetId);
		widgetDataModel.AppWidgetManager.UpdateAppWidget(widgetDataModel.AppWidgetId, remoteViews);
		if (widgetDataModel.DepartureBoard != null && widgetDataModel.DepartureBoard.Count > 0)
		{
			remoteViews.SetViewVisibility(2131231056, ViewStates.Invisible);
		}
		else
		{
			remoteViews.SetViewVisibility(2131231056, ViewStates.Visible);
		}
		return remoteViews;
	}

	internal static RemoteViews CreateLayout(Context context, int appWidgetId, Bundle options, WidgetDataModel widgetDataModel)
	{
		options.GetInt("appWidgetMinHeight");
		return JustGetLayout(context, appWidgetId, widgetDataModel);
	}

	internal static RemoteViews CreateLayout(Context context, int appWidgetId, WidgetDataModel widgetDataModel)
	{
		return JustGetLayout(context, appWidgetId, widgetDataModel);
	}

	private static RemoteViews JustGetLayout(Context context, int appWidgetId, WidgetDataModel widgetDataModel)
	{
		RemoteViews remoteViews = new RemoteViews(context.PackageName, 2131361833);
		try
		{
			if (widgetDataModel.IsUpdating)
			{
				ShowProgressLayout(context, appWidgetId, widgetDataModel);
			}
			else
			{
				HideProgressLayout(context, appWidgetId, widgetDataModel);
			}
			remoteViews.SetTextViewText(2131231247, $"Last updated: {DateTime.Now.ToShortTimeString()}");
			remoteViews.SetTextViewText(2131231248, widgetDataModel.SelectedStationName);
			SetClickHeaderPrefs(context, remoteViews, appWidgetId);
			BuildRainbowBoardRemoteViews(remoteViews, widgetDataModel);
			List<WidgetDepartureItem> widgetDepartureItemList = GetWidgetDepartureItemList(widgetDataModel);
			Intent intent = new Intent(context, typeof(DeparturesWidgetService));
			intent.SetData(Android.Net.Uri.FromParts("content", $"{new Random().Next()}{appWidgetId}", null));
			intent.PutExtra("appWidgetId", appWidgetId);
			intent.PutExtra("random", new Random().Next());
			string value = JsonConvert.SerializeObject(widgetDepartureItemList);
			intent.PutExtra("widgetDepartureItemListJson", value);
			remoteViews.SetRemoteAdapter(appWidgetId, 2131231007, intent);
			remoteViews.SetEmptyView(2131231007, 2131230858);
			Intent intent2 = new Intent(context, typeof(DeparturesWidgetProvider));
			intent2.SetAction("glow.merseyrail.widget.STATION_SELECTED_NAME");
			intent2.PutExtra("appWidgetId", appWidgetId);
			PendingIntent broadcast = PendingIntent.GetBroadcast(context, 0, intent2, PendingIntentFlags.UpdateCurrent);
			remoteViews.SetPendingIntentTemplate(2131231007, broadcast);
			SetClickRefresh(context, remoteViews, appWidgetId);
			remoteViews.SetViewVisibility(2131231087, ViewStates.Invisible);
			widgetDataModel.AppWidgetManager.UpdateAppWidget(widgetDataModel.AppWidgetId, remoteViews);
			return remoteViews;
		}
		catch (Exception)
		{
			throw;
		}
	}

	private static void SetClickHeaderPrefs(Context context, RemoteViews remoteViews, int appWidgetId)
	{
		Intent intent = new Intent(context, typeof(DeparturesWidgetPreferencesActivity));
		intent.PutExtra("appWidgetId", appWidgetId);
		intent.SetFlags(ActivityFlags.ClearTask);
		PendingIntent activity = PendingIntent.GetActivity(context, appWidgetId, intent, (PendingIntentFlags)0);
		remoteViews.SetOnClickPendingIntent(2131230850, activity);
	}

	private static void SetClickRefresh(Context context, RemoteViews remoteViews, int appWidgetId)
	{
		Intent intent = new Intent(context, typeof(DeparturesWidgetProvider));
		intent.SetAction("glow.merseyrail.widget.REFRESH");
		intent.PutExtra("appWidgetId", appWidgetId);
		PendingIntent broadcast = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
		remoteViews.SetOnClickPendingIntent(2131230849, broadcast);
	}

	public static void GetWidgetPrefs(Context context, int appWidgetId)
	{
		ISharedPreferences sharedPreferencesForAppWidget = DeparturesWidgetProvider.GetSharedPreferencesForAppWidget(context, appWidgetId);
		sharedPreferencesForAppWidget.GetString("USUAL_START_STATION", "Not Set");
		sharedPreferencesForAppWidget.GetString("USUAL_END_STATION", "Not Set");
	}

	private static PendingIntent CreateMessageTemplateIntent(Context context, int appWidgetId)
	{
		Intent intent = new Intent(context, typeof(Loader));
		intent.PutExtra("glow.merseyrail.widget.STATION_SELECTED", appWidgetId);
		intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
		return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
	}

	private static RemoteViews BuildRainbowBoardRemoteViews(RemoteViews parent, WidgetDataModel widgetDataModel)
	{
		parent.RemoveAllViews(2131231084);
		_ = widgetDataModel.RainbowBoard;
		foreach (IEnumerable<RainbowBoardStatus> item in widgetDataModel.RainbowBoard.Batch(2))
		{
			RemoteViews lineStatusRow = GetLineStatusRow(item);
			parent.AddView(2131231084, lineStatusRow);
		}
		return parent;
	}

	private static RemoteViews GetLineStatusRow(IEnumerable<RainbowBoardStatus> statusItemBatch)
	{
		RainbowBoardStatus rainbowBoardStatus = null;
		RainbowBoardStatus rainbowBoardStatus2 = null;
		if (statusItemBatch.Count() > 0)
		{
			rainbowBoardStatus = statusItemBatch.ToList()[0];
		}
		if (statusItemBatch.Count() > 1)
		{
			rainbowBoardStatus2 = statusItemBatch.ToList()[1];
		}
		RemoteViews remoteViews = new RemoteViews(Application.Context.PackageName, 2131361835);
		RemoteViews remoteViews2 = new RemoteViews(Application.Context.PackageName, 2131361834);
		if (rainbowBoardStatus != null)
		{
			remoteViews2.SetImageViewResource(2131231285, GetStatusIcon(rainbowBoardStatus.StatusId));
			remoteViews2.SetTextViewText(2131231284, rainbowBoardStatus.RouteName);
			remoteViews2.SetTextViewText(2131231286, rainbowBoardStatus.StatusName);
			remoteViews.AddView(2131231282, remoteViews2);
		}
		RemoteViews remoteViews3 = new RemoteViews(Application.Context.PackageName, 2131361834);
		if (rainbowBoardStatus2 != null)
		{
			remoteViews3.SetImageViewResource(2131231285, GetStatusIcon(rainbowBoardStatus2.StatusId));
			remoteViews3.SetTextViewText(2131231284, rainbowBoardStatus2.RouteName);
			remoteViews3.SetTextViewText(2131231286, rainbowBoardStatus2.StatusName);
			remoteViews.AddView(2131231282, remoteViews3);
		}
		else
		{
			remoteViews3.SetViewVisibility(2131231285, ViewStates.Invisible);
			remoteViews3.SetImageViewResource(2131231285, 0);
			remoteViews3.SetTextViewText(2131231284, string.Empty);
			remoteViews3.SetTextViewText(2131231286, string.Empty);
			remoteViews.AddView(2131231282, remoteViews3);
		}
		return remoteViews;
	}

	private static int GetStatusIcon(int statusId)
	{
		switch (statusId)
		{
		case 2:
			return 2131165376;
		case 1:
			return 2131165377;
		case 0:
			return 2131165374;
		case 3:
		case 5:
			return 2131165375;
		case 4:
			return 2131165373;
		default:
			return 2131165374;
		}
	}

	private static List<WidgetDepartureItem> GetWidgetDepartureItemList(WidgetDataModel widgetDataModel)
	{
		return widgetDataModel.DepartureBoard.Select((DepartureBoardItem x) => new WidgetDepartureItem
		{
			StationCrs = x.DestinationStation.CrsCode,
			StationName = x.DestinationStation.Name,
			Platform = x.Platform,
			ArrivalSchedule = $"{x.ArrivalSchedule.Hours:00}:{x.ArrivalSchedule.Minutes:00}",
			ArrivalActual = $"{x.ArrivalActual.Hours:00}:{x.ArrivalActual.Minutes:00}",
			DepartureSchedule = $"{x.DepartureSchedule.Hours:00}:{x.DepartureSchedule.Minutes:00}",
			DepartureActual = $"{x.DepartureActual.Hours:00}:{x.DepartureActual.Minutes:00}",
			ArrivalScheduleTime = x.ArrivalSchedule,
			ArrivalActualTime = x.ArrivalActual,
			DepartureScheduleTime = x.DepartureSchedule,
			DepartureActualTime = x.DepartureActual,
			TrainId = x.TrainId
		}).ToList();
	}
}
