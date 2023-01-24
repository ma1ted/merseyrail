using System;
using System.Collections.Generic;
using Android.Appwidget;
using Android.Content;
using Common.Domain;

namespace Merseyrail.AppWidgets;

public class WidgetDataModel
{
	public bool IsUpdating;

	public Context Context;

	public AppWidgetManager AppWidgetManager;

	public int AppWidgetId;

	public string SelectedStationName;

	public bool UseNearest;

	public Station SelectedStation { get; set; }

	public DepartureBoard DepartureBoard { get; set; }

	public List<RainbowBoardStatus> RainbowBoard { get; set; }

	public DateTime LastUpdated { get; set; }

	public WidgetDataModel(Context _context, AppWidgetManager _appWidgetManager, int _appWidgetId, bool _useNearest, string _selectedStationName)
	{
		Context = _context;
		AppWidgetManager = _appWidgetManager;
		AppWidgetId = _appWidgetId;
		UseNearest = _useNearest;
		SelectedStationName = _selectedStationName;
	}
}
