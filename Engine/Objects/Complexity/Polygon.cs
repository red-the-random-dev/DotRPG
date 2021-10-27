using System;
using DotRPG.Algebra;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects.Complexity
{
    public class Polygon
    {
        public Point[] Perimeter;
        public Point MassCenter;
        public Single Turn = 0.0f;

        public Point[] TurnedPerimeter
        {
            get
            {
                Point[] newGeometry = new Point[Perimeter.Length];

                for (int i = 0; i < newGeometry.Length; i++)
                {
                    Vector2 radiusLengthAngle = SharedVectorMethods.ToLengthAngle(Perimeter[i].ToVector2() - MassCenter.ToVector2());
                    radiusLengthAngle = new Vector2(radiusLengthAngle.X, radiusLengthAngle.Y + Turn);
                    Vector2 newLocation = SharedVectorMethods.FromLengthAngle(radiusLengthAngle);
                    newGeometry[i] = (MassCenter.ToVector2() + newLocation).ToPoint();
                }

                return newGeometry;
            }
        }
    }
}
