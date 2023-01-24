using Android.Content.Res;

namespace Merseyrail.Helpers;

public static class UnitHelpers
{
	public static float convertDpToPixel(float dp)
	{
		return dp * Resources.System!.DisplayMetrics!.Density;
	}

	public static float convertPixelsToDp(float px)
	{
		return px / Resources.System!.DisplayMetrics!.Density;
	}
}
