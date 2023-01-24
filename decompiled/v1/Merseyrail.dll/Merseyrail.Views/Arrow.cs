using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace Merseyrail.Views;

public class Arrow : ImageView
{
	private int direction;

	public Arrow(Context context)
		: base(context)
	{
		Initialize();
	}

	public Arrow(Context context, IAttributeSet attrs)
		: base(context, attrs)
	{
		Initialize();
	}

	public Arrow(Context context, IAttributeSet attrs, int defStyle)
		: base(context, attrs, defStyle)
	{
		Initialize();
	}

	private void Initialize()
	{
		SetImageResource(2131165304);
	}

	protected override void OnDraw(Canvas canvas)
	{
		int height = base.Height;
		int width = base.Width;
		canvas.Rotate(direction, width / 2, height / 2);
		base.OnDraw(canvas);
	}

	public void setDirection(int direction)
	{
		this.direction = direction;
		Invalidate();
	}
}
