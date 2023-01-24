using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace Merseyrail.Pager;

public class NonSwipePager : ViewPager
{
	public bool LockScroll { get; set; }

	public float startDragX { get; set; }

	public NonSwipePager(Context context)
		: base(context)
	{
		LockScroll = true;
	}

	public NonSwipePager(Context context, IAttributeSet attrs)
		: base(context, attrs)
	{
		LockScroll = true;
	}

	public override bool OnInterceptTouchEvent(MotionEvent ev)
	{
		return false;
	}

	public override bool OnTouchEvent(MotionEvent e)
	{
		return false;
	}
}
