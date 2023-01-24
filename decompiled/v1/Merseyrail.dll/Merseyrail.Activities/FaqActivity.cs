using System;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail.Activities;

[Activity(Label = "New Trains")]
public class FaqActivity : Activity
{
	private Faq _faq;

	private TextView _tvQuestion;

	private TextView _tvAnswer;

	private TextView _tvLastModified;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361819);
		_faq = new Faq
		{
			Id = Intent!.GetIntExtra("id", 0),
			Question = Intent!.GetStringExtra("question"),
			Answer = Intent!.GetStringExtra("answer"),
			LastModified = new DateTime(Intent!.GetLongExtra("lastModified", 0L))
		};
		BindBackButton();
		FindViews();
		SetValues();
	}

	private void SetValues()
	{
		_tvQuestion.Text = _faq.Question;
		_tvAnswer.TextFormatted = Html.FromHtml(_faq.Answer);
		_tvLastModified.Text = "Last Modified: " + _faq.LastModified.ToString("dd/MM/yyyy HH:mm");
	}

	private void FindViews()
	{
		_tvQuestion = FindViewById<TextView>(2131231255);
		_tvAnswer = FindViewById<TextView>(2131231244);
		_tvLastModified = FindViewById<TextView>(2131231246);
	}

	private void BindBackButton()
	{
		FindViewById<ImageButton>(2131230788)!.Click += delegate
		{
			Finish();
		};
	}
}
