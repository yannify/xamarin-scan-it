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

namespace com.bytewild.imaging.cropper
{
    public class CropHandle
    {
        public int Postion { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Rect Handle { get; set; }

        public CropHandle(int position, int x, int y, Rect handle)
        {
            Postion = position;
            X = x;
            Y = y;
            Handle = handle;
        }
    }
}