using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Newtonsoft.Json;

namespace Merseyrail;

[Activity(Label = "StationDetailActivity", NoHistory = true)]
public class RainbowBoardIncidentsActivity : Activity
{
	private RainbowBoardStatus rainbowBoardStatus;

	private List<RainbowBoardStatus> rainbowBoardStatusList;

	public string StationName { get; set; }

	public TextView RouteHeader { get; set; }

	public TextView IncidentsText { get; set; }

	public LinearLayout IncidentPanel { get; set; }

	public ListView IncidentList { get; set; }

	public LinearLayout NoIncidentPanel { get; set; }

	private string Route { get; set; }

	public List<RouteIncident> RouteIncidentlist { get; set; }

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		RequestWindowFeature(WindowFeatures.NoTitle);
		SetContentView(2131361823);
		RouteHeader = (TextView)FindViewById(2131231069);
		IncidentPanel = (LinearLayout)FindViewById(2131230746);
		IncidentsText = (TextView)FindViewById(2131231093);
		IncidentList = (ListView)FindViewById(2131230747);
		NoIncidentPanel = (LinearLayout)FindViewById(2131230748);
		((ImageButton)FindViewById(2131231068)).Click += delegate
		{
			Finish();
			Dispose();
		};
		Route = Intent!.GetStringExtra("route");
		string stringExtra = Intent!.GetStringExtra("rainbowBoardStatusListJson");
		rainbowBoardStatusList = JsonConvert.DeserializeObject<List<RainbowBoardStatus>>(stringExtra);
		UpdateUI();
	}

	private void UpdateIncidents(DateTime lastUpdated, List<RainbowBoardStatus> rainbowBoard)
	{
		rainbowBoardStatus = rainbowBoard.Where((RainbowBoardStatus x) => x.RouteName == Route).FirstOrDefault();
		RunOnUiThread(delegate
		{
			RouteHeader.Text = rainbowBoardStatus.RouteName + " line incidents";
			IncidentsText.Text = rainbowBoardStatus.StatusName;
		});
		if (rainbowBoardStatus != null)
		{
			RouteIncidentlist = rainbowBoardStatus.Incidents;
		}
		if (RouteIncidentlist.Count > 0)
		{
			ShowIncidents(RouteIncidentlist);
		}
		else
		{
			ShowNoIncidents();
		}
	}

	private void UpdateUI()
	{
		rainbowBoardStatus = rainbowBoardStatusList.Where((RainbowBoardStatus x) => x.RouteName == Route).FirstOrDefault();
		RunOnUiThread(delegate
		{
			RouteHeader.Text = rainbowBoardStatus.RouteName + " line incidents";
			IncidentsText.Text = rainbowBoardStatus.StatusName;
		});
		if (rainbowBoardStatus != null)
		{
			RouteIncidentlist = rainbowBoardStatus.Incidents;
		}
		if (RouteIncidentlist.Count > 0)
		{
			ShowIncidents(RouteIncidentlist);
		}
		else
		{
			ShowNoIncidents();
		}
	}

	private void ShowIncidents(List<RouteIncident> routeIncidentlist)
	{
		RunOnUiThread(delegate
		{
			IncidentPanel.Visibility = ViewStates.Visible;
			NoIncidentPanel.Visibility = ViewStates.Gone;
			IncidentList.Adapter = new RainbowBoardIncidentsListAdapter(this, routeIncidentlist);
		});
	}

	private void ShowNoIncidents()
	{
		RunOnUiThread(delegate
		{
			IncidentPanel.Visibility = ViewStates.Gone;
			NoIncidentPanel.Visibility = ViewStates.Visible;
		});
	}
}
