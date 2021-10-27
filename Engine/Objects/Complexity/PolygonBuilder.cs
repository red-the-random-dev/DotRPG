using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects.Complexity
{
    public static class PolygonBuilder
    {
        public static Polygon ToPolygon(Rectangle rect, Vector2 colliderOrigin)
        {
            Point[] points = new Point[4];
            points[0] = new Point(rect.X, rect.Y);
            points[1] = new Point(rect.X + rect.Width, rect.Y);
            points[2] = new Point(rect.X + rect.Width, rect.Y + rect.Height);
            points[3] = new Point(rect.X, rect.Y + rect.Height);
            Vector2 MassCenter = new Vector2(rect.X + (rect.Width * colliderOrigin.X), rect.Y + (rect.Height * colliderOrigin.Y));
            Polygon p = new Polygon();
            p.Perimeter = points;
            p.MassCenter = MassCenter.ToPoint();
            return p;
        }
    }
}
