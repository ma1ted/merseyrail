using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Common.Services;
using Merseyrail.Events;
using Merseyrail.Shared;
using Merseyrail.Timers;
using Square.Picasso;

namespace Merseyrail.Activities;

[Activity(Label = "OfferActivity", ScreenOrientation = ScreenOrientation.Portrait)]
public class OfferActivity : Activity
{
	private ImageView _offerImageView;

	private TextView _titleTextView;

	private TextView _descriptionTextView;

	private TextView _expiryTextView;

	private TextView _termsTextView;

	private TextView _timeLeftTextView;

	private TextView _codeTextView;

	private Button _btnRedeem;

	private LinearLayout _codeLayout;

	private string _offerTitle;

	private string _offerDescription;

	private string _offerImage;

	private string _offerTerms;

	private string _offerCode;

	private DateTime _offerStartDate;

	private DateTime _offerExpiryDate;

	private DateTime? _dateRedeemed;

	private int _offerId;

	private RedeemTimer timer;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361822);
		GetValues();
		SetValues();
		_btnRedeem.Click += _btnRedeem_Click;
		BindBackButton();
	}

	private void BindBackButton()
	{
		FindViewById<ImageButton>(2131230791)!.Click += delegate
		{
			Finish();
		};
	}

	private void _btnRedeem_Click(object sender, EventArgs e)
	{
		if (_offerStartDate < DateTime.Now)
		{
			ConfirmRedeem();
		}
		else
		{
			ShowStartDateDialog();
		}
	}

	private void ShowStartDateDialog()
	{
		AlertDialog.Builder builder = new AlertDialog.Builder(this);
		builder.SetTitle("Voucher not yet valid!");
		builder.SetMessage($"This voucher is not valid until {_offerStartDate.TimeOfDay} on {_offerStartDate.ToShortDateString()}");
		builder.SetPositiveButton("Ok", delegate
		{
		});
		builder.Show();
	}

	private void ConfirmRedeem()
	{
		AlertDialog.Builder builder = new AlertDialog.Builder(this);
		builder.SetTitle("Activate voucher?");
		builder.SetMessage(2131492950);
		builder.SetPositiveButton("Yes", delegate
		{
			SharedServices.IoC.Resolve<OfferService>().RedeemOffer(_offerId, SharedSettings.DeviceInstanceId, DeviceType.Android, delegate(string response)
			{
				RunOnUiThread(delegate
				{
					_codeTextView.Text = response;
					timer = new RedeemTimer(600000L, 1000L);
					timer.Tick += Timer_Tick;
					timer.Start();
					_codeLayout.Visibility = ViewStates.Visible;
					_btnRedeem.Visibility = ViewStates.Gone;
					_expiryTextView.Visibility = ViewStates.Gone;
				});
			}, delegate
			{
			});
		});
		builder.SetNegativeButton("No", delegate
		{
		});
		builder.Show();
	}

	private void Timer_Tick(object sender, RedeemTimerTickEventArgs e)
	{
		_timeLeftTextView.Text = TimeSpan.FromMilliseconds(e.MillisUntilFinished).ToString("mm\\:ss");
		if (e.MillisUntilFinished < 60000)
		{
			_expiryTextView.SetTextColor(Color.Red);
		}
	}

	private void SetValues()
	{
		_titleTextView.Text = _offerTitle;
		_descriptionTextView.Text = _offerDescription;
		_expiryTextView.Text = "Expires " + _offerExpiryDate.ToShortDateString();
		_termsTextView.Text = _offerTerms;
		DisplayMetrics displayMetrics = new DisplayMetrics();
		WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
		Picasso.With(this).Load(_offerImage + "?width=" + displayMetrics.WidthPixels).Placeholder(2131165324)
			.Into(_offerImageView);
		if (_dateRedeemed.HasValue)
		{
			if (_dateRedeemed.Value.AddMinutes(10.0) > DateTime.Now)
			{
				timer = new RedeemTimer((long)(_dateRedeemed.Value.AddMinutes(10.0) - DateTime.Now).TotalMilliseconds, 1000L);
				timer.Tick += Timer_Tick;
				timer.Finish += Timer_Finish;
				timer.Start();
				_codeTextView.Text = _offerCode;
				_codeLayout.Visibility = ViewStates.Visible;
				_btnRedeem.Visibility = ViewStates.Gone;
				_expiryTextView.Visibility = ViewStates.Gone;
			}
			else
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetTitle("Expired voucher!");
				builder.SetMessage("Your voucher has expired, this page will now close.");
				builder.SetPositiveButton("Ok", delegate
				{
					Finish();
				});
				builder.Show();
			}
		}
		else
		{
			_codeLayout.Visibility = ViewStates.Gone;
			_btnRedeem.Visibility = ViewStates.Visible;
			_expiryTextView.Visibility = ViewStates.Visible;
		}
	}

	private void Timer_Finish(object sender, RedeemTimerFinishEventArgs e)
	{
		Finish();
	}

	private void GetValues()
	{
		_offerImageView = FindViewById<ImageView>(2131230905);
		_titleTextView = FindViewById<TextView>(2131231254);
		_descriptionTextView = FindViewById<TextView>(2131231251);
		_expiryTextView = FindViewById<TextView>(2131231252);
		_termsTextView = FindViewById<TextView>(2131231253);
		_btnRedeem = FindViewById<Button>(2131230783);
		_codeLayout = FindViewById<LinearLayout>(2131230834);
		_codeTextView = FindViewById<TextView>(2131231249);
		_timeLeftTextView = FindViewById<TextView>(2131231250);
		_offerTitle = Intent!.GetStringExtra("title");
		_offerDescription = Intent!.GetStringExtra("description");
		_offerId = Intent!.GetIntExtra("id", 0);
		_offerCode = Intent!.GetStringExtra("code");
		_offerImage = Intent!.GetStringExtra("image");
		_offerTerms = Intent!.GetStringExtra("terms");
		long longExtra = Intent!.GetLongExtra("startTicks", 0L);
		_offerStartDate = new DateTime(longExtra);
		long longExtra2 = Intent!.GetLongExtra("expiryTicks", 0L);
		_offerExpiryDate = new DateTime(longExtra2);
		long longExtra3 = Intent!.GetLongExtra("dateRedeemedTicks", 0L);
		if (longExtra3 > 0)
		{
			_dateRedeemed = new DateTime(longExtra3);
		}
	}
}
