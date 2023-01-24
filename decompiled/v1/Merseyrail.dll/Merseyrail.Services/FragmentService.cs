using System;
using Android.OS;
using Android.Support.V4.App;

namespace Merseyrail.Services;

public class FragmentService
{
	private bool isInitialised;

	private int containerId;

	private FragmentManager FragmentManager { get; set; }

	public bool Initialise(FragmentManager fragmentManager, int container_Id)
	{
		if (!isInitialised)
		{
			FragmentManager = fragmentManager;
			containerId = container_Id;
		}
		isInitialised = true;
		return isInitialised;
	}

	public bool InitSection(Type fragmenttype, string tag)
	{
		return InitSection(fragmenttype, null, tag);
	}

	public bool InitSection(Type fragmenttype, Bundle bundle, string tag)
	{
		Fragment fragment = new Fragment();
		fragment = Activator.CreateInstance(fragmenttype) as Fragment;
		fragment.RetainInstance = true;
		if (bundle != null)
		{
			fragment.Arguments = bundle;
		}
		OpenFragment(fragment, tag);
		return true;
	}

	public T InitSection<T>(string tag) where T : Fragment
	{
		return InitSection<T>(null, tag);
	}

	public T InitSection<T>(Bundle bundle, string tag) where T : Fragment
	{
		int backStackEntryCount = FragmentManager.BackStackEntryCount;
		T val = (T)FragmentManager.FindFragmentByTag(tag);
		bool flag = false;
		if (backStackEntryCount > 0)
		{
			try
			{
				flag = FragmentManager.PopBackStackImmediate(tag, 1);
			}
			catch (Exception)
			{
				flag = false;
			}
		}
		if (!flag && val == null)
		{
			val = new T
			{
				RetainInstance = true
			};
			if (bundle != null)
			{
				val.Arguments = bundle;
			}
			OpenFragment(val, tag);
			return val;
		}
		return val;
	}

	private void OpenFragment(Fragment fragment, string tag)
	{
		FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
		fragmentTransaction.Replace(containerId, fragment, tag);
		fragmentTransaction.Commit();
	}

	public void ClearBackstack()
	{
		if (FragmentManager.BackStackEntryCount > 0)
		{
			for (int i = 0; i < FragmentManager.BackStackEntryCount; i++)
			{
				FragmentManager.PopBackStack();
			}
		}
	}
}
