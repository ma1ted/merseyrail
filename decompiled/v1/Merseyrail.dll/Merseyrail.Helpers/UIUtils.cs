using Android.Views;
using Android.Widget;

namespace Merseyrail.Helpers;

public class UIUtils
{
	public static bool SetListViewHeightBasedOnItems(ListView listView)
	{
		IListAdapter adapter = listView.Adapter;
		if (adapter != null)
		{
			int count = adapter.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				View view = adapter.GetView(i, null, listView);
				view.Measure(0, 0);
				num += view.MeasuredHeight;
			}
			int num2 = listView.DividerHeight * (count - 1);
			ViewGroup.LayoutParams layoutParameters = listView.LayoutParameters;
			layoutParameters.Height = num + num2 * 3;
			listView.LayoutParameters = layoutParameters;
			listView.RequestLayout();
			return true;
		}
		return false;
	}
}
