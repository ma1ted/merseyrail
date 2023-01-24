using System;
using Android.Content;
using Android.Database.Sqlite;
using Common;

namespace Merseyrail.Providers;

public class MerseyrailDatabase : SQLiteOpenHelper
{
	public new static readonly string DatabaseName = Utils.AppFolderRelativeToFullpath("/data/data/glow.merseyrail/files/stations.db");

	public static readonly int DatabaseVersion = 1;

	public MerseyrailDatabase(Context context)
		: base(context, DatabaseName, null, DatabaseVersion)
	{
		_ = DatabaseName;
		_ = DatabaseVersion;
	}

	public override void OnOpen(SQLiteDatabase db)
	{
		base.OnOpen(db);
	}

	public void Test()
	{
		_ = WritableDatabase;
	}

	public override void OnCreate(SQLiteDatabase db)
	{
	}

	public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
	{
		throw new NotImplementedException();
	}
}
