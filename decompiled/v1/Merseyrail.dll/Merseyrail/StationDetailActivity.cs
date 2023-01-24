using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;

namespace Merseyrail;

[Activity(Label = "StationDetailActivity")]
public class StationDetailActivity : Activity
{
	public WebView StationDetailWebView { get; set; }

	public string StationName { get; set; }

	public string StationHTMLInfo { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		SetContentView(2131361827);
		StationName = Intent!.GetStringExtra("Name");
		StationHTMLInfo = Intent!.GetStringExtra("HTMLInfo");
	}

	protected override void OnStart()
	{
		((TextView)FindViewById(2131231076)).Text = StationName;
		((ImageButton)FindViewById(2131231075)).Click += delegate
		{
			Finish();
		};
		SetupWebView();
		base.OnStart();
	}

	protected override void OnPause()
	{
		base.OnPause();
		Finish();
	}

	private void SetupWebView()
	{
		string mimeType = "text/html";
		string encoding = "utf-8";
		StationDetailWebView = (WebView)FindViewById(2131231273);
		StationDetailWebView.Settings!.JavaScriptEnabled = true;
		StationDetailWebView.LoadDataWithBaseURL(null, StationHTMLInfo, mimeType, encoding, null);
	}
}
