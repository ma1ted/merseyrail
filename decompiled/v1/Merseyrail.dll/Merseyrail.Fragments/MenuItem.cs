using System;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Fragments;

public class MenuItem : Android.Support.V4.App.Fragment
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

	public View MenuItemView { get; set; }

	public TextView HeaderTextView { get; set; }

	public event EventHandler OnClick;

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override void OnInflate(Activity activity, IAttributeSet attributes, Bundle bundle)
	{
		base.OnInflate(activity, attributes, bundle);
		HeaderText = attributes.GetAttributeValue(null, "HeaderText");
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		MenuItemView = inflater.Inflate(2131361860, null, attachToRoot: false);
		((LinearLayout)MenuItemView.FindViewById(2131231020)).Click += delegate(object sender, EventArgs e)
		{
			if (this.OnClick != null)
			{
				this.OnClick(sender, e);
			}
		};
		HeaderTextView = (TextView)MenuItemView.FindViewById(2131231031);
		UpdateHeaderText();
		return MenuItemView;
	}

	private void UpdateHeaderText()
	{
		HeaderText = HeaderText;
	}
}
