using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Java.Interop;

namespace Merseyrail.AppWidgets;

[Activity(Label = "Merseyrail Widget Settings")]
public class DeparturesWidgetPreferencesActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener, IJavaObject, IDisposable, IJavaPeerable
{
	private int appWidgetId;

	private bool dirty;

	public const string USE_NEAREST_STATION = "USE_NEAREST_STATION";

	public const string SELECTED_STATION = "SELECTED_STATION";

	public const string PREFSFILE = "DeparturesWidgetPreferences_";

	public const string USE_RB_SOUTHPORT = "USE_RB_SOUTHPORT";

	public const string USE_RB_KIRKBY = "USE_RB_KIRKBY";

	public const string USE_RB_WEST_KIRBY = "USE_RB_WEST_KIRBY";

	public const string USE_RB_ELLESMERE_PORT = "USE_RB_ELLESMERE_PORT";

	public const string USE_RB_ORMSKIRK = "USE_RB_ORMSKIRK";

	public const string USE_RB_HUNTS_CROSS = "USE_RB_HUNTS_CROSS";

	public const string USE_RB_NEW_BRIGHTON = "USE_RB_NEW_BRIGHTON";

	public const string USE_RB_CHESTER = "USE_RB_CHESTER";

	private ISharedPreferences prefs;

	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		SetContentView(2131361917);
		ProcessIntent();
		AddPreferencesFromResource(2131689474);
	}

	private void ProcessIntent()
	{
		if (Intent != null && Intent!.Extras != null && Intent!.Extras!.ContainsKey("appWidgetId"))
		{
			appWidgetId = Intent!.Extras!.GetInt("appWidgetId");
			string sharedPreferencesNameForAppWidget = DeparturesWidgetProvider.GetSharedPreferencesNameForAppWidget(BaseContext, appWidgetId);
			PreferenceManager!.SharedPreferencesName = sharedPreferencesNameForAppWidget;
			prefs = BaseContext!.GetSharedPreferences(sharedPreferencesNameForAppWidget, FileCreationMode.Private);
		}
	}

	protected override void OnResume()
	{
		base.OnResume();
		PreferenceScreen!.SharedPreferences!.RegisterOnSharedPreferenceChangeListener(this);
	}

	protected override void OnPause()
	{
		if (dirty)
		{
			UpdateWidgets();
		}
		PreferenceScreen!.SharedPreferences!.UnregisterOnSharedPreferenceChangeListener(this);
		base.OnPause();
	}

	protected override void OnStop()
	{
		base.OnStop();
		if (dirty)
		{
			UpdateWidgets();
		}
	}

	public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
	{
		dirty = true;
	}

	private void UpdateWidgets()
	{
		dirty = false;
		Intent intent = new Intent("glow.merseyrail.widget.REFRESH");
		intent.PutExtra("appWidgetId", appWidgetId);
		SendBroadcast(intent);
	}
}
