using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Fragments;

public class ListHeader : Android.Support.V4.App.Fragment
{
	private string headerText;

	public string HeaderText
	{
		get
		{
			return headerText;
		}
		set
		{
			headerText = value;
			if (HeaderTextView != null)
			{
				HeaderTextView.Text = value;
			}
		}
	}

	public TextView HeaderTextView { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		View view = inflater.Inflate(2131361851, null, attachToRoot: false);
		HeaderTextView = (TextView)view.FindViewById(2131230978);
		return view;
	}

	public override void OnInflate(Activity activity, IAttributeSet attributes, Bundle savedInstance)
	{
		base.OnInflate(activity, attributes, savedInstance);
		HeaderText = attributes.GetAttributeValue(null, "HeaderText");
	}

	private void SetHeaderText()
	{
		Typeface tf = Typeface.CreateFromAsset(base.Activity.BaseContext!.Assets, "VAG-Rounded-Bold.ttf");
		if (!string.IsNullOrEmpty(HeaderText))
		{
			HeaderTextView.Text = HeaderText;
		}
		HeaderTextView.SetTypeface(tf, TypefaceStyle.Normal);
	}

	public override void OnStart()
	{
		base.OnResume();
		SetHeaderText();
	}
}
