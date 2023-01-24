using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Merseyrail.Shared;

namespace Merseyrail;

public class HelpDisplay
{
	public enum HelpItemLayoutHint
	{
		Top,
		Center,
		Bottom
	}

	private Fragment parentFragment;

	private int totalHelpPages;

	private int activeIndex;

	private HelpOverlayFragment CurrentHelpItem { get; set; }

	private HelpDisplayItem CurrentItem { get; set; }

	public List<HelpDisplayItem> HelpListItems { get; set; }

	public List<HelpDisplayItem> AllHelpListItems { get; set; }

	public HelpDisplay(Fragment pFragment, List<HelpDisplayItem> helpItems)
	{
		parentFragment = pFragment;
		HelpListItems = helpItems;
		if (AllHelpListItems == null)
		{
			AllHelpListItems = new List<HelpDisplayItem>();
			AllHelpListItems.AddRange(helpItems);
		}
		totalHelpPages = helpItems.Count;
	}

	public HelpDisplay()
	{
	}

	public void ShowHelp()
	{
		if (HelpListItems.Count > 0)
		{
			ShowHelp(HelpListItems.First());
		}
	}

	public static HelpDisplayItem NewHelpItem(string idTag, View targetView, string msg)
	{
		return new HelpDisplayItem
		{
			IdTag = idTag,
			TargetView = targetView,
			LayoutHint = HelpItemLayoutHint.Bottom,
			Message = msg
		};
	}

	public static HelpDisplayItem NewHelpItem(string idTag, View targetView, HelpItemLayoutHint hint, string msg)
	{
		return new HelpDisplayItem
		{
			IdTag = idTag,
			TargetView = targetView,
			LayoutHint = hint,
			Message = msg
		};
	}

	public void ShowHelp(HelpDisplayItem item)
	{
		string idTag = item.IdTag;
		if (SharedSettings.HasViewedHelpItem(idTag))
		{
			return;
		}
		View targetView = item.TargetView;
		string message = item.Message;
		ClearOverlays();
		int[] array = new int[2];
		targetView.GetLocationInWindow(array);
		int x = array[0] + targetView.MeasuredWidth / 2;
		int y = array[1];
		FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(-1, -2);
		switch (item.LayoutHint)
		{
		case HelpItemLayoutHint.Top:
			layoutParams.Gravity = GravityFlags.Top;
			break;
		case HelpItemLayoutHint.Center:
			layoutParams.Gravity = GravityFlags.Center;
			break;
		case HelpItemLayoutHint.Bottom:
			layoutParams.Gravity = GravityFlags.Bottom;
			layoutParams.BottomMargin = 80;
			break;
		}
		using FragmentTransaction fragmentTransaction = parentFragment.ChildFragmentManager.BeginTransaction();
		CurrentItem = item;
		activeIndex = AllHelpListItems.IndexOf(item);
		CurrentHelpItem = new HelpOverlayFragment(x, y, message, layoutParams, totalHelpPages, activeIndex);
		View view = parentFragment.View.FindViewById(2131230729);
		if (view != null)
		{
			fragmentTransaction.Add(view.Id, CurrentHelpItem, idTag);
		}
		else
		{
			fragmentTransaction.Add(parentFragment.Id, CurrentHelpItem, idTag);
		}
		fragmentTransaction.CommitAllowingStateLoss();
		CurrentHelpItem.OnViewClicked += delegate
		{
			RemoveOverlay(CurrentHelpItem);
		};
	}

	public void ClearOverlays()
	{
		if (CurrentHelpItem != null)
		{
			using (FragmentTransaction fragmentTransaction = parentFragment.ChildFragmentManager.BeginTransaction())
			{
				fragmentTransaction.Remove(CurrentHelpItem);
				fragmentTransaction.CommitAllowingStateLoss();
			}
		}
	}

	public void RemoveOverlay(Fragment frag)
	{
		_ = frag.Tag;
		SharedSettings.HelpItemViewed(frag.Tag);
		using (FragmentTransaction fragmentTransaction = parentFragment.ChildFragmentManager.BeginTransaction())
		{
			fragmentTransaction.Remove(frag);
			fragmentTransaction.CommitAllowingStateLoss();
		}
		if (HelpListItems.Count > 0)
		{
			HelpListItems.Remove(CurrentItem);
			CurrentHelpItem = null;
			CurrentItem = null;
		}
		if (HelpListItems.Count > 0)
		{
			ShowHelp(HelpListItems.First());
		}
		else
		{
			NoHelpToShow();
		}
	}

	private void NoHelpToShow()
	{
		SharedSettings.HelpItemViewed(parentFragment.Class.Name);
	}
}
