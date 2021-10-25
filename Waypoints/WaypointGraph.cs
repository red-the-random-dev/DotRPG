using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Waypoints
{
    public class WaypointGraph
    {
        public static Waypoint[] BuildPath(Waypoint start, Waypoint destination, out Single totalDistance)
        {
            HashSet<Waypoint> Visited = new HashSet<Waypoint>();
            Visited.Add(start);
            Stack<Waypoint> CurrentPath = new Stack<Waypoint>();
            CurrentPath.Push(start);
            List<Waypoint[]> Solutions = new List<Waypoint[]>();
            totalDistance = Single.PositiveInfinity;
            if (TraceTo(CurrentPath, destination, Visited, Solutions))
            {
                Waypoint[] path = null;
                foreach (Waypoint[] wp in Solutions)
                {
                    Single dist = 0.0f;
                    for (int i = 0; i < wp.Length - 1; i++)
                    {
                        dist += wp[i].GetDistanceTo(wp[i + 1]);
                    }
                    if (dist < totalDistance)
                    {
                        path = wp;
                        totalDistance = dist;
                    }
                }
                return path;
            }
            return null;
        }
        protected static Boolean TraceTo(Stack<Waypoint> route, Waypoint destination, HashSet<Waypoint> alreadyVisited, List<Waypoint[]> Solutions)
        {
            Waypoint c = route.Peek();
            if (c == destination)
            {
                return true;
            }
            foreach (Waypoint w in c.GetNeighbors())
            {
                if (w == c)
                {
                    continue;
                }
                if (alreadyVisited.Contains(w))
                {
                    continue;
                }
                route.Push(w);
                alreadyVisited.Add(w);
                if (TraceTo(route, destination, alreadyVisited, Solutions))
                {
                    Solutions.Add(route.ToArray());
                }
                route.Pop();
            }
            return false;
        }
    }
}
