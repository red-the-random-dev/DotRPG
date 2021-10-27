using System;
using DotRPG.Algebra;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects.Complexity
{
    public class Polygon
    {
        public Vector2[] Vertices;
        public Vector2 OriginPoint;
        public Single Turn = 0.0f;

        public Vector2[] TurnedVertices
        {
            get
            {
                if (Turn == 0.0f)
                {
                    return Vertices;
                }
                Vector2[] newGeometry = new Vector2[Vertices.Length];

                for (int i = 0; i < newGeometry.Length; i++)
                {
                    Vector2 radiusLengthAngle = SharedVectorMethods.ToLengthAngle(Vertices[i] - OriginPoint);
                    radiusLengthAngle = new Vector2(radiusLengthAngle.X, radiusLengthAngle.Y + Turn);
                    Vector2 newLocation = SharedVectorMethods.FromLengthAngle(radiusLengthAngle);
                    newGeometry[i] = OriginPoint + newLocation;
                }

                return newGeometry;
            }
        }
        public Vector2 ActualCenter
        {
            get
            {
                Vector2 sigma = Vector2.Zero;
                foreach (Vector2 v in Vertices)
                {
                    sigma += v;
                }
                return sigma / Vertices.Length;
            }
        }
        public Vector2 Location
        {
            get
            {
                return OriginPoint;
            }
            set
            {
                Vector2 diff = value - OriginPoint;
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i] += diff;
                }
                OriginPoint = value;
            }
        }

        public LineFragment[] Edges
        {
            get
            {
                return FindEdges(TurnedVertices);
            }
        }

        public Vector2[] FindContactsWith(Polygon other)
        {
            return FindContactsOf(Edges, other.Edges);
        }
        public Vector2[] FindContactsWith(LineFragment[] edgeSet)
        {
            return FindContactsOf(Edges, edgeSet);
        }

        public static LineFragment[] FindEdges(Vector2[] vertices)
        {
            LineFragment[] edges = new LineFragment[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                edges[i] = new LineFragment(vertices[i], vertices[(i + 1) % vertices.Length]);
            }
            return edges;
        }

        public static Vector2[] FindContactsOf(LineFragment[] e1, LineFragment[] e2)
        {
            List<Vector2> contacts = new List<Vector2>();
            foreach (LineFragment l1 in e1)
            {
                foreach (LineFragment l2 in e2)
                {
                    if (l1.Intersects(l2, out Vector2 intPoint))
                    {
                        contacts.Add(intPoint);
                    }
                }
            }
            return contacts.ToArray();
        }
    }
}
