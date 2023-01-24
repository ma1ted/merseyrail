using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace Merseyrail.Fragments;

public abstract class BaseFragment : Fragment
{
	public Type PreviousFragment = typeof(Departures);

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		return base.OnCreateView(inflater, container, savedInstanceState);
	}
}
