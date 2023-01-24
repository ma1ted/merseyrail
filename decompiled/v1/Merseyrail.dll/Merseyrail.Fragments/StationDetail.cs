using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Merseyrail.Fragments;

public class StationDetail : BaseFragment
{
	private View _view;

	public WebView StationDetailWebView { get; set; }

	public string StationName { get; set; }

	public string StationHTMLInfo { get; set; }

	private ImageButton btnBack { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup viewGroup, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361827, null, attachToRoot: false);
		}
		((TextView)_view.FindViewById(2131231076)).Text = StationName;
		btnBack = (ImageButton)_view.FindViewById(2131231075);
		btnBack.Click += delegate
		{
			((Main)base.Activity).OnBackPressed();
		};
		UpdateWebView();
		return _view;
	}

	public void UpdateWebView()
	{
		string mimeType = "text/html";
		string encoding = "utf-8";
		StationDetailWebView = (WebView)_view.FindViewById(2131231273);
		StationDetailWebView.Settings!.JavaScriptEnabled = true;
		string data = "<link rel=\"stylesheet\" type=\"text/css\" href=\"stationinfo.css\" />" + StationHTMLInfo;
		StationDetailWebView.LoadDataWithBaseURL("file:///android_asset/", data, mimeType, encoding, null);
	}
}
