using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Adapters;
using Merseyrail.Helpers;
using Merseyrail.Shared;
using Square.Picasso;

namespace Merseyrail.Activities;

[Activity(Label = "FAQs")]
public class FaqsActivity : Activity
{
	private ListView _faqsView;

	private ListView _imagesView;

	private TextView _tvIntro;

	private TextView _tvFaqHeader;

	private TextView _tvDesignHeader;

	private TextView _tvDesignText;

	private ImageView _imgTrainImage;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361820);
		BindBackButton();
		FindViews();
		SetFaqsAndImages();
	}

	private void SetFaqsAndImages()
	{
		SharedServices.IoC.Resolve<FaqService>().GetFaqs(delegate(List<Faq> faqs)
		{
			if (faqs != null)
			{
				RunOnUiThread(delegate
				{
					_tvIntro.TextFormatted = Html.FromHtml(faqs.Single((Faq f) => f.Question == "Intro").Answer);
					_tvDesignHeader.Visibility = ViewStates.Visible;
					_tvFaqHeader.Visibility = ViewStates.Visible;
					_tvDesignText.Visibility = ViewStates.Visible;
					FaqAdapter adapter2 = new FaqAdapter(faqs.Where((Faq f) => f.Question != "Intro").ToList(), this);
					_faqsView.Adapter = adapter2;
					UIUtils.SetListViewHeightBasedOnItems(_faqsView);
					_faqsView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
					{
						Faq faq = faqs[args.Position];
						Intent intent = new Intent(this, typeof(FaqActivity));
						intent.PutExtra("id", faq.Id);
						intent.PutExtra("question", faq.Question);
						intent.PutExtra("answer", faq.Answer);
						intent.PutExtra("lastModified", faq.LastModified.Ticks);
						StartActivity(intent);
					};
				});
			}
		}, delegate
		{
			throw new Exception("Unable to retrieve FAQs");
		});
		SharedServices.IoC.Resolve<FaqService>().GetTrainImages(delegate(List<NewTrainImage> images)
		{
			if (images != null)
			{
				List<string> filePaths = images.Select((NewTrainImage i) => "https://merseyrail.app/admin/Uploads/NewTrainImages" + i.ImageFile).ToList();
				RunOnUiThread(delegate
				{
					DisplayMetrics displayMetrics = new DisplayMetrics();
					WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
					Picasso.With(this).Load(filePaths.First() + "?width=" + displayMetrics.WidthPixels).Placeholder(2131165324)
						.Into(_imgTrainImage);
					ImageAdapter adapter = new ImageAdapter((from i in images.Skip(1)
						select "https://merseyrail.app/admin/Uploads/NewTrainImages" + i.ImageFile).ToList(), this);
					_imagesView.Adapter = adapter;
					UIUtils.SetListViewHeightBasedOnItems(_imagesView);
				});
			}
		}, delegate
		{
			throw new Exception("Unable to retrieve Images");
		});
	}

	private void BindBackButton()
	{
		FindViewById<ImageButton>(2131230789)!.Click += delegate
		{
			Finish();
		};
	}

	private void FindViews()
	{
		_faqsView = FindViewById<ListView>(2131230864);
		_imagesView = FindViewById<ListView>(2131230901);
		_imgTrainImage = FindViewById<ImageView>(2131230902);
		_tvIntro = FindViewById<TextView>(2131231245);
		_tvFaqHeader = FindViewById<TextView>(2131230863);
		_tvDesignHeader = FindViewById<TextView>(2131230851);
		_tvDesignText = FindViewById<TextView>(2131230852);
	}
}
