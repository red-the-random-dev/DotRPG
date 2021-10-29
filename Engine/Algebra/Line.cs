using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Algebra
{
    public class Line : IEquatable<Line>
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
                return (Single)Math.Round(Tangent * x + Offset);
            }
        }

        public Line(Vector2 P1, Vector2 P2)
        {
            if (P1.X == P2.X)
            {
                IsVertical = true;
                Tangent = Single.PositiveInfinity;
                HorizontalOffset = (Single)Math.Round(P1.X);
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
        public Line(Single k, Single b)
        {
            IsVertical = false;
            HorizontalOffset = 0.0f;
            Tangent = k;
            Offset = b;
        }

        public Boolean Intersects(Vector2 p)
        {
            if (IsVertical)
            {
                return Math.Round(p.X) == HorizontalOffset;
            }
            return Math.Round(this[p.X]) == Math.Round(p.Y);
        }

        public Boolean Intersects(Line other, out Vector2 intPoint)
        {
            intPoint = Vector2.Zero;
            if (this.Tangent == other.Tangent || this.IsVertical && other.IsVertical)
            {
                return false;
            }
            else if (IsVertical && !other.IsVertical)
            {
                intPoint = new Vector2(HorizontalOffset, (Single)Math.Round(other[HorizontalOffset]));
                return true;
            }
            else if (!IsVertical && other.IsVertical)
            {
                intPoint = new Vector2(other.HorizontalOffset, (Single)Math.Round(this[HorizontalOffset]));
                return true;
            }
            else
            {
                Single x = (Single)Math.Round((other.Offset - Offset) / (Tangent - other.Tangent));
                intPoint = new Vector2(x, this[x]);
                return true;
            }
        }
        public Vector2 GetPointProjection(Vector2 p)
        {
            if (Tangent == 0 && !IsVertical)
            {
                return new Vector2(p.X, Offset);
            }
            else if (IsVertical)
            {
                return new Vector2(HorizontalOffset, p.Y);
            }
            else
            {
                Single newTangent = -1 / Tangent;
                Single newOffset = p.Y - (p.X * newTangent);
                Line s = new Line(newTangent, newOffset);
                Intersects(s, out Vector2 np);
                return np;
            }
        }
        public Single GetDistanceTo(Vector2 p)
        {
            Vector2 proj = GetPointProjection(p);
            Vector2 d = proj - p;
            return d.Length();
        }
        public Single GetDistanceTo(Vector2 p, out Vector2 dist)
        {
            Vector2 proj = GetPointProjection(p);
            dist = proj - p;
            return dist.Length();
        }
        public Single GetDistanceTo(Vector2 p, out Vector2 projection, out Vector2 dist)
        {
            projection = GetPointProjection(p);
            dist = projection - p;
            return dist.Length();
        }
        #region IEquatable implementation
        public Boolean Equals(Line other)
        {
            if (this.IsVertical != other.IsVertical)
            {
                return false;
            }
            else if (IsVertical && HorizontalOffset == other.HorizontalOffset)
            {
                return true;
            }
            else if (Tangent == other.Tangent && Offset == other.Offset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override Boolean Equals(Object o)
        {
            if (o is Line a)
            {
                return Equals(a);
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Tangent.GetHashCode() ^ Offset.GetHashCode() ^ HorizontalOffset.GetHashCode();
        }

        public static Boolean operator ==(Line one, Line other)
        {
            return one.Equals(other);
        }
        public static Boolean operator !=(Line one, Line other)
        {
            return !one.Equals(other);
        }
        #endregion
    }
}
