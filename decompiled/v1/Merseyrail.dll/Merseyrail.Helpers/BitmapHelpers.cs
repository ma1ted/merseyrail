using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;

namespace Merseyrail.Helpers;

public static class BitmapHelpers
{
	public static void RecycleBitmap(this ImageView imageView)
	{
		if (imageView != null)
		{
			Drawable drawable = imageView.Drawable;
			if (drawable != null)
			{
				((BitmapDrawable)drawable).Bitmap!.Recycle();
			}
		}
	}

	public static Bitmap LoadBitmap(this string fileName)
	{
		return BitmapFactory.DecodeFile(fileName);
	}

	public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
	{
		BitmapFactory.Options options = new BitmapFactory.Options
		{
			InJustDecodeBounds = true
		};
		BitmapFactory.DecodeFile(fileName, options);
		int outHeight = options.OutHeight;
		int outWidth = options.OutWidth;
		int inSampleSize = 1;
		if (outHeight > height || outWidth > width)
		{
			inSampleSize = ((outWidth > outHeight) ? (outHeight / height) : (outWidth / width));
		}
		options.InSampleSize = inSampleSize;
		options.InJustDecodeBounds = false;
		return BitmapFactory.DecodeFile(fileName, options);
	}

	public static string EncodeTobase64(Bitmap image)
	{
		if (image != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Compress(Bitmap.CompressFormat.Jpeg, 40, memoryStream);
			return Base64.EncodeToString(memoryStream.ToArray(), Base64Flags.Default);
		}
		return string.Empty;
	}

	public static byte[] ReadFully(Stream input)
	{
		using MemoryStream memoryStream = new MemoryStream();
		input.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static Bitmap DecodeBase64(string input)
	{
		byte[] array = Base64.Decode(input, Base64Flags.Default);
		return BitmapFactory.DecodeByteArray(array, 0, array.Length);
	}
}
