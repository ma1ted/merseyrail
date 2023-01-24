using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Merseyrail.Adapters;
using Merseyrail.Shared;

namespace Merseyrail.Activities;

[Activity(Label = "DeveloperActivity", ScreenOrientation = ScreenOrientation.Portrait)]
public class DeveloperActivity : Activity
{
	private ListView DeveloperListView { get; set; }

	private DeveloperListAdapter Adapter { get; set; }

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361818);
		DeveloperListView = FindViewById<ListView>(2131230853);
		Adapter = new DeveloperListAdapter(this, GetDeveloperListItems());
		DeveloperListView.Adapter = Adapter;
		FindViewById<ImageButton>(2131230787)!.Click += delegate
		{
			Finish();
		};
	}

	protected override void OnResume()
	{
		base.OnResume();
		Adapter = new DeveloperListAdapter(this, GetDeveloperListItems());
		DeveloperListView.Adapter = Adapter;
	}

	private List<DeveloperItem> GetDeveloperListItems()
	{
		bool flag = SharedSettings.HasConnection(this);
		return new List<DeveloperItem>
		{
			new DeveloperItem
			{
				Title = "Has Connection",
				Description = flag.ToString()
			},
			GetAppDomain(),
			GetAppVersion(),
			GetApiUrl(),
			GetBusApiUrl(),
			GetOfferImagePath(),
			GetNewTrainsImagePath(),
			new DeveloperItem
			{
				Title = "Force Crash",
				Description = "Test crash reporting"
			}
		};
	}

	private static DeveloperItem GetAppDomain()
	{
		return new DeveloperItem
		{
			Title = "Application Domain",
			Description = Android.App.Application.Context.PackageName
		};
	}

	private static DeveloperItem GetAppVersion()
	{
		return new DeveloperItem
		{
			Title = "Application Version",
			Description = SharedSettings.AppVersion
		};
	}

	private DeveloperItem GetApiUrl()
	{
		DeveloperItem developerItem = new DeveloperItem
		{
			Title = "API URL"
		};
		try
		{
			developerItem.Description = "https://merseyrail.app";
			return developerItem;
		}
		catch
		{
			developerItem.Description = "Unable to get api url";
			return developerItem;
		}
	}

	private DeveloperItem GetBusApiUrl()
	{
		DeveloperItem developerItem = new DeveloperItem
		{
			Title = "Rail Replacement Bus API URL"
		};
		try
		{
			developerItem.Description = "http://XXXXXX/MobileApi/CheckBusStatus";
			return developerItem;
		}
		catch
		{
			developerItem.Description = "Unable to get bus api url";
			return developerItem;
		}
	}

	private DeveloperItem GetOfferImagePath()
	{
		DeveloperItem developerItem = new DeveloperItem
		{
			Title = "Offer Images Path"
		};
		try
		{
			developerItem.Description = "https://merseyrail.app/admin/Uploads/Offers";
			return developerItem;
		}
		catch
		{
			developerItem.Description = "Unable to get path";
			return developerItem;
		}
	}

	private DeveloperItem GetNewTrainsImagePath()
	{
		DeveloperItem developerItem = new DeveloperItem
		{
			Title = "New Trains Images Path"
		};
		try
		{
			developerItem.Description = "https://merseyrail.app/admin/Uploads/NewTrainImages";
			return developerItem;
		}
		catch
		{
			developerItem.Description = "Unable to get path";
			return developerItem;
		}
	}
}
