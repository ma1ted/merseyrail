using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common;
using Common.Domain;
using Merseyrail.Adapters;
using Newtonsoft.Json;

namespace Merseyrail.Fragments;

public class TrainJourneyList : ListFragment
{
	public int PageSize = 24;

	public int CurrentPage;

	private bool actionsSet;

	public List<DepartureBoardItem> TrainJourneyListItems { get; set; }

	private TrainJourneyListAdapter TrainJourneyAdapter { get; set; }

	public Action<int> GetPagedData { get; set; }

	public int Offset => CurrentPage * PageSize;

	public event EventHandler TrainJourneyListReady;

	public TrainJourneyList()
	{
		TrainJourneyListItems = new List<DepartureBoardItem>();
	}

	public TrainJourneyList(List<DepartureBoardItem> departureBoardItems)
	{
		TrainJourneyListItems = departureBoardItems;
	}

	public override void OnActivityCreated(Bundle savedInstanceState)
	{
		base.OnActivityCreated(savedInstanceState);
		TrainJourneyAdapter = new TrainJourneyListAdapter(TrainJourneyListItems, base.Activity);
		ListAdapter = TrainJourneyAdapter;
		int selectIndex = GetSelectIndex(TrainJourneyListItems);
		SetSelection(selectIndex);
		View.Post(delegate
		{
			if (this.TrainJourneyListReady != null)
			{
				this.TrainJourneyListReady(this, null);
			}
		});
	}

	public override void OnViewCreated(View view, Bundle savedInstanceState)
	{
		base.OnViewCreated(view, savedInstanceState);
		if (!actionsSet)
		{
			ListView.ItemClick += OnListClicked;
			actionsSet = true;
		}
	}

	public override void OnResume()
	{
		base.OnResume();
	}

	public override void OnPause()
	{
		ListView.ItemClick -= OnListClicked;
		base.OnPause();
	}

	private void OnListClicked(object sender, AdapterView.ItemClickEventArgs e)
	{
		ListView.SetItemChecked(e.Position, value: true);
		Departures.ShowTrain(TrainJourneyAdapter.items[e.Position].TrainId);
	}

	private void BroadcastSelectedItem(DepartureBoardItem item)
	{
		Intent intent = new Intent("glow.merseyrail.DeparturesListItemClickReceiver");
		intent.PutExtra("selecteditem", JsonConvert.SerializeObject(item));
		base.Activity.SendBroadcast(intent);
	}

	public override void OnListItemClick(ListView l, View v, int index, long id)
	{
	}

	public void UpdateList(List<DepartureBoardItem> items)
	{
		if (base.Activity != null && items != null && items.Count > 0)
		{
			base.Activity.RunOnUiThread(delegate
			{
				TrainJourneyAdapter = new TrainJourneyListAdapter(items, base.Activity);
				TrainJourneyListItems = items;
				int selectIndex = GetSelectIndex(items);
				ListAdapter = TrainJourneyAdapter;
				SetSelection(selectIndex);
			});
		}
		else if (base.Activity != null)
		{
			base.Activity.RunOnUiThread(delegate
			{
				Toast.MakeText(base.Activity, "No departure board items returned", ToastLength.Short)!.Show();
			});
		}
	}

	public void ClearList()
	{
		base.Activity.RunOnUiThread(delegate
		{
			List<DepartureBoardItem> list = new List<DepartureBoardItem>();
			TrainJourneyAdapter = new TrainJourneyListAdapter(list, base.Activity);
			ListAdapter = TrainJourneyAdapter;
			TrainJourneyListItems = list;
			int selectIndex = GetSelectIndex(list);
			SetSelection(selectIndex);
		});
	}

	private int GetSelectIndex(List<DepartureBoardItem> items)
	{
		int result = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (i > 1)
			{
				DepartureBoardItem departureBoardItem = items[i];
				_ = items[i - 1];
				if (Utils.OverNightTime((departureBoardItem.DepartureActual.TotalSeconds == 0.0) ? departureBoardItem.ArrivalActual : departureBoardItem.DepartureActual).Subtract(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)).TotalMinutes < 1.0)
				{
					result = i;
				}
			}
		}
		return result;
	}
}
