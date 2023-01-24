using System;
using Android.Content;

namespace Merseyrail.Notifications;

public interface INotificationHandler
{
	Type[] SupportedTypes { get; }

	void Process(Context context, object stanza);
}
