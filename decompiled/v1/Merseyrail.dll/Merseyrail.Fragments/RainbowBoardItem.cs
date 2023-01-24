using System;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Fragments;

public class RainbowBoardItem : Android.Support.V4.App.Fragment
{
	public enum RainbowStatus
	{
		red,
		amber,
		green,
		purple,
		black
	}

	private string headerText;

	private string statusText;

	private bool hasIncidents;

	private RainbowStatus itemStatus;

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
				base.Activity.RunOnUiThread(delegate
				{
					HeaderTextView.Text = value;
				});
			}
		}
	}

	public string StatusText
	{
		get
		{
			return statusText;
		}
		set
		{
			statusText = value;
			if (StatusTextView != null)
			{
				base.Activity.RunOnUiThread(delegate
				{
					StatusTextView.Text = statusText;
				});
			}
		}
	}

	public bool HasIncidents
	{
		get
		{
			return hasIncidents;
		}
		set
		{
			hasIncidents = value;
			SetIncidentArrowVisibility(value);
		}
	}

	public RainbowStatus ItemStatus
	{
		get
		{
			return itemStatus;
		}
		set
		{
			itemStatus = value;
			if (StatusIcon == null)
			{
				return;
			}
			base.Activity.RunOnUiThread(delegate
			{
				switch (ItemStatus)
				{
				case RainbowStatus.red:
					StatusIcon.SetImageResource(2131165376);
					break;
				case RainbowStatus.amber:
					StatusIcon.SetImageResource(2131165377);
					break;
				case RainbowStatus.green:
					StatusIcon.SetImageResource(2131165374);
					break;
				case RainbowStatus.purple:
					StatusIcon.SetImageResource(2131165375);
					break;
				case RainbowStatus.black:
					StatusIcon.SetImageResource(2131165373);
					break;
				}
			});
		}
	}

	public TextView HeaderTextView { get; set; }

	public TextView StatusTextView { get; set; }

	public ImageView StatusIcon { get; set; }

	public ImageView IncidentArrow { get; set; }

	public event EventHandler OnClick;

	private void SetIncidentArrowVisibility(bool vis)
	{
		if (IncidentArrow != null)
		{
			base.Activity.RunOnUiThread(delegate
			{
				IncidentArrow.Visibility = ((!vis) ? ViewStates.Gone : ViewStates.Visible);
			});
		}
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override void OnInflate(Activity activity, IAttributeSet attributes, Bundle bundle)
	{
		base.OnInflate(activity, attributes, bundle);
		HeaderText = attributes.GetAttributeValue(null, "HeaderText");
		StatusText = attributes.GetAttributeValue(null, "StatusText");
		string attributeValue = attributes.GetAttributeValue(null, "ItemStatus");
		switch (attributeValue)
		{
		case "red":
		case "amber":
		case "green":
			ItemStatus = (RainbowStatus)Enum.Parse(typeof(RainbowStatus), attributeValue);
			break;
		}
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		View view = inflater.Inflate(2131361867, null, attachToRoot: false);
		((LinearLayout)view.FindViewById(2131231094)).Click += delegate(object sender, EventArgs e)
		{
			if (this.OnClick != null)
			{
				this.OnClick(sender, e);
			}
		};
		HeaderTextView = view.FindViewById<TextView>(2131231097);
		StatusTextView = view.FindViewById<TextView>(2131231096);
		StatusIcon = view.FindViewById<ImageView>(2131230749);
		IncidentArrow = view.FindViewById<ImageView>(2131231095);
		UpdateItem();
		return view;
	}

	private void UpdateItem()
	{
		HeaderText = HeaderText;
		StatusText = StatusText;
		ItemStatus = ItemStatus;
	}
}
