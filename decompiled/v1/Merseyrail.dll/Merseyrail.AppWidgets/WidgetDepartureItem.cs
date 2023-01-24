using System;

namespace Merseyrail.AppWidgets;

public class WidgetDepartureItem
{
	public string StationCrs { get; set; }

	public string StationName { get; set; }

	public string Platform { get; set; }

	public string ArrivalSchedule { get; set; }

	public string ArrivalActual { get; set; }

	public string DepartureSchedule { get; set; }

	public string DepartureActual { get; set; }

	public string TrainId { get; set; }

	public TimeSpan ArrivalScheduleTime { get; set; }

	public TimeSpan ArrivalActualTime { get; set; }

	public TimeSpan DepartureScheduleTime { get; set; }

	public TimeSpan DepartureActualTime { get; set; }
}
