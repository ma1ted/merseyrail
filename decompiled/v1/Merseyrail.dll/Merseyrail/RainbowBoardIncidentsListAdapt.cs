using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common.Domain;
using Java.Interop;
using Java.Lang;

namespace Merseyrail;

public class RainbowBoardIncidentsListAdapter : BaseAdapter<RouteIncident>, ISectionIndexer, IJavaObject, IDisposable, IJavaPeerable
{
	private List<RouteIncident> items;

	private Activity context;

	private Dictionary<string, int> alphaIndex { get; set; }

	private string[] sections { get; set; }

	private Java.Lang.Object[] sectionsObjects { get; set; }

	public override RouteIncident this[int position] => items[position];

	public override int Count => items.Count;

	public RainbowBoardIncidentsListAdapter(Activity context, List<RouteIncident> items)
	{
		this.context = context;
		this.items = items;
		alphaIndex = new Dictionary<string, int>();
		for (int i = 0; i < items.Count; i++)
		{
			string summary = items[i].Summary;
			if (!alphaIndex.ContainsKey(summary))
			{
				alphaIndex.Add(summary, i);
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
		RouteIncident routeIncident = items[position];
		View view = convertView;
		if (view == null)
		{
			view = context.LayoutInflater.Inflate(2131361824, null);
		}
		view.FindViewById<TextView>(2131230722).Text = routeIncident.Summary;
		view.FindViewById<TextView>(2131230721).Text = routeIncident.Description;
		return view;
	}

	public int GetPositionForSection(int section)
	{
		return alphaIndex[sections[section]];
	}

	public int GetSectionForPosition(int position)
	{
		int num = 0;
		for (int i = 0; i < sections.Length; i++)
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
