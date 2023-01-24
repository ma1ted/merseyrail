using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Common;
using Common.Domain;
using Common.IoC;
using Common.Services;
using Java.Interop;
using Java.Lang;
using Merseyrail.Helpers;
using Merseyrail.Services;
using TinyIoC;

namespace Merseyrail.AppWidgets;

[BroadcastReceiver(Label = "@string/widget_name")]
[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", "glow.merseyrail.widget.REFRESH", "glow.merseyrail.widget.STATION_SELECTED", "glow.merseyrail.widget.STATION_SELECTED_ROW_NUMBER", "android.intent.action.BOOT_COMPLETED", "android.net.conn.CONNECTIVITY_CHANGE" })]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_word")]
public class DeparturesWidgetProvider : AppWidgetProvider, ILocationListener, IJavaObject, IDisposable, IJavaPeerable
{
	public const string REFRESH_ACTION = "glow.merseyrail.widget.REFRESH";

	public const string STATION_SELECTED_ACTION = "glow.merseyrail.widget.STATION_SELECTED";

	public const string STATION_SELECTED_ROW_NUMBER = "glow.merseyrail.widget.STATION_SELECTED_ROW_NUMBER";

	public const string STATION_SELECTED_NAME = "glow.merseyrail.widget.STATION_SELECTED_NAME";

	public const string STATION_SELECTED_TRAIN_ID = "glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID";

	private static HandlerThread workerThread;

	private static Handler workerQueue;

	private DateTime _rainbowBoardStatusDateTime;

	private List<RainbowBoardStatus> _rainbowBoardStatusList;

	private DepartureBoard _departureBoard;

	private TinyIoCContainer ioc = TinyIoCContainer.Current;

	private DepartureBoardService departureBoardService;

	private WidgetService widgetService;

	private WidgetDataModel WidgetData;

	private LocationUpdater locationUpdater;

	public event EventHandler<Location> OnLocationFound;

	public override void OnReceive(Context context, Intent intent)
	{
		string? action = intent.Action;
		if (action == "android.intent.action.BOOT_COMPLETED")
		{
			ScheduleUpdate(context);
		}
		if (action == "glow.merseyrail.widget.REFRESH")
		{
			intent.Extras!.GetInt("appWidgetId");
			ScheduleUpdate(context);
		}
		if (action == "glow.merseyrail.widget.STATION_SELECTED")
		{
			Toast.MakeText(context, $"Item Selected", ToastLength.Short)!.Show();
		}
		if (action == "glow.merseyrail.widget.STATION_SELECTED_NAME" && intent.HasExtra("SelectedStation"))
		{
			string stringExtra = intent.GetStringExtra("SelectedStation");
			string stringExtra2 = intent.GetStringExtra("TrainId");
			Intent intent2 = new Intent(context, typeof(Loader));
			intent2.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ReorderToFront);
			intent2.SetAction("glow.merseyrail.widget.STATION_SELECTED_NAME");
			intent2.PutExtra("glow.merseyrail.widget.STATION_SELECTED_NAME", stringExtra);
			intent2.PutExtra("glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID", stringExtra2);
		}
		base.OnReceive(context, intent);
	}

	private void ScheduleUpdate(Context context)
	{
		IoCUtils.RegisterSingletons();
		ioc.Register<ISettingsService, SettingsService>();
		ioc.Register<IConnectivityService, ConnectivityService>();
		departureBoardService = ioc.Resolve<DepartureBoardService>();
		widgetService = ioc.Resolve<WidgetService>();
		AppWidgetManager instance = AppWidgetManager.GetInstance(context);
		ComponentName provider = new ComponentName(context, Java.Lang.Class.FromType(typeof(DeparturesWidgetProvider)));
		OnUpdate(context, instance, instance.GetAppWidgetIds(provider));
	}

	public override void OnEnabled(Context context)
	{
		base.OnEnabled(context);
		AppInitHelpers.AppIoCInit();
	}

	public override void OnDeleted(Context context, int[] appWidgetIds)
	{
		base.OnDeleted(context, appWidgetIds);
	}

	public override void OnDisabled(Context context)
	{
		base.OnDisabled(context);
	}

	public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
	{
		base.OnUpdate(context, appWidgetManager, appWidgetIds);
		new RemoteViews(context.PackageName, 2131361833);
		foreach (int id in appWidgetIds)
		{
			WidgetDataModel modelFromWidgetPrefs = GetModelFromWidgetPrefs(context, appWidgetManager, id);
			modelFromWidgetPrefs.IsUpdating = true;
			RemoteViewsFactory.ShowProgressLayout(modelFromWidgetPrefs.Context, modelFromWidgetPrefs.AppWidgetId, modelFromWidgetPrefs);
			UpdateWidgetData(modelFromWidgetPrefs);
		}
	}

	private static WidgetDataModel GetModelFromWidgetPrefs(Context context, AppWidgetManager appWidgetManager, int id)
	{
		ISharedPreferences sharedPreferencesForAppWidget = GetSharedPreferencesForAppWidget(context, id);
		bool boolean = sharedPreferencesForAppWidget.GetBoolean("USE_NEAREST_STATION", defValue: false);
		string @string = sharedPreferencesForAppWidget.GetString("SELECTED_STATION", "Liverpool Central");
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_SOUTHPORT", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_KIRKBY", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_WEST_KIRBY", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_ELLESMERE_PORT", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_ORMSKIRK", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_HUNTS_CROSS", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_NEW_BRIGHTON", defValue: true);
		sharedPreferencesForAppWidget.GetBoolean("USE_RB_CHESTER", defValue: true);
		return new WidgetDataModel(context, appWidgetManager, id, boolean, @string);
	}

	private WidgetDataModel UpdateWidgetData(WidgetDataModel model)
	{
		model.IsUpdating = true;
		StationService stationService = ioc.Resolve<StationService>();
		if (model.UseNearest)
		{
			Location currentLocation = ioc.Resolve<LocationService>().CurrentLocation;
			if (currentLocation != null)
			{
				Station station = stationService.GetNearestStations(currentLocation.Latitude, currentLocation.Longitude, 3, merseyrailOnly: true).FirstOrDefault();
				model.SelectedStation = station;
				model.SelectedStationName = station.Name;
				UpdateRainbowBoardModelData(model);
			}
			else
			{
				LocationManager locationManager = Application.Context.GetSystemService("location") as LocationManager;
				string provider = "network";
				if (locationManager.IsProviderEnabled(provider))
				{
					locationUpdater = new LocationUpdater(model);
					locationUpdater.OnLocationFound += delegate(object sender, WidgetDataModel e)
					{
						UpdateRainbowBoardModelData(e);
					};
					locationManager.RequestSingleUpdate(provider, locationUpdater, null);
				}
			}
		}
		else
		{
			Station station2 = (from x in stationService.GetStations(merseyrailOnly: true)
				where x.Name.ToUpper() == model.SelectedStationName.ToUpper()
				select x).FirstOrDefault();
			model.SelectedStation = station2;
			model.SelectedStationName = station2.Name;
			UpdateRainbowBoardModelData(model);
		}
		return model;
	}

	private async void UpdateRainbowBoardModelData(WidgetDataModel model)
	{
		try
		{
			ioc.Resolve<RainbowBoardService>().GetRainbowBoard(delegate(DateTime updated, List<RainbowBoardStatus> rboard)
			{
				model.RainbowBoard = rboard.ForWidget(model.AppWidgetId);
				ioc.Resolve<DepartureBoardService>().GetDepartureBoard(model.SelectedStation.CrsCode, isLive: true, 0, 3, delegate(DepartureBoard dboard)
				{
					if (dboard.IsClosed)
					{
						model.DepartureBoard = dboard;
					}
					else
					{
						model.DepartureBoard = dboard;
					}
					model.IsUpdating = false;
					UpdateWidgetViews(this, model);
				}, 0);
			});
		}
		catch (System.Exception)
		{
		}
	}

	private void UpdateWidgetViews(object sender, WidgetDataModel widgetModel)
	{
		Bundle appWidgetOptions = widgetModel.AppWidgetManager.GetAppWidgetOptions(widgetModel.AppWidgetId);
		RemoteViewsFactory.CreateLayout(widgetModel.Context, widgetModel.AppWidgetId, appWidgetOptions, widgetModel);
	}

	private async Task UpdateRainbowBoard()
	{
		List<RainbowBoardStatus> rainbowBoard = await widgetService.GetRainbowBoard();
		WidgetData.RainbowBoard = rainbowBoard;
	}

	private async Task LoadDepartures()
	{
		Station selectedStation = (from x in ioc.Resolve<StationService>().GetStations(merseyrailOnly: true)
			where x.Name.ToUpper() == ""
			select x).FirstOrDefault();
		WidgetData.SelectedStation = selectedStation;
	}

	public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
	{
		base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
		ScheduleUpdate(context);
	}

	public static string GetSharedPreferencesNameForAppWidget(Context context, int appWidgetId)
	{
		return context.PackageName + "DeparturesWidgetPreferences_" + appWidgetId;
	}

	public static ISharedPreferences GetSharedPreferencesForAppWidget(Context context, int appWidgetId)
	{
		return context.GetSharedPreferences(GetSharedPreferencesNameForAppWidget(context, appWidgetId), FileCreationMode.Private);
	}

	public void OnLocationChanged(Location location)
	{
		if (this.OnLocationFound != null)
		{
			this.OnLocationFound(this, location);
		}
	}

	public void OnProviderDisabled(string provider)
	{
	}

	public void OnProviderEnabled(string provider)
	{
	}

	public void OnStatusChanged(string provider, Availability status, Bundle extras)
	{
	}
}
