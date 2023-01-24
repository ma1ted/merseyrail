using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

namespace Merseyrail;

[Activity(Label = "AppPreferencesActivity", MainLauncher = false, ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ScreenOrientation = ScreenOrientation.Portrait)]
public class AppPreferencesActivity : PreferenceActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		AddPreferencesFromResource(2131689472);
		BindPrefs();
	}

	private void BindPrefs()
	{
		FindPreference("FIRST_NAME")!.Summary = GetStringPref("FIRST_NAME", "Email Address");
		FindPreference("FIRST_NAME")!.PreferenceChange += delegate(object sender, Preference.PreferenceChangeEventArgs e)
		{
			RunOnUiThread(delegate
			{
				FindPreference("FIRST_NAME")!.Summary = e.NewValue!.ToString();
			});
		};
		FindPreference("SURNAME")!.Summary = GetStringPref("SURNAME", "Email Address");
		FindPreference("SURNAME")!.PreferenceChange += delegate(object sender, Preference.PreferenceChangeEventArgs e)
		{
			RunOnUiThread(delegate
			{
				FindPreference("SURNAME")!.Summary = e.NewValue!.ToString();
			});
		};
		FindPreference("DEFAULT_EMAIL")!.Summary = GetStringPref("DEFAULT_EMAIL", "Email Address");
		FindPreference("DEFAULT_EMAIL")!.PreferenceChange += delegate(object sender, Preference.PreferenceChangeEventArgs e)
		{
			RunOnUiThread(delegate
			{
				FindPreference("DEFAULT_EMAIL")!.Summary = e.NewValue!.ToString();
			});
		};
		FindPreference("POSTCODE")!.Summary = GetStringPref("POSTCODE", "Email Address");
		FindPreference("POSTCODE")!.PreferenceChange += delegate(object sender, Preference.PreferenceChangeEventArgs e)
		{
			RunOnUiThread(delegate
			{
				FindPreference("POSTCODE")!.Summary = e.NewValue!.ToString();
			});
		};
		((ListPreference)FindPreference("USUAL_START_STATION")).SetEntries(new string[4] { "Station One", "Station Two", "Station Three", "Station Four" });
		((ListPreference)FindPreference("USUAL_END_STATION")).SetEntries(new string[4] { "Station One", "Station Two", "Station Three", "Station Four" });
		((ListPreference)FindPreference("I_TRAVEL")).SetEntries(new string[5] { "Daily", "Mon-Fri", "Weekends", "Weekly", "Monthly" });
		((ListPreference)FindPreference("TRAVEL_USAGE")).SetEntries(new string[5] { "Student", "Work", "Shopping", "Leisure", "Other" });
	}

	private string GetStringPref(string key, string defaultVal)
	{
		return Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context)!.GetString(key, defaultVal);
	}
}
