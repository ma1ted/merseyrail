using System;
using Android.App;
using Android.Content;
using Common.Domain;
using Newtonsoft.Json;

namespace Merseyrail.BroadcastReceivers;

[BroadcastReceiver]
[IntentFilter(new string[] { "glow.merseyrail.DeparturesListItemClickReceiver" })]
public class DeparturesListItemClickReceiver : BroadcastReceiver
{
	public const string SelectedItemAction = "selecteditem";

	public event EventHandler<DepartureBoardItem> OnDepartureBoardItemSelected;

	public override void OnReceive(Context context, Intent intent)
	{
		DepartureBoardItem departureBoardItem = JsonConvert.DeserializeObject<DepartureBoardItem>(intent.GetStringExtra("selecteditem"));
		if (this.OnDepartureBoardItemSelected != null && departureBoardItem != null)
		{
			this.OnDepartureBoardItemSelected(this, departureBoardItem);
		}
	}
}
