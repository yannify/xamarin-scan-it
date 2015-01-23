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
using System.Drawing;

namespace com.bytewild.imaging.cropper
{
    public class ConvexHull
    {
        public static List<PointF> CH2(List<PointF> points)
        {
            return CH2(points, false);
        }

        public static List<PointF> CH2(List<PointF> points, bool removeFirst)
        {
            List<PointF> vertices = new List<PointF>();

            if (points.Count == 0)
                return null;
            else if (points.Count == 1)
            {
                // If it's a single point, return it
                vertices.Add(points[0]);
                return vertices;
            }


            PointF leftMost = CH2Init(points);
            vertices.Add(leftMost);

            PointF prev = leftMost;
            PointF? next;
            double rot = 0;
            do
            {
                next = CH2Step(prev, points, ref rot);

                // If it's not the first vertex (leftmost) or we want spiral (instead of CH2)
                // remove it
                if (prev != leftMost || removeFirst)
                    points.Remove(prev);

                // If this isn't the last vertex, save it
                if (next.HasValue)
                {
                    vertices.Add(next.Value);
                    prev = next.Value;
                }

            } while (points.Count > 0 && next.HasValue && next.Value != leftMost);
            points.Remove(leftMost);

            return vertices;

        }

        private static PointF CH2Init(List<PointF> points)
        {
            // Initialization - Find the leftmost point
            PointF leftMost = points[0];
            float leftX = leftMost.X;

            foreach (PointF p in points)
            {
                if (p.X < leftX)
                {
                    leftMost = p;
                    leftX = p.X;
                }
            }
            return leftMost;
        }

        private static PointF? CH2Step(PointF currentPoint, List<PointF> points, ref double rot)
        {
            double angle, angleRel, smallestAngle = 2 * Math.PI, smallestAngleRel = 4 * Math.PI;
            PointF? chosen = null;
            float xDiff, yDiff;

            foreach (PointF candidate in points)
            {
                if (candidate == currentPoint)
                    continue;

                xDiff = candidate.X - currentPoint.X;
                yDiff = -(candidate.Y - currentPoint.Y); //Y-axis starts on top
                angle = ComputeAngle(new PointF(xDiff, yDiff));

                // angleRel is the angle between the line and the rotated y-axis
                // y-axis has the direction of the last computed supporting line
                // given by variable rot.
                angleRel = 2 * Math.PI - (rot - angle);

                if (angleRel >= 2 * Math.PI)
                    angleRel -= 2 * Math.PI;
                if (angleRel < smallestAngleRel)
                {
                    smallestAngleRel = angleRel;
                    smallestAngle = angle;
                    chosen = candidate;
                }

            }

            // Save the smallest angle as the rotation of the y-axis for the
            // computation of the next supporting line.
            rot = smallestAngle;

            return chosen;
        }


        private static double ComputeAngle(PointF p)
        {
            if (p.X > 0 && p.Y > 0)
                return Math.Atan((double)p.X / p.Y);
            else if (p.X > 0 && p.Y == 0)
                return (Math.PI / 2);
            else if (p.X > 0 && p.Y < 0)
                return (Math.PI + Math.Atan((double)p.X / p.Y));
            else if (p.X == 0 && p.Y >= 0)
                return 0;
            else if (p.X == 0 && p.Y < 0)
                return Math.PI;
            else if (p.X < 0 && p.Y > 0)
                return (2 * Math.PI + Math.Atan((double)p.X / p.Y));
            else if (p.X < 0 && p.Y == 0)
                return (3 * Math.PI / 2);
            else if (p.X < 0 && p.Y < 0)
                return (Math.PI + Math.Atan((double)p.X / p.Y));
            else
                return 0;
        }
    }
}