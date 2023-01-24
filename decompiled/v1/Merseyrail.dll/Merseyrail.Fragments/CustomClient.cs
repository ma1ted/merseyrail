using Android.App;
using Android.Graphics;
using Android.Webkit;

namespace Merseyrail.Fragments;

public class CustomClient : WebViewClient
{
	private ProgressDialog dialog;

	public override void OnPageStarted(WebView view, string url, Bitmap favicon)
	{
		base.OnPageStarted(view, url, favicon);
		dialog = new ProgressDialog(view.Context);
		dialog.SetTitle("Loading");
		dialog.SetMessage("Loading, please wait...");
		dialog.Show();
	}

	public override void OnPageFinished(WebView view, string url)
	{
		base.OnPageFinished(view, url);
		if (dialog.IsShowing)
		{
			dialog.Dismiss();
		}
	}
}
