using Android.App;
using Android.Content;
using Android.Widget;

namespace Merseyrail.AppWidgets;

[Service(Exported = false, Permission = "android.permission.BIND_REMOTEVIEWS")]
public class DeparturesWidgetService : RemoteViewsService
{
	public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
	{
		return new StackRemoteViewsFactory(ApplicationContext, intent);
	}
}
