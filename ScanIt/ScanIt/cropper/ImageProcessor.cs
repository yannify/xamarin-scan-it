
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

namespace com.bytewild.imaging.cropper
{
    public class ImageProcessor
    {
        private Bitmap imageBitmap;
        public ImageProcessor(Bitmap bitmap)
        {
            //dummy code to load the opencv libraries
            CvInvoke.CV_FOURCC('m', 'j', 'p', 'g');

            this.imageBitmap = bitmap;
        }

        public RectF GetCropRect()
        {
           using( var image = new Image<Bgr, Byte>(imageBitmap))
           {
               using(var edgeImage = GetImageAsEdges(image))
               {
                   var boxes = GetQuadrilaterals(edgeImage);
                   var box = GetLargestQuadrilateral(boxes);
                   var vertices = box.GetVertices();


                   return new RectF(GetCoordinate("left", vertices), GetCoordinate("top", vertices), GetCoordinate("right", vertices), GetCoordinate("bottom", vertices));
               }   
           }
        }

        public Image<Gray, Byte> GetImageAsEdges(Image<Bgr, Byte> image)
        {
            // Convert to GreyScale
            Image<Gray, Byte> imageGray = image.Convert<Gray, Byte>().PyrDown().PyrUp();

            //Canny Edge Detector
            //Image<Gray, Byte> cannyGray = imageGray.Canny(120, 180);
            Image<Gray, Byte> cannyGray = imageGray.Canny(10, 180);
            Image<Gray, Byte> imageDilate = cannyGray.Dilate(1);
            Image<Gray, Byte> imageErode = imageDilate.Erode(1);

            imageGray.Dispose();
            cannyGray.Dispose();
            imageDilate.Dispose();

            return imageErode;
        }


        private float GetCoordinate(string position, System.Drawing.PointF[] vertices)
        {
            float thePoint = float.MinValue;
            switch (position)
            {
                case "bottom":
                    {
                        // Find max y
                        foreach (var v in vertices)
                        {
                            if (v.Y > thePoint || thePoint == float.MinValue)
                                thePoint = v.Y;
                        }
                        break;
                    }
                case "top":
                    {
                        // Find min y
                        foreach (var v in vertices)
                        {
                            if (v.Y < thePoint || thePoint == float.MinValue)
                                thePoint = v.Y;
                        }
                        break;
                    }
                case "right":
                    {
                        // Find max x
                        foreach (var v in vertices)
                        {
                            if (v.X > thePoint || thePoint == float.MinValue)
                                thePoint = v.X;
                        }
                        break;
                    }
                case "left":
                    {
                        foreach (var v in vertices)
                        {
                            if (v.X < thePoint || thePoint == float.MinValue)
                                thePoint = v.X;
                        }
                        break;
                    }
            }

            return thePoint;
        }

        private List<MCvBox2D> GetQuadrilaterals(Image<Gray, Byte> image)
        {
            //List to store rectangles
            List<MCvBox2D> rectList = new List<MCvBox2D>();

            using (MemStorage storage1 = new MemStorage())
                for (Contour<System.Drawing.Point> contours1 = image.FindContours(); contours1 != null; contours1 = contours1.HNext)
                {
                    //Polygon Approximations
                    Contour<System.Drawing.Point> contoursAP = contours1.ApproxPoly(contours1.Perimeter * 0.05, storage1);
                    //Use area to wipe out the unnecessary result
                    if (contours1.Area >= 200)
                    {
                        //Use vertices to determine the shape
                        if (contoursAP.Total == 4)
                        {
                            //Rectangle
                            bool isRectangle = true;
                            System.Drawing.Point[] points = contoursAP.ToArray();
                            LineSegment2D[] edges = PointCollection.PolyLine(points, true);
                            //degree within the range of [75, 105] will be detected
                            for (int i = 0; i < edges.Length; i++)
                            {
                                double angle = Math.Abs(edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                                if (angle < 75 || angle > 105)
                                {
                                    isRectangle = false;
                                    break;
                                }
                            }
                            if (isRectangle)
                            {
                                rectList.Add(contoursAP.GetMinAreaRect());
                            }
                        }
                    }
                }

            return rectList;
        }

        private MCvBox2D GetLargestQuadrilateral(List<MCvBox2D> rectList)
        {
            double maxArea = 0;
            MCvBox2D largestRect = new MCvBox2D();
            foreach (MCvBox2D rect in rectList)
            {
                if (maxArea < (rect.size.Height * rect.size.Width))
                {
                    maxArea = rect.size.Height * rect.size.Width;
                    largestRect = rect;
                }
            }

            return largestRect;
        }
    }
}