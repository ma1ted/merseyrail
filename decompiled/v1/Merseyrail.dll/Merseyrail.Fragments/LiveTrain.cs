using System;
using System.Collections.Generic;
using System.Timers;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Adapters;
using Merseyrail.Services;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class LiveTrain : BaseFragment, IOnMapReadyCallback, IJavaObject, IDisposable
{
	private HelpDisplay helpDisplay;

	public SupportMapFragment mapFragment;

	private GoogleMap _map;

	private bool actionsBound;

	private View _view;

	private bool networkPlotted;

	public string TrainId { get; set; }

	public ListView StationslistView { get; set; }

	public List<Marker> CurrentMarkers { get; set; }

	public ImageButton BackButton { get; set; }

	public ImageButton ShareButton { get; set; }

	public ImageButton AlertButton { get; set; }

	public TextView Header { get; set; }

	public TextView DestinationName { get; set; }

	public TextView LastUpdated { get; set; }

	private Timer TrainTimer { get; set; }

	private LatLng TrainPosition { get; set; }

	public Marker TrainMarker { get; set; }

	public MarkerOptions TrainMarkerOptions { get; set; }

	private DepartureBoardService departureBoardService { get; set; }

	private LiveMapService liveMapService { get; set; }

	public LiveTrain()
	{
		departureBoardService = SharedServices.IoC.Resolve<DepartureBoardService>();
		liveMapService = SharedServices.IoC.Resolve<LiveMapService>();
	}

	private bool IsShareable(string trainId)
	{
		int result = 0;
		return !int.TryParse(trainId, out result);
	}

	private bool IsRemindable(string trainId)
	{
		if (IsShareable(trainId))
		{
			return SharedValues.CurrentDeviceLocation != null;
		}
		return false;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		CurrentMarkers = new List<Marker>();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361853, null, attachToRoot: false);
		}
		BackButton = (ImageButton)_view.FindViewById(2131230999);
		ShareButton = (ImageButton)_view.FindViewById(2131231001);
		AlertButton = (ImageButton)_view.FindViewById(2131230994);
		Header = (TextView)_view.FindViewById(2131231000);
		StationslistView = (ListView)_view.FindViewById(2131231002);
		DestinationName = (TextView)_view.FindViewById(2131230995);
		LastUpdated = (TextView)_view.FindViewById(2131230997);
		InitMapFragment();
		ShowHelp();
		((Main)base.Activity).CurrentFragment = this;
		return _view;
	}

	private void InitMapFragment()
	{
		mapFragment = base.ChildFragmentManager.FindFragmentByTag("map_livetrain") as SupportMapFragment;
		if (mapFragment == null)
		{
			GoogleMapOptions options = new GoogleMapOptions().InvokeMapType(1).InvokeZoomControlsEnabled(enabled: true).InvokeCompassEnabled(enabled: true)
				.InvokeUseViewLifecycleInFragment(useViewLifecycleInFragment: true);
			FragmentTransaction fragmentTransaction = base.ChildFragmentManager.BeginTransaction();
			mapFragment = SupportMapFragment.NewInstance(options);
			mapFragment.RetainInstance = true;
			mapFragment.GetMapAsync(this);
			fragmentTransaction.Add(2131230998, mapFragment, "map_livetrain");
			fragmentTransaction.Commit();
		}
	}

	public override void OnStart()
	{
		mapFragment.View.Post(delegate
		{
			UpdateView();
		});
		base.OnStart();
	}

	public override void OnResume()
	{
		base.OnResume();
		if (actionsBound)
		{
			return;
		}
		BackButton.Click += delegate
		{
			((Main)base.Activity).OnBackPressed();
		};
		ShareButton.Click += delegate
		{
			if (ShareButton.Alpha == 1f)
			{
				Intent intent = new Intent(base.Activity, typeof(ShareTrain));
				intent.PutExtra("trainid", TrainId);
				base.Activity.StartActivity(intent);
			}
		};
		AlertButton.Click += delegate
		{
			if (AlertButton.Alpha == 1f)
			{
				SharedServices.IoC.Resolve<FragmentService>().InitSection<LiveTrainAlert>("livetrainalert").TrainId = TrainId;
			}
		};
		actionsBound = true;
	}

	public void UpdateView()
	{
		if (!string.IsNullOrEmpty(TrainId))
		{
			departureBoardService.GetTrainProgress(TrainId, UpdateViewContent);
		}
	}

	public void UpdateViewContent(TrainProgress progress)
	{
		base.Activity.RunOnUiThread(delegate
		{
			NullifyTrainMarker();
			if (TrainTimer != null)
			{
				TrainTimer.Dispose();
			}
			TrainTimer = new Timer(2000.0);
			TrainTimer.Elapsed += delegate
			{
				GeoPoint positionForTrain = liveMapService.GetPositionForTrain(progress);
				if (positionForTrain != null)
				{
					TrainPosition = new LatLng(positionForTrain.Lat, positionForTrain.Lon);
					InitTrainMarker();
					UpdateTrainMarker();
				}
			};
			TrainTimer.Start();
			if (StationslistView != null)
			{
				LiveTrainStationsListAdapter adapter = new LiveTrainStationsListAdapter(progress, base.Activity, base.ChildFragmentManager);
				StationslistView.Adapter = adapter;
			}
			if (Header != null)
			{
				Header.Text = "Live Train";
			}
			if (DestinationName != null)
			{
				DestinationName.Text = progress.DestinationStation.Name;
			}
			if (LastUpdated != null)
			{
				LastUpdated.Text = DateTime.Now.ToShortTimeString();
			}
			PlotProgress(progress);
			SetButtonVisibility();
		});
	}

	private void NullifyTrainMarker()
	{
		if (TrainMarker != null)
		{
			TrainMarker.Remove();
			TrainMarker = null;
		}
	}

	public void InitTrainMarker()
	{
		if (TrainMarkerOptions == null)
		{
			TrainMarkerOptions = new MarkerOptions();
			TrainMarkerOptions.SetTitle("Train position");
			TrainMarkerOptions.SetPosition(new LatLng(TrainPosition.Latitude, TrainPosition.Longitude));
			TrainMarkerOptions.InvokeIcon(BitmapDescriptorFactory.FromResource(2131165400));
		}
		base.Activity.RunOnUiThread(delegate
		{
			if (TrainMarker == null && _map != null)
			{
				TrainMarker = _map.AddMarker(TrainMarkerOptions);
				CameraPosition cameraPosition = new CameraPosition.Builder().Target(TrainPosition).Zoom(15f).Bearing(0f)
					.Tilt(65f)
					.Build();
				_map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 2000, null);
			}
		});
	}

	private void UpdateTrainMarker()
	{
		if (TrainMarker == null)
		{
			return;
		}
		base.Activity.RunOnUiThread(delegate
		{
			TrainMarker.Position = TrainPosition;
			TrainMarker.Visible = true;
			Location obj = new Location("")
			{
				Latitude = _map.CameraPosition.Target.Latitude,
				Longitude = _map.CameraPosition.Target.Longitude
			};
			Location dest = new Location("")
			{
				Latitude = TrainPosition.Latitude,
				Longitude = TrainPosition.Longitude
			};
			float bearing = obj.BearingTo(dest);
			if (obj.DistanceTo(dest) > 10f)
			{
				CameraPosition cameraPosition = new CameraPosition.Builder().Target(TrainPosition).Zoom(_map.CameraPosition.Zoom).Bearing(bearing)
					.Tilt(65f)
					.Build();
				_map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 2000, null);
			}
		});
	}

	private void SetButtonVisibility()
	{
		AlertButton.Alpha = (IsRemindable(TrainId) ? 1f : 0.1f);
		ShareButton.Alpha = (IsShareable(TrainId) ? 1f : 0.1f);
	}

	public void PlotMarker(LatLng latLng, string markerTitle)
	{
		if (_map != null)
		{
			MarkerOptions markerOptions = new MarkerOptions();
			markerOptions.SetPosition(latLng);
			markerOptions.SetTitle(markerTitle);
			markerOptions.InvokeIcon(BitmapDescriptorFactory.FromResource(2131165394));
			Marker item = _map.AddMarker(markerOptions);
			CurrentMarkers.Add(item);
		}
	}

	private void PlotProgress(TrainProgress progress)
	{
		if (!networkPlotted)
		{
			if (mapFragment != null && _map != null)
			{
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034230).Add(new LatLng(53.4088708, -3.0687992), new LatLng(53.4077899, -3.0629089), new LatLng(53.4074703, -3.0618652), new LatLng(53.407093, -3.061223), new LatLng(53.4069078, -3.0610043), new LatLng(53.405917, -3.0601905), new LatLng(53.4054455, -3.0596704), new LatLng(53.4051685, -3.0591942), new LatLng(53.4046513, -3.0580038), new LatLng(53.4021611, -3.0519699), new LatLng(53.4017978, -3.0510679), new LatLng(53.4017978, -3.0510679), new LatLng(53.4015129, -3.0502852), new LatLng(53.4015129, -3.0502852), new LatLng(53.400636, -3.0475948), new LatLng(53.3996689, -3.0441772), new LatLng(53.399361, -3.0435198), new LatLng(53.3992236, -3.0433122), new LatLng(53.3992236, -3.0433122), new LatLng(53.3987524, -3.0426948), new LatLng(53.3987524, -3.0426948), new LatLng(53.39865, -3.0425609), new LatLng(53.3982191, -3.0418401), new LatLng(53.397871, -3.0409825), new LatLng(53.3975304, -3.0398804), new LatLng(53.3971833, -3.03851), new LatLng(53.3969516, -3.0372983), new LatLng(53.396796, -3.0357637), new LatLng(53.396796, -3.0357637), new LatLng(53.3967483, -3.0352517), new LatLng(53.3965373, -3.0341324), new LatLng(53.3952585, -3.0296255), new LatLng(53.393397, -3.0227837), new LatLng(53.3924215, -3.0192527), new LatLng(53.3922752, -3.0184613), new LatLng(53.3922611, -3.0174246), new LatLng(53.3924027, -3.0164908), new LatLng(53.3925589, -3.0160211), new LatLng(53.3925589, -3.0160211), new LatLng(53.39309, -3.0150803), new LatLng(53.3941600451948, -3.01422889771269), new LatLng(53.3947651619612, -3.01349664547039), new LatLng(53.3951686, -3.0126244), new LatLng(53.3959789, -3.0107913), new LatLng(53.3959789, -3.0107913), new LatLng(53.4031846, -2.9951461), new LatLng(53.4031846, -2.9951461), new LatLng(53.4045689143244, -2.99235774304306), new LatLng(53.4045689143244, -2.99235774304306), new LatLng(53.4051032, -2.990959), new LatLng(53.4052505, -2.990239), new LatLng(53.4052422, -2.9893195), new LatLng(53.404681, -2.9832664), new LatLng(53.4046171, -2.9818395), new LatLng(53.4045084, -2.9807988), new LatLng(53.4042973, -2.9796508), new LatLng(53.4042973, -2.9785135), new LatLng(53.404438, -2.9777947), new LatLng(53.4046619, -2.9771939), new LatLng(53.4049177, -2.9767325), new LatLng(53.4052055, -2.9763785), new LatLng(53.4055317, -2.976121), new LatLng(53.4061777, -2.9759279), new LatLng(53.4067853, -2.9761103), new LatLng(53.4074185, -2.9767647), new LatLng(53.4085134051492, -2.97905577989876), new LatLng(53.4110447, -2.9842535), new LatLng(53.4111894, -2.9847126), new LatLng(53.4113133, -2.9853478), new LatLng(53.4113453, -2.9860559), new LatLng(53.4113005, -2.9866353), new LatLng(53.4111214, -2.9874292), new LatLng(53.4109104, -2.9880086), new LatLng(53.4106418, -2.9884699), new LatLng(53.4102197, -2.9889098), new LatLng(53.4097592, -2.9891136), new LatLng(53.4087296, -2.9891887), new LatLng(53.4071946, -2.9891351), new LatLng(53.4067469, -2.9893067), new LatLng(53.4064207, -2.9895857), new LatLng(53.405698, -2.990841), new LatLng(53.4045689143244, -2.99235774304306)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034230).Add(new LatLng(53.1970765766306, -2.87936870166765), new LatLng(53.1974423, -2.8803276), new LatLng(53.1977958, -2.8822588), new LatLng(53.1977958, -2.8822588), new LatLng(53.1978636, -2.8824919), new LatLng(53.1981236, -2.8833853), new LatLng(53.1981236, -2.8833853), new LatLng(53.1987467, -2.8849569), new LatLng(53.1989725, -2.8853883), new LatLng(53.1993243, -2.8858244), new LatLng(53.199968, -2.8862011), new LatLng(53.2008124, -2.8863409), new LatLng(53.2043838, -2.8889743), new LatLng(53.2065616, -2.8903274), new LatLng(53.2081184, -2.8912948), new LatLng(53.2091783, -2.8919391), new LatLng(53.2091783, -2.8919391), new LatLng(53.2093163, -2.8920255), new LatLng(53.2093163, -2.8920255), new LatLng(53.214813, -2.895335), new LatLng(53.2153723, -2.8957626), new LatLng(53.2260847, -2.9064074), new LatLng(53.228498, -2.9088058), new LatLng(53.228498, -2.9088058), new LatLng(53.2287224, -2.9090739), new LatLng(53.2287224, -2.9090739), new LatLng(53.233009, -2.9139673), new LatLng(53.2389936, -2.9202017), new LatLng(53.2443962, -2.9257652), new LatLng(53.2443962, -2.9257652), new LatLng(53.2447946, -2.9262105), new LatLng(53.2447946, -2.9262105), new LatLng(53.2491851, -2.9308144), new LatLng(53.2509485, -2.9325807), new LatLng(53.2600057, -2.9418991), new LatLng(53.2600057, -2.9418991), new LatLng(53.2798458, -2.96613), new LatLng(53.2808627, -2.9671511), new LatLng(53.2832817, -2.969388), new LatLng(53.2862135, -2.9715958), new LatLng(53.2921289, -2.9750938), new LatLng(53.2921289, -2.9750938), new LatLng(53.2944901, -2.9761423), new LatLng(53.2952307, -2.976613), new LatLng(53.2968447, -2.9772687), new LatLng(53.3043324, -2.9800119), new LatLng(53.3075789, -2.9812049), new LatLng(53.3078456, -2.9812993), new LatLng(53.3119944, -2.9829301), new LatLng(53.3131174, -2.9833936), new LatLng(53.3152095, -2.9842347), new LatLng(53.3176912, -2.9852132), new LatLng(53.320876, -2.9864294), new LatLng(53.3218288, -2.9867925), new LatLng(53.3297601, -2.9899485), new LatLng(53.3309741, -2.9904575), new LatLng(53.3350196, -2.9920621), new LatLng(53.3350196, -2.9920621), new LatLng(53.3352912, -2.9921637), new LatLng(53.3352912, -2.9921637), new LatLng(53.3400533, -2.9940655), new LatLng(53.3422194, -2.9949421), new LatLng(53.3469245, -2.9974185), new LatLng(53.3475357, -2.997745), new LatLng(53.3475357, -2.997745), new LatLng(53.3476919, -2.9978219), new LatLng(53.3476919, -2.9978219), new LatLng(53.3490641, -2.9984812), new LatLng(53.3501304, -2.9990422), new LatLng(53.3501304, -2.9990422), new LatLng(53.3503223, -2.9991361), new LatLng(53.3503223, -2.9991361), new LatLng(53.3532526, -3.0007308), new LatLng(53.3569305, -3.002676), new LatLng(53.3569305, -3.002676), new LatLng(53.357058, -3.0027402), new LatLng(53.357058, -3.0027402), new LatLng(53.3576418, -3.0030315), new LatLng(53.3592183, -3.0038237), new LatLng(53.3623195, -3.0052177), new LatLng(53.3677433, -3.0079386), new LatLng(53.3719874, -3.0104417), new LatLng(53.3727898, -3.0108336), new LatLng(53.3727898, -3.0108336), new LatLng(53.3745454, -3.0116673), new LatLng(53.3754617, -3.0120475), new LatLng(53.3754617, -3.0120475), new LatLng(53.3757274, -3.0121743), new LatLng(53.3757274, -3.0121743), new LatLng(53.37684, -3.0127722), new LatLng(53.3789762, -3.0138665), new LatLng(53.3789762, -3.0138665), new LatLng(53.3791694, -3.0139652), new LatLng(53.3791694, -3.0139652), new LatLng(53.3796425, -3.0142075), new LatLng(53.3800729, -3.0145561), new LatLng(53.381288, -3.0151406), new LatLng(53.3824787, -3.0157309), new LatLng(53.3824787, -3.0157309), new LatLng(53.3827631, -3.0158758), new LatLng(53.3827631, -3.0158758), new LatLng(53.3830922, -3.0160582), new LatLng(53.3830922, -3.0160582), new LatLng(53.3833266, -3.0161778), new LatLng(53.3838321, -3.0164498), new LatLng(53.3844817, -3.0173065), new LatLng(53.3847469, -3.0177767), new LatLng(53.385655, -3.0201006), new LatLng(53.385867, -3.020532), new LatLng(53.3861099, -3.0208456), new LatLng(53.3863539, -3.0211251), new LatLng(53.3863539, -3.0211251), new LatLng(53.3865634, -3.0212436), new LatLng(53.3869585, -3.0213841), new LatLng(53.3875451, -3.0213259), new LatLng(53.388559, -3.0206233), new LatLng(53.3888343, -3.0202933), new LatLng(53.3888343, -3.0202933), new LatLng(53.3890057, -3.019956), new LatLng(53.3897853, -3.018116), new LatLng(53.3902859, -3.0171961), new LatLng(53.3904939, -3.0169776), new LatLng(53.39309, -3.0150803)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034230).Add(new LatLng(53.2921289, -2.9750938), new LatLng(53.2899073, -2.9724901), new LatLng(53.2889769, -2.9709794), new LatLng(53.2882502, -2.9692832), new LatLng(53.2878048, -2.9681092), new LatLng(53.2872484, -2.9660838), new LatLng(53.2868199, -2.9635977), new LatLng(53.2868199, -2.9635977), new LatLng(53.2853961, -2.944042), new LatLng(53.2844566, -2.9298691), new LatLng(53.2840288, -2.9241159), new LatLng(53.2840288, -2.9241159), new LatLng(53.282261, -2.8982843), new LatLng(53.2821484, -2.8963918)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034230).Add(new LatLng(53.4374252445649, -3.04814376947437), new LatLng(53.4373220514661, -3.04891355609653), new LatLng(53.4373050631082, -3.04971886118076), new LatLng(53.4375143, -3.0544677), new LatLng(53.4375007, -3.0559049), new LatLng(53.4373922, -3.0565905), new LatLng(53.4371171, -3.0575569), new LatLng(53.4356993, -3.0607951), new LatLng(53.4356371, -3.0609871), new LatLng(53.4356371, -3.0609871), new LatLng(53.4355572, -3.061251), new LatLng(53.4355572, -3.061251), new LatLng(53.4347332, -3.0649369), new LatLng(53.4346808, -3.0651531), new LatLng(53.4346808, -3.0651531), new LatLng(53.4346088, -3.0654386), new LatLng(53.4346088, -3.0654386), new LatLng(53.4345621, -3.065604), new LatLng(53.4340913, -3.0668839), new LatLng(53.433545, -3.0679059), new LatLng(53.4328969, -3.0687321), new LatLng(53.432047, -3.069472), new LatLng(53.431292, -3.0698266), new LatLng(53.4306151, -3.0699561), new LatLng(53.42844, -3.0697617), new LatLng(53.4229794, -3.0691894), new LatLng(53.4227424, -3.0691631), new LatLng(53.4227424, -3.0691631), new LatLng(53.4224656, -3.0691361), new LatLng(53.4224656, -3.0691361), new LatLng(53.4213148, -3.0690222), new LatLng(53.419813, -3.0687767), new LatLng(53.4174581, -3.0675346), new LatLng(53.4170525, -3.0674114), new LatLng(53.4165643, -3.0674374), new LatLng(53.41596, -3.0677116), new LatLng(53.4116579, -3.0709149), new LatLng(53.4109616, -3.0711326), new LatLng(53.4104292, -3.0710505), new LatLng(53.4101246, -3.0709035), new LatLng(53.4097354, -3.0705796), new LatLng(53.4095152, -3.070314), new LatLng(53.4091554, -3.0696792), new LatLng(53.4088708, -3.0687992), new LatLng(53.4088708, -3.0687992), new LatLng(53.4090204, -3.0695607), new LatLng(53.4094956, -3.0723045), new LatLng(53.4095943, -3.0735189), new LatLng(53.4095238, -3.0759043), new LatLng(53.4093067, -3.0787675), new LatLng(53.4093067, -3.0787675), new LatLng(53.4092875, -3.079098), new LatLng(53.4092519, -3.0799044), new LatLng(53.4092519, -3.0799044), new LatLng(53.4092323, -3.0804732), new LatLng(53.4091796, -3.0815197), new LatLng(53.4091022, -3.0826741), new LatLng(53.4089355, -3.0854262), new LatLng(53.4085298, -3.0921159), new LatLng(53.4083273, -3.0954681), new LatLng(53.4081259, -3.0987996), new LatLng(53.4080669, -3.0999565), new LatLng(53.4079893, -3.1012739), new LatLng(53.4079044, -3.1025781), new LatLng(53.4076836, -3.1063117), new LatLng(53.4074739, -3.1097878), new LatLng(53.4074739, -3.1097878), new LatLng(53.4072656, -3.1132696), new LatLng(53.4071407, -3.1152107), new LatLng(53.4066551, -3.1232153), new LatLng(53.4062881, -3.1265542), new LatLng(53.4060213, -3.1282593), new LatLng(53.4053946, -3.1314181), new LatLng(53.4039243, -3.1370258), new LatLng(53.4039243, -3.1370258), new LatLng(53.4029298, -3.1407943), new LatLng(53.4022886, -3.1431863), new LatLng(53.3994243, -3.1539137), new LatLng(53.3980918, -3.1589164), new LatLng(53.3970985, -3.1626331), new LatLng(53.3959036, -3.1670946), new LatLng(53.3955461, -3.1683321), new LatLng(53.3955461, -3.1683321), new LatLng(53.3953604, -3.1689038), new LatLng(53.3949422, -3.170031), new LatLng(53.3946063, -3.1708664), new LatLng(53.3937149, -3.1727538), new LatLng(53.3930818, -3.1738668), new LatLng(53.391404, -3.1765072), new LatLng(53.391404, -3.1765072), new LatLng(53.3898917, -3.1788086), new LatLng(53.3896596, -3.1791742), new LatLng(53.3892989, -3.1796885), new LatLng(53.388193, -3.1809716), new LatLng(53.3872, -3.1818147), new LatLng(53.3860876, -3.1824837), new LatLng(53.3852569, -3.1828087), new LatLng(53.384557, -3.182981), new LatLng(53.3773758, -3.1839634), new LatLng(53.3765982, -3.1840386), new LatLng(53.3751419, -3.1839589), new LatLng(53.3751419, -3.1839589), new LatLng(53.3748401, -3.1839368), new LatLng(53.3740284, -3.1838756), new LatLng(53.373138845629, -3.18357824520149)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034199).Add(new LatLng(53.5692018, -2.8812674), new LatLng(53.5691815, -2.881289), new LatLng(53.5678432, -2.8827061), new LatLng(53.5662552, -2.8842996), new LatLng(53.5628937, -2.8873469), new LatLng(53.5628937, -2.8873469), new LatLng(53.5627294, -2.8874959), new LatLng(53.5627294, -2.8874959), new LatLng(53.5592057, -2.89069), new LatLng(53.554227, -2.895223), new LatLng(53.5523175, -2.8969734), new LatLng(53.5508994, -2.8981303), new LatLng(53.5494669, -2.8993694), new LatLng(53.5472302, -2.9011215), new LatLng(53.545685, -2.9023635), new LatLng(53.5430388, -2.9044192), new LatLng(53.5418351, -2.9052909), new LatLng(53.5396221, -2.9066765), new LatLng(53.5369824, -2.9083502), new LatLng(53.5367847, -2.908479), new LatLng(53.5334626, -2.9106033), new LatLng(53.5301274, -2.9126847), new LatLng(53.5249087, -2.9160698), new LatLng(53.5249087, -2.9160698), new LatLng(53.5247223, -2.916192), new LatLng(53.5247223, -2.916192), new LatLng(53.5159672, -2.9215842), new LatLng(53.5135199, -2.9235456), new LatLng(53.5123919, -2.9245755), new LatLng(53.509503, -2.9276311), new LatLng(53.5064608, -2.93103), new LatLng(53.5059916, -2.931506), new LatLng(53.5048511, -2.9326629), new LatLng(53.5048511, -2.9326629), new LatLng(53.5044919, -2.9330025), new LatLng(53.5043923, -2.9331239), new LatLng(53.4976804, -2.9404856), new LatLng(53.4976804, -2.9404856), new LatLng(53.4966717, -2.941573), new LatLng(53.4966717, -2.941573), new LatLng(53.4937538, -2.9445599), new LatLng(53.4937538, -2.9445599), new LatLng(53.4936255, -2.9446869), new LatLng(53.4936255, -2.9446869), new LatLng(53.4918052, -2.946515), new LatLng(53.4918052, -2.946515), new LatLng(53.4911796, -2.9471051), new LatLng(53.4911796, -2.9471051), new LatLng(53.4900753, -2.9480815), new LatLng(53.4884667, -2.9493904), new LatLng(53.4884667, -2.9493904), new LatLng(53.4876769, -2.9500326), new LatLng(53.4876769, -2.9500326), new LatLng(53.4865708, -2.9508495), new LatLng(53.4865708, -2.9508495), new LatLng(53.4863259, -2.9510102), new LatLng(53.4863259, -2.9510102), new LatLng(53.4861086, -2.9511543), new LatLng(53.4861086, -2.9511543), new LatLng(53.4850415, -2.9518525), new LatLng(53.4850415, -2.9518525), new LatLng(53.4848062, -2.9520068), new LatLng(53.4848062, -2.9520068), new LatLng(53.4835128, -2.9528021), new LatLng(53.4822462, -2.9534767), new LatLng(53.4801291, -2.954508), new LatLng(53.477971, -2.9553342), new LatLng(53.4760682, -2.9558491), new LatLng(53.4737502, -2.9562676), new LatLng(53.4726455, -2.9565465), new LatLng(53.4709404, -2.9571795), new LatLng(53.4698268, -2.9576396), new LatLng(53.4672933, -2.9592315), new LatLng(53.4672933, -2.9592315), new LatLng(53.4666163, -2.9596499), new LatLng(53.4666163, -2.9596499), new LatLng(53.4650288, -2.9606494), new LatLng(53.4618403, -2.962718), new LatLng(53.4610176, -2.9632415), new LatLng(53.4582376, -2.9650354), new LatLng(53.4571388, -2.9656276), new LatLng(53.4563671, -2.9659452), new LatLng(53.4555648, -2.9663915), new LatLng(53.4551201, -2.9667777), new LatLng(53.4535409, -2.9682626), new LatLng(53.4535409, -2.9682626), new LatLng(53.4511709, -2.9707889), new LatLng(53.4497627, -2.9722899), new LatLng(53.4497627, -2.9722899), new LatLng(53.4463533, -2.9754142), new LatLng(53.4463533, -2.9754142), new LatLng(53.4454689, -2.9763669), new LatLng(53.4454689, -2.9763669), new LatLng(53.444005, -2.9779955), new LatLng(53.444005, -2.9779955), new LatLng(53.4418657, -2.9799699), new LatLng(53.4418657, -2.9799699), new LatLng(53.4414302, -2.9804353), new LatLng(53.4414302, -2.9804353), new LatLng(53.4410826, -2.9807786), new LatLng(53.4410826, -2.9807786), new LatLng(53.4407512, -2.9810771), new LatLng(53.4400508, -2.9817466), new LatLng(53.4388289, -2.9829826), new LatLng(53.4361906, -2.985712), new LatLng(53.4360579, -2.9858554), new LatLng(53.4345544, -2.9874801), new LatLng(53.433905, -2.9880809), new LatLng(53.433905, -2.9880809), new LatLng(53.432688, -2.9892826), new LatLng(53.432688, -2.9892826), new LatLng(53.4318204, -2.990202), new LatLng(53.4314812, -2.9905614), new LatLng(53.4310465, -2.990982), new LatLng(53.4296607, -2.9919776), new LatLng(53.4289549, -2.9923038), new LatLng(53.4256359, -2.993264), new LatLng(53.4256359, -2.993264), new LatLng(53.4251109, -2.9933879), new LatLng(53.4234632, -2.9938313), new LatLng(53.4220749, -2.9941733), new LatLng(53.4217501, -2.9942656), new LatLng(53.4197236, -2.9948025), new LatLng(53.4183793, -2.9951684), new LatLng(53.4175017, -2.9954063), new LatLng(53.4168978, -2.9955325), new LatLng(53.4164784, -2.9955782), new LatLng(53.4162242, -2.9955541), new LatLng(53.415386, -2.9954477), new LatLng(53.415386, -2.9954477), new LatLng(53.4148909, -2.9953848), new LatLng(53.4137392, -2.9952869), new LatLng(53.4134533, -2.9952118), new LatLng(53.4131632, -2.9951209), new LatLng(53.4127258, -2.9948753), new LatLng(53.4127258, -2.9948753), new LatLng(53.4126351, -2.9948199), new LatLng(53.4120493, -2.9942735), new LatLng(53.4116955, -2.993919), new LatLng(53.4114795, -2.9936482), new LatLng(53.4111566, -2.9931543), new LatLng(53.4107719, -2.9925448), new LatLng(53.4103588, -2.9918677), new LatLng(53.4101214, -2.9915649), new LatLng(53.4098246, -2.9912423), new LatLng(53.4096038, -2.9909675), new LatLng(53.4092461, -2.990667), new LatLng(53.4088767, -2.9904135), new LatLng(53.4084293, -2.9901358), new LatLng(53.4078837, -2.989841), new LatLng(53.4073253, -2.9894502), new LatLng(53.4069341, -2.9891067), new LatLng(53.4063737, -2.9884534), new LatLng(53.4061268, -2.9880101), new LatLng(53.4058751, -2.9874417), new LatLng(53.405659, -2.9868721), new LatLng(53.4054951, -2.9862667), new LatLng(53.4053618, -2.9856004), new LatLng(53.4052957, -2.984992), new LatLng(53.4052441, -2.9845725), new LatLng(53.4050296, -2.9828295), new LatLng(53.4049992, -2.9826613), new LatLng(53.4048039, -2.9815814), new LatLng(53.4047068, -2.9809052), new LatLng(53.4045477, -2.9802719), new LatLng(53.4042973, -2.9796508), new LatLng(53.4038942, -2.9787816), new LatLng(53.4033972, -2.9778633), new LatLng(53.4030613, -2.9772268), new LatLng(53.4026785, -2.9768438), new LatLng(53.402392999999996, -2.9766128), new LatLng(53.4020236, -2.9764325), new LatLng(53.4017012, -2.9763818), new LatLng(53.4013351, -2.9763311), new LatLng(53.4007003, -2.9763199), new LatLng(53.4003577, -2.9763142), new LatLng(53.3998902, -2.9762788), new LatLng(53.399289, -2.9762816), new LatLng(53.3987096, -2.9762844), new LatLng(53.3966469, -2.9762264), new LatLng(53.3949941, -2.9762318), new LatLng(53.3943828, -2.9762149), new LatLng(53.3943828, -2.9762149), new LatLng(53.3937042, -2.9762544), new LatLng(53.3937042, -2.9762544), new LatLng(53.3921977, -2.9762938), new LatLng(53.3921977, -2.9762938), new LatLng(53.391808, -2.9763166), new LatLng(53.391808, -2.9763166), new LatLng(53.3903848, -2.9764037), new LatLng(53.3903848, -2.9764037), new LatLng(53.3901194, -2.9764239), new LatLng(53.3901194, -2.9764239), new LatLng(53.3888809, -2.9765456), new LatLng(53.3888809, -2.9765456), new LatLng(53.3887612, -2.9765546), new LatLng(53.3887612, -2.9765546), new LatLng(53.3870282, -2.9765887), new LatLng(53.3870282, -2.9765887), new LatLng(53.3849497, -2.9766863), new LatLng(53.3844762, -2.9766863), new LatLng(53.3838796, -2.9764994), new LatLng(53.3833779, -2.976096), new LatLng(53.3828557, -2.9756154), new LatLng(53.3817203, -2.9741247), new LatLng(53.3813428, -2.9733415), new LatLng(53.3807694, -2.9712558), new LatLng(53.3807694, -2.9712558), new LatLng(53.3773186, -2.9574028), new LatLng(53.3773186, -2.9574028), new LatLng(53.376868, -2.9558664), new LatLng(53.3761752, -2.9541098), new LatLng(53.3756627, -2.9529484), new LatLng(53.3707831, -2.9416969), new LatLng(53.369146, -2.9378308), new LatLng(53.368666, -2.9360566), new LatLng(53.368666, -2.9360566), new LatLng(53.3681806, -2.9343125), new LatLng(53.3681806, -2.9343125), new LatLng(53.3680105, -2.9339023), new LatLng(53.3680105, -2.9339023), new LatLng(53.3674297, -2.9325319), new LatLng(53.3674297, -2.9325319), new LatLng(53.3654989, -2.9289492), new LatLng(53.3649411, -2.9277478), new LatLng(53.3645136, -2.926799), new LatLng(53.3641807, -2.9256746), new LatLng(53.3636326, -2.924061), new LatLng(53.3628951, -2.9220698), new LatLng(53.3623419, -2.9208424), new LatLng(53.3617683, -2.919615), new LatLng(53.3611434, -2.9185249), new LatLng(53.3605531, -2.9172585), new LatLng(53.3587009, -2.9119039), new LatLng(53.3582728, -2.91071), new LatLng(53.3577758, -2.9089851), new LatLng(53.3575419, -2.907348), new LatLng(53.3575419, -2.907348), new LatLng(53.3574542, -2.9047169), new LatLng(53.3574542, -2.9047169), new LatLng(53.3572203, -2.895391), new LatLng(53.3571457, -2.8932261), new LatLng(53.3571457, -2.8932261), new LatLng(53.3571252, -2.8924193), new LatLng(53.3571252, -2.8924193), new LatLng(53.3570625, -2.8905001), new LatLng(53.3569511, -2.8873811), new LatLng(53.3569511, -2.8873811), new LatLng(53.3569306, -2.8863168), new LatLng(53.3569306, -2.8863168), new LatLng(53.3569101, -2.8855872), new LatLng(53.3568845, -2.8836989), new LatLng(53.3569152, -2.8823342), new LatLng(53.3570177, -2.8809523), new LatLng(53.3571611, -2.8794589), new LatLng(53.3572892, -2.8783688), new LatLng(53.3575248, -2.877193), new LatLng(53.3575248, -2.877193), new LatLng(53.3578437, -2.8750433), new LatLng(53.3579272, -2.8744802), new LatLng(53.3581805, -2.8727727), new LatLng(53.3587183, -2.8689446), new LatLng(53.3594713, -2.8640866), new LatLng(53.3600348, -2.8601985), new LatLng(53.3607242, -2.8554611)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034199).Add(new LatLng(53.6468807, -3.0027158), new LatLng(53.6460203, -3.0014501), new LatLng(53.6460203, -3.0014501), new LatLng(53.6452239, -3.0005313), new LatLng(53.6445091, -3.0002397), new LatLng(53.6440155, -3.0002912), new LatLng(53.6434558, -3.0006174), new LatLng(53.6434558, -3.0006174), new LatLng(53.642947, -3.0012182), new LatLng(53.6414548, -3.0035601), new LatLng(53.6395975, -3.0066482), new LatLng(53.6376457, -3.0098353), new LatLng(53.6374404, -3.0101501), new LatLng(53.6364959, -3.011584), new LatLng(53.6356003, -3.0127169), new LatLng(53.6342301, -3.0142387), new LatLng(53.6338563, -3.0146134), new LatLng(53.6293955, -3.0184744), new LatLng(53.6218673, -3.0251079), new LatLng(53.6176585, -3.0288041), new LatLng(53.6086562, -3.0367177), new LatLng(53.6021377, -3.0424314), new LatLng(53.59945, -3.0449767), new LatLng(53.5962374, -3.0484107), new LatLng(53.5949738, -3.0497476), new LatLng(53.5937048, -3.0511174), new LatLng(53.5825999, -3.0629645), new LatLng(53.5799295, -3.0658006), new LatLng(53.5785261, -3.0672895), new LatLng(53.5778694, -3.0679602), new LatLng(53.5772493, -3.0685396), new LatLng(53.5761456, -3.0694448), new LatLng(53.5755775, -3.0698471), new LatLng(53.5750084, -3.0702149), new LatLng(53.5743678, -3.0705848), new LatLng(53.5738809, -3.0708428), new LatLng(53.5732129, -3.0711408), new LatLng(53.5723203, -3.0714798), new LatLng(53.5718552, -3.0716208), new LatLng(53.5713626, -3.0717503), new LatLng(53.5708624, -3.0718633), new LatLng(53.5703064, -3.0719514), new LatLng(53.5699534, -3.0720037), new LatLng(53.5693139, -3.0720431), new LatLng(53.5685085, -3.0720552), new LatLng(53.5676644, -3.0719995), new LatLng(53.5660468, -3.0718743), new LatLng(53.5658589, -3.0718577), new LatLng(53.5549741, -3.0709949), new LatLng(53.5528526, -3.0708165), new LatLng(53.551174, -3.0706225), new LatLng(53.5505082, -3.0704508), new LatLng(53.5492315, -3.0699492), new LatLng(53.5483987, -3.0695162), new LatLng(53.5475616, -3.0690427), new LatLng(53.5446105, -3.0673721), new LatLng(53.5422413, -3.0660236), new LatLng(53.5422413, -3.0660236), new LatLng(53.5420661, -3.0659279), new LatLng(53.5420661, -3.0659279), new LatLng(53.5346116, -3.0617194), new LatLng(53.5299409, -3.0589967), new LatLng(53.5279868, -3.0580249), new LatLng(53.5264839, -3.0574586), new LatLng(53.5250419, -3.0570475), new LatLng(53.5231285, -3.0566183), new LatLng(53.5189167, -3.0556693), new LatLng(53.5133871, -3.0544343), new LatLng(53.5073514, -3.0530441), new LatLng(53.5035343, -3.0520859), new LatLng(53.5017539, -3.0515923), new LatLng(53.5005542, -3.0511632), new LatLng(53.4991502, -3.0505302), new LatLng(53.4973867, -3.0495306), new LatLng(53.4972999, -3.0494715), new LatLng(53.4961703, -3.0487775), new LatLng(53.4931679, -3.0462197), new LatLng(53.49197, -3.0450585), new LatLng(53.4875272, -3.0401554), new LatLng(53.4874059, -3.0400267), new LatLng(53.4817139, -3.0338277), new LatLng(53.4788726, -3.0306065), new LatLng(53.4773122, -3.0289223), new LatLng(53.476182, -3.027549), new LatLng(53.4755307, -3.0266263), new LatLng(53.4749121, -3.0255638), new LatLng(53.4740109, -3.0239871), new LatLng(53.4734298, -3.0224636), new LatLng(53.4729445, -3.0205538), new LatLng(53.4725549, -3.0186977), new LatLng(53.4721973, -3.0172815), new LatLng(53.4717503, -3.0158653), new LatLng(53.4709947, -3.0139785), new LatLng(53.4696561, -3.0113435), new LatLng(53.4682379, -3.0085483), new LatLng(53.4674332, -3.007175), new LatLng(53.4670895, -3.006633), new LatLng(53.4670895, -3.006633), new LatLng(53.4666712, -3.0060322), new LatLng(53.4666712, -3.0060322), new LatLng(53.4654022, -3.0044552), new LatLng(53.4640801, -3.0031034), new LatLng(53.4629368, -3.0020412), new LatLng(53.4620426, -3.0011615), new LatLng(53.4593902, -2.9987099), new LatLng(53.4591665, -2.9985077), new LatLng(53.4591665, -2.9985077), new LatLng(53.4589836, -2.9983282), new LatLng(53.4589836, -2.9983282), new LatLng(53.4571456, -2.9965633), new LatLng(53.4558833, -2.995439), new LatLng(53.4551524, -2.9950356), new LatLng(53.45453, -2.9948161), new LatLng(53.45453, -2.9948161), new LatLng(53.4541326, -2.9947112), new LatLng(53.4535601, -2.994632), new LatLng(53.4535601, -2.994632), new LatLng(53.4529983, -2.9946334), new LatLng(53.4524198, -2.9947082), new LatLng(53.4518093, -2.9948713), new LatLng(53.4515695, -2.9949677), new LatLng(53.4515695, -2.9949677), new LatLng(53.4512474, -2.9950909), new LatLng(53.4512474, -2.9950909), new LatLng(53.4503736, -2.995439), new LatLng(53.4503736, -2.995439), new LatLng(53.4494842, -2.9957222), new LatLng(53.4488912, -2.9958338), new LatLng(53.4485215, -2.9958595), new LatLng(53.4485215, -2.9958595), new LatLng(53.4467085, -2.9956278), new LatLng(53.4454254, -2.9954218), new LatLng(53.4447711, -2.9952244), new LatLng(53.4439532, -2.9947437), new LatLng(53.4427671, -2.9939627), new LatLng(53.4417879, -2.992927), new LatLng(53.4417879, -2.992927), new LatLng(53.4416543, -2.9928516), new LatLng(53.4416543, -2.9928516), new LatLng(53.4416132, -2.9928029), new LatLng(53.4398639, -2.9907291), new LatLng(53.4391935, -2.989963), new LatLng(53.4379511, -2.9880747), new LatLng(53.4376927, -2.9877181), new LatLng(53.4371586, -2.9871305), new LatLng(53.4366831, -2.9868731), new LatLng(53.4360491, -2.9867443), new LatLng(53.4353946, -2.9868559), new LatLng(53.4345544, -2.9874801)));
				_map.AddPolyline(new PolylineOptions().InvokeWidth(8f).InvokeColor(2131034199).Add(new LatLng(53.4535409, -2.9682626), new LatLng(53.4547164, -2.9668893), new LatLng(53.4547164, -2.9668893), new LatLng(53.4555545, -2.9660568), new LatLng(53.4563357, -2.9650631), new LatLng(53.457144, -2.9636621), new LatLng(53.4574915, -2.9629754), new LatLng(53.457655, -2.9626407), new LatLng(53.4600262, -2.957611), new LatLng(53.4607622, -2.9562441), new LatLng(53.4631877, -2.9508309), new LatLng(53.463688, -2.9493356), new LatLng(53.4640383, -2.9479464), new LatLng(53.4645145, -2.9463162), new LatLng(53.4651655, -2.9445384), new LatLng(53.4669008, -2.9410811), new LatLng(53.4680807, -2.938754), new LatLng(53.4690601, -2.9367027), new LatLng(53.4726057, -2.9295787), new LatLng(53.4742558, -2.9263172), new LatLng(53.4761371, -2.9225138), new LatLng(53.4761371, -2.9225138), new LatLng(53.4764946, -2.9219738), new LatLng(53.4764946, -2.9219738), new LatLng(53.4768428, -2.9214995), new LatLng(53.4781415, -2.9194717), new LatLng(53.4781415, -2.9194717), new LatLng(53.4788247, -2.918405), new LatLng(53.4788247, -2.918405), new LatLng(53.4809203, -2.9150524), new LatLng(53.4818716, -2.9135611), new LatLng(53.4818716, -2.9135611), new LatLng(53.4823249, -2.9127672), new LatLng(53.4823249, -2.9127672), new LatLng(53.4835698, -2.9104819), new LatLng(53.4843511, -2.9086506), new LatLng(53.485102, -2.906759), new LatLng(53.4856431, -2.9053146), new LatLng(53.486144, -2.9036946), new LatLng(53.4864363, -2.9026439), new LatLng(53.4865944, -2.9020517)));
			}
			networkPlotted = true;
		}
		foreach (Marker currentMarker in CurrentMarkers)
		{
			currentMarker.Remove();
		}
		CurrentMarkers.Clear();
		LatLngBounds.Builder builder = new LatLngBounds.Builder();
		foreach (TrainStop item in progress)
		{
			LatLng latLng = new LatLng(item.Station.Lat, item.Station.Lon);
			builder.Include(latLng);
			PlotMarker(latLng, item.Station.Name);
		}
		if (_map != null)
		{
			_map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 50));
		}
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}

	private void ShowHelp()
	{
		bool flag = !SharedSettings.HasViewedHelpItem(base.Class.Name);
		if (helpDisplay == null && flag)
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
			HelpDisplay.NewHelpItem("ShareButton", ShareButton, HelpDisplay.HelpItemLayoutHint.Center, "Tap to share this train with friends"),
			HelpDisplay.NewHelpItem("AlertButton", AlertButton, HelpDisplay.HelpItemLayoutHint.Center, "Tap set an alert for this train")
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

	public void OnMapReady(GoogleMap googleMap)
	{
		_map = googleMap;
	}
}
