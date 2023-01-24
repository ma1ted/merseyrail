using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Shared;
using TinyIoC;

namespace Merseyrail;

[Activity(Label = "Merseyrail: Select a station", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false, AlwaysRetainTaskState = true)]
public class StationSelectorActivity : Activity
{
	public ListView StationListRecent { get; set; }

	public ListView StationListNearest { get; set; }

	public ListView StationListAtoZ { get; set; }

	public List<string> StationsRecent { get; set; }

	public List<string> StationsNearest { get; set; }

	public List<string> StationsAtoZ { get; set; }

	public LinearLayout RecentsContainer { get; set; }

	public LinearLayout NearestContainer { get; set; }

	public LinearLayout AZContainer { get; set; }

	public Location currentLocation => SharedValues.CurrentDeviceLocation;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361828);
		StationsNearest = GetStationsListNearest();
		StationsAtoZ = GetStationsListAZ();
		StationsRecent = GetStationsListRecents();
	}

	protected override void OnStart()
	{
		StationListRecent = (ListView)FindViewById(2131231181);
		StationListNearest = (ListView)FindViewById(2131231180);
		StationListAtoZ = (ListView)FindViewById(2131231171);
		RecentsContainer = (LinearLayout)FindViewById(2131231179);
		NearestContainer = (LinearLayout)FindViewById(2131231177);
		AZContainer = (LinearLayout)FindViewById(2131231173);
		InitLayouts();
		InitRecentList();
		InitNearestList();
		InitAtoZList();
		base.OnStart();
	}

	private void InitRecentList()
	{
		StationListRecent.Adapter = new StationListAdapter(this, StationsRecent);
		StationListRecent.FastScrollEnabled = true;
		StationListRecent.ItemClick += stationListRecent_ItemClick;
	}

	private void ReturnIntent(Station item)
	{
		Intent intent = new Intent();
		intent.PutExtra("result", item.Name);
		intent.PutExtra("crscode", item.CrsCode);
		SetResult(Result.Ok, intent);
		Finish();
	}

	private void stationListRecent_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
	{
		string item = StationListRecent.Adapter!.GetItem(e.Position)!.ToString();
		Station item2 = (from x in TinyIoCContainer.Current.Resolve<StationService>().GetStations(merseyrailOnly: true)
			where x.Name == item
			select x).FirstOrDefault();
		ReturnIntent(item2);
	}

	private void InitLayouts()
	{
		if (currentLocation != null)
		{
			SetStandardLayout();
		}
		else
		{
			SetRestrictedLayout();
		}
	}

	private void InitNearestList()
	{
		StationListNearest.Adapter = new StationListAdapter(this, StationsNearest);
		StationListNearest.FastScrollEnabled = true;
		StationListNearest.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
		{
			string item = StationListNearest.Adapter!.GetItem(e.Position)!.ToString();
			AddToRecentsList(item);
			Station item2 = (from x in TinyIoCContainer.Current.Resolve<StationService>().GetStations(merseyrailOnly: true)
				where x.Name == item
				select x).FirstOrDefault();
			ReturnIntent(item2);
		};
	}

	protected override void OnResume()
	{
		base.OnResume();
		RecentsContainer.Visibility = ((StationsRecent.Count <= 0) ? ViewStates.Gone : ViewStates.Visible);
	}

	private void SetStandardLayout()
	{
		NearestContainer.Visibility = ViewStates.Visible;
		LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(AZContainer.LayoutParameters);
		layoutParams.Weight = 5f;
		AZContainer.Visibility = ViewStates.Visible;
		AZContainer.LayoutParameters = layoutParams;
	}

	private void SetRestrictedLayout()
	{
		NearestContainer.Visibility = ViewStates.Gone;
		LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(AZContainer.LayoutParameters);
		layoutParams.Weight = 8f;
		AZContainer.Visibility = ViewStates.Visible;
		AZContainer.LayoutParameters = layoutParams;
	}

	private void InitAtoZList()
	{
		StationListAtoZ.Adapter = new StationListAdapter(this, StationsAtoZ);
		StationListAtoZ.FastScrollEnabled = true;
		StationListAtoZ.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
		{
			string item = StationListAtoZ.Adapter!.GetItem(e.Position)!.ToString();
			AddToRecentsList(item);
			Station item2 = (from x in TinyIoCContainer.Current.Resolve<StationService>().GetStations(merseyrailOnly: true)
				where x.Name == item
				select x).FirstOrDefault();
			ReturnIntent(item2);
		};
	}

	private void AddToRecentsList(string item)
	{
		if (StationsRecent.Contains(item))
		{
			StationsRecent.Remove(item);
		}
		StationsRecent.Insert(0, item);
		InitNearestList();
	}

	protected override void OnStop()
	{
		base.OnStop();
		string[] value = StationsRecent.Take(3).ToArray();
		string value2 = string.Join(";", value);
		SharedSettings.SettingsService.SaveSetting("nearest_stations", value2);
	}

	private List<string> GetStationsListAZ()
	{
		return (from x in SharedServices.IoC.Resolve<StationService>().GetStations(merseyrailOnly: true)
			select x.Name into x
			orderby x
			select x).ToList();
	}

	private List<string> GetStationsListNearest()
	{
		if (currentLocation != null)
		{
			return (from x in SharedServices.IoC.Resolve<StationService>().GetNearestStations(currentLocation.Latitude, currentLocation.Longitude, 16, merseyrailOnly: true)
				select x.Name).ToList();
		}
		return GetStationsListAZ();
	}

	private List<string> GetStationsListRecents()
	{
		string setting = SharedSettings.SettingsService.GetSetting("nearest_stations", string.Empty);
		if (!string.IsNullOrEmpty(setting))
		{
			return setting.Split(';').ToList();
		}
		return new List<string>();
	}
}
