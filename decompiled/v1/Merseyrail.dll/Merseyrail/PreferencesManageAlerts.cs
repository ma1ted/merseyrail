using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Adapters;
using Merseyrail.Helpers;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail;

[Activity(Label = "PreferencesManageAlerts", ScreenOrientation = ScreenOrientation.Portrait)]
public class PreferencesManageAlerts : Activity
{
	private ISettingsService settingsService;

	private List<Alert> alertsList;

	private ListView listView;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		settingsService = SharedServices.IoC.Resolve<ISettingsService>();
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361904);
		FindViewById<ImageButton>(2131230794)!.Click += delegate
		{
			Finish();
		};
		FindViewById<Button>(2131230784)!.Click += delegate
		{
			string value2 = JsonConvert.SerializeObject(new Alert());
			Intent intent2 = new Intent(this, typeof(PreferencesAlertActivity));
			intent2.PutExtra("alertJson", value2);
			intent2.PutExtra("alertId", 0);
			StartActivity(intent2);
		};
		listView = (ListView)FindViewById(2131231005);
		listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
		{
			Alert alert = alertsList.Where((Alert x) => x.Id == e.Id).SingleOrDefault();
			alert.StartStation = null;
			alert.EndStation = null;
			string value = JsonConvert.SerializeObject(alert);
			Intent intent = new Intent(this, typeof(PreferencesAlertActivity));
			intent.PutExtra("alertJson", value);
			intent.PutExtra("alertId", e.Id);
			StartActivity(intent);
		};
	}

	protected override void OnResume()
	{
		base.OnResume();
		if (listView != null)
		{
			populateAlertsList(listView);
		}
		CalamityHelpers.CheckForCalamity(this);
	}

	private void populateAlertsList(ListView lv)
	{
		string firebaseToken = SharedSettings.FirebaseToken;
		string deviceInstanceId = SharedSettings.DeviceInstanceId;
		SharedServices.IoC.Resolve<AlertService>().FetchAlerts(firebaseToken, deviceInstanceId, DeviceType.Android, SharedSettings.IncidentsAllowed, SharedSettings.CalamitiesAllowed, SharedSettings.RemindersAllowed, SharedSettings.AppVersion, delegate(Alert[] alerts)
		{
			alertsList = alerts.ToList();
			RunOnUiThread(delegate
			{
				lv.Adapter = new SettingsV2ListAdapter(this, alerts.ToList());
			});
		}, delegate
		{
			RunOnUiThread(delegate
			{
				Toast.MakeText(this, "Could not load settings. Please try again.", ToastLength.Long);
			});
		});
	}
}
