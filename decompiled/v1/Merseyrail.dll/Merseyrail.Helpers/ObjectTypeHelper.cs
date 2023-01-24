using System.Reflection;
using Java.Lang;

namespace Merseyrail.Helpers;

public static class ObjectTypeHelper
{
	public static T Cast<T>(this Object obj) where T : class
	{
		PropertyInfo property = obj.GetType().GetProperty("Instance");
		if (!(property == null))
		{
			return property.GetValue(obj, null) as T;
		}
		return null;
	}
}
