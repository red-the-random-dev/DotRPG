using System;
using System.Collections.Generic;
using DotRPG.Waypoints;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class PathfindingManager
    {
        protected Dictionary<String, Waypoint> NavMesh = new Dictionary<string, Waypoint>();
        protected Dictionary<String, Stack<Waypoint>> TracedPaths = new Dictionary<string, Stack<Waypoint>>();

        public String GetClosestWaypointTo(Single x, Single y)
        {
            String n = "";
            Single dist = Single.PositiveInfinity;

            foreach (String a in NavMesh.Keys)
            {
                if (NavMesh[a].GetDistanceTo(x, y) < dist)
                {
                    dist = NavMesh[a].GetDistanceTo(x, y);
                    n = a;
                }
            }
            return n;
        }

        public String GetClosestWaypointTo(Single x, Single y, out Single dist)
        {
            String n = "";
            dist = Single.PositiveInfinity;

            foreach (String a in NavMesh.Keys)
            {
                if (NavMesh[a].GetDistanceTo(x, y) < dist)
                {
                    dist = NavMesh[a].GetDistanceTo(x, y);
                    n = a;
                }
            }
            return n;
        }

        public Boolean BuildPath(String PathID, String start, String end)
        {
            Waypoint[] w = WaypointGraph.BuildPath(NavMesh[start], NavMesh[end], out Single t);
            if (w == null)
            {
                return false;
            }
            Stack<Waypoint> ws = new Stack<Waypoint>();
            foreach (Waypoint x in w)
            {
                ws.Push(x);
            }
            if (TracedPaths.ContainsKey(PathID))
            {
                TracedPaths[PathID] = ws;
            }
            else
            {
                TracedPaths.Add(PathID, ws);
            }
            return true;
        }
    }
}
