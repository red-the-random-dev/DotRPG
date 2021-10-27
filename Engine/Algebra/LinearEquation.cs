using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Algebra
{
    public class LinearEquation
    {
        public readonly Single Tangent;
        public readonly Single Offset;
        public readonly Single HorizontalOffset = 0.0f;
        public readonly Boolean IsVertical = false;

        public Single this[Single x]
        {
            get
            {
                // y = kx + b
                return Tangent * x + Offset;
            }
        }

        public LinearEquation(Vector2 P1, Vector2 P2)
        {
            if (P1.X == P2.X)
            {
                IsVertical = true;
                Tangent = Single.PositiveInfinity;
                HorizontalOffset = P1.X;
                Offset = Single.PositiveInfinity;
            }
            else
            {
                Tangent = (P2.Y - P1.Y) / (P2.X - P1.X);
                Offset = P1.Y - (P1.X * Tangent);
                IsVertical = false;
                HorizontalOffset = 0.0f;
            }
        }

        public Boolean Intersects(Point p)
        {
            if (IsVertical)
            {
                return p.X == HorizontalOffset;
            }
            return this[p.X] == p.Y;
        }

        public Boolean Intersects(LinearEquation other, out Vector2 intPoint)
        {
            intPoint = Vector2.Zero;
            if (this.Tangent == other.Tangent || this.IsVertical && other.IsVertical)
            {
                return false;
            }
            else if (IsVertical && !other.IsVertical)
            {
                intPoint = new Vector2(HorizontalOffset, other[HorizontalOffset]);
                return true;
            }
            else if (!IsVertical && other.IsVertical)
            {
                intPoint = new Vector2(other.HorizontalOffset, this[HorizontalOffset]);
                return true;
            }
            else
            {
                Single x = (other.Offset - Offset) / (Tangent - other.Tangent);
                intPoint = new Vector2(x, this[x]);
                return true;
            }
        }
    }
}
