using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Fragments;

public class PageHeader : Android.Support.V4.App.Fragment
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

	public ImageButton BackButton { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		View view = inflater.Inflate(2131361862, null, attachToRoot: false);
		HeaderTextView = (TextView)view.FindViewById(2131231067);
		SetBackButtonAction(view);
		return view;
	}

	private void SetHeaderText()
	{
		Typeface tf = Typeface.CreateFromAsset(base.Activity.BaseContext!.Assets, "VAG-Rounded-Bold.ttf");
		HeaderTextView.Text = HeaderText;
		HeaderTextView.SetTypeface(tf, TypefaceStyle.Normal);
	}

	public override void OnInflate(Activity activity, IAttributeSet attributes, Bundle savedInstance)
	{
		base.OnInflate(activity, attributes, savedInstance);
		HeaderText = attributes.GetAttributeValue(null, "HeaderText");
	}

	private void SetBackButtonAction(View view)
	{
		BackButton = (ImageButton)view.FindViewById(2131231066);
		BackButton.Click += delegate
		{
			if (base.Activity != null && typeof(Main).IsAssignableFrom(base.Activity.GetType()))
			{
				((Main)base.Activity).OpenDrawerLayout();
			}
			else
			{
				base.Activity.Finish();
			}
		};
	}

	public override void OnStart()
	{
		base.OnResume();
		SetHeaderText();
	}
}
