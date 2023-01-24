using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace Merseyrail.Fragments;

public class MapFragment : BaseFragment
{
	private static readonly LatLng Passchendaele = new LatLng(53.4174, -2.957);

	private static readonly LatLng VimyRidge = new LatLng(52.4174, -2.957);

	public SupportMapFragment Map;

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		InitMapFragment();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		return inflater.Inflate(2131361857, null, attachToRoot: false);
	}

	private void InitMapFragment()
	{
		GoogleMapOptions options = new GoogleMapOptions().InvokeMapType(1).InvokeZoomControlsEnabled(enabled: false).InvokeCompassEnabled(enabled: true)
			.InvokeZOrderOnTop(zOrderOnTop: true)
			.InvokeUseViewLifecycleInFragment(useViewLifecycleInFragment: true);
		FragmentTransaction fragmentTransaction = base.ChildFragmentManager.BeginTransaction();
		Map = SupportMapFragment.NewInstance(options);
		fragmentTransaction.Add(2131231014, Map, "map_fragmentlayout");
		fragmentTransaction.Commit();
	}
}
