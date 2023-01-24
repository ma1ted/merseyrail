using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Merseyrail.Activities;
using Merseyrail.Adapters.ViewHolders;
using Merseyrail.Events;
using Merseyrail.Timers;
using Square.Picasso;

namespace Merseyrail.Adapters;

public class OfferAdapter : RecyclerView.Adapter
{
	private List<Offer> _offers;

	private string _baseImagePath;

	public override int ItemCount => _offers.Count;

	public event EventHandler<EventArgs> RefreshOffers;

	public OfferAdapter(List<Offer> offers, string baseImagePath)
	{
		_offers = offers;
		_baseImagePath = baseImagePath;
	}

	public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
	{
		OfferViewHolder offerViewHolder = holder as OfferViewHolder;
		Offer offer = _offers[position];
		Context context = offerViewHolder.ItemView.Context;
		if (offerViewHolder == null)
		{
			return;
		}
		offerViewHolder.Title.Text = offer.Title;
		offerViewHolder.Description.Text = offer.Description;
		CheckOfferStatus(offerViewHolder.ExpiryDateTextView, offer);
		string imgPath = _baseImagePath + offer.Image;
		if (!string.IsNullOrEmpty(offer.Image))
		{
			offerViewHolder.Image.Visibility = ViewStates.Visible;
			offerViewHolder.RedeemedImage.Visibility = ViewStates.Visible;
			DisplayMetrics displayMetrics = new DisplayMetrics();
			((Activity)context).WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
			Picasso.With(context).Load(imgPath + "?width=" + displayMetrics.WidthPixels).Placeholder(2131165324)
				.Into(offerViewHolder.Image);
		}
		else
		{
			offerViewHolder.Image.Visibility = ViewStates.Gone;
			offerViewHolder.RedeemedImage.Visibility = ViewStates.Gone;
		}
		ToggleRedeemedIcon(offerViewHolder, offer);
		offerViewHolder.ViewButton.Click += delegate
		{
			Intent intent = new Intent(context, typeof(OfferActivity));
			intent.PutExtra("title", offer.Title);
			intent.PutExtra("image", imgPath);
			intent.PutExtra("terms", offer.TermsAndConditions);
			intent.PutExtra("description", offer.Description);
			intent.PutExtra("expiryTicks", offer.EndDate.Ticks);
			intent.PutExtra("startTicks", offer.StartDate.Ticks);
			if (offer.DateRedeemed.HasValue)
			{
				intent.PutExtra("dateRedeemedTicks", offer.DateRedeemed.Value.Ticks);
			}
			intent.PutExtra("code", offer.Code);
			intent.PutExtra("id", offer.Id);
			context.StartActivity(intent);
		};
	}

	private void CheckOfferStatus(TextView tv, Offer offer)
	{
		if (offer.DateRedeemed.HasValue)
		{
			if (offer.DateRedeemed.Value.AddMinutes(10.0) > DateTime.Now)
			{
				RedeemTimer redeemTimer = new RedeemTimer((long)(offer.DateRedeemed.Value.AddMinutes(10.0) - DateTime.Now).TotalMilliseconds, 1000L);
				redeemTimer.Tick += delegate(object sender, RedeemTimerTickEventArgs args)
				{
					tv.Text = "Expires " + TimeSpan.FromMilliseconds(args.MillisUntilFinished).ToString("mm\\:ss");
					tv.SetTextColor((args.MillisUntilFinished < 60000) ? Color.Red : new Color(255, 206, 0));
				};
				redeemTimer.Finish += delegate
				{
					this.RefreshOffers?.Invoke(this, new EventArgs());
				};
				redeemTimer.Start();
			}
			else
			{
				tv.Text = "EXPIRED";
				tv.SetTextColor(Color.Red);
			}
		}
		else if (offer.EndDate > DateTime.Now)
		{
			tv.Text = "Expires " + offer.EndDate.ToShortDateString();
		}
		else
		{
			tv.Text = "EXPIRED";
			tv.SetTextColor(Color.Red);
		}
	}

	private static void ToggleRedeemedIcon(OfferViewHolder vh, Offer offer)
	{
		if (offer.DateRedeemed.HasValue)
		{
			if (!string.IsNullOrEmpty(offer.Image))
			{
				vh.RedeemedImage.Visibility = ViewStates.Visible;
			}
			if (offer.DateRedeemed.Value.AddMinutes(10.0) > DateTime.Now)
			{
				vh.RedeemedImage.SetImageResource(2131165369);
				vh.ViewButton.Visibility = ViewStates.Visible;
			}
			else
			{
				vh.RedeemedImage.SetImageResource(2131165372);
				vh.ViewButton.Visibility = ViewStates.Gone;
			}
		}
		else if (!string.IsNullOrEmpty(offer.Image))
		{
			vh.RedeemedImage.Visibility = ViewStates.Invisible;
		}
	}

	public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
	{
		return new OfferViewHolder(LayoutInflater.From(parent.Context)!.Inflate(2131361909, parent, attachToRoot: false));
	}
}
