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
        public Vector2 MassCenter;
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
                    Vector2 radiusLengthAngle = SharedVectorMethods.ToLengthAngle(Vertices[i] - MassCenter);
                    radiusLengthAngle = new Vector2(radiusLengthAngle.X, radiusLengthAngle.Y + Turn);
                    Vector2 newLocation = SharedVectorMethods.FromLengthAngle(radiusLengthAngle);
                    newGeometry[i] = MassCenter + newLocation;
                }

                return newGeometry;
            }
        }

        public LineFragment[] Edges
        {
            get
            {
                Vector2[] vertices = TurnedVertices;
                LineFragment[] edges = new LineFragment[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    edges[i] = new LineFragment(vertices[i], vertices[(i + 1) % vertices.Length]);
                }
                return edges;
            }
        }

        public Vector2[] Contacts(Polygon other)
        {
            LineFragment[] e1 = Edges;
            LineFragment[] e2 = other.Edges;
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
