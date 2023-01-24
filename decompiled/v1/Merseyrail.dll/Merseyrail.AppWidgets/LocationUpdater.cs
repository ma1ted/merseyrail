using System;
using System.Linq;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Common.Domain;
using Common.Services;
using Java.Interop;
using Java.Lang;
using Merseyrail.Helpers;
using TinyIoC;

namespace Merseyrail.AppWidgets;

public class LocationUpdater : Java.Lang.Object, ILocationListener, IJavaObject, IDisposable, IJavaPeerable
{
	private WidgetDataModel model;

	public event EventHandler<WidgetDataModel> OnLocationFound;

	public LocationUpdater(WidgetDataModel _model)
	{
		model = _model;
	}

	public void OnLocationChanged(Location location)
	{
		if (this.OnLocationFound != null)
		{
			AppInitHelpers.AppIoCInit();
			Station station = TinyIoCContainer.Current.Resolve<StationService>().GetNearestStations(location.Latitude, location.Longitude, 3, merseyrailOnly: true).FirstOrDefault();
			model.SelectedStation = station;
			model.SelectedStationName = station.Name;
			this.OnLocationFound(this, model);
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
