using System;
using System.Collections.Generic;

namespace DotRPG.Waypoint
{
    public class Waypoint : IEquatable<Waypoint>
    {
        public Single Pos_X;
        public Single Pos_Y;
        protected HashSet<Waypoint> Neighbors;

        public Single GetDistanceTo(Single x, Single y)
        {
            return (Single)Math.Sqrt(Math.Pow(x - Pos_X, 2) + Math.Pow(y - Pos_Y, 2));
        }
        public Single GetDistanceTo(Waypoint other)
        {
            return (Single)Math.Sqrt(Math.Pow(other.Pos_X - Pos_X, 2) + Math.Pow(other.Pos_Y - Pos_Y, 2));
        }

        public Boolean Equals(Waypoint other)
        {
            return this.Pos_X == other.Pos_X && this.Pos_Y == other.Pos_Y;
        }

        public override Boolean Equals(Object other)
        {
            if (other is Waypoint waypoint)
            {
                return Equals(waypoint);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Pos_X.GetHashCode() ^ Pos_Y.GetHashCode();
        }

        public static Boolean operator == (Waypoint a, Waypoint b)
        {
            return a.Equals(b);
        }

        public static Boolean operator != (Waypoint a, Waypoint b)
        {
            return !a.Equals(b);
        }

        public static Boolean operator == (Waypoint a, Object b)
        {
            return a.Equals(b);
        }

        public static Boolean operator != (Waypoint a, Object b)
        {
            return !a.Equals(b);
        }

        public void SetNeighbor(Waypoint target)
        {
            Neighbors.Add(target);
            target.Neighbors.Add(this);
        }

        public IEnumerable<Waypoint> GetNeighbors()
        {
            foreach (Waypoint w in Neighbors)
            {
                yield return w;
            }
        }
    }
}
