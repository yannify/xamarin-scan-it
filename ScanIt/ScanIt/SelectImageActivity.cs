using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Preferences;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace com.bytewild.imaging
{
    [Activity(Label = "Edge Detection")]
    public class SelectImageActivity: SelectImageActivityBase
    {
        public SelectImageActivity() : base("Get Image")  { }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            OnButtonClick += delegate
            {
                var dir = ImageUtils.CreateDirectoryForPictures();
                var fileName = string.Format("myScanItPhoto_{0}.jpg", Guid.NewGuid());
                var imageUri = ImageUtils.GetImageUri(new Java.IO.File(dir, fileName));

                using( Image<Bgr, Byte> image = PickImage(fileName))
                {
                    ImageUtils.SaveBitmap(image.ToBitmap(), imageUri, Bitmap.CompressFormat.Jpeg, this);
                }
  
                Bundle extras = new Bundle();
                extras.PutParcelable("image-uri", imageUri);

                SetResult(Result.Ok, (new Intent()).SetAction("inline-data").PutExtras(extras));
                Finish();
            };
        }
    }
}