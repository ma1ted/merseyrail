using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Fragments;
using Merseyrail.Helpers;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail;

[Activity(Label = "Merseyrail", Theme = "@android:style/Theme.Black.NoTitleBar", MainLauncher = false, ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
public class PreferencesAlertActivity : FragmentActivity
{
	private ISettingsService settingsService;

	private Alert alert;

	private Button btnTimeFrom;

	private Button btnTimeTo;

	private Button btnStationFrom;

	private Button btnStationTo;

	private CheckBox chkMonday;

	private CheckBox chkTuesday;

	private CheckBox chkWednesday;

	private CheckBox chkThurday;

	private CheckBox chkFriday;

	private CheckBox chkSaturday;

	private CheckBox chkSunday;

	private Button btnSave;

	private Button btnDelete;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361830);
		long longExtra = Intent!.GetLongExtra("alertId", 0L);
		string stringExtra = Intent!.GetStringExtra("alertJson");
		if (longExtra > 0)
		{
			alert = JsonConvert.DeserializeObject<Alert>(stringExtra);
		}
		else
		{
			alert = new Alert();
			alert.StartTime = new TimeSpan(7, 30, 0);
			alert.EndTime = new TimeSpan(9, 30, 0);
		}
		settingsService = SharedServices.IoC.Resolve<ISettingsService>();
		StationService stationService = SharedServices.IoC.Resolve<StationService>();
		if (alert.StartCRS != null)
		{
			alert.StartStation = stationService.GetStationByCrs(alert.StartCRS, withDescription: false);
		}
		if (alert.EndCRS != null)
		{
			alert.EndStation = stationService.GetStationByCrs(alert.EndCRS, withDescription: false);
		}
		FindViewById<ImageButton>(2131230785)!.Click += delegate
		{
			Finish();
		};
		btnTimeFrom = FindViewById<Button>(2131230799);
		btnTimeFrom.Text = alert.StartTime.ToString();
		btnTimeFrom.Click += delegate
		{
			using Android.Support.V4.App.FragmentTransaction transaction2 = SupportFragmentManager.BeginTransaction();
			TimePickerFragment timePickerFragment2 = new TimePickerFragment();
			timePickerFragment2.OnTimeSetEvent += delegate(object tse_sender, TimePickerDialog.TimeSetEventArgs tse_e)
			{
				alert.StartTime = new TimeSpan(tse_e.HourOfDay, tse_e.Minute, 0);
				btnTimeFrom.Text = alert.StartTime.ToString("hh\\:mm\\:ss");
			};
			timePickerFragment2.Show(transaction2, "timeFrom");
		};
		btnTimeTo = FindViewById<Button>(2131230808);
		btnTimeTo.Text = alert.EndTime.ToString();
		btnTimeTo.Click += delegate
		{
			using Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
			TimePickerFragment timePickerFragment = new TimePickerFragment();
			timePickerFragment.OnTimeSetEvent += delegate(object tse_sender, TimePickerDialog.TimeSetEventArgs tse_e)
			{
				alert.EndTime = new TimeSpan(tse_e.HourOfDay, tse_e.Minute, 0);
				btnTimeTo.Text = alert.EndTime.ToString("hh\\:mm\\:ss");
			};
			timePickerFragment.Show(transaction, "timeTo");
		};
		btnStationFrom = FindViewById<Button>(2131230798);
		if (alert.StartStation != null)
		{
			btnStationFrom.Text = alert.StartStation.Name;
		}
		else
		{
			btnStationFrom.Text = "";
		}
		btnStationFrom.Click += delegate
		{
			Intent intent2 = new Intent(this, typeof(StationSelectorActivity));
			StartActivityForResult(intent2, 7001);
		};
		btnStationTo = FindViewById<Button>(2131230807);
		if (alert.EndStation != null)
		{
			btnStationTo.Text = alert.EndStation.Name;
		}
		else
		{
			btnStationTo.Text = "";
		}
		btnStationTo.Click += delegate
		{
			Intent intent = new Intent(this, typeof(StationSelectorActivity));
			StartActivityForResult(intent, 7002);
		};
		chkMonday = FindViewById<CheckBox>(2131230826);
		chkMonday.Checked = IsDayChecked(alert, 0);
		chkTuesday = FindViewById<CheckBox>(2131230830);
		chkTuesday.Checked = IsDayChecked(alert, 1);
		chkWednesday = FindViewById<CheckBox>(2131230831);
		chkWednesday.Checked = IsDayChecked(alert, 2);
		chkThurday = FindViewById<CheckBox>(2131230829);
		chkThurday.Checked = IsDayChecked(alert, 3);
		chkFriday = FindViewById<CheckBox>(2131230825);
		chkFriday.Checked = IsDayChecked(alert, 4);
		chkSaturday = FindViewById<CheckBox>(2131230827);
		chkSaturday.Checked = IsDayChecked(alert, 5);
		chkSunday = FindViewById<CheckBox>(2131230828);
		chkSunday.Checked = IsDayChecked(alert, 6);
		btnSave = FindViewById<Button>(2131230803);
		btnSave.Click += delegate
		{
			string firebaseToken2 = SharedSettings.FirebaseToken;
			alert.DaysOfWeek = GetDayBitwise();
			if (canSave())
			{
				SharedServices.IoC.Resolve<AlertService>().SaveAlert(firebaseToken2, GetAppGuid(), DeviceType.Android, alert, delegate(bool done)
				{
					if (done)
					{
						RunOnUiThread(delegate
						{
							Toast.MakeText(this, "Alert saved.", ToastLength.Short)!.Show();
						});
						Finish();
					}
					else
					{
						RunOnUiThread(delegate
						{
							Toast.MakeText(this, "Alert could not be saved. Please try again.", ToastLength.Short)!.Show();
						});
					}
				});
			}
			else
			{
				RunOnUiThread(delegate
				{
					Toast.MakeText(this, "Alert could not be saved. Please ensure you have completed the form.", ToastLength.Short)!.Show();
				});
			}
		};
		FindViewById<ImageButton>(2131230785)!.Click += delegate
		{
			Finish();
		};
		btnDelete = FindViewById<Button>(2131230796);
		if (alert.Id < 1)
		{
			btnDelete.Visibility = ViewStates.Invisible;
		}
		else
		{
			btnDelete.Visibility = ViewStates.Visible;
		}
		btnDelete.Click += delegate
		{
			new AlertDialog.Builder(this).SetMessage("Delete this alert?")!.SetPositiveButton("Ok", delegate
			{
				string firebaseToken = SharedSettings.FirebaseToken;
				SharedServices.IoC.Resolve<AlertService>().DeleteAlert(firebaseToken, GetAppGuid(), DeviceType.Android, alert, delegate(bool done)
				{
					if (done)
					{
						RunOnUiThread(delegate
						{
							Toast.MakeText(this, "Alert deleted.", ToastLength.Short)!.Show();
						});
						Finish();
					}
					else
					{
						RunOnUiThread(delegate
						{
							Toast.MakeText(this, "Alert could not be deleted. please try again.", ToastLength.Short)!.Show();
						});
					}
				});
			})!.SetNegativeButton("Cancel", delegate
			{
				RunOnUiThread(delegate
				{
					Toast.MakeText(this, "Alert not deleted.", ToastLength.Short)!.Show();
				});
			})!.SetTitle("Confirm alert delete")!.Show();
		};
	}

	private void AlertUpdated(bool success)
	{
		Intent intent = new Intent("glow.merseyrail.AlertsUpdated");
		intent.PutExtra("success", success);
		Android.App.Application.Context.SendBroadcast(intent);
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

	private bool canSave()
	{
		if ((chkMonday.Checked || chkTuesday.Checked || chkWednesday.Checked || chkThurday.Checked || chkFriday.Checked || chkSaturday.Checked || chkSunday.Checked) && alert.StartStation != null && alert.EndStation != null && !string.IsNullOrEmpty(alert.StartCRS) && !string.IsNullOrEmpty(alert.EndCRS))
		{
			return alert.StartCRS != alert.EndCRS;
		}
		return false;
	}

	private int GetDayBitwise()
	{
		int num = 0;
		num = (chkMonday.Checked ? (num + Convert.ToInt32("01000000", 2)) : num);
		num = (chkTuesday.Checked ? (num + Convert.ToInt32("00100000", 2)) : num);
		num = (chkWednesday.Checked ? (num + Convert.ToInt32("00010000", 2)) : num);
		num = (chkThurday.Checked ? (num + Convert.ToInt32("00001000", 2)) : num);
		num = (chkFriday.Checked ? (num + Convert.ToInt32("00000100", 2)) : num);
		num = (chkSaturday.Checked ? (num + Convert.ToInt32("00000010", 2)) : num);
		return chkSunday.Checked ? (num + Convert.ToInt32("00000001", 2)) : num;
	}

	protected override void OnResume()
	{
		base.OnResume();
		CalamityHelpers.CheckForCalamity(this);
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		base.OnActivityResult(requestCode, resultCode, data);
		if (requestCode == 7001 && resultCode == Result.Ok)
		{
			string result = data.GetStringExtra("result");
			StationService stationService = SharedServices.IoC.Resolve<StationService>();
			alert.StartStation = (from x in stationService.GetStations(merseyrailOnly: true)
				where x.Name == result
				select x).SingleOrDefault();
			alert.StartCRS = alert.StartStation.CrsCode;
			btnStationFrom.Text = alert.StartStation.Name;
		}
		if (requestCode == 7002 && resultCode == Result.Ok)
		{
			string result2 = data.GetStringExtra("result");
			StationService stationService2 = SharedServices.IoC.Resolve<StationService>();
			alert.EndStation = (from x in stationService2.GetStations(merseyrailOnly: true)
				where x.Name == result2
				select x).SingleOrDefault();
			alert.EndCRS = alert.EndStation.CrsCode;
			btnStationTo.Text = alert.EndStation.Name;
		}
	}

	private bool IsDayChecked(Alert alert, int dayIndex)
	{
		if (alert == null)
		{
			return false;
		}
		return (alert.DaysOfWeek & Alert.DaysLookup.Keys.ElementAt(dayIndex)) > 0;
	}
}
