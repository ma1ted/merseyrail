using Android.Views;

namespace Merseyrail.Fragments;

public class PagedFragmentBase : BaseFragment
{
	public PagedFragmentSection PagedSection { get; set; }

	public View PagedFragmentView { get; set; }

	public PagedFragmentBase()
	{
		PagedSection = (PagedFragmentSection)base.ParentFragment;
	}

	public void MovePrev()
	{
		PagedSection = (PagedFragmentSection)base.ParentFragment;
		int currentItem = PagedSection.Pager.CurrentItem;
		PagedSection.Pager.SetCurrentItem(currentItem - 1, smoothScroll: false);
	}

	public void MoveNext()
	{
		PagedSection = (PagedFragmentSection)base.ParentFragment;
		int currentItem = PagedSection.Pager.CurrentItem;
		PagedSection.Pager.SetCurrentItem(currentItem + 1, smoothScroll: false);
	}
}
