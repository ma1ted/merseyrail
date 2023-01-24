using Android.OS;
using Android.Views;
using Android.Widget;
using Merseyrail.Fragments;

namespace Merseyrail;

public class HelpPageIndicator : BaseFragment
{
	private View _view;

	private FrameLayout background;

	private bool isActive;

	private bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	public HelpPageIndicator(bool isActive)
	{
		IsActive = isActive;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361842, null, attachToRoot: false);
		}
		background = _view.FindViewById<FrameLayout>(2131230888);
		SetBackgroundImage(IsActive);
		return _view;
	}

	private void SetBackgroundImage(bool b)
	{
		if (b)
		{
			background.SetBackgroundResource(2131165312);
		}
		else
		{
			background.SetBackgroundResource(2131165311);
		}
	}
}
