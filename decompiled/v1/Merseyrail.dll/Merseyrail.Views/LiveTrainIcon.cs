using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Views;

public class LiveTrainIcon : LinearLayout
{
	public bool TrainIcon { get; set; }

	public bool CircleBlue { get; set; }

	public bool CircleDark { get; set; }

	public bool CircleGreen { get; set; }

	public bool CircleGrey { get; set; }

	public bool HorisBlue { get; set; }

	public bool HorisDark { get; set; }

	public bool HorisGreen { get; set; }

	public bool HorisGrey { get; set; }

	public bool HorisNone { get; set; }

	public bool TopBlue { get; set; }

	public bool TopDark { get; set; }

	public bool TopGreen { get; set; }

	public bool TopGrey { get; set; }

	public bool BottomBlue { get; set; }

	public bool BottomDark { get; set; }

	public bool BottomGreen { get; set; }

	public bool BottomGrey { get; set; }

	private ImageView Layer1 { get; set; }

	private ImageView Layer2 { get; set; }

	private ImageView Layer3 { get; set; }

	private ImageView Layer4 { get; set; }

	private LayoutInflater _layoutInflater { get; set; }

	public LiveTrainIcon(Context context)
		: base(context)
	{
		Initialize(context);
	}

	public LiveTrainIcon(Context context, IAttributeSet attrs)
		: base(context, attrs)
	{
		Initialize(context);
	}

	private void Initialize(Context context)
	{
		_layoutInflater = (LayoutInflater)context.GetSystemService("layout_inflater");
		_layoutInflater.Inflate(2131361855, this, attachToRoot: true);
		Layer1 = FindViewById(2131230980) as ImageView;
		Layer2 = FindViewById(2131230981) as ImageView;
		Layer3 = FindViewById(2131230982) as ImageView;
		Layer4 = FindViewById(2131230983) as ImageView;
		InitLayers();
	}

	private void InitLayers()
	{
		ResetLayers();
		InitLayer1();
		InitLayer2();
		InitLayer3();
		InitLayer4();
	}

	public void UpdateView()
	{
		InitLayers();
		Invalidate();
	}

	private void InitLayer4()
	{
		if (TrainIcon)
		{
			Layer4.SetImageResource(2131165352);
		}
		if (CircleBlue)
		{
			Layer4.SetImageResource(2131165339);
		}
		if (CircleDark)
		{
			Layer4.SetImageResource(2131165340);
		}
		if (CircleGreen)
		{
			Layer4.SetImageResource(2131165341);
		}
		if (CircleGrey)
		{
			Layer4.SetImageResource(2131165342);
		}
	}

	private void InitLayer3()
	{
		if (HorisBlue)
		{
			Layer3.SetImageResource(2131165343);
		}
		if (HorisDark)
		{
			Layer3.SetImageResource(2131165344);
		}
		if (HorisGreen)
		{
			Layer3.SetImageResource(2131165345);
		}
		if (HorisGrey)
		{
			Layer3.SetImageResource(2131165346);
		}
	}

	private void InitLayer2()
	{
		if (TopBlue)
		{
			Layer2.SetImageResource(2131165348);
		}
		if (TopDark)
		{
			Layer2.SetImageResource(2131165349);
		}
		if (TopGreen)
		{
			Layer2.SetImageResource(2131165350);
		}
		if (TopGrey)
		{
			Layer2.SetImageResource(2131165351);
		}
	}

	private void InitLayer1()
	{
		if (BottomBlue)
		{
			Layer1.SetImageResource(2131165335);
		}
		if (BottomDark)
		{
			Layer1.SetImageResource(2131165336);
		}
		if (BottomGreen)
		{
			Layer1.SetImageResource(2131165337);
		}
		if (BottomGrey)
		{
			Layer1.SetImageResource(2131165338);
		}
	}

	private void ResetLayers()
	{
		Layer4.SetImageResource(2131165347);
		Layer3.SetImageResource(2131165347);
		Layer2.SetImageResource(2131165347);
		Layer1.SetImageResource(2131165347);
	}
}
