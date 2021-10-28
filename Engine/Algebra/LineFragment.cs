﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Algebra
{
    public class LineFragment : IEquatable<LineFragment>
    {
        public readonly Line FullLine;
        public readonly Single Clamp_MinX;
        public readonly Single Clamp_MinY;
        public readonly Single Clamp_MaxX;
        public readonly Single Clamp_MaxY;

        public LineFragment(Vector2 P1, Vector2 P2)
        {
            FullLine = new Line(P1, P2);
            Clamp_MinX = Math.Min(P1.X, P2.X);
            Clamp_MaxX = Math.Max(P1.X, P2.X);
            Clamp_MinY = Math.Min(P1.Y, P2.Y);
            Clamp_MaxY = Math.Max(P1.Y, P2.Y);
        }

        public Boolean Intersects(Vector2 i)
        {
            return FullLine.Intersects(i) && (i.X <= Clamp_MaxX && i.X >= Clamp_MinX && i.Y <= Clamp_MaxY && i.Y >= Clamp_MinY);
        }
        public Boolean Intersects(Line line, out Vector2 intPoint)
        {
            intPoint = Vector2.Zero;
            if (FullLine.Intersects(line, out Vector2 i))
            {
                if (Intersects(i))
                {
                    intPoint = i;
                    return true;
                }
            }
            return false;
        }
        public Boolean Intersects(LineFragment other, out Vector2 intPoint)
        {
            intPoint = Vector2.Zero;
            if (FullLine.Intersects(other.FullLine, out Vector2 i))
            {
                if (Intersects(i) && other.Intersects(i))
                {
                    intPoint = i;
                    return true;
                }
            }
            return false;
        }

        public Vector2 MedianPoint
        {
            get
            {
                return new Vector2((Clamp_MaxX + Clamp_MinX) / 2, (Clamp_MaxY + Clamp_MinY) / 2);
            }
        }
        #region IEquatable implementation
        public Boolean Equals(LineFragment o)
        {
            return (Clamp_MaxX == o.Clamp_MaxX && Clamp_MaxY == o.Clamp_MaxY && Clamp_MinX == o.Clamp_MinX && Clamp_MinY == o.Clamp_MinY) && FullLine == o.FullLine;
        }
        public override Boolean Equals(Object o)
        {
            if (o is LineFragment a)
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
            return Clamp_MaxX.GetHashCode() ^ Clamp_MaxY.GetHashCode() ^ Clamp_MinX.GetHashCode() ^ Clamp_MinY.GetHashCode() ^ FullLine.GetHashCode(); 
        }

        public static Boolean operator ==(LineFragment a, LineFragment b)
        {
            return a.Equals(b);
        }
        public static Boolean operator !=(LineFragment a, LineFragment b)
        {
            return !a.Equals(b);
        }
        #endregion
    }
}