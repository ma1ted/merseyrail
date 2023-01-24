using Android.Views;

namespace Merseyrail;

public class HelpDisplayItem
{
	public View TargetView { get; set; }

	public string Message { get; set; }

	public string IdTag { get; set; }

	public HelpDisplay.HelpItemLayoutHint LayoutHint { get; set; }
}
