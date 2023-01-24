using System;
using Android.Content;
using Android.Database;
using Android.Net;
using Java.Lang;

namespace Merseyrail.Providers;

[ContentProvider(new string[] { "merseyrail.providers.DeparturesWidgetProvider" }, Name = "merseyrail.providers.DeparturesWidgetProvider")]
public class DeparturesWidgetContentProvider : ContentProvider
{
	public new static class InterfaceConsts
	{
		public static readonly string CrsCode = "CrsCode";

		public static readonly string TipLoc = "TipLoc";

		public static readonly string IsMerseyrail = "IsMerseyrail";

		public static readonly string Name = "Name";

		public static readonly string Lat = "Lat";

		public static readonly string Lon = "Lon";
	}

	public static readonly string AUTHORITY = "merseyrail.providers.DeparturesWidgetProvider";

	private static string BASE_PATH = "departures";

	public static readonly Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse("content://" + AUTHORITY + "/" + BASE_PATH);

	public static readonly string DEPARTURES_MIME_TYPE = "vnd.android.cursor.dir/vnd.glow.merseyrail.stations";

	public static readonly string DEPARTURE_MIME_TYPE = "vnd.android.cursor.item/vnd.glow.merseyrail.somethingelse";

	private MerseyrailDatabase merseyrailDatabase;

	private const int GET_ALL = 0;

	private const int GET_ONE = 1;

	private static UriMatcher uriMatcher = BuildUriMatcher();

	public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
	{
		throw new NotImplementedException();
	}

	public override string GetType(Android.Net.Uri uri)
	{
		throw new NotImplementedException();
	}

	public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
	{
		throw new NotImplementedException();
	}

	public override bool OnCreate()
	{
		return true;
	}

	public override ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
	{
		switch (uriMatcher.Match(uri))
		{
		case 0:
			return GetFromDatabase();
		case 1:
			_ = uri.LastPathSegment;
			return GetFromDatabase();
		default:
			throw new IllegalArgumentException("Unknown Uri: " + (object)uri);
		}
	}

	public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
	{
		throw new NotImplementedException();
	}

	public ICursor GetFromDatabase()
	{
		InitDbIfRequired();
		return merseyrailDatabase.ReadableDatabase!.RawQuery("SELECT * FROM Station WHERE IsMerseyrail = 1;", null);
	}

	public ICursor GetNearestStation()
	{
		InitDbIfRequired();
		return merseyrailDatabase.ReadableDatabase!.RawQuery("SELECT * FROM Station WHERE IsMerseyrail = 1;", null);
	}

	public ICursor GetNextNDeparturesNearestStation()
	{
		InitDbIfRequired();
		return merseyrailDatabase.ReadableDatabase!.RawQuery("SELECT * FROM Station WHERE IsMerseyrail = 1;", null);
	}

	public ICursor GetRainbowBoardByWidgetPrefs()
	{
		InitDbIfRequired();
		return merseyrailDatabase.ReadableDatabase!.RawQuery("SELECT * FROM Station WHERE IsMerseyrail = 1;", null);
	}

	private void InitDbIfRequired()
	{
		if (merseyrailDatabase == null)
		{
			merseyrailDatabase = new MerseyrailDatabase(base.Context);
		}
	}

	private static UriMatcher BuildUriMatcher()
	{
		UriMatcher obj = new UriMatcher(-1);
		obj.AddURI(AUTHORITY, BASE_PATH, 0);
		obj.AddURI(AUTHORITY, BASE_PATH + "/#", 1);
		return obj;
	}
}
