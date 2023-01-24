using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common;
using Common.Domain;
using Common.Services;
using Merseyrail.Helpers;
using Merseyrail.Shared;
using TinyIoC;

namespace Merseyrail;

[Activity(Label = "PreferencesProfileActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
public class PreferencesProfileActivity : Activity
{
	private ISettingsService settingService;

	private ProfileService profileService;

	private StationService stationService;

	private UserProfile userProfile;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		settingService = TinyIoCContainer.Current.Resolve<ISettingsService>();
		profileService = TinyIoCContainer.Current.Resolve<ProfileService>();
		stationService = TinyIoCContainer.Current.Resolve<StationService>();
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361905);
		FindViewById<ImageButton>(2131230795)!.Click += delegate
		{
			Finish();
		};
		FindViewById<TextView>(2131231085)!.Click += delegate
		{
			string uriString = "https://www.merseyrail.org/privacy.aspx";
			Intent intent = new Intent("android.intent.action.VIEW");
			intent.SetData(Android.Net.Uri.Parse(uriString));
			StartActivity(intent);
		};
		GetProfileFromServer();
		CloseKeyboard();
	}

	protected override void OnResume()
	{
		base.OnResume();
		CalamityHelpers.CheckForCalamity(this);
	}

	private void GetProfileFromServer()
	{
		ShowProgress();
		try
		{
			GetUserProfile().ContinueWith(delegate(Task<UserProfile> task)
			{
				userProfile = task.Result;
				RunOnUiThread(delegate
				{
					PutFormValues();
					HideProgress();
				});
			});
		}
		catch (Exception)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("The profile information could not be loaded at this time. Please check your internet connection and try again.");
			alert.SetPositiveButton("Ok", delegate
			{
				Finish();
			});
			RunOnUiThread(delegate
			{
				alert.Show();
			});
		}
	}

	private void PutFormValues()
	{
		FindViewById<EditText>(2131231261)!.Text = userProfile.FirstName ?? string.Empty;
		FindViewById<EditText>(2131231263)!.Text = userProfile.Surname ?? string.Empty;
		FindViewById<EditText>(2131231260)!.Text = userProfile.Email ?? string.Empty;
		FindViewById<EditText>(2131231262)!.Text = userProfile.Postcode ?? string.Empty;
		Button? button = FindViewById<Button>(2131230814);
		button!.Text = ((userProfile.StartStation != null) ? userProfile.StartStation.Name : "Select start station");
		button!.Click += delegate
		{
			Intent intent2 = new Intent(this, typeof(StationSelectorActivity));
			StartActivityForResult(intent2, 615);
		};
		Button? button2 = FindViewById<Button>(2131230813);
		button2!.Text = ((userProfile.EndStation != null) ? userProfile.EndStation.Name : "Select end station");
		button2!.Click += delegate
		{
			Intent intent = new Intent(this, typeof(StationSelectorActivity));
			StartActivityForResult(intent, 625);
		};
		List<string> list = new List<string>();
		Spinner spn_profs_i_travel = FindViewById<Spinner>(2131231162);
		spn_profs_i_travel.Prompt = "Select your usuals end station";
		list = Utils.GetDescriptionArrayFromEnumValue<TravelFrequency>().ToList();
		if (!userProfile.TravelReason.HasValue)
		{
			list.Insert(0, "Please Select");
		}
		ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, 2131361906, list.ToArray());
		arrayAdapter.SetDropDownViewResource(2131361906);
		spn_profs_i_travel.Adapter = arrayAdapter;
		if (userProfile.TravelReason.HasValue)
		{
			spn_profs_i_travel.SetSelection((int)userProfile.TravelFrequency.Value);
		}
		spn_profs_i_travel.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			string text = (string?)spn_profs_i_travel.Adapter!.GetItem(e.Position);
			if (text != "Please Select" && !string.IsNullOrEmpty(text))
			{
				userProfile.TravelFrequency = Utils.GetEnumValueFromDescription<TravelFrequency>(text);
			}
		};
		List<string> list2 = new List<string>();
		Spinner spn_profs_travel_usage = FindViewById<Spinner>(2131231163);
		list2 = Utils.GetDescriptionArrayFromEnumValue<TravelReason>().ToList();
		if (string.IsNullOrEmpty(userProfile.TravelReason.ToString()))
		{
			list2.Insert(0, "Please Select");
		}
		ArrayAdapter<string> arrayAdapter2 = new ArrayAdapter<string>(this, 2131361906, list2.ToArray());
		arrayAdapter2.SetDropDownViewResource(2131361906);
		spn_profs_travel_usage.Adapter = arrayAdapter2;
		spn_profs_travel_usage.Prompt = "Select your travel usage";
		if (!string.IsNullOrEmpty(userProfile.TravelReason.ToString()))
		{
			spn_profs_travel_usage.SetSelection(arrayAdapter2.GetPosition(userProfile.TravelReason.ToString()));
		}
		spn_profs_travel_usage.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			if ((string?)spn_profs_travel_usage.Adapter!.GetItem(e.Position) != "Please Select")
			{
				TravelReason enumValueFromDescription = Utils.GetEnumValueFromDescription<TravelReason>(((Spinner)sender).SelectedItem!.ToString());
				userProfile.TravelReason = enumValueFromDescription;
			}
		};
		CheckBox? checkBox = FindViewById<CheckBox>(2131230822);
		checkBox!.Checked = userProfile.OptedInDataGathering;
		checkBox!.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			userProfile.OptedInDataGathering = e.IsChecked;
		};
		CheckBox? checkBox2 = FindViewById<CheckBox>(2131230823);
		checkBox2!.Checked = userProfile.OptedInMarketing;
		checkBox2!.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			userProfile.OptedInMarketing = e.IsChecked;
		};
		FindViewById<Button>(2131230802)!.Click += async delegate
		{
			if (ValidToSave())
			{
				await SaveUserProfile();
				Toast.MakeText(this, "Your profile has been updated", ToastLength.Long)!.Show();
				Finish();
			}
			else
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Can not save profile");
				alert.SetMessage("Please ensure you have made both 'How often' and 'Reason for travel' selections");
				alert.SetPositiveButton("Ok", delegate
				{
				});
				RunOnUiThread(delegate
				{
					alert.Show();
				});
			}
		};
	}

	private bool ValidToSave()
	{
		if (userProfile.TravelReason.HasValue)
		{
			return userProfile.TravelFrequency.HasValue;
		}
		return false;
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		switch (requestCode)
		{
		case 615:
			if (resultCode == Result.Ok)
			{
				data.GetStringExtra("result");
				string crscode2 = data.GetStringExtra("crscode");
				Station station2 = (from x in stationService.GetStations(merseyrailOnly: true)
					where x.CrsCode == crscode2
					select x).FirstOrDefault();
				userProfile.StartStation = station2;
				userProfile.StartStationCRS = station2.CrsCode;
				FindViewById<Button>(2131230814)!.Text = station2.Name;
			}
			break;
		case 625:
			if (resultCode == Result.Ok)
			{
				data.GetStringExtra("result");
				string crscode = data.GetStringExtra("crscode");
				Station station = (from x in stationService.GetStations(merseyrailOnly: true)
					where x.CrsCode == crscode
					select x).FirstOrDefault();
				userProfile.EndStation = station;
				userProfile.EndStationCRS = station.CrsCode;
				FindViewById<Button>(2131230813)!.Text = station.Name;
			}
			break;
		}
	}

	private void GetFormValues()
	{
		userProfile.InstanceId = SharedSettings.DeviceInstanceId;
		userProfile.DeviceId = SharedSettings.FirebaseToken;
		userProfile.FirstName = FindViewById<EditText>(2131231261)!.Text;
		userProfile.Surname = FindViewById<EditText>(2131231263)!.Text;
		userProfile.Email = FindViewById<EditText>(2131231260)!.Text;
		userProfile.Postcode = FindViewById<EditText>(2131231262)!.Text;
	}

	private async Task<UserProfile> GetUserProfile()
	{
		string deviceInstanceId = SharedSettings.DeviceInstanceId;
		string firebaseToken = SharedSettings.FirebaseToken;
		return await profileService.LoadProfile(deviceInstanceId, firebaseToken, DeviceType.Android);
	}

	private async Task SaveUserProfile()
	{
		FindViewById<Button>(2131230802)!.Enabled = false;
		CloseKeyboard();
		ShowProgress();
		GetFormValues();
		await profileService.SaveProfile(userProfile);
		HideProgress();
		FindViewById<Button>(2131230802)!.Enabled = true;
	}

	private void CloseKeyboard()
	{
		RunOnUiThread(delegate
		{
			FindViewById<EditText>(2131231261)!.ClearFocus();
			Window!.SetSoftInputMode(SoftInput.StateHidden);
		});
	}

	private void ShowProgress()
	{
		RunOnUiThread(delegate
		{
			FindViewById<LinearLayout>(2131231088)!.Visibility = ViewStates.Visible;
		});
	}

	private void HideProgress()
	{
		RunOnUiThread(delegate
		{
			FindViewById<LinearLayout>(2131231088)!.Visibility = ViewStates.Invisible;
		});
	}
}
