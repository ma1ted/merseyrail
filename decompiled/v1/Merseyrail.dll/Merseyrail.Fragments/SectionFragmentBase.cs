using System.Collections.Generic;
using Android.Support.V4.App;

namespace Merseyrail.Fragments;

public class SectionFragmentBase : Fragment
{
	public Dictionary<string, Fragment> MasterFragments { get; set; }

	public Dictionary<string, Fragment> DetailFragments { get; set; }

	public SectionFragmentBase()
	{
		MasterFragments = new Dictionary<string, Fragment>();
		DetailFragments = new Dictionary<string, Fragment>();
	}

	public void Transit(string fragmentTag, SectionFragmentType type)
	{
		Transit(fragmentTag, type, null, null, null, null);
	}

	public void Transit(string fragmentTag, SectionFragmentType type, int? enter, int? exit)
	{
		Transit(fragmentTag, type, enter, exit, null, null);
	}

	public void Transit(string fragmentTag, SectionFragmentType type, int? enter, int? exit, int? popEnter, int? popExit)
	{
		Fragment fragment = ((type == SectionFragmentType.Master) ? MasterFragments[fragmentTag] : DetailFragments[fragmentTag]);
		FragmentTransaction fragmentTransaction = base.ChildFragmentManager.BeginTransaction();
		new Departures();
		if (enter.HasValue && exit.HasValue && popEnter.HasValue && popExit.HasValue)
		{
			fragmentTransaction.SetCustomAnimations(enter.Value, exit.Value, popEnter.Value, popExit.Value);
		}
		else if (enter.HasValue && exit.HasValue && !popEnter.HasValue && !popExit.HasValue)
		{
			fragmentTransaction.SetCustomAnimations(enter.Value, exit.Value);
		}
		fragmentTransaction.Replace(base.Id, fragment, fragmentTag);
		fragmentTransaction.Commit();
	}
}
