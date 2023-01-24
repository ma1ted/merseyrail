using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Merseyrail.Helpers;

namespace Merseyrail;

[Activity(Label = "PreferencesActivity", ScreenOrientation = ScreenOrientation.Portrait)]
public class PreferencesActivity : Activity
{
	private long alertId;

	private string alertJson;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		if (Intent != null)
		{
			alertId = Intent!.GetLongExtra("alertId", 0L);
			alertJson = Intent!.GetStringExtra("alertJson");
		}
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361902);
		BindEvents();
	}

	protected override void OnResume()
	{
		base.OnResume();
		CalamityHelpers.CheckForCalamity(this);
	}

	private void BindEvents()
	{
		FindViewById<Button>(2131230797)!.Click += delegate
		{
			StartEditProfile();
		};
		FindViewById<Button>(2131230800)!.Click += delegate
		{
			StartManageAlerts();
		};
		FindViewById<ImageButton>(2131230792)!.Click += delegate
		{
			Finish();
		};
	}

	public void StartEditProfile()
	{
		StartActivity(typeof(PreferencesProfileActivity));
	}

	public void StartManageAlerts()
	{
		StartActivity(typeof(PreferencesManageAlerts));
	}
}
