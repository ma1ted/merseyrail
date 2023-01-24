using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;

namespace Merseyrail.Adapters.LayoutManagers;

public class NestedLayoutManager : LinearLayoutManager
{
	public NestedLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
	}

	public NestedLayoutManager(Context context)
		: base(context)
	{
	}

	public NestedLayoutManager(Context context, int orientation, bool reverseLayout)
		: base(context, orientation, reverseLayout)
	{
	}

	public override bool CanScrollVertically()
	{
		return false;
	}
}
