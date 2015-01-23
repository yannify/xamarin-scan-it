using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Provider;
using com.bytewild.imaging.cropper;

namespace com.bytewild.imaging
{
    [Activity(Label = "ScanIt", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, View.IOnClickListener
    {
        enum IntentActions { ImageSelector, CropPicture }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //retrieve a reference to the UI button
            Button captureBtn = FindViewById<Button>(Resource.Id.capture_btn);
            //handle button clicks
            captureBtn.SetOnClickListener(this);

            // hide the image view for now
            ImageView picView = FindViewById<ImageView>(Resource.Id.picture);
            picView.Visibility = ViewStates.Invisible;
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.capture_btn)
            {
                try
                {
                    Intent edgeDetectorIntent = new Intent(this, typeof(SelectImageActivity));
                    StartActivityForResult(edgeDetectorIntent, (int)IntentActions.ImageSelector);
                }
                catch (ActivityNotFoundException)
                {
                    //display an error message
                    String errorMessage = "Whoops - your device doesn't support capturing images!";
                    Toast toast = Toast.MakeText(this, errorMessage, ToastLength.Short);
                    toast.Show();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                if (requestCode == (int)IntentActions.ImageSelector)
                {
                     //get the returned data
                    Bundle extras = data.Extras;
                    //get the bitmap
                    Android.Net.Uri picUri = (Android.Net.Uri)extras.GetParcelable("image-uri");  // TODO:  prolly check for nulls

                    PerformCrop(picUri);
                }
                else if (requestCode == (int)IntentActions.CropPicture)
                {
                    //get the returned data
                    Bundle extras = data.Extras;
                    //get the cropped bitmap
                    Bitmap thePic = (Bitmap)extras.GetParcelable("data");  // TODO:  prolly check for nulls

                    //retrieve a reference to the ImageView
                    ImageView picView = (ImageView)FindViewById<ImageView>(Resource.Id.picture);
                    //display the returned cropped image
                    picView.SetImageBitmap(thePic);
                    picView.Visibility = ViewStates.Visible;
                }
            }
        }

        private void PerformCrop(Android.Net.Uri picUri)
        {
            try
            {
                Bundle extras = new Bundle();
                extras.PutString("image-path", picUri.Path);
                extras.PutBoolean("scale", true);
                extras.PutBoolean("return-data", true);

                // use our custom croppinglibrary
                Intent cropIntent = new Intent(this, typeof(CropImageActivity));
                cropIntent.PutExtras(extras);

                StartActivityForResult(cropIntent, (int)IntentActions.CropPicture);
            }
            catch (ActivityNotFoundException)
            {
                //display an error message
                String errorMessage = "Unable to find CropImageActivity";
                Toast toast = Toast.MakeText(this, errorMessage, ToastLength.Short);
                toast.Show();
            }
        }
    }
}