using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Adapters;
using Merseyrail.Shared;

namespace Merseyrail;

[Activity(Label = "Mtogo Offers", ScreenOrientation = ScreenOrientation.Portrait)]
public class MtogoActivity : Activity
{
	private RecyclerView _recyclerView;

	private RecyclerView.LayoutManager _layoutManager;

	private List<Offer> _offers;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361821);
		_recyclerView = FindViewById<RecyclerView>(2131231062);
		SetLayoutManager();
		SetAdapter();
		BindEvents();
	}

	private void BindEvents()
	{
		FindViewById<ImageButton>(2131230790)!.Click += delegate
		{
			Finish();
		};
	}

	private void SetLayoutManager()
	{
		_layoutManager = new LinearLayoutManager(this, 1, reverseLayout: false);
		_recyclerView.SetLayoutManager(_layoutManager);
	}

	private void SetAdapter()
	{
		SharedServices.IoC.Resolve<OfferService>().GetOffers(SharedSettings.DeviceInstanceId, DeviceType.Android, delegate(List<Offer> result)
		{
			if (result != null)
			{
				DateTime now = DateTime.Now;
				result = result.Where((Offer r) => !r.DateRedeemed.HasValue || AddTen(r.DateRedeemed.Value) > now).ToList();
				_offers = result;
				if (_offers.Count > 0)
				{
					List<Offer> orderedOffers = SharedServices.IoC.Resolve<OfferService>().OrderOffers(_offers);
					RunOnUiThread(delegate
					{
						OfferAdapter offerAdapter = new OfferAdapter(orderedOffers, "https://merseyrail.app/admin/Uploads/Offers");
						offerAdapter.RefreshOffers += delegate
						{
							SetAdapter();
						};
						_recyclerView.SetAdapter(offerAdapter);
					});
				}
				else
				{
					RunOnUiThread(delegate
					{
						ShowNoOffersDialog();
					});
				}
			}
			else
			{
				RunOnUiThread(delegate
				{
					ShowNoOffersDialog();
				});
			}
		}, delegate
		{
			RunOnUiThread(delegate
			{
				ShowNoOffersDialog(error: true);
			});
		});
	}

	private DateTime AddTen(DateTime dateTime)
	{
		return dateTime.AddMinutes(10.0);
	}

	private void ShowNoOffersDialog(bool error = false)
	{
		AlertDialog.Builder builder = new AlertDialog.Builder(this);
		builder.SetCancelable(cancelable: false);
		if (!error)
		{
			builder.SetTitle("No Offers Available!");
			builder.SetMessage("There are currently no offers available to you at this time, please try again later.");
		}
		else
		{
			builder.SetTitle("Unable to retrieve offers");
			builder.SetMessage("We are currently unable to show offers to you at this time, please try again later.");
		}
		builder.SetPositiveButton("Ok", delegate
		{
			Finish();
		});
		builder.Show();
	}

	protected override void OnResume()
	{
		base.OnResume();
		SetAdapter();
	}
}
