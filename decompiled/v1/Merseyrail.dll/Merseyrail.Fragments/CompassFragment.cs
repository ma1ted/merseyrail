using Android.Hardware;
using Android.OS;
using Android.Views;

namespace Merseyrail.Fragments;

public class CompassFragment : BaseFragment
{
	private SensorManager sensor_manager;

	private RotateView rotate_view;

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		sensor_manager = (SensorManager)base.Activity.GetSystemService("sensor");
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		View child = inflater.Inflate(2131361837, null, attachToRoot: false);
		rotate_view = new RotateView(base.Activity.BaseContext);
		rotate_view.AddView(child);
		return rotate_view;
	}
}
