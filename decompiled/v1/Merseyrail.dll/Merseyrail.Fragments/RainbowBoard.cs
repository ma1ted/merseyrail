using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Shared;
using Newtonsoft.Json;

namespace Merseyrail.Fragments;

[IntentFilter(new string[] { "android.intent.action.MAIN" }, Categories = new string[] { "SectionFragment" })]
public class RainbowBoard : Android.Support.V4.App.Fragment
{
	public Action OnRainbowBoardUpdated;

	public LinearLayout ItemContainer { get; set; }

	public RainbowBoardItem rainbowBoard_WestKirby { get; set; }

	public RainbowBoardItem rainbowBoard_EllesmerePort { get; set; }

	public RainbowBoardItem rainbowBoard_NewBrighton { get; set; }

	public RainbowBoardItem rainbowBoard_Chester { get; set; }

	public RainbowBoardItem rainbowBoard_Southport { get; set; }

	public RainbowBoardItem rainbowBoard_Kirkby { get; set; }

	public RainbowBoardItem rainbowBoard_HuntsCross { get; set; }

	public RainbowBoardItem rainbowBoard_Ormskirk { get; set; }

	public List<RainbowBoardStatus> RainbowBoardStatusList { get; set; }

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RainbowBoardStatusList = new List<RainbowBoardStatus>();
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
	{
		View? result = inflater.Inflate(2131361866, null, attachToRoot: false);
		rainbowBoard_WestKirby = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231105);
		if (rainbowBoard_WestKirby != null)
		{
			rainbowBoard_WestKirby.OnClick += delegate
			{
				if (rainbowBoard_WestKirby.HasIncidents)
				{
					StartActivity("West Kirby");
				}
			};
		}
		rainbowBoard_EllesmerePort = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231099);
		if (rainbowBoard_EllesmerePort != null)
		{
			rainbowBoard_EllesmerePort.OnClick += delegate
			{
				if (rainbowBoard_EllesmerePort.HasIncidents)
				{
					StartActivity("Ellesmere Port");
				}
			};
		}
		rainbowBoard_NewBrighton = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231102);
		if (rainbowBoard_NewBrighton != null)
		{
			rainbowBoard_NewBrighton.OnClick += delegate
			{
				if (rainbowBoard_NewBrighton.HasIncidents)
				{
					StartActivity("New Brighton");
				}
			};
		}
		rainbowBoard_Chester = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231098);
		if (rainbowBoard_Chester != null)
		{
			rainbowBoard_Chester.OnClick += delegate
			{
				if (rainbowBoard_Chester.HasIncidents)
				{
					StartActivity("Chester");
				}
			};
		}
		rainbowBoard_Southport = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231104);
		if (rainbowBoard_Southport != null)
		{
			rainbowBoard_Southport.OnClick += delegate
			{
				if (rainbowBoard_Southport.HasIncidents)
				{
					StartActivity("Southport");
				}
			};
		}
		rainbowBoard_Kirkby = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231101);
		if (rainbowBoard_Kirkby != null)
		{
			rainbowBoard_Kirkby.OnClick += delegate
			{
				if (rainbowBoard_Kirkby.HasIncidents)
				{
					StartActivity("Kirkby");
				}
			};
		}
		rainbowBoard_HuntsCross = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231100);
		if (rainbowBoard_HuntsCross != null)
		{
			rainbowBoard_HuntsCross.OnClick += delegate
			{
				if (rainbowBoard_HuntsCross.HasIncidents)
				{
					StartActivity("Hunts Cross");
				}
			};
		}
		rainbowBoard_Ormskirk = (RainbowBoardItem)base.ChildFragmentManager.FindFragmentById(2131231103);
		if (rainbowBoard_Ormskirk != null)
		{
			rainbowBoard_Ormskirk.OnClick += delegate
			{
				if (rainbowBoard_Ormskirk.HasIncidents)
				{
					StartActivity("Ormskirk");
				}
			};
		}
		return result;
	}

	public void UpdateRainbowBoard()
	{
		SharedServices.IoC.Resolve<RainbowBoardService>().GetRainbowBoard(UpdateRainbowBoardStatus);
	}

	public override void OnResume()
	{
		UpdateRainbowBoard();
		base.OnResume();
	}

	private void StartActivity(string route)
	{
		Intent intent = new Intent(base.Activity, typeof(RainbowBoardIncidentsActivity));
		intent.PutExtra("route", route);
		string value = JsonConvert.SerializeObject(RainbowBoardStatusList);
		intent.PutExtra("rainbowBoardStatusListJson", value);
		base.Activity.StartActivity(intent);
	}

	public void UpdateRainbowBoardStatus(DateTime dateTime, List<RainbowBoardStatus> rainbowBoardStatusList)
	{
		RainbowBoardStatusList = rainbowBoardStatusList;
		base.Activity.RunOnUiThread(delegate
		{
			if (rainbowBoardStatusList != null)
			{
				foreach (RainbowBoardStatus rainbowBoardStatus in rainbowBoardStatusList)
				{
					RainbowBoardItem currentItem = GetCurrentItem(rainbowBoardStatus.RouteName);
					if (currentItem != null)
					{
						currentItem.HasIncidents = rainbowBoardStatus.Incidents != null && rainbowBoardStatus.Incidents.Count > 0;
						switch (rainbowBoardStatus.StatusId)
						{
						case 0:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.green;
							break;
						case 1:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.amber;
							break;
						case 2:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.red;
							break;
						case 3:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.red;
							break;
						case 4:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.black;
							break;
						case 5:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.purple;
							break;
						case 6:
							currentItem.ItemStatus = RainbowBoardItem.RainbowStatus.purple;
							break;
						}
						currentItem.StatusText = rainbowBoardStatus.StatusName;
					}
				}
			}
		});
		if (OnRainbowBoardUpdated != null)
		{
			OnRainbowBoardUpdated();
		}
	}

	private RainbowBoardItem GetCurrentItem(string routeName)
	{
		return routeName switch
		{
			"West Kirby" => rainbowBoard_WestKirby, 
			"Ellesmere Port" => rainbowBoard_EllesmerePort, 
			"New Brighton" => rainbowBoard_NewBrighton, 
			"Chester" => rainbowBoard_Chester, 
			"Southport" => rainbowBoard_Southport, 
			"Kirkby" => rainbowBoard_Kirkby, 
			"Hunts Cross" => rainbowBoard_HuntsCross, 
			"Ormskirk" => rainbowBoard_Ormskirk, 
			_ => rainbowBoard_WestKirby, 
		};
	}
}
