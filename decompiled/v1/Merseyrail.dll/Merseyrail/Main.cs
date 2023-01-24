using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Analytics;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Common;
using Common.Analytics;
using Common.Domain;
using Common.Services;
using Merseyrail.Activities;
using Merseyrail.Fragments;
using Merseyrail.Helpers;
using Merseyrail.Services;
using Merseyrail.Shared;
using Microsoft.AppCenter.Crashes;

namespace Merseyrail;

[Activity(Label = "Merseyrail", MainLauncher = false, ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
public class Main : FragmentActivity
{
	private LocationService locationService;

	private const int RequestLocationId = 0;

	private FragmentService FragmentService => SharedServices.IoC.Resolve<FragmentService>();

	public static bool IsActive { get; set; }

	public DrawerLayout drawerLayout { get; set; }

	public BaseFragment CurrentFragment
	{
		get
		{
			return (BaseFragment)SharedValues.CurrentFragment;
		}
		set
		{
			SharedValues.CurrentFragment = value;
		}
	}

	public string locationProvider { get; set; }

	public Tracker Tracker { get; set; }

	public Dictionary<string, object> Sections { get; set; }

	public Main()
	{
		AppInitHelpers.AppIoCInit();
		FragmentService.Initialise(SupportFragmentManager, 2131230726);
		Sections = new Dictionary<string, object>();
	}

	private void GetLocation()
	{
		locationService = SharedServices.IoC.Resolve<LocationService>();
		locationService.OnLocationUpdated += delegate(object sender, Location e)
		{
			Location currentDeviceLocation = SharedValues.CurrentDeviceLocation;
			SharedValues.CurrentDeviceLocation = e;
			if (currentDeviceLocation == null && CurrentFragment is Departures)
			{
				((Departures)CurrentFragment).OnLocationPermissionGranted();
			}
		};
	}

	protected override void OnStart()
	{
		try
		{
			InitGoogleAnalytics();
			ShowSavedStation();
			base.OnStart();
			IsActive = true;
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		IsActive = false;
	}

	public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
	{
		if (requestCode == 0 && grantResults != null && grantResults.Any())
		{
			if (grantResults[0] == Permission.Granted)
			{
				GetLocation();
			}
			else
			{
				ShowWarningDialog();
			}
		}
	}

	private void ShowWarningDialog()
	{
		AlertDialog.Builder builder = new AlertDialog.Builder(this);
		builder.SetTitle("ATTENTION");
		builder.SetMessage("The application requires device location. Please enable this permission in your device settings");
		builder.SetPositiveButton("Ok", delegate
		{
		});
		builder.Show();
	}

	public override void OnBackPressed()
	{
		if (drawerLayout.IsDrawerOpen(8388611))
		{
			drawerLayout.CloseDrawers();
		}
		else if (CurrentFragment != null && (CurrentFragment.PreviousFragment == null || CurrentFragment.PreviousFragment == typeof(Departures)))
		{
			if (CurrentFragment is Departures)
			{
				Process.KillProcess(Process.MyPid());
			}
			else
			{
				ShowDepartures();
			}
		}
		else if (CurrentFragment != null)
		{
			GoToPreviousFragment();
		}
	}

	private void GoToPreviousFragment()
	{
		if (CurrentFragment.PreviousFragment == typeof(JourneyPlanner))
		{
			ShowJourneyPlanner();
		}
	}

	private void InitGoogleAnalytics()
	{
		GoogleAnalytics instance = GoogleAnalytics.GetInstance(ApplicationContext);
		Tracker = instance.NewTracker("UA-50042330-1");
		instance.ReportActivityStart(this);
		instance.SetLocalDispatchPeriod(5);
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
		{
			Dictionary<string, string> p = new Dictionary<string, string> { 
			{
				"Merseyrail Android App Exception: ",
				string.Format("EXCEPTION: {0}. MESSAGE: {1} INNER_EXCEPTION: {2}", e.ToString(), ((Exception)e.ExceptionObject).Message, (((Exception)e.ExceptionObject).InnerException != null) ? ((Exception)e.ExceptionObject).InnerException.Message : "none")
			} };
			Tracker.Send(p);
		};
	}

	protected override void OnNewIntent(Intent intent)
	{
		intent.GetStringExtra("glow.merseyrail.widget.STATION_SELECTED_NAME");
		intent.GetStringExtra("glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID");
		base.OnNewIntent(intent);
	}

	protected override void OnCreate(Bundle bundle)
	{
		try
		{
			base.OnCreate(bundle);
			if (PermissionHelper.HasLocationPermission(this))
			{
				GetLocation();
			}
			else
			{
				PermissionHelper.GetLocationPermission(this, 0);
			}
			if (bundle != null)
			{
				bundle.GetString("glow.merseyrail.widget.STATION_SELECTED_NAME", "");
				string @string = bundle.GetString("glow.merseyrail.widget.STATION_SELECTED_TRAIN_ID", "");
				if (SharedSettings.SettingsService != null)
				{
					SharedSettings.SettingsService.SaveSetting("Widget_Selected_Train_Id", @string);
				}
			}
			AndroidEnvironment.UnhandledExceptionRaiser += delegate(object sender, RaiseThrowableEventArgs e)
			{
				if (Tracker != null)
				{
					EasyException(e.Exception.Message + " : " + e.Exception.ToString(), isfatal: true);
				}
			};
			RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView(2131361883);
			drawerLayout = FindViewById<DrawerLayout>(2131230855);
			drawerLayout.DrawerOpened += delegate
			{
				((RainbowBoard)SupportFragmentManager.FindFragmentById(2131231092))?.UpdateRainbowBoard();
			};
			SetMenuButtonActions();
			InitTrainReminderServiceButton();
			CurrentFragment = (Departures)SupportFragmentManager.FindFragmentById(2131230725);
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
	}

	protected override void OnResume()
	{
		try
		{
			base.OnResume();
			SetTheme(16973833);
			CalamityHelpers.CheckForCalamity(this);
			locationService?.ConnectClient();
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
	}

	protected override void OnStop()
	{
		locationService?.DisconnectClient();
		GoogleAnalytics.GetInstance(this).ReportActivityStop(this);
		base.OnStop();
	}

	private void InitTrainReminderServiceButton()
	{
		TrainReminderService trainReminderService = SharedServices.IoC.Resolve<TrainReminderService>();
		trainReminderService.ReminderActivated += delegate
		{
			ShowActiveReminderButton();
		};
		trainReminderService.ReminderDeactivated += delegate
		{
			HideActiveReminderButton();
		};
		if (HasActiveReminder())
		{
			ShowActiveReminderButton();
		}
		else
		{
			HideActiveReminderButton();
		}
	}

	public void ToggleDrawerLayout()
	{
		View view = drawerLayout.FindViewById(2131230856);
		if (drawerLayout.IsDrawerOpen(view))
		{
			drawerLayout.CloseDrawers();
		}
		else
		{
			drawerLayout.OpenDrawer(view);
		}
	}

	public void OpenDrawerLayout()
	{
		View drawerView = drawerLayout.FindViewById(2131230856);
		drawerLayout.OpenDrawer(drawerView);
	}

	public void CloseDrawerLayout()
	{
		drawerLayout.CloseDrawers();
	}

	private void SetMenuButtonActions()
	{
		MenuFragment menuFragment = (MenuFragment)SupportFragmentManager.FindFragmentById(2131231018);
		menuFragment.BtnActiveReminder.Click += delegate
		{
			ShowLiveTrainAlert();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "ShowLiveTrainAlert selected by user.");
		};
		menuFragment.BtnDepartures.OnClick += delegate
		{
			ShowDepartures();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "Departures selected by user.");
		};
		menuFragment.BtnLiveMap.OnClick += delegate
		{
			ShowLiveMap();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "LiveMap selected by user.");
		};
		if (Utils.DisruptionActive())
		{
			menuFragment.BtnPlatformUpgrades.OnClick += delegate
			{
				ShowPlatformUpgrades();
				ToggleDrawerLayout();
			};
		}
		menuFragment.BtnJourneyPlanner.OnClick += delegate
		{
			ShowJourneyPlanner();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "JourneyPlanner selected by user.");
		};
		menuFragment.BtnFeedback.OnClick += delegate
		{
			ShowFeedback();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "Feedback selected by user.");
		};
		menuFragment.BtnSettings.OnClick += delegate
		{
			ShowPrefs();
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "Settings selected by user.");
		};
		menuFragment.BtnMtogo.OnClick += delegate
		{
			StartActivity(typeof(MtogoActivity));
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "Motogo Offers selected by user");
		};
		menuFragment.BtnNewTrains.OnClick += delegate
		{
			StartActivity(typeof(FaqsActivity));
			ToggleDrawerLayout();
			EasyTrack(EventCategory.Interaction, EventAction.Click, EventLabel.Navigate, "New Trains selected by user");
		};
		if (menuFragment.BtnDeveloperArea != null)
		{
			menuFragment.BtnDeveloperArea.OnClick += delegate
			{
				Intent intent = new Intent(this, typeof(DeveloperActivity));
				StartActivity(intent);
			};
		}
	}

	private bool HasActiveReminder()
	{
		return SharedServices.IoC.Resolve<TrainReminderService>().GetActiveReminder()?.IsActive ?? false;
	}

	private void ShowActiveReminderButton()
	{
		RunOnUiThread(delegate
		{
			((MenuFragment)SupportFragmentManager.FindFragmentById(2131231018)).BtnActiveReminder.Visibility = ViewStates.Visible;
		});
	}

	private void HideActiveReminderButton()
	{
		RunOnUiThread(delegate
		{
			((MenuFragment)SupportFragmentManager.FindFragmentById(2131231018)).BtnActiveReminder.Visibility = ViewStates.Gone;
		});
	}

	public PagedFragmentSection GetSectionByKey(string sectionKey)
	{
		if (!Sections.Any((KeyValuePair<string, object> x) => x.Key == sectionKey))
		{
			return null;
		}
		return (PagedFragmentSection)Sections.SingleOrDefault((KeyValuePair<string, object> x) => x.Key == sectionKey).Value;
	}

	private void ShowLiveTrainAlert()
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "livetrainalert")
		{
			FragmentService.ClearBackstack();
			CurrentFragment = FragmentService.InitSection<LiveTrainAlert>("livetrainalert");
		}
	}

	private void ShowDepartures()
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "departures")
		{
			FragmentService.ClearBackstack();
			CurrentFragment = FragmentService.InitSection<Departures>("departures");
		}
		ShowSavedStation();
	}

	private void ShowSavedStation()
	{
		string setting = SharedSettings.SettingsService.GetSetting("selectedStation", string.Empty);
		if (!string.IsNullOrEmpty(setting) && CurrentFragment is Departures)
		{
			((Departures)CurrentFragment).SetSelectedStation(setting);
		}
	}

	private void ShowLiveMap()
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "livemap")
		{
			CurrentFragment = FragmentService.InitSection<LiveMap>("livemap");
		}
	}

	public void ShowPlatformUpgrades()
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "webView")
		{
			if (SharedSettings.HasConnection(Android.App.Application.Context))
			{
				Bundle bundle = new Bundle();
				bundle.PutString("url", "http://XXXXXX/");
				bundle.PutBoolean("showButtons", value: false);
				CurrentFragment = FragmentService.InitSection<WebViewFragment>(bundle, "webView");
			}
			else
			{
				Toast.MakeText(Android.App.Application.Context, "An internet or data connection is required to view this", ToastLength.Short)!.Show();
			}
		}
	}

	public void ShowLiveMap(string stationName)
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "LiveMap")
		{
			Bundle bundle = new Bundle();
			bundle.PutString("passedStationName", stationName);
			CurrentFragment = FragmentService.InitSection<LiveMap>(bundle, "livemap");
		}
	}

	private void ShowJourneyPlanner()
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "JourneyPlanner")
		{
			CurrentFragment = FragmentService.InitSection<JourneyPlanner>("journeyplanner");
		}
	}

	public void ShowWebView(string url)
	{
		if (CurrentFragment == null || CurrentFragment.Tag != "WebView")
		{
			FragmentService.ClearBackstack();
			Bundle bundle = new Bundle();
			bundle.PutString("url", url);
			CurrentFragment = FragmentService.InitSection<WebViewFragment>(bundle, "webView");
		}
	}

	public void ShowJourneyPlannerDetail(Journey journey)
	{
		if (CurrentFragment == null || CurrentFragment.GetType().Name != "JourneyPlannerDetail")
		{
			CurrentFragment = FragmentService.InitSection<JourneyPlannerDetail>("journeyplannerdetail");
		}
		((JourneyPlannerDetail)CurrentFragment).SelectedJourney = journey;
	}

	private void ShowFeedback()
	{
		FragmentService.ClearBackstack();
		Intent intent = new Intent(this, typeof(ReportAProblem));
		StartActivityForResult(intent, 411);
	}

	private void ShowPrefs()
	{
		StartActivity(typeof(PreferencesActivity));
	}

	public void ShowDepartureStation(string station)
	{
		if (CurrentFragment == null || CurrentFragment.GetType().Name != "Departures")
		{
			CurrentFragment = FragmentService.InitSection<Departures>("departures");
		}
		((Departures)CurrentFragment).SetSelectedStation(station);
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		if (requestCode == 111 && resultCode == Result.Ok)
		{
			string stringExtra = data.GetStringExtra("result");
			if (!string.IsNullOrEmpty(stringExtra))
			{
				SharedSettings.SettingsService.Editor.PutString("selectedStation", stringExtra);
				SharedSettings.SettingsService.Editor.Apply();
				SharedSettings.SettingsService.Editor.Commit();
				((Departures)CurrentFragment).ClearDepartureItemsList();
				((Departures)CurrentFragment).SetSelectedStation(stringExtra);
			}
			else
			{
				Toast.MakeText(this, "Could not set selected station.", ToastLength.Long);
			}
		}
		if (requestCode == 211)
		{
			_ = -1;
		}
		if (requestCode == 311 && resultCode == Result.Ok)
		{
			string stringExtra2 = data.GetStringExtra("result");
			data.GetStringExtra("crscode");
			if (CurrentFragment.GetType().Name != "JourneyPlanner")
			{
				CurrentFragment = FragmentService.InitSection<JourneyPlanner>("journeyplanner");
			}
			((JourneyPlanner)CurrentFragment).SetSelectedFromStation(stringExtra2);
		}
		if (requestCode == 322 && resultCode == Result.Ok)
		{
			string stringExtra3 = data.GetStringExtra("result");
			data.GetStringExtra("crscode");
			if (CurrentFragment.GetType().Name != "JourneyPlanner")
			{
				CurrentFragment = FragmentService.InitSection<JourneyPlanner>("journeyplanner");
			}
			((JourneyPlanner)CurrentFragment).SetSelectedToStation(stringExtra3);
		}
		if (requestCode == 411)
		{
			_ = -1;
		}
		if (requestCode == 622 && resultCode == Result.Ok)
		{
			data.GetStringExtra("result");
			if (CurrentFragment.GetType().Name != "Settings")
			{
				CurrentFragment = FragmentService.InitSection<SettingsV2>("settings");
			}
		}
		_ = 611;
		base.OnActivityResult(requestCode, resultCode, data);
	}

	private int ConvertPixelsToDp(float pixelValue)
	{
		return (int)(pixelValue * Resources!.DisplayMetrics!.Density);
	}

	public void EasyTrack(EventCategory category, EventAction action, EventLabel label, string description)
	{
		if (Tracker != null)
		{
			HitBuilders.EventBuilder eventBuilder = new HitBuilders.EventBuilder(category.ToString(), action.ToString()).SetLabel(label.ToString()).SetValue(DateTime.Now.Ticks);
			Tracker.Send(eventBuilder.Build());
		}
	}

	public void EasyException(string description, bool isfatal)
	{
		if (Tracker != null)
		{
			HitBuilders.ExceptionBuilder exceptionBuilder = new HitBuilders.ExceptionBuilder().SetDescription(description).SetFatal(isfatal);
			Tracker.Send(exceptionBuilder.Build());
		}
	}

	public void ShowAlertDialogue(string message, string title = null, string buttonText = null, string buttonUrl = null)
	{
		RunOnUiThread(delegate
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle(title ?? "Alert");
			builder.SetMessage(message);
			builder.SetCancelable(cancelable: false);
			builder.SetPositiveButton(buttonText ?? "OK", delegate
			{
				if (buttonUrl != null)
				{
					Android.Net.Uri uri = Android.Net.Uri.Parse(buttonUrl);
					Intent intent = new Intent("android.intent.action.VIEW", uri);
					StartActivity(intent);
				}
			});
			if (!string.IsNullOrEmpty(buttonUrl))
			{
				builder.SetNegativeButton("OK", delegate
				{
				});
			}
			builder.Show();
		});
	}
}
