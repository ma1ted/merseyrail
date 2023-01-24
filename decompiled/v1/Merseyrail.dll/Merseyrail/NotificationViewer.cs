using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Shared;

namespace Merseyrail;

[Activity(Label = "ReportAProblem", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true, ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]
public class NotificationViewer : Activity
{
	private bool ActionsBound { get; set; }

	private LinearLayout IncidentPanel { get; set; }

	private LinearLayout ReminderPanel { get; set; }

	private string TitleText { get; set; }

	private string DescriptionText { get; set; }

	private TextView msgTitle { get; set; }

	private TextView msgDescription { get; set; }

	private ImageButton BackButton { get; set; }

	private NotificationType? notificationType { get; set; }

	private string RouteCrs { get; set; }

	public List<RouteIncident> RouteIncidentlist { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361826);
		GetSharedPreferences(PackageName, FileCreationMode.Private);
		RouteIncidentlist = new List<RouteIncident>();
		TitleText = Intent!.GetStringExtra("Title") ?? "No message title found...";
		DescriptionText = Intent!.GetStringExtra("Description") ?? "No message Description found...";
		if (Intent!.Extras!.ContainsKey("NotificationType"))
		{
			notificationType = (NotificationType)Enum.Parse(typeof(NotificationType), Intent!.Extras!.GetString("NotificationType"));
		}
	}

	protected override void OnStart()
	{
		IncidentPanel = (LinearLayout)FindViewById(2131231156);
		ReminderPanel = (LinearLayout)FindViewById(2131231157);
		msgTitle = (TextView)FindViewById(2131231159);
		msgDescription = (TextView)FindViewById(2131231155);
		msgTitle.Text = TitleText;
		msgDescription.Text = DescriptionText;
		SetUIActions();
		if (notificationType.HasValue)
		{
			switch (notificationType)
			{
			case NotificationType.Incident:
				ShowIncidents();
				break;
			case NotificationType.Reminder:
				ShowReminder();
				break;
			}
		}
		base.OnStart();
	}

	private void ShowIncidents()
	{
		msgDescription.Text = (Intent!.Extras!.ContainsKey("Alert") ? Intent!.Extras!.GetString("Alert") : "n/a");
		RouteCrs = (Intent!.Extras!.ContainsKey("RouteCrs") ? Intent!.Extras!.GetString("RouteCrs") : "");
		ResetPanels();
	}

	private void UpdateIncidents(DateTime lastUpdated, List<RainbowBoardStatus> rainbowBoard)
	{
		Station station = SharedServices.IoC.Resolve<StationService>().GetStationByCrs(RouteCrs, withDescription: true);
		RainbowBoardStatus rainbowBoardStatus = rainbowBoard.Where((RainbowBoardStatus x) => x.RouteName == station.Name).FirstOrDefault();
		if (rainbowBoardStatus != null)
		{
			RouteIncidentlist = rainbowBoardStatus.Incidents;
		}
		if (RouteIncidentlist.Count > 0)
		{
			PopulateIncidentList(RouteIncidentlist);
		}
		else
		{
			NoIncidents();
		}
	}

	private void PopulateIncidentList(List<RouteIncident> incidentList)
	{
	}

	private void NoIncidents()
	{
	}

	private void ShowReminder()
	{
		msgDescription.Text = (Intent!.Extras!.ContainsKey("Alert") ? Intent!.Extras!.GetString("Alert") : "n/a");
		ResetPanels();
	}

	private void ResetPanels()
	{
	}

	private void SetUIActions()
	{
		if (!ActionsBound)
		{
			BackButton = (ImageButton)FindViewById(2131231074);
			BackButton.Click += delegate
			{
				OnBackPressed();
			};
			ActionsBound = true;
		}
	}

	public override void OnBackPressed()
	{
		Finish();
		Dispose();
	}
}
