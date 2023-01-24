using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using Java.Interop;

namespace Merseyrail.Rotator;

internal class RotateView : ViewGroup, ISensorEventListener, IJavaObject, IDisposable, IJavaPeerable
{
	private const float SQ2 = 1.41421354f;

	private const float ROTATE_TOLERANCE = 2f;

	private float heading;

	private object lock_obj = new object();

	public RotateView(Context context)
		: base(context)
	{
	}

	public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
	{
	}

	public void OnSensorChanged(SensorEvent e)
	{
		lock (lock_obj)
		{
			IList<float> values = e.Values;
			if (Math.Abs(heading - values[0]) > 2f)
			{
				heading = values[0];
				Invalidate();
			}
			((IDisposable)values).Dispose();
		}
	}

	protected override void DispatchDraw(Canvas canvas)
	{
		canvas.Save(SaveFlags.Matrix);
		canvas.Rotate(0f - heading, (float)base.Width * 0.5f, (float)base.Height * 0.5f);
		base.DispatchDraw(canvas);
		canvas.Restore();
	}

	protected override void OnLayout(bool changed, int l, int t, int r, int b)
	{
		int width = base.Width;
		int height = base.Height;
		int childCount = ChildCount;
		for (int i = 0; i < childCount; i++)
		{
			View? childAt = GetChildAt(i);
			int measuredWidth = childAt!.MeasuredWidth;
			int measuredHeight = childAt!.MeasuredHeight;
			int num = (width - measuredWidth) / 2;
			int num2 = (height - measuredHeight) / 2;
			childAt!.Layout(num, num2, num + measuredWidth, num2 + measuredHeight);
		}
	}

	protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
	{
		int defaultSize = View.GetDefaultSize(SuggestedMinimumWidth, widthMeasureSpec);
		int defaultSize2 = View.GetDefaultSize(SuggestedMinimumHeight, heightMeasureSpec);
		int num = ((defaultSize <= defaultSize2) ? MeasureSpec.MakeMeasureSpec((int)((float)defaultSize2 * 1.41421354f), MeasureSpecMode.Exactly) : MeasureSpec.MakeMeasureSpec((int)((float)defaultSize * 1.41421354f), MeasureSpecMode.Exactly));
		int childCount = ChildCount;
		for (int i = 0; i < childCount; i++)
		{
			GetChildAt(i)!.Measure(num, num);
		}
		base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
	}

	public override bool DispatchTouchEvent(MotionEvent ev)
	{
		return base.DispatchTouchEvent(ev);
	}
}
