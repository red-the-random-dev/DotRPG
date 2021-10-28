using System;
using DotRPG.Algebra;
using DotRPG.Objects.Complexity;
using Microsoft.Xna.Framework;

namespace _PolygonTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector2[] v1 = PolygonBuilder.BuildFromRect(new Rectangle(0, 0, 4, 4), new Vector2(0.5f, 0.5f)).TurnedVertices;
            Vector2[] v2 = PolygonBuilder.BuildFromRect(new Rectangle(4, 4, 4, 4), new Vector2(0.5f, 0.5f)).TurnedVertices;

            LineFragment[] lf1 = Polygon.FindEdges(v1);
            LineFragment[] lf2 = Polygon.FindEdges(v2);

            Vector2[] contacts = Polygon.FindContactsOf(lf1, lf2);

            foreach (Vector2 p in contacts)
            {
                Console.WriteLine(p);
            }

            LineFragment lf = new LineFragment(new Vector2(0, 0), new Vector2(3, 4));
            Vector2 proj = lf.FullLine.GetPointProjection(new Vector2(3, 0));
            Console.WriteLine(proj);
            Console.WriteLine(lf.Intersects(proj));
        }
    }
}
