using System;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Common.Services;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class StationSelector : Fragment
{
	public string SelectedValue { get; set; }

	public event EventHandler<AdapterView.ItemSelectedEventArgs> OnItemSelected;

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup viewGroup, Bundle bundle)
	{
		View? view = inflater.Inflate(2131361872, null, attachToRoot: false);
		Spinner spinner = view!.FindViewById<Spinner>(2131231170);
		spinner.ItemSelected += spinner_ItemSelected;
		ArrayAdapter arrayAdapter = new ArrayAdapter(objects: (from x in SharedServices.IoC.Resolve<StationService>().GetStations(merseyrailOnly: true)
			select x.Name).ToArray(), context: base.Activity, resource: 17367048);
		arrayAdapter.SetDropDownViewResource(17367049);
		spinner.Adapter = arrayAdapter;
		return view;
	}

	private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
	{
		_ = e.View;
		_ = e.Id;
		SelectedValue = e.Id.ToString();
		if (this.OnItemSelected != null)
		{
			this.OnItemSelected(this, e);
		}
	}

	public override void OnStart()
	{
		base.OnStart();
	}
}
