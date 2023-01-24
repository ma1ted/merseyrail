using Android.App;
using Android.Gms.Maps;
using Android.Hardware;
using Android.OS;

namespace Merseyrail;

[Activity(Label = "MapView and Compass")]
public class CompassMapActivity : Activity
{
	private const string TAG = "MapViewCompassDemo";

	private SensorManager sensor_manager;

	private RotateView rotate_view;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		sensor_manager = (SensorManager)GetSystemService("sensor");
		new GoogleMapOptions();
		MapView child = new MapView(this);
		rotate_view = new RotateView(this);
		rotate_view.AddView(child);
		SetContentView(rotate_view);
	}

	protected override void OnResume()
	{
		base.OnResume();
		Sensor defaultSensor = sensor_manager.GetDefaultSensor(SensorType.Orientation);
		sensor_manager.RegisterListener(rotate_view, defaultSensor, SensorDelay.Ui);
	}

	protected override void OnStop()
	{
		sensor_manager.UnregisterListener(rotate_view);
		base.OnStop();
	}
}
