using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Common.Domain;

namespace Merseyrail.AppWidgets;

public static class DeparturesWidgetProviderExtensions
{
	public static List<RainbowBoardStatus> ForWidget(this List<RainbowBoardStatus> statusList, int widgetId)
	{
		ISharedPreferences sharedPreferencesForAppWidget = DeparturesWidgetProvider.GetSharedPreferencesForAppWidget(Application.Context, widgetId);
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_SOUTHPORT", defValue: true))
		{
			RainbowBoardStatus item = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Southport").First();
			statusList.Remove(item);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_KIRKBY", defValue: true))
		{
			RainbowBoardStatus item2 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Kirkby").First();
			statusList.Remove(item2);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_WEST_KIRBY", defValue: true))
		{
			RainbowBoardStatus item3 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "West Kirby").First();
			statusList.Remove(item3);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_ELLESMERE_PORT", defValue: true))
		{
			RainbowBoardStatus item4 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Ellesmere Port").First();
			statusList.Remove(item4);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_ORMSKIRK", defValue: true))
		{
			RainbowBoardStatus item5 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Ormskirk").First();
			statusList.Remove(item5);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_HUNTS_CROSS", defValue: true))
		{
			RainbowBoardStatus item6 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Hunts Cross").First();
			statusList.Remove(item6);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_NEW_BRIGHTON", defValue: true))
		{
			RainbowBoardStatus item7 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "New Brighton").First();
			statusList.Remove(item7);
		}
		if (!sharedPreferencesForAppWidget.GetBoolean("USE_RB_CHESTER", defValue: true))
		{
			RainbowBoardStatus item8 = statusList.Where((RainbowBoardStatus x) => x.RouteName == "Chester").First();
			statusList.Remove(item8);
		}
		return statusList;
	}
}
