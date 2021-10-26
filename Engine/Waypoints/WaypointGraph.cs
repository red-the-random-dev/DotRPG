using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
            TraceTo(CurrentPath, destination, Solutions);
            if (Solutions.Count > 0)
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
                Array.Reverse(path);
                return path;
            }
            return null;
        }
        protected static Boolean TraceTo(Stack<Waypoint> route, Waypoint destination, List<Waypoint[]> Solutions)
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
                if (route.Contains(w))
                {
                    continue;
                }
                route.Push(w);
                if (TraceTo(route, destination, Solutions))
                {
                    Solutions.Add(route.ToArray());
                }
                route.Pop();
            }
            return false;
        }
        public static void LoadNavMapFromXML(XElement root, Dictionary<String, Waypoint> output)
        {
            foreach (XElement x in root.Elements())
            {
                switch (x.Name.LocalName.ToLower())
                {
                    case "node":
                        ResolveVector2(x.Attribute(XName.Get("location")).Value, out Single px, out Single py);
                        output.Add(x.Attribute(XName.Get("id")).Value, new Waypoint(px, py));
                        break;
                    case "weld":
                        output[x.Attribute(XName.Get("first")).Value].SetNeighbor(output[x.Attribute(XName.Get("second")).Value]);
                        break;
                }
            }
        }
        public static void ResolveVector2(String intake, out Single x, out Single y)
        {
            String a = "";
            foreach (Char c in intake)
            {
                if (c != ' ')
                {
                    a += c;
                }
            }
            String[] dims = a.Split(',');
            if (dims.Length < 2)
            {
                throw new InvalidCastException(String.Format("Unable to load Vector2 from {0}-dimensional string vector.", dims.Length));
            }
            x = Single.Parse(dims[0]);
            y = Single.Parse(dims[1]);
        }
    }
}
