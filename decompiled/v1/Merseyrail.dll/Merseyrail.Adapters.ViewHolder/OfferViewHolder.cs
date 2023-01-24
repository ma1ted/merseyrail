using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Merseyrail.Adapters.ViewHolders;

public class OfferViewHolder : RecyclerView.ViewHolder
{
	public ImageView Image { get; set; }

	public ImageView RedeemedImage { get; set; }

	public TextView Title { get; set; }

	public TextView ExpiryDateTextView { get; set; }

	public TextView Description { get; set; }

	public Button ViewButton { get; set; }

	public OfferViewHolder(View itemView)
		: base(itemView)
	{
		Image = itemView.FindViewById<ImageView>(2131230905);
		Title = itemView.FindViewById<TextView>(2131231254);
		Description = itemView.FindViewById<TextView>(2131231251);
		ExpiryDateTextView = itemView.FindViewById<TextView>(2131231252);
		ViewButton = itemView.FindViewById<Button>(2131230782);
		RedeemedImage = itemView.FindViewById<ImageView>(2131230906);
	}
}
