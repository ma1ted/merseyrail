using System;
using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Merseyrail.Fragments;

namespace Merseyrail;

public class HelpOverlayFragment : BaseFragment, View.IOnTouchListener, IJavaObject, IDisposable, IJavaPeerable
{
	private View _view;

	private int X;

	private int Y;

	private string message;

	private FrameLayout.LayoutParams layoutParams;

	private int totalIndicators;

	private int activeIndex;

	private List<HelpPageIndicator> helpPageIndicators;

	public event EventHandler OnViewClicked;

	public void Dismiss()
	{
		if (this.OnViewClicked != null)
		{
			this.OnViewClicked(this, null);
		}
	}

	public HelpOverlayFragment(int x, int y, string msg, FrameLayout.LayoutParams lp, int totalIndicatorItems, int activeIndicatorIndex)
	{
		X = x;
		Y = y;
		message = msg;
		layoutParams = lp;
		totalIndicators = totalIndicatorItems;
		activeIndex = activeIndicatorIndex;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		helpPageIndicators = new List<HelpPageIndicator>();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361841, null, attachToRoot: false);
			_view.FindViewById<TextView>(2131230886).Text = message;
			AbsoluteLayout absoluteLayout = _view.FindViewById<AbsoluteLayout>(2131230889);
			absoluteLayout.Clickable = true;
			absoluteLayout.Click += delegate(object sender, EventArgs e)
			{
				if (this.OnViewClicked != null)
				{
					this.OnViewClicked(sender, e);
				}
			};
			FrameLayout frameLayout = _view.FindViewById<FrameLayout>(2131230887);
			frameLayout.LayoutParameters = layoutParams;
			frameLayout.RequestLayout();
			absoluteLayout.TranslationX += X;
			absoluteLayout.TranslationY += Y;
			_view.Clickable = true;
			_view.Click += delegate(object sender, EventArgs e)
			{
				if (this.OnViewClicked != null)
				{
					this.OnViewClicked(sender, e);
				}
			};
			LinearLayout linearLayout = _view.FindViewById<LinearLayout>(2131230885);
			using FragmentTransaction fragmentTransaction = base.ChildFragmentManager.BeginTransaction();
			for (int i = 0; i < totalIndicators; i++)
			{
				HelpPageIndicator fragment = new HelpPageIndicator(activeIndex == i);
				fragmentTransaction.Add(linearLayout.Id, fragment, "indicatorTag_" + i);
			}
			fragmentTransaction.Commit();
		}
		return _view;
	}

	public bool OnTouch(View v, MotionEvent e)
	{
		return true;
	}
}
