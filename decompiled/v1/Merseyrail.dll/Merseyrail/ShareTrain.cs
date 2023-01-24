using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Services;
using Java.Lang;
using Merseyrail.Shared;

namespace Merseyrail;

[Activity(Label = "ReportAProblem", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
public class ShareTrain : Activity
{
	private bool ActionsBound { get; set; }

	private EditText Name { get; set; }

	private EditText Email { get; set; }

	private ImageButton BackButton { get; set; }

	private Button ShareButton { get; set; }

	private string TrainId { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361825);
		TrainId = Intent!.GetStringExtra("trainid");
		Name = (EditText)FindViewById(2131231150);
		BackButton = (ImageButton)FindViewById(2131231072);
		Name = (EditText)FindViewById(2131231150);
		Name.Text = SharedSettings.SettingsService.Prefs.GetString("DEFAULT_NAME", string.Empty);
		ShareButton = (Button)FindViewById(2131231149);
		BackButton = (ImageButton)FindViewById(2131231072);
		SetUIActions();
	}

	private void ShareTrainNow(string trainid, string name, string email)
	{
		if (!SharedSettings.IsRegistered)
		{
			return;
		}
		SharedServices.IoC.Resolve<ShareService>().ShareTrain(trainid, name, email, delegate(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				OpenShareIntent(url);
			}
			else
			{
				ToastMessage("You need an internet connection to share a train.");
			}
		});
	}

	private void ToastMessage(string response)
	{
		RunOnUiThread(new Runnable(delegate
		{
			Toast.MakeText(this, response, ToastLength.Short)!.Show();
		}));
	}

	private void OpenShareIntent(string url)
	{
		RunOnUiThread(delegate
		{
			Intent intent = new Intent("android.intent.action.SEND");
			intent.SetType("text/plain");
			intent.PutExtra("android.intent.extra.SUBJECT", "Sharing URL");
			intent.PutExtra("android.intent.extra.TEXT", url);
			StartActivity(Android.Content.Intent.CreateChooser(intent, "ShareURL"));
		});
	}

	private void SetUIActions()
	{
		if (!ActionsBound)
		{
			BackButton.Click += delegate
			{
				Finish();
			};
			ShareButton.Click += delegate
			{
				SaveNameAndEmail();
				ShareTrainNow(TrainId, SharedSettings.DefaultName, SharedSettings.DefaultEmail);
			};
			ActionsBound = true;
		}
	}

	private void SaveNameAndEmail()
	{
		SharedSettings.SettingsService.Editor.PutString("DEFAULT_NAME", Name.Text);
		SharedSettings.SettingsService.Editor.Apply();
		SharedSettings.SettingsService.Editor.Commit();
	}

	private void ProcessShareTrainResponse(string response)
	{
		RunOnUiThread(delegate
		{
			Toast.MakeText(this, "The response " + response, ToastLength.Short)!.Show();
		});
		Finish();
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		if (requestCode == 511 && resultCode == Result.Ok)
		{
			data.GetStringExtra("result");
			data.GetStringExtra("crscode");
		}
		if (requestCode == 433 && resultCode == Result.Ok)
		{
			new Intent("android.intent.action.MEDIA_SHARED");
		}
		base.OnActivityResult(requestCode, resultCode, data);
	}
}
