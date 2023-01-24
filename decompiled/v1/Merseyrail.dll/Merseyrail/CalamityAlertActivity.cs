using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Common;
using Newtonsoft.Json;

namespace Merseyrail;

[Activity(Label = "Merseyrail Alert", ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ScreenOrientation = ScreenOrientation.Portrait)]
public class CalamityAlertActivity : Activity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361831);
		FindViewById<Button>(2131230819)!.Click += delegate
		{
			OnBackPressed();
		};
		FindViewById<ImageButton>(2131230786)!.Click += delegate
		{
			OnBackPressed();
		};
		if (Intent != null)
		{
			CalamityAlert calamityAlert = JsonConvert.DeserializeObject<CalamityAlert>(Intent!.GetStringExtra("calamityJson"));
			FindViewById<TextView>(2131230820)!.Text = calamityAlert.Title;
			FindViewById<WebView>(2131230818)!.LoadData(calamityAlert.Body, "text/html", "charset=utf-8");
		}
	}

	public override void OnBackPressed()
	{
		Finish();
	}
}
