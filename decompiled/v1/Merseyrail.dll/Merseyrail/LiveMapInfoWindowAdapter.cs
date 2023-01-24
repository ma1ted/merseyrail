using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Merseyrail;

internal class LiveMapInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter, IJavaObject, IDisposable
{
	private View _Window;

	private View _Contents;

	public LiveMapInfoWindowAdapter(Fragment liveMap)
	{
		_Window = liveMap.Activity.LayoutInflater.Inflate(2131361879, null);
		_Contents = liveMap.Activity.LayoutInflater.Inflate(2131361878, null);
	}

	public View GetInfoContents(Marker marker)
	{
		Render(marker, _Contents);
		return _Contents;
	}

	public View GetInfoWindow(Marker marker)
	{
		Render(marker, _Window);
		return _Window;
	}

	private void Render(Marker marker, View view)
	{
		_ = marker.Title;
		((TextView)view.FindViewById(2131230993)).Text = marker.Title;
	}
}
