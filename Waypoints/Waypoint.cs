using System;
using System.Collections.Generic;

namespace DotRPG.Waypoints
{
    public class Waypoint : IEquatable<Waypoint>
    {
        public Single X;
        public Single Y;
        protected HashSet<Waypoint> Neighbors = new HashSet<Waypoint>();

        public Waypoint(Single x, Single y)
        {
            X = x; Y = y;
        }
        public Single GetDistanceTo(Single x, Single y)
        {
            return (Single)Math.Sqrt(Math.Pow(x - X, 2) + Math.Pow(y - X, 2));
        }
        public Single GetDistanceTo(Waypoint other)
        {
            return (Single)Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }

        public Boolean Equals(Waypoint other)
        {
            return this.X == other.X && this.Y == other.Y;
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
            return X.GetHashCode() ^ Y.GetHashCode();
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
