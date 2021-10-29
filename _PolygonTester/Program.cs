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
            Vector2[] v1 = PolygonBuilder.BuildFromRect(new Rectangle(0, 0, 64, 64), new Vector2(0.5f, 0.5f)).TurnedVertices;
            Vector2[] v2 = PolygonBuilder.BuildFromRect(new Rectangle(64, 0, 4, 4), new Vector2(0.5f, 0.5f)).TurnedVertices;
            foreach (Vector2 v in v1)
            {
                Console.Write(v.ToString() + " ");
            }
            Console.Write("\n");
            foreach (Vector2 v in v2)
            {
                Console.Write(v.ToString() + " ");
            }
            Console.Write("\n");
            LineFragment[] lf1 = Polygon.FindEdges(v1);
            LineFragment[] lf2 = Polygon.FindEdges(v2);

            Vector2[] contacts = Polygon.FindContactsOf(lf1, lf2, v1, v2);

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
