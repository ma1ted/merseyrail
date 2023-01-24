using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Common.Services;
using Java.IO;
using Merseyrail.Helpers;
using Merseyrail.Shared;

namespace Merseyrail;

[Activity(Label = "ReportAProblem", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
public class ReportAProblem : Activity
{
	private Java.IO.File _file;

	private Java.IO.File _dir;

	private ImageView _reportImageView;

	private Button StationSelect { get; set; }

	private Button AddPhoto { get; set; }

	private Button AddGallery { get; set; }

	private Button RemovePhoto { get; set; }

	private TextView PrivacyPolicyLink { get; set; }

	private EditText Name { get; set; }

	private EditText EmailAddress { get; set; }

	private EditText Message { get; set; }

	private string reportImageStr { get; set; }

	private Button SendReport { get; set; }

	private Bitmap reportImage { get; set; }

	private ImageButton BackButton { get; set; }

	private bool ActionsBound { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361869);
	}

	private void SetUIActions()
	{
		if (!ActionsBound)
		{
			BackButton.Click += delegate
			{
				Finish();
			};
			StationSelect.Click += delegate
			{
				Intent intent4 = new Intent(this, typeof(StationSelectorActivity));
				StartActivityForResult(intent4, 422);
			};
			PrivacyPolicyLink.Click += delegate
			{
				string uriString = "https://www.merseyrail.org/privacy.aspx";
				Intent intent3 = new Intent("android.intent.action.VIEW");
				intent3.SetData(Android.Net.Uri.Parse(uriString));
				StartActivity(intent3);
			};
			if (IsThereAnAppToTakePictures())
			{
				_reportImageView = FindViewById<ImageView>(2131231116);
				_reportImageView.SetBackgroundDrawable(Resources!.GetDrawable(2131165277));
				CreateDirectoryForPictures();
				AddPhoto.Click += delegate
				{
					reportImageStr = string.Empty;
					Intent intent2 = new Intent("android.media.action.IMAGE_CAPTURE");
					_file = new Java.IO.File(_dir, $"rphoto_{Guid.NewGuid()}.jpg");
					intent2.PutExtra("output", Android.Net.Uri.FromFile(_file));
					StartActivityForResult(intent2, 433);
				};
				AddGallery.Click += delegate
				{
					reportImageStr = string.Empty;
					Intent intent = new Intent("android.intent.extra.album");
					intent.SetType("image/*");
					intent.SetAction("android.intent.action.GET_CONTENT");
					_file = new Java.IO.File(_dir, $"rphoto_{Guid.NewGuid()}.jpg");
					intent.PutExtra("output", Android.Net.Uri.FromFile(_file));
					StartActivityForResult(intent, 443);
				};
				RemovePhoto.Click += delegate
				{
					reportImageStr = string.Empty;
					RunOnUiThread(delegate
					{
						_reportImageView.SetImageResource(2131165277);
					});
				};
			}
			else
			{
				RunOnUiThread(delegate
				{
					_reportImageView.Visibility = ViewStates.Invisible;
					AddPhoto.Visibility = ViewStates.Invisible;
					RemovePhoto.Visibility = ViewStates.Invisible;
					reportImageStr = string.Empty;
				});
			}
			SendReport.Click += delegate
			{
				if (ValidSubmission())
				{
					string text = StationSelect.Text;
					string text2 = Name.Text;
					string text3 = EmailAddress.Text;
					string text4 = Message.Text;
					string imagedata = reportImageStr ?? string.Empty;
					BeginSendingAnim();
					SharedServices.IoC.Resolve<RemoteServices>().PostProblemReport(text, text2, text3, text4, imagedata, delegate(string rmsg)
					{
						RunOnUiThread(delegate
						{
							Toast.MakeText(Android.App.Application.Context, rmsg, ToastLength.Long)!.Show();
							EndSendingAnim();
						});
					});
					Finish();
				}
				else if (string.IsNullOrEmpty(StationSelect.Text) && string.IsNullOrEmpty(Message.Text))
				{
					AlertDialog.Builder alert3 = new AlertDialog.Builder(this);
					alert3.SetTitle("Missing Information");
					alert3.SetMessage("Please ensure you have selected a Station and composed a message before sending.");
					alert3.SetNeutralButton("Ok", delegate
					{
					});
					RunOnUiThread(delegate
					{
						alert3.Show();
					});
				}
				else if (string.IsNullOrEmpty(StationSelect.Text))
				{
					AlertDialog.Builder alert2 = new AlertDialog.Builder(this);
					alert2.SetTitle("Missing Information");
					alert2.SetMessage("Please ensure you have selected a Station before sending.");
					alert2.SetNeutralButton("Ok", delegate
					{
					});
					RunOnUiThread(delegate
					{
						alert2.Show();
					});
				}
				else if (string.IsNullOrEmpty(Message.Text))
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Missing Information");
					alert.SetMessage("Please ensure you have composed a message before sending..");
					alert.SetNeutralButton("Ok", delegate
					{
					});
					RunOnUiThread(delegate
					{
						alert.Show();
					});
				}
			};
		}
		ActionsBound = true;
	}

	private bool ValidSubmission()
	{
		if (!string.IsNullOrEmpty(StationSelect.Text))
		{
			return !string.IsNullOrEmpty(Message.Text);
		}
		return false;
	}

	private void BeginSendingAnim()
	{
	}

	private void EndSendingAnim()
	{
	}

	protected override void OnStart()
	{
		StationSelect = (Button)FindViewById(2131231111);
		AddPhoto = (Button)FindViewById(2131231113);
		AddGallery = (Button)FindViewById(2131231112);
		RemovePhoto = (Button)FindViewById(2131231114);
		Name = (EditText)FindViewById(2131231110);
		Name.Text = (string.IsNullOrEmpty(Name.Text) ? SharedSettings.DefaultName : Name.Text);
		EmailAddress = (EditText)FindViewById(2131231109);
		EmailAddress.Text = (string.IsNullOrEmpty(EmailAddress.Text) ? SharedSettings.DefaultEmail : EmailAddress.Text);
		Message = (EditText)FindViewById(2131231108);
		SendReport = (Button)FindViewById(2131231115);
		BackButton = (ImageButton)FindViewById(2131231070);
		PrivacyPolicyLink = FindViewById<TextView>(2131231085);
		SetUIActions();
		base.OnStart();
	}

	private void ProcessReoprtProblemResponse(string response)
	{
		RunOnUiThread(delegate
		{
			Toast.MakeText(this, "The response " + response, ToastLength.Long)!.Show();
		});
		Finish();
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		base.OnActivityResult(requestCode, resultCode, data);
		if (requestCode == 422 && resultCode == Result.Ok)
		{
			string stringExtra = data.GetStringExtra("result");
			data.GetStringExtra("crscode");
			StationSelect.Text = stringExtra;
		}
		if (requestCode == 433)
		{
			GC.Collect();
			try
			{
				if (resultCode == Result.Ok && _file != null)
				{
					RunOnUiThread(delegate
					{
						_reportImageView.Visibility = ViewStates.Visible;
					});
					Intent intent = new Intent("android.intent.action.MEDIA_SCANNER_SCAN_FILE");
					Android.Net.Uri data2 = Android.Net.Uri.FromFile(_file);
					intent.SetData(data2);
					SendBroadcast(intent);
					int height = _reportImageView.Height;
					int widthPixels = Resources!.DisplayMetrics!.WidthPixels;
					Bitmap bitmap2 = (reportImage = _file.Path.LoadAndResizeBitmap(widthPixels, height));
					using (bitmap2)
					{
						_reportImageView.SetImageBitmap(reportImage);
					}
					reportImageStr = string.Empty;
					using Bitmap image = _file.Path.LoadAndResizeBitmap(1024, 1024);
					reportImageStr = BitmapHelpers.EncodeTobase64(image);
				}
			}
			catch (Exception ex3)
			{
				Exception ex2 = ex3;
				RunOnUiThread(delegate
				{
					Toast.MakeText(this, ex2.Message, ToastLength.Long);
				});
			}
		}
		if (requestCode != 443)
		{
			return;
		}
		GC.Collect();
		try
		{
			if (resultCode != Result.Ok || _file == null)
			{
				return;
			}
			RunOnUiThread(delegate
			{
				_reportImageView.Visibility = ViewStates.Visible;
			});
			int height2 = _reportImageView.Height;
			int widthPixels2 = Resources!.DisplayMetrics!.WidthPixels;
			Bitmap bitmap2;
			if (data.Data == null)
			{
				bitmap2 = (reportImage = _file.Path.LoadAndResizeBitmap(widthPixels2, height2));
				using (bitmap2)
				{
					_reportImageView.SetImageBitmap(Bitmap.CreateScaledBitmap(reportImage, height2, widthPixels2, filter: true));
					reportImageStr = BitmapHelpers.EncodeTobase64(reportImage);
					return;
				}
			}
			bitmap2 = (reportImage = MediaStore.Images.Media.GetBitmap(ContentResolver, data.Data));
			using (bitmap2)
			{
				_reportImageView.SetImageBitmap(Bitmap.CreateScaledBitmap(reportImage, height2, widthPixels2, filter: true));
				reportImageStr = BitmapHelpers.EncodeTobase64(reportImage);
			}
		}
		catch (Exception ex4)
		{
			Exception ex = ex4;
			RunOnUiThread(delegate
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long);
			});
		}
	}

	public byte[] ReadFully(Stream input)
	{
		byte[] array = new byte[16384];
		using MemoryStream memoryStream = new MemoryStream();
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			memoryStream.Write(array, 0, count);
		}
		return memoryStream.ToArray();
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

	private void CreateDirectoryForPictures()
	{
		_dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "MerseyRail");
		if (!_dir.Exists())
		{
			_dir.Mkdirs();
		}
	}
}
