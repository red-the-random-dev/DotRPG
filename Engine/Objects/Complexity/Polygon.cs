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
                Vector2[] newGeometry = new Vector2[Vertices.Length];

                for (int i = 0; i < newGeometry.Length; i++)
                {
                    Vector2 radiusLengthAngle = SharedVectorMethods.ToLengthAngle(Vertices[i] - OriginPoint);
                    radiusLengthAngle = new Vector2(radiusLengthAngle.X, radiusLengthAngle.Y + Turn);
                    Vector2 newLocation = SharedVectorMethods.FromLengthAngle(radiusLengthAngle);
                    newGeometry[i] = (OriginPoint + newLocation);
                    newGeometry[i] = new Vector2((int)Math.Round(newGeometry[i].X), (int)Math.Round(newGeometry[i].Y));
                }

                return newGeometry;
            }
        }
        public Vector2 ActualCenter
        {
            get
            {
                return FindActualCenter(TurnedVertices);
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
            Vector2[] thisVertices = TurnedVertices;
            Vector2[] otherVertices = other.TurnedVertices;
            return FindContactsOf(Edges, other.Edges, thisVertices, otherVertices);
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

        public static Vector2[] FindContactsOf(LineFragment[] e1, LineFragment[] e2, Vector2[] v1 = null, Vector2[] v2 = null)
        {
            HashSet<Vector2> contacts = new HashSet<Vector2>();
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
            if (v1 != null && v2 != null)
            {
                foreach (Vector2 p1 in v1)
                {
                    foreach (Vector2 p2 in v2)
                    {
                        if (p1 == p2)
                        {
                            contacts.Add(p1);
                        }
                    }
                }
                foreach (Vector2 p in v1)
                {
                    foreach (LineFragment lf in e2)
                    {
                        if (lf.Intersects(p))
                        {
                            contacts.Add(p);
                        }
                    }
                }
                foreach (Vector2 p in v2)
                {
                    foreach (LineFragment lf in e1)
                    {
                        if (lf.Intersects(p))
                        {
                            contacts.Add(p);
                        }
                    }
                }
            }
            Vector2[] cArr = new Vector2[contacts.Count];
            Int32 c = 0;
            foreach (Vector2 v in contacts)
            {
                cArr[c++] = v;
            }
            return cArr;
        }
        public static Vector2 FindActualCenter(Vector2[] Vertices)
        {
            Vector2 sigma = Vector2.Zero;
            foreach (Vector2 v in Vertices)
            {
                sigma += v;
            }
            return sigma / Vertices.Length;
        }
        public static Single FindMedianRadius(Vector2[] Vertices)
        {
            Vector2 v = FindActualCenter(Vertices);
            Single sigma = 0.0f;
            foreach (Vector2 x in Vertices)
            {
                sigma += (x - v).Length();
            }
            return sigma / Vertices.Length;
        }
        public Boolean Overlaps(Polygon other)
        {
            Vector2[] v1 = TurnedVertices;
            Vector2[] v2 = other.TurnedVertices;
            return (FindActualCenter(v2) - FindActualCenter(v1)).Length() > (FindMedianRadius(v1) + FindMedianRadius(v2));
        }
        public static Boolean Overlaps(Vector2[] v1, Vector2[] v2)
        {
            return (FindActualCenter(v2) - FindActualCenter(v1)).Length() > (FindMedianRadius(v1) + FindMedianRadius(v2));
        }
    }
}
