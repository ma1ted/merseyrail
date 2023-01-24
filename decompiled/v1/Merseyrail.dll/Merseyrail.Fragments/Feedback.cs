using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Services;
using Java.Lang;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class Feedback : BaseFragment
{
	private bool actionsBound;

	private View _view { get; set; }

	private Button Feedback_btn { get; set; }

	private Button Report_btn { get; set; }

	private ImageButton BackButton { get; set; }

	private EditText Name { get; set; }

	private EditText EmailAddress { get; set; }

	private EditText Message { get; set; }

	private string reportImageStr { get; set; }

	private Button SendReport { get; set; }

	private LinearLayout ChoosePanel { get; set; }

	private LinearLayout FeedbackPanel { get; set; }

	private bool ActionsBound { get; set; }

	public Feedback()
	{
		actionsBound = false;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	private void SetDefaultView()
	{
		base.Activity.RunOnUiThread(delegate
		{
			ChoosePanel.Visibility = ViewStates.Visible;
			FeedbackPanel.Visibility = ViewStates.Gone;
		});
	}

	private void SetFeedbackView()
	{
		base.Activity.RunOnUiThread(delegate
		{
			ChoosePanel.Visibility = ViewStates.Gone;
			FeedbackPanel.Visibility = ViewStates.Visible;
		});
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup vireGroup, Bundle savedInstance)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361840, null, attachToRoot: false);
		}
		ChoosePanel = (LinearLayout)_view.FindViewById(2131230875);
		FeedbackPanel = (LinearLayout)_view.FindViewById(2131230876);
		Feedback_btn = (Button)_view.FindViewById(2131230869);
		Report_btn = (Button)_view.FindViewById(2131230870);
		ImageButton imageButton = (ImageButton)_view.FindViewById(2131230874);
		if (!actionsBound)
		{
			imageButton.Click += delegate
			{
				((Main)base.Activity).OpenDrawerLayout();
			};
			Feedback_btn.Click += delegate
			{
				SetFeedbackView();
			};
			Report_btn.Click += delegate
			{
				Intent intent = new Intent((Main)base.Activity, typeof(ReportAProblem));
				base.Activity.StartActivityForResult(intent, 411);
			};
			actionsBound = true;
		}
		Name = (EditText)_view.FindViewById(2131230873);
		Name.Text = SharedSettings.DefaultName;
		EmailAddress = (EditText)_view.FindViewById(2131230872);
		EmailAddress.Text = SharedSettings.DefaultEmail;
		Message = (EditText)_view.FindViewById(2131230871);
		SendReport = (Button)_view.FindViewById(2131230868);
		SetUIActions();
		return _view;
	}

	public override void OnResume()
	{
		base.OnResume();
		SetDefaultView();
	}

	private void SetUIActions()
	{
		if (!ActionsBound)
		{
			SendReport.Click += delegate
			{
				string text = Name.Text;
				string text2 = EmailAddress.Text;
				string text3 = Message.Text;
				_ = reportImageStr;
				SharedServices.IoC.Resolve<RemoteServices>().PostFeedbackReport(text, text2, text3, delegate(bool response)
				{
					if (response)
					{
						ProcessReoprtProblemResponse("Feedback sent successfully.");
						ResetForm();
						reportImageStr = string.Empty;
					}
					else
					{
						ProcessReoprtProblemResponse("Feedback could not be sent.");
						reportImageStr = string.Empty;
					}
				});
			};
		}
		ActionsBound = true;
	}

	private void ProcessReoprtProblemResponse(string response)
	{
		base.Activity.RunOnUiThread(new Runnable(delegate
		{
			Toast.MakeText(base.Activity, response, ToastLength.Short)!.Show();
		}));
	}

	private void ResetForm()
	{
		base.Activity.RunOnUiThread(new Runnable(delegate
		{
			Message.Text = "";
		}));
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}
}
