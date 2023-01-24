using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;

namespace Merseyrail;

public class StationListAdapter : BaseAdapter<string>, ISectionIndexer, IJavaObject, IDisposable, IJavaPeerable
{
	private List<string> items;

	private Activity context;

	private Dictionary<string, int> alphaIndex { get; set; }

	private string[] sections { get; set; }

	private Java.Lang.Object[] sectionsObjects { get; set; }

	public override string this[int position] => items[position];

	public override int Count => items.Count;

	public StationListAdapter(Activity context, List<string> items)
	{
		this.context = context;
		this.items = items;
		alphaIndex = new Dictionary<string, int>();
		for (int i = 0; i < items.Count; i++)
		{
			string key = items[i].Substring(0, 1);
			if (!alphaIndex.ContainsKey(key))
			{
				alphaIndex.Add(key, i);
			}
		}
		sections = alphaIndex.Keys.ToArray();
		sectionsObjects = new Java.Lang.Object[sections.Length];
		for (int j = 0; j < sections.Length; j++)
		{
			sectionsObjects[j] = new Java.Lang.String(sections[j]);
		}
	}

	public override long GetItemId(int position)
	{
		return position;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		string text = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361829, null);
		}
		view.FindViewById<TextView>(2131230723).Text = text;
		return view;
	}

	public int GetPositionForSection(int section)
	{
		return alphaIndex[sections[section]];
	}

	public int GetSectionForPosition(int position)
	{
		int num = 0;
		for (int i = 0; i < sections.Length - 1; i++)
		{
			if (GetPositionForSection(i) > position && num <= position)
			{
				num = i;
				break;
			}
		}
		return num;
	}

	public Java.Lang.Object[] GetSections()
	{
		return sectionsObjects;
	}
}
