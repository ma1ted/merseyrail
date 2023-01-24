using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Merseyrail.Services;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class WirralTrackReplacementView : BaseFragment
{
	private const string Url = "file:///XXXXXX/pdfviewer/index.html";

	private const string PlanningUrl = "http://XXXXXX/embed";

	private Button ViewPlanningTool { get; set; }

	private Button ViewGuide { get; set; }

	private ImageButton BackButton { get; set; }

	private View view { get; set; }

	private FragmentService FragmentService => SharedServices.IoC.Resolve<FragmentService>();

	public void CreateWebView(string url)
	{
		if (SharedSettings.HasConnection(Application.Context))
		{
			Bundle bundle = new Bundle();
			bundle.PutString("url", url);
			SharedValues.CurrentFragment = FragmentService.InitSection<WebViewFragment>(bundle, "webView");
		}
		else
		{
			Toast.MakeText(Application.Context, "An internet or data connection is required to view this", ToastLength.Short)!.Show();
		}
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		view = inflater.Inflate(2131361874, container, attachToRoot: false);
		ViewPlanningTool = (Button)view.FindViewById(2131231280);
		ViewGuide = (Button)view.FindViewById(2131231279);
		BackButton = (ImageButton)view.FindViewById(2131230737);
		ViewPlanningTool.Click += delegate
		{
			CreateWebView("http://XXXXXX/embed");
		};
		BackButton.Click += delegate
		{
			((Main)base.Activity).OpenDrawerLayout();
		};
		ViewGuide.Click += delegate
		{
			CreateWebView("file:///XXXXXX/pdfviewer/index.html");
		};
		return view;
	}

	public override void OnResume()
	{
		base.OnResume();
	}

	public override void OnPause()
	{
		base.OnPause();
	}
}
