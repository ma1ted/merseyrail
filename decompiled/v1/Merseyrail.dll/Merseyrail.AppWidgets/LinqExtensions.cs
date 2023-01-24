using System.Collections.Generic;
using System.Linq;

namespace Merseyrail.AppWidgets;

public static class LinqExtensions
{
	public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
	{
		T[] array = null;
		int num = 0;
		foreach (T item in source)
		{
			if (array == null)
			{
				array = new T[size];
			}
			array[num++] = item;
			if (num == size)
			{
				yield return array.Select((T x) => x);
				array = null;
				num = 0;
			}
		}
		if (array != null && num > 0)
		{
			yield return array.Take(num);
		}
	}
}
