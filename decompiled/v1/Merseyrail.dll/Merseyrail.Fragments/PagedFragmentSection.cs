using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Merseyrail.Pager;

namespace Merseyrail.Fragments;

public class PagedFragmentSection : Fragment
{
	protected class PagerAdapter : FragmentPagerAdapter
	{
		private List<Type> PagedFragmentList { get; set; }

		public Dictionary<int, object> PagedFragments { get; set; }

		public override int Count => PagedFragmentList.Count;

		public PagerAdapter(FragmentManager fm, List<Type> pagedFragmentList, Dictionary<int, object> pagedFragments)
			: base(fm)
		{
			PagedFragmentList = pagedFragmentList;
			PagedFragments = pagedFragments;
		}

		public override Fragment GetItem(int position)
		{
			Fragment fragment = PagedFragments.Where((KeyValuePair<int, object> x) => x.Key == position).FirstOrDefault().Value as Fragment;
			if (fragment == null)
			{
				fragment = (Fragment)Activator.CreateInstance(PagedFragmentList[position]);
				PagedFragments.Add(position, fragment);
			}
			return fragment;
		}
	}

	private const int NUM_ITEMS = 10;

	private PagerAdapter pagerAdapter;

	public NonSwipePager Pager { get; set; }

	public List<Type> PagedFragmentList { get; set; }

	public Dictionary<int, object> PagedFragments { get; set; }

	public object CurrentFragment { get; set; }

	public View _view { get; set; }

	public PagedFragmentSection()
	{
		PagedFragmentList = new List<Type>();
		PagedFragments = new Dictionary<int, object>();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361863, null, attachToRoot: false);
			pagerAdapter = new PagerAdapter(base.ChildFragmentManager, PagedFragmentList, PagedFragments);
			Pager = _view.FindViewById<NonSwipePager>(2131231077);
			Pager.LockScroll = true;
			Pager.OffscreenPageLimit = 3;
			Pager.Adapter = pagerAdapter;
		}
		return _view;
	}

	public object MoveTo(int index)
	{
		Pager.CurrentItem = index;
		return PagedFragments[index];
	}

	public object MoveTo(Type type)
	{
		IEnumerable<KeyValuePair<int, object>> enumerable = PagedFragments.Where((KeyValuePair<int, object> x) => x.Value is LiveTrain);
		if (enumerable != null)
		{
			return MoveTo(enumerable.First().Key);
		}
		return null;
	}
}
