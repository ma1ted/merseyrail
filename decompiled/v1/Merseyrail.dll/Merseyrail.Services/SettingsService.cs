using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Common.Services;

namespace Merseyrail.Services;

public class SettingsService : ISettingsService
{
	public ISharedPreferences Prefs { get; set; }

	public ISharedPreferencesEditor Editor { get; set; }

	public object this[string key]
	{
		get
		{
			return GetSetting<object>(key, null);
		}
		set
		{
			SaveSetting(key, value);
		}
	}

	public void Initialise(Context context)
	{
		Prefs = PreferenceManager.GetDefaultSharedPreferences(context);
		Editor = Prefs.Edit();
	}

	public T GetSetting<T>(string key, T defaultValue)
	{
		if (Prefs == null)
		{
			Initialise(Application.Context);
		}
		try
		{
			if (Prefs.All.Any<KeyValuePair<string, object>>((KeyValuePair<string, object> x) => x.Key == key))
			{
				if (Prefs.All!.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value.ToString()))
				{
					return (T)Convert.ChangeType(value, typeof(T));
				}
				return defaultValue;
			}
			return defaultValue;
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	public void SaveSetting(string key, object value)
	{
		switch (Type.GetTypeCode(value.GetType()))
		{
		case TypeCode.String:
			Editor.PutString(key, value.ToString());
			break;
		case TypeCode.Int32:
			Editor.PutInt(key, (int)value);
			break;
		case TypeCode.Single:
			Editor.PutFloat(key, (float)value);
			break;
		case TypeCode.Int64:
			Editor.PutLong(key, (long)value);
			break;
		case TypeCode.Boolean:
			Editor.PutBoolean(key, (bool)value);
			break;
		case TypeCode.Double:
			Editor.PutFloat(key, (float)value);
			break;
		default:
			Log.Debug("SaveSetting", "The switch did not find the type to save and hit default.");
			break;
		}
		Editor.Commit();
	}
}
