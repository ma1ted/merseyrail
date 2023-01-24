using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Services;
using Merseyrail.Shared;

namespace Merseyrail;

[Activity(Label = "ReportAProblem", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
public class Feedback : Activity
{
	private ImageView _reportImageView;

	private ImageButton BackButton { get; set; }

	private EditText Name { get; set; }

	private EditText EmailAddress { get; set; }

	private EditText Message { get; set; }

	private string reportImageStr { get; set; }

	private Button SendReport { get; set; }

	private bool ActionsBound { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		SetContentView(2131361840);
	}

	private void SetUIActions()
	{
		if (!ActionsBound)
		{
			if (IsThereAnAppToTakePictures())
			{
				_reportImageView = FindViewById<ImageView>(2131231116);
			}
			else
			{
				RunOnUiThread(delegate
				{
					_reportImageView.Visibility = ViewStates.Invisible;
				});
			}
			SendReport.Click += delegate
			{
				string text = Name.Text;
				string text2 = EmailAddress.Text;
				string text3 = Message.Text;
				_ = reportImageStr;
				SharedServices.IoC.Resolve<RemoteServices>().PostFeedbackReport(text, text2, text3, delegate(bool sent)
				{
					if (sent)
					{
						Toast.MakeText(this, "Feedback sent successfully.", ToastLength.Short)!.Show();
					}
					else
					{
						Toast.MakeText(this, "Feedback could not be sent.", ToastLength.Short)!.Show();
					}
				});
				Finish();
			};
		}
		ActionsBound = true;
	}

	protected override void OnStart()
	{
		base.OnStart();
		Name = (EditText)FindViewById(2131230873);
		Name.Text = SharedSettings.DefaultName;
		EmailAddress = (EditText)FindViewById(2131230872);
		EmailAddress.Text = SharedSettings.DefaultEmail;
		Message = (EditText)FindViewById(2131230871);
		SendReport = (Button)FindViewById(2131230868);
		SetUIActions();
	}

	private void ProcessReoprtProblemResponse(string response)
	{
		RunOnUiThread(delegate
		{
			Toast.MakeText(this, "The response " + response, ToastLength.Short)!.Show();
		});
		Finish();
	}

	private bool IsThereAnAppToTakePictures()
	{
		Intent intent = new Intent("android.media.action.IMAGE_CAPTURE");
		IList<ResolveInfo> list = PackageManager!.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
		if (list != null)
		{
			return list.Count > 0;
		}
		return false;
	}
}
