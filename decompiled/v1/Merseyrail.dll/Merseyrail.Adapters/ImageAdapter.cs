using System.Collections.Generic;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace Merseyrail.Adapters;

public class ImageAdapter : BaseAdapter<string>
{
	private List<string> _filePaths;

	private Activity _context;

	public override int Count => _filePaths.Count;

	public override string this[int position] => _filePaths[position];

	public ImageAdapter(List<string> filePaths, Activity context)
	{
		_filePaths = filePaths;
		_context = context;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		View obj = convertView ?? _context.LayoutInflater.Inflate(2131361908, null);
		string text = _filePaths[position];
		ImageView p = obj.FindViewById<ImageView>(2131230904);
		DisplayMetrics displayMetrics = new DisplayMetrics();
		_context.WindowManager!.DefaultDisplay!.GetMetrics(displayMetrics);
		Picasso.With(_context).Load(text + "?width=" + displayMetrics.WidthPixels).Placeholder(2131165324)
			.Into(p);
		return obj;
	}
}
