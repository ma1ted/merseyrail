using System;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Java.Lang;

namespace Merseyrail.Services;

public class LocationService : Java.Lang.Object, Android.Gms.Location.ILocationListener, IJavaObject, IDisposable, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
{
	public Location CurrentLocation;

	public bool IsGooglePlayServicesInstalled { get; set; }

	public GoogleApiClient LocClient { get; set; }

	public LocationRequest LocRequest { get; set; }

	public event EventHandler<Location> OnLocationUpdated;

	public LocationService()
	{
		IsGooglePlayServicesInstalled = CheckGooglePlayServicesInstalled();
		if (IsGooglePlayServicesInstalled)
		{
			LocClient = new GoogleApiClient.Builder(Application.Context).AddApi(LocationServices.API).AddConnectionCallbacks(this).AddOnConnectionFailedListener(this)
				.Build();
			LocClient.RegisterConnectionCallbacks(this);
			LocClient.RegisterConnectionFailedListener(this);
			LocRequest = new LocationRequest();
		}
		else
		{
			Log.Error("OnCreate", "Google Play Services is not installed");
			Toast.MakeText(Application.Context, "Google Play Services is not installed", ToastLength.Long)!.Show();
		}
	}

	public void ConnectClient()
	{
		LocClient.Connect();
	}

	public void DisconnectClient()
	{
		LocClient.Disconnect();
	}

	private bool CheckGooglePlayServicesInstalled()
	{
		GoogleApiAvailability instance = GoogleApiAvailability.Instance;
		int num = instance.IsGooglePlayServicesAvailable(Application.Context);
		if (num == 0)
		{
			Log.Info("MainActivity", "Google Play Services is installed on this device.");
			return true;
		}
		if (instance.IsUserResolvableError(num))
		{
			string errorString = GooglePlayServicesUtil.GetErrorString(num);
			Log.Error("ManActivity", "There is a problem with Google Play Services on this device: {0} - {1}", num, errorString);
		}
		return false;
	}

	public void OnLocationChanged(Location location)
	{
		if (this.OnLocationUpdated != null)
		{
			this.OnLocationUpdated(this, location);
		}
		CurrentLocation = location;
	}

	public void OnProviderDisabled(string provider)
	{
	}

	public void OnProviderEnabled(string provider)
	{
	}

	public void OnConnected(Bundle bundle)
	{
		Log.Info("LocationClient", "Now connected to client");
		if (LocClient.IsConnected)
		{
			if (LocationServices.FusedLocationApi.GetLastLocation(LocClient) != null)
			{
				CurrentLocation = LocationServices.FusedLocationApi.GetLastLocation(LocClient);
				Log.Debug("LocationClient", "Last location printed");
			}
			LocRequest.SetPriority(100);
			LocRequest.SetFastestInterval(5000L);
			LocRequest.SetInterval(10000L);
			Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms", LocRequest.Priority.ToString(), LocRequest.Interval.ToString());
			LocationServices.FusedLocationApi.RequestLocationUpdates(LocClient, LocRequest, this);
		}
		else
		{
			Log.Info("LocationClient", "Please wait for client to connect");
		}
	}

	public void OnConnectionSuspended(int cause)
	{
		Log.Info("LocationClient", "Now disconnected from client");
		LocationServices.FusedLocationApi.RemoveLocationUpdates(LocClient, this);
	}

	public void OnDisconnected()
	{
		Log.Info("LocationClient", "Now disconnected from client");
		LocationServices.FusedLocationApi.RemoveLocationUpdates(LocClient, this);
	}

	public void OnConnectionFailed(ConnectionResult bundle)
	{
		Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
	}
}
