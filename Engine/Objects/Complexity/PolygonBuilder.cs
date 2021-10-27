using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects.Complexity
{
    public static class PolygonBuilder
    {
        public static Polygon BuildFromRect(Rectangle rect, Vector2 colliderOrigin)
        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(rect.X, rect.Y);
            points[1] = new Vector2(rect.X + rect.Width, rect.Y);
            points[2] = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
            points[3] = new Vector2(rect.X, rect.Y + rect.Height);
            Vector2 MassCenter = new Vector2(rect.X + (rect.Width * colliderOrigin.X), rect.Y + (rect.Height * colliderOrigin.Y));
            Polygon p = new Polygon
            {
                Vertices = points,
                OriginPoint = MassCenter
            };
            return p;
        }
    }
}
