using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Common.Domain;

namespace Merseyrail.Adapters;

public class FaqAdapter : BaseAdapter<Faq>
{
	private List<Faq> _faqs;

	private Activity _context;

	public override int Count => _faqs.Count;

	public override Faq this[int position] => _faqs[position];

	public FaqAdapter(List<Faq> faqs, Activity context)
	{
		_faqs = faqs;
		_context = context;
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		View obj = convertView ?? _context.LayoutInflater.Inflate(2131361907, null);
		Faq faq = _faqs[position];
		obj.FindViewById<TextView>(2131231255).Text = faq.Question;
		return obj;
	}
}
