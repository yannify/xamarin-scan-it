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
using Emgu.CV;

namespace com.bytewild.imaging.cropper
{
    public class CropPolygon: IConvexPolygonF
    {
        public System.Drawing.PointF[] Vertices {get; set;}

        public CropPolygon() 
        {
            Vertices = new System.Drawing.PointF[0];
        }
        public CropPolygon(System.Drawing.PointF[] vertices)
        {
            Vertices = vertices;
        }

        public CropPolygon(System.Drawing.Point[] vertices)
        {
            List<System.Drawing.PointF> pf = new List<System.Drawing.PointF>();
            foreach(var p in vertices)
                pf.Add(new System.Drawing.PointF(p.X, p.Y));


            Vertices = pf.ToArray();
        }

        public System.Drawing.PointF[] GetVertices()
        {
            return Vertices;
        }

        public double Area()
        {
            // TODO:  Tis is not right but should be good enough to figure who si the biggist polygon for the prototype
            var verts = this.GetOrderedVertices().ToArray();

            int num_points = verts.Length;
            System.Drawing.PointF[] pts = new System.Drawing.PointF[num_points + 1];

            verts.CopyTo(pts, 0);

            pts[num_points] = verts[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return -(area); // reverse the sign, the value is negative if this is a clockwise polygon
        }

        public List<System.Drawing.PointF> GetOrderedVertices()
        {
            var points = ConvexHull.CH2(new List<System.Drawing.PointF>(this.Vertices));
            
            if (points == null)
                points = new List<System.Drawing.PointF>();
            return points;
        }
    }
}