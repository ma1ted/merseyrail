using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Merseyrail.Shared;

namespace Merseyrail.Fragments;

public class Settings : Android.Support.V4.App.Fragment
{
	private bool actionsBound;

	private View _view { get; set; }

	public EditText Name { get; set; }

	public EditText Email { get; set; }

	public Button HomeStationButton { get; set; }

	public string HomeStation { get; set; }

	public Button WorkStationButton { get; set; }

	public string WorkStation { get; set; }

	public ToggleButton Southport { get; set; }

	public ToggleButton Ormskirk { get; set; }

	public ToggleButton Kirkby { get; set; }

	public ToggleButton HuntsCross { get; set; }

	public ToggleButton WestKirby { get; set; }

	public ToggleButton NewBrighton { get; set; }

	public ToggleButton EllesmerePort { get; set; }

	public ToggleButton Chester { get; set; }

	public Button UpdateSettings { get; set; }

	public ImageButton BackButton { get; set; }

	private bool ActionsSet { get; set; }

	public Settings()
	{
		actionsBound = false;
	}

	public override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	public override View OnCreateView(LayoutInflater inflater, ViewGroup viewGroup, Bundle savedInstanceState)
	{
		if (_view == null)
		{
			_view = inflater.Inflate(2131361870, null, attachToRoot: false);
		}
		Name = (EditText)_view.FindViewById(2131231140);
		Email = (EditText)_view.FindViewById(2131231139);
		BackButton = (ImageButton)_view.FindViewById(2131231138);
		UpdateSettings = (Button)_view.FindViewById(2131231137);
		if (!actionsBound)
		{
			UpdateSettings.Click += delegate
			{
				SaveSettings();
				SetSettings();
			};
			actionsBound = true;
		}
		SetActions();
		SetSettings();
		return _view;
	}

	private void SetActions()
	{
		if (!ActionsSet)
		{
			BackButton.Click += delegate
			{
				base.FragmentManager.PopBackStackImmediate();
			};
			ActionsSet = true;
		}
	}

	public void SetSettings()
	{
		base.Activity.RunOnUiThread(delegate
		{
			Name.Text = SharedSettings.SettingsService.Prefs.GetString("DEFAULT_NAME", string.Empty);
			Email.Text = SharedSettings.SettingsService.Prefs.GetString("DEFAULT_EMAIL", string.Empty);
			Southport.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_SOUTHPORT", defValue: false);
			Ormskirk.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_ORMSKIRK", defValue: false);
			Kirkby.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_KIRKBY", defValue: false);
			HuntsCross.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_HUNTSCROSS", defValue: false);
			WestKirby.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_WESTKIRBY", defValue: false);
			NewBrighton.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_NEWBRIGHTON", defValue: false);
			EllesmerePort.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_ELLESMEREPORT", defValue: false);
			Chester.Checked = SharedSettings.SettingsService.Prefs.GetBoolean("ALERT_CHESTER", defValue: false);
		});
		Toast.MakeText(Application.Context, "Settings have been saved", ToastLength.Short);
	}

	public void SaveSettings()
	{
		SharedSettings.SettingsService.Editor.PutString("DEFAULT_NAME", Name.Text);
		SharedSettings.SettingsService.Editor.PutString("DEFAULT_EMAIL", Email.Text);
		SharedSettings.SettingsService.Editor.PutString("HOME_STATION", HomeStation);
		SharedSettings.SettingsService.Editor.PutString("WORK_STATION", WorkStation);
		SharedSettings.SettingsService.Editor.Apply();
		SharedSettings.SettingsService.Editor.Commit();
		Toast.MakeText(base.Activity, "Your settings have been updated", ToastLength.Short);
	}

	private void SaveRemoteSettings(string settingName, ToggleButton tb, string crs)
	{
		_ = SharedSettings.IsRegistered;
	}

	public override void OnActivityCreated(Bundle p0)
	{
		base.OnActivityCreated(p0);
	}

	public override void OnDestroyView()
	{
		base.OnDestroyView();
		((ViewGroup)_view.Parent)?.RemoveView(_view);
	}
}
