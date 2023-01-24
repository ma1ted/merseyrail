using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Adapters;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail.Fragments;

public class SettingsV2 : BaseFragment
{
	private View _view;

	private List<Alert> alertsList;

	private ISettingsService settingsService;

	private bool actionsBound;

	public EditText Name { get; set; }

	public EditText Email { get; set; }

	public SettingsV2()
	{
		actionsBound = false;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		settingsService = SharedServices.IoC.Resolve<ISettingsService>();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup viewGroup, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361871, null, attachToRoot: false);
		}
		Name = (EditText)_view.FindViewById(2131231140);
		Name.FocusChange += delegate
		{
			SaveSettings();
		};
		Email = (EditText)_view.FindViewById(2131231139);
		Email.FocusChange += delegate
		{
			SaveSettings();
		};
		ListView listView = (ListView)_view.FindViewById(2131231006);
		Button button = (Button)_view.FindViewById(2131230804);
		ImageButton imageButton = (ImageButton)_view.FindViewById(2131231138);
		if (!actionsBound)
		{
			imageButton.Click += delegate
			{
				((Main)base.Activity).OpenDrawerLayout();
			};
			listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
			{
				Alert alert = alertsList.Where((Alert x) => x.Id == e.Id).SingleOrDefault();
				alert.StartStation = null;
				alert.EndStation = null;
				string value2 = JsonConvert.SerializeObject(alert);
				Intent intent2 = new Intent(base.Activity, typeof(PreferencesAlertActivity));
				intent2.PutExtra("alertJson", value2);
				intent2.PutExtra("alertId", e.Id);
				StartActivity(intent2);
			};
			button.Click += delegate
			{
				string value = JsonConvert.SerializeObject(new Alert());
				Intent intent = new Intent(base.Activity, typeof(PreferencesAlertActivity));
				intent.PutExtra("alertJson", value);
				intent.PutExtra("alertId", 0);
				StartActivity(intent);
			};
			actionsBound = true;
		}
		populateAlertsList(listView);
		SetSettings();
		return _view;
	}

	private void populateAlertsList(ListView lv)
	{
		string firebaseToken = SharedSettings.FirebaseToken;
		string appGuid = GetAppGuid();
		SharedServices.IoC.Resolve<AlertService>().FetchAlerts(firebaseToken, appGuid, DeviceType.Android, SharedSettings.IncidentsAllowed, SharedSettings.CalamitiesAllowed, SharedSettings.RemindersAllowed, SharedSettings.AppVersion, delegate(Alert[] alerts)
		{
			alertsList = alerts.ToList();
			if (base.Activity != null)
			{
				base.Activity.RunOnUiThread(delegate
				{
					lv.Adapter = new SettingsV2ListAdapter(base.Activity, alerts.ToList());
				});
			}
		}, delegate
		{
			if (base.Activity != null)
			{
				base.Activity.RunOnUiThread(delegate
				{
					Toast.MakeText(base.Activity, "Could not load settings. Please try again.", ToastLength.Long);
				});
			}
		});
	}

	private string GetAppGuid()
	{
		string text = settingsService.GetSetting("InstanceId", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			text = Guid.NewGuid().ToString();
			settingsService.SaveSetting("InstanceId", text);
		}
		return text;
	}

	public override void OnResume()
	{
		ListView lv = (ListView)_view.FindViewById(2131231006);
		populateAlertsList(lv);
		base.OnResume();
	}

	public void SaveSettings()
	{
		SharedSettings.SettingsService.Editor.PutString("DEFAULT_NAME", Name.Text);
		SharedSettings.SettingsService.Editor.PutString("DEFAULT_EMAIL", Email.Text);
		SharedSettings.SettingsService.Editor.Apply();
		SharedSettings.SettingsService.Editor.Commit();
	}

	public void SetSettings()
	{
		base.Activity.RunOnUiThread(delegate
		{
			Name.Text = SharedSettings.SettingsService.Prefs.GetString("DEFAULT_NAME", string.Empty);
			Email.Text = SharedSettings.SettingsService.Prefs.GetString("DEFAULT_EMAIL", string.Empty);
		});
	}
}
