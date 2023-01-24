using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.Animations;
using Android.Webkit;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Java.Interop;
using Merseyrail.BroadcastReceivers;
using Merseyrail.Services;
using Merseyrail.Shared;
using Merseyrail.Views;
using Microsoft.AppCenter.Crashes;

namespace Merseyrail.Fragments;

[IntentFilter(new string[] { "android.intent.action.MAIN" }, Categories = new string[] { "SectionFragment" })]
public class Departures : BaseFragment, ISensorEventListener, IJavaObject, IDisposable, IJavaPeerable
{
	private WidgetStationSelectedReceiver stationSelectedReceiver;

	private DeparturesListItemClickReceiver departuresListItemClickReceiver;

	private HelpDisplay helpDisplay;

	private static readonly LatLng Liverpool_1 = new LatLng(53.403012, -2.984521);

	private static readonly LatLng Liverpool_2 = new LatLng(53.3174, -2.957);

	private float[] sensorValues = new float[3];

	private bool actionsBound;

	private View _view;

	private string bannerUrl;

	private bool bannerActionsSet;

	private int timeOffset_minutes;

	private bool DepartureListActionsSet;

	private LatLng SelectedLatLng { get; set; }

	private LatLng DeviceLatLng { get; set; }

	private WebView closedStationBannerView { get; set; }

	private TrainJourneyList _departuresList { get; set; }

	private float bearing { get; set; }

	private float distanceTo { get; set; }

	private string selected_CRS_Code { get; set; }

	private SensorManager sensorManager { get; set; }

	private TextView distanceTextView { get; set; }

	private Button btn_StationSelector { get; set; }

	private ImageButton btnBack { get; set; }

	private Button btnPrev { get; set; }

	private Button btnNext { get; set; }

	private LinearLayout MapareaDepartures { get; set; }

	private ImageView compass_Arrow { get; set; }

	private ImageView adImage { get; set; }

	public ImageView btn_StationInfo { get; set; }

	public ImageView RefreshButton { get; set; }

	public TextView ListHeader { get; set; }

	public Station SelectedStation { get; set; }

	private List<TimeSpan> prevPageTimes { get; set; }

	private RelativeLayout WarningPanel { get; set; }

	private TextView WarningMessage { get; set; }

	private TextView LatenessTime { get; set; }

	private bool _actionsApplied { get; set; }

	public float fromDeg { get; set; }

	public float toDeg { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		DeviceLatLng = Liverpool_1;
		SelectedLatLng = Liverpool_2;
		selected_CRS_Code = "LVC";
		sensorManager = (SensorManager)base.Activity.GetSystemService("sensor");
		Sensor defaultSensor = sensorManager.GetDefaultSensor(SensorType.Orientation);
		sensorManager.RegisterListener(this, defaultSensor, SensorDelay.Normal);
		InitWidgetListener();
		InitDeparturesListItemClickReceiver();
	}

	private void InitWidgetListener()
	{
		if (stationSelectedReceiver == null)
		{
			stationSelectedReceiver = new WidgetStationSelectedReceiver();
			base.Activity.RegisterReceiver(stationSelectedReceiver, new IntentFilter("glow.merseyrail.WidgetStationSelected"));
			if (stationSelectedReceiver != null)
			{
				stationSelectedReceiver.OnWidgetStationSelected += WidgetStationSelected;
			}
		}
	}

	private void WidgetStationSelected(object sender, WidgetStationSelectedEventArgs args)
	{
		SetSelectedStation(args.StationName);
		string.IsNullOrEmpty(args.TrainId);
		SharedSettings.SettingsService.SaveSetting("Widget_Selected_Train_Id", args.TrainId);
	}

	private void InitDeparturesListItemClickReceiver()
	{
		if (departuresListItemClickReceiver == null)
		{
			departuresListItemClickReceiver = new DeparturesListItemClickReceiver();
			base.Activity.RegisterReceiver(departuresListItemClickReceiver, new IntentFilter("glow.merseyrail.DeparturesListItemClickReceiver"));
			if (departuresListItemClickReceiver != null)
			{
				departuresListItemClickReceiver.OnDepartureBoardItemSelected += DepartureSelected;
			}
		}
	}

	private void UnregisterDeparturesListItemClickReceiver()
	{
		if (departuresListItemClickReceiver != null)
		{
			base.Activity.UnregisterReceiver(departuresListItemClickReceiver);
			if (departuresListItemClickReceiver != null)
			{
				departuresListItemClickReceiver.OnDepartureBoardItemSelected -= DepartureSelected;
			}
			departuresListItemClickReceiver = null;
		}
	}

	private void DepartureSelected(object sender, DepartureBoardItem e)
	{
		if (!string.IsNullOrEmpty(e.TrainId))
		{
			ShowTrain(e.TrainId);
		}
	}

	public void RefreshListData(PageDirection pageDirection)
	{
		bool islive = SharedSettings.HasMobileDataConnection(base.Activity);
		BindDepartures(selected_CRS_Code, islive, pageDirection);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		try
		{
			if (_view == null)
			{
				_view = inflater.Inflate(2131361839, container, attachToRoot: false);
				closedStationBannerView = (WebView)_view.FindViewById(2131230833);
				closedStationBannerView.Visibility = ViewStates.Gone;
				compass_Arrow = (ImageView)_view.FindViewById(2131230836);
				MapareaDepartures = (LinearLayout)_view.FindViewById(2131231015);
				MapareaDepartures.Click += delegate
				{
					((Main)base.Activity).ShowLiveMap(SelectedStation.Name);
				};
				distanceTextView = (TextView)_view.FindViewById(2131230837);
				ListHeader = (TextView)_view.FindViewById(2131230848);
				RefreshButton = (ImageView)_view.FindViewById(2131230847);
				btnBack = (ImageButton)_view.FindViewById(2131230801);
				btn_StationInfo = (ImageView)_view.FindViewById(2131230805);
				btn_StationSelector = _view.FindViewById<Button>(2131230806);
				btnPrev = (Button)_view.FindViewById(2131230810);
				btnNext = (Button)_view.FindViewById(2131230809);
				WarningPanel = _view.FindViewById<RelativeLayout>(2131231271);
				WarningPanel.Visibility = ViewStates.Gone;
				adImage = (ImageView)_view.FindViewById(2131230845);
			}
			BindActions();
			Task.Delay(2000);
			ShowHelp();
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
		return _view;
	}

	private void ShowHelp()
	{
		bool flag = !SharedSettings.HasViewedHelpItem(base.Class.Name);
		if (helpDisplay == null && flag && _view != null)
		{
			_view.Post(delegate
			{
				InitHelpDisplay();
			});
		}
	}

	private List<HelpDisplayItem> GetHelpItemList()
	{
		return new List<HelpDisplayItem>
		{
			HelpDisplay.NewHelpItem("help_btn_PageHeader_BackButton", _view.FindViewById(2131230801), HelpDisplay.HelpItemLayoutHint.Center, "Tap or swipe right to access the main menu"),
			HelpDisplay.NewHelpItem("help_refreshbutton", RefreshButton, HelpDisplay.HelpItemLayoutHint.Center, "Tap to get latest departures information"),
			HelpDisplay.NewHelpItem("help_stationinfo", btn_StationInfo, HelpDisplay.HelpItemLayoutHint.Center, "View more information about the selected station")
		};
	}

	private void InitHelpDisplay()
	{
		if (_view != null)
		{
			helpDisplay = helpDisplay ?? new HelpDisplay(this, GetHelpItemList());
			helpDisplay.ShowHelp();
		}
	}

	private void BindActions()
	{
		if (actionsBound)
		{
			return;
		}
		btnBack.Click += delegate
		{
			((Main)base.Activity).ToggleDrawerLayout();
		};
		RefreshButton.Click += delegate
		{
			RefreshListData(PageDirection.None);
		};
		btnPrev.Click += delegate
		{
			PagePrevMove();
		};
		btnNext.Click += delegate
		{
			PageNextMove();
		};
		btn_StationSelector.Click += delegate
		{
			Intent intent = new Intent(base.Activity, typeof(StationSelectorActivity));
			base.Activity.StartActivityForResult(intent, 111);
		};
		btn_StationInfo.Click += delegate
		{
			if (SelectedStation != null)
			{
				StationDetail stationDetail = SharedServices.IoC.Resolve<FragmentService>().InitSection<StationDetail>("stationdetail");
				stationDetail.StationName = SelectedStation.Name;
				stationDetail.StationHTMLInfo = SelectedStation.HTMLInfo;
				((Main)base.Activity).CurrentFragment = stationDetail;
			}
		};
		actionsBound = true;
	}

	private void PagePrevMove()
	{
		_departuresList = base.ChildFragmentManager.FindFragmentByTag("departuresList_departures") as TrainJourneyList;
		if (_departuresList != null)
		{
			if (_departuresList.CurrentPage > 0)
			{
				_departuresList.CurrentPage--;
			}
			RefreshListData(PageDirection.Down);
		}
	}

	private void PageNextMove()
	{
		_departuresList = base.ChildFragmentManager.FindFragmentByTag("departuresList_departures") as TrainJourneyList;
		if (_departuresList != null)
		{
			_departuresList.CurrentPage++;
			RefreshListData(PageDirection.Up);
		}
	}

	public override void OnStart()
	{
		base.OnStart();
		InitWidgetListener();
		OnLocationPermissionGranted();
	}

	public void OnLocationPermissionGranted()
	{
		if (SharedValues.CurrentDeviceLocation != null)
		{
			DeviceLatLng = new LatLng(SharedValues.CurrentDeviceLocation.Latitude, SharedValues.CurrentDeviceLocation.Longitude);
			GetCurrentBearing();
		}
	}

	public override void OnResume()
	{
		try
		{
			base.OnResume();
			InitDeparturesListItemClickReceiver();
			BindActions();
			((Main)base.Activity).CurrentFragment = this;
			SelectDefaultStation();
			SelectTrainIfWidget();
			RefreshListData(PageDirection.None);
			InitBannerAd();
		}
		catch (Exception exception)
		{
			Crashes.TrackError(exception);
			throw;
		}
	}

	public override void OnPause()
	{
		UnregisterDeparturesListItemClickReceiver();
		base.OnPause();
	}

	public override void OnDestroy()
	{
		if (stationSelectedReceiver != null)
		{
			stationSelectedReceiver.OnWidgetStationSelected -= WidgetStationSelected;
		}
		base.OnDestroy();
	}

	private void SelectTrainIfWidget()
	{
		if (!string.IsNullOrEmpty(SharedSettings.SettingsService.GetSetting("Widget_Selected_Train_Id", string.Empty)))
		{
			SharedSettings.SettingsService.SaveSetting("Widget_Selected_Train_Id", string.Empty);
		}
	}

	private void InitBannerAd()
	{
		SharedServices.IoC.Resolve<BannerService>().GetBanners(delegate(List<Banner> bl)
		{
			if (bl != null && bl.Count > 0)
			{
				int num = new Random().Next(1, bl.Count);
				Banner banner = bl[num - 1];
				Bitmap img = BitmapFactory.DecodeByteArray(banner.Image, 0, banner.Image.Length);
				int w = base.Resources.DisplayMetrics!.WidthPixels;
				if (img != null)
				{
					float num2 = (float)img.Height / (float)img.Width;
					int h = (int)(num2 * (float)w);
					base.Activity.RunOnUiThread(delegate
					{
						adImage.Visibility = ViewStates.Visible;
						adImage.SetImageBitmap(img);
						adImage.LayoutParameters!.Width = w;
						adImage.LayoutParameters!.Height = h;
						bannerUrl = banner.Url;
						if (!bannerActionsSet)
						{
							adImage.Click += delegate
							{
								SetBannerUrlAction();
							};
							bannerActionsSet = true;
						}
						InitHelpDisplay();
					});
				}
				else
				{
					base.Activity.RunOnUiThread(delegate
					{
						adImage.Visibility = ViewStates.Gone;
						InitHelpDisplay();
					});
				}
			}
			else
			{
				base.Activity.RunOnUiThread(delegate
				{
					adImage.Visibility = ViewStates.Gone;
					InitHelpDisplay();
				});
			}
		});
	}

	private void SetBannerUrlAction()
	{
		Intent intent = new Intent("android.intent.action.VIEW", Android.Net.Uri.Parse(bannerUrl));
		base.Activity.StartActivity(intent);
	}

	private void SelectDefaultStation()
	{
		if (!WidgetStationIfClicked())
		{
			if (SelectedStation == null)
			{
				SetSelectedStation("Liverpool Central");
			}
			else
			{
				SetSelectedStation(SelectedStation.Name);
			}
		}
		SetStationButtonText(SelectedStation.Name);
	}

	private void LoadMapImage(LatLng pos)
	{
		if (MapareaDepartures == null)
		{
			return;
		}
		int widthPixels = Application.Context.Resources!.DisplayMetrics!.WidthPixels;
		int num = (int)((double)widthPixels / 2.909);
		MapareaDepartures.LayoutParameters!.Width = widthPixels;
		MapareaDepartures.LayoutParameters!.Height = num;
		LoadMapBackgroundImage(pos, 15, widthPixels, num, delegate(Drawable bd)
		{
			if (base.Activity != null)
			{
				MapareaDepartures.SetBackgroundDrawable(bd);
			}
		});
	}

	private async void LoadImageAsync(string url, ImageView mapImage)
	{
		Bitmap imageBitmap = BitmapFactory.DecodeStream(new MemoryStream(await new WebClient().DownloadDataTaskAsync(url)));
		mapImage.SetImageBitmap(imageBitmap);
	}

	private void LoadMapBackgroundImage(LatLng pos, int zoom, int mapWidth, int mapHeight, Action<Drawable> process)
	{
		mapWidth /= 2;
		mapHeight /= 2;
		string url = $"http://maps.googleapis.com/maps/api/staticmap?center={pos.Latitude},{pos.Longitude}&zoom={zoom}&size={mapWidth}x{mapHeight}&sensor=false&scale=2";
		LoadBitmapDrawableAsync(url, process);
	}

	private async void LoadBitmapDrawableAsync(string url, Action<Drawable> process)
	{
		try
		{
			Drawable obj = Drawable.CreateFromStream(new MemoryStream(await new WebClient().DownloadDataTaskAsync(url)), "googleMaps");
			process(obj);
		}
		catch (Exception)
		{
			process?.Invoke(base.Resources.GetDrawable(2131165305));
		}
	}

	public void ClearDepartureItemsList()
	{
		if (_departuresList != null)
		{
			_departuresList = base.ChildFragmentManager.FindFragmentByTag("departuresList_departures") as TrainJourneyList;
			_departuresList.ClearList();
		}
	}

	public void SetSelectedStation(string stationName)
	{
		IEnumerable<Station> source = from x in SharedServices.IoC.Resolve<StationService>().GetStations(merseyrailOnly: true)
			where x.Name == stationName
			select x;
		if (source.Count() > 0)
		{
			SetStationButtonText(stationName);
			Station station2 = (SelectedStation = source.Single());
			SelectedLatLng = new LatLng(station2.Lat, station2.Lon);
			LoadMapImage(new LatLng(station2.Lat, station2.Lon));
			selected_CRS_Code = station2.CrsCode;
		}
	}

	private void SetStationButtonText(string stationName)
	{
		if (base.Activity == null)
		{
			return;
		}
		base.Activity.RunOnUiThread(delegate
		{
			if (btn_StationSelector != null)
			{
				btn_StationSelector.Text = stationName;
			}
		});
	}

	private void StartUpdatingAnim()
	{
		base.Activity.RunOnUiThread(delegate
		{
			RotateAnimation animation = new RotateAnimation(180f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f)
			{
				Duration = 500L,
				RepeatMode = RepeatMode.Restart,
				RepeatCount = -1
			};
			if (RefreshButton != null)
			{
				RefreshButton.StartAnimation(animation);
			}
		});
	}

	private void StopUpdatingAnim()
	{
		base.Activity.RunOnUiThread(delegate
		{
			RefreshButton.ClearAnimation();
		});
	}

	public void BindDepartures(string crsCode, bool islive, PageDirection pageDirection)
	{
		StartUpdatingAnim();
		DepartureBoardService departureBoardService = SharedServices.IoC.Resolve<DepartureBoardService>();
		_departuresList = base.ChildFragmentManager.FindFragmentByTag("departuresList_departures") as TrainJourneyList;
		if (_departuresList == null)
		{
			departureBoardService.GetDepartureBoard(crsCode, islive, 0, 24, delegate(DepartureBoard board)
			{
				if (board != null)
				{
					if (board.IsClosed)
					{
						ShowClosedStationBanner(board);
					}
					else if (board.DepartureStationCRS == crsCode)
					{
						base.Activity.RunOnUiThread(delegate
						{
							CreateTrainJourneyListFragment(board);
						});
						SetLabelListText(false);
						StopUpdatingAnim();
					}
				}
			}, timeOffset_minutes);
			return;
		}
		List<DepartureBoardItem> trainJourneyListItems = _departuresList.TrainJourneyListItems;
		if (prevPageTimes == null)
		{
			prevPageTimes = new List<TimeSpan>();
		}
		if (trainJourneyListItems.Count > 0)
		{
			if (pageDirection == PageDirection.Up && (prevPageTimes.Count < 1 || (trainJourneyListItems.First().DepartureSchedule != prevPageTimes.Last() && trainJourneyListItems.Count > 9)))
			{
				prevPageTimes.Add(trainJourneyListItems.First().DepartureSchedule);
				timeOffset_minutes = (int)(trainJourneyListItems.Last().DepartureSchedule.TotalMinutes - DateTime.Now.TimeOfDay.TotalMinutes);
			}
			if (pageDirection == PageDirection.Down)
			{
				timeOffset_minutes = ((prevPageTimes.Count > 0) ? ((int)(prevPageTimes.Last().TotalMinutes - DateTime.Now.TimeOfDay.TotalMinutes)) : 0);
				if (prevPageTimes.Count > 0)
				{
					prevPageTimes.Remove(prevPageTimes.Last());
				}
			}
		}
		departureBoardService.GetDepartureBoard(crsCode, islive, _departuresList.Offset, _departuresList.PageSize, delegate(DepartureBoard board)
		{
			if (board != null && board.DepartureStationCRS == crsCode)
			{
				if (board.IsClosed)
				{
					ShowClosedStationBanner(board);
				}
				else
				{
					InitTrainJourneyListView();
					_departuresList.ListView.Post(delegate
					{
						_departuresList.UpdateList(board);
					});
					StopUpdatingAnim();
					SetLabelListText();
				}
			}
		}, timeOffset_minutes);
	}

	private void CreateTrainJourneyListFragment(DepartureBoard board)
	{
		try
		{
			InitTrainJourneyListView();
			using Android.Support.V4.App.FragmentTransaction fragmentTransaction = base.ChildFragmentManager.BeginTransaction();
			_departuresList = new TrainJourneyList(board);
			fragmentTransaction.Add(2131230846, _departuresList, "departuresList_departures");
			fragmentTransaction.Commit();
		}
		catch (Exception)
		{
		}
	}

	private void ShowClosedStationBanner(DepartureBoard board)
	{
		base.Activity.RunOnUiThread(delegate
		{
			string text = "<html><head><title>Example</title><meta name=\"viewport\"\"content=\"width=" + Android.Runtime.Extensions.JavaCast<IWindowManager>(base.Activity.GetSystemService("window"))!.DefaultDisplay!.Width + ", initial-scale=0.65 \" /></head>";
			if (board != null)
			{
				text = text + "<body style='margin:0;padding:0;border:0;'><center><a href=\"" + board.BannerUrl + "\"><img style=\"margin:0;padding:0;border:0;width:100%;\" src=\"" + board.BannerImage + "\" /></a></center></body></html>";
			}
			closedStationBannerView.LoadData(text, "text/html", null);
			closedStationBannerView.RequestFocus();
			StopUpdatingAnim();
			InitClosedStationView();
		});
	}

	private void InitClosedStationView()
	{
		_view.FindViewById(2131230846)!.Visibility = ViewStates.Gone;
		_view.FindViewById(2131231078)!.Visibility = ViewStates.Gone;
		closedStationBannerView.Visibility = ViewStates.Visible;
	}

	private void InitTrainJourneyListView()
	{
		base.Activity.RunOnUiThread(delegate
		{
			_view.FindViewById(2131230846)!.Visibility = ViewStates.Visible;
			_view.FindViewById(2131231078)!.Visibility = ViewStates.Visible;
			closedStationBannerView.Visibility = ViewStates.Gone;
		});
	}

	private void SetLabelListText(bool? forceIslive)
	{
		base.Activity.RunOnUiThread(delegate
		{
			if (forceIslive.Value)
			{
				ListHeader.Text = $"Live Information, last updated {DateTime.Now.ToShortTimeString():hh:mm}";
			}
			else
			{
				ListHeader.Text = $"Based on scheduled trains, refresh for live information";
			}
		});
	}

	private void SetLabelListText()
	{
		base.Activity.RunOnUiThread(delegate
		{
			if (SharedSettings.HasMobileDataConnection(base.Activity))
			{
				ListHeader.Text = $"Live Information, last updated {DateTime.Now.ToShortTimeString():hh:mm}";
			}
			else
			{
				ListHeader.Text = $"Based on scheduled trains, refresh for live information";
			}
		});
	}

	public void DepartureListActions()
	{
		if (!DepartureListActionsSet)
		{
			DepartureListActionsSet = true;
		}
	}

	public static void ShowTrain(string trainId)
	{
		try
		{
			LiveTrain liveTrain = SharedServices.IoC.Resolve<FragmentService>().InitSection<LiveTrain>("livetrain");
			if (liveTrain != null)
			{
				liveTrain.TrainId = trainId;
			}
		}
		catch (Exception)
		{
		}
	}

	public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
	{
	}

	public void OnSensorChanged(SensorEvent e)
	{
		_ = e.Values;
		if (e.Sensor!.Type == SensorType.Orientation)
		{
			for (int i = 0; i < 3; i++)
			{
				sensorValues[i] = e.Values![i];
			}
			GetCurrentBearing();
			bearing -= sensorValues[0];
			if (bearing < 0f)
			{
				bearing += 360f;
			}
			int direction = (int)bearing;
			((Arrow)compass_Arrow).setDirection(direction);
			if (distanceTextView != null)
			{
				distanceTextView.Text = distanceTo.ToString();
			}
		}
	}

	public float getDistanceInMeters(LatLng p1, LatLng p2)
	{
		double startLatitude = p1.Latitude;
		double startLongitude = p1.Longitude;
		double endLatitude = p2.Latitude;
		double endLongitude = p2.Longitude;
		float[] array = new float[1];
		Location.DistanceBetween(startLatitude, startLongitude, endLatitude, endLongitude, array);
		return array[0];
	}

	private double DegreesToRadians(double x)
	{
		return Math.PI * x / 180.0;
	}

	private double RadiansToDegrees(double x)
	{
		return x * 180.0 / Math.PI;
	}

	private float RadiansToDegrees(float x)
	{
		return (float)((double)x * 180.0 / Math.PI);
	}

	private void GetCurrentBearing()
	{
		Location location = new Location("")
		{
			Latitude = DeviceLatLng.Latitude,
			Longitude = DeviceLatLng.Longitude
		};
		Location dest = new Location("")
		{
			Latitude = SelectedLatLng.Latitude,
			Longitude = SelectedLatLng.Longitude
		};
		float num2 = (bearing = location.BearingTo(dest));
		distanceTo = location.DistanceTo(dest);
	}

	private double GetHeadingForDirectionFromCoordinate(LatLng fromLoc, LatLng toLoc)
	{
		double num = DegreesToRadians(fromLoc.Latitude);
		double num2 = DegreesToRadians(fromLoc.Longitude);
		double num3 = DegreesToRadians(toLoc.Latitude);
		double num4 = DegreesToRadians(toLoc.Longitude);
		double num5 = RadiansToDegrees(Math.Atan2(Math.Sin(num4 - num2) * Math.Cos(num3), Math.Cos(num) * Math.Sin(num3) - Math.Sin(num) * Math.Cos(num3) * Math.Cos(num4 - num2)));
		if (num5 >= 0.0)
		{
			return num5;
		}
		return 360.0 + num5;
	}

	public decimal DmsToDD(double d, double m = 0.0, double s = 0.0)
	{
		return Convert.ToDecimal((d + m / 60.0 + s / 3600.0) * (double)((!(d < 0.0)) ? 1 : (-1)));
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		helpDisplay = null;
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}

	public bool WidgetStationIfClicked()
	{
		string setting = SharedSettings.SettingsService.GetSetting("Widget_Selected_Station", string.Empty);
		if (!string.IsNullOrEmpty(setting))
		{
			SetSelectedStation(setting);
			SharedSettings.SettingsService.SaveSetting("Widget_Selected_Station", string.Empty);
			return true;
		}
		return false;
	}
}
