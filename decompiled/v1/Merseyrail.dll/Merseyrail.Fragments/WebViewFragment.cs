using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Merseyrail.Fragments;

public class WebViewFragment : BaseFragment
{
	private View view { get; set; }

	private ImageButton BackButton { get; set; }

	private WebView WebView { get; set; }

	private string Url { get; set; }

	private bool ShowButtons { get; set; }

	private LinearLayout LeftRightButtonsLayout { get; set; }

	private void CreateWebView()
	{
		if (WebView == null)
		{
			WebView = new WebView(Application.Context);
		}
		WebView.ClearCache(includeDiskFiles: true);
		WebView.ClearHistory();
		WebView.Settings!.JavaScriptEnabled = true;
		WebView.Settings!.JavaScriptCanOpenWindowsAutomatically = true;
		WebView.Settings!.AllowFileAccessFromFileURLs = true;
		WebView.Settings!.AllowFileAccess = true;
		if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
		{
			WebView.Settings!.AllowUniversalAccessFromFileURLs = true;
		}
		WebView.SetWebChromeClient(new WebChromeClient());
		WebView.SetWebViewClient(new CustomClient());
		WebView.LoadUrl(Url);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		view = inflater.Inflate(2131361865, container, attachToRoot: false);
		BackButton = (ImageButton)view.FindViewById(2131230735);
		WebView = (WebView)view.FindViewById(2131231272);
		LeftRightButtonsLayout = view.FindViewById<LinearLayout>(2131230960);
		Url = base.Arguments.GetString("url");
		ShowButtons = base.Arguments.GetBoolean("showButtons", defaultValue: true);
		if (!ShowButtons)
		{
			LeftRightButtonsLayout.Visibility = ViewStates.Gone;
		}
		else
		{
			LeftRightButtonsLayout.Visibility = ViewStates.Visible;
		}
		BackButton.Click += delegate
		{
			((Main)base.Activity).OpenDrawerLayout();
		};
		CreateWebView();
		return view;
	}

	public override void OnPause()
	{
		base.OnPause();
		if (WebView != null)
		{
			WebView.ClearCache(includeDiskFiles: true);
		}
	}

	public override void OnResume()
	{
		base.OnResume();
		if (WebView != null)
		{
			WebView.LoadUrl("javascript:window.location.reload( true )");
		}
	}
}
