using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace com.bytewild.imaging
{
    public static class ImageUtils
    {
        public static Java.IO.File CreateDirectoryForPictures()
        {
            var dir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "ScanItPics");
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            return dir;
        }

        public static void SaveBitmap(Bitmap image, Android.Net.Uri saveUri, Bitmap.CompressFormat outputFormat, Context context)
        {
            // TODO: Clean this up some
            if (saveUri != null)
            {
                try
                {
                    using (var outputStream = context.ContentResolver.OpenOutputStream(saveUri))
                    {
                        if (outputStream != null)
                        {
                            image.Compress(outputFormat, 75, outputStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // display an error message

                }
            }
            else
            {
                // throw exectoin
            }
            //image.Recycle();
        }

        public static Android.Net.Uri GetImageUri(String path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }

        public static Android.Net.Uri GetImageUri(Java.IO.File file)
        {
            return Android.Net.Uri.FromFile(file);
        }
    }
}