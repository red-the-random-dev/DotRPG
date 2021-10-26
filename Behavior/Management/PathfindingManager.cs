using System;
using System.Collections.Generic;
using DotRPG.Waypoints;
using System.Text;
using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;

namespace DotRPG.Behavior.Management
{
    public class PathfindingManager
    {
        protected Dictionary<String, Waypoint> NavMesh;
        protected Dictionary<String, Stack<Waypoint>> TracedPaths = new Dictionary<string, Stack<Waypoint>>();
        protected Dictionary<String, DynamicRectObject> Objects;
        protected PlayerObject Player;

        public PathfindingManager(Dictionary<String, Waypoint> navMesh, Dictionary<String, DynamicRectObject> objectHeap, PlayerObject player)
        {
            NavMesh = navMesh;
            Objects = objectHeap;
            Player = player;
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

        public String GetClosestWaypointToObject(String target)
        {
            return GetClosestWaypointTo(Objects[target].Location.X, Objects[target].Location.Y);
        }
        public String GetClosestWaypointToObject(String target, out Single dist)
        {
            return GetClosestWaypointTo(Objects[target].Location.X, Objects[target].Location.Y, out dist);
        }
        public String GetClosestWaypointToPlayer()
        {
            return GetClosestWaypointTo(Player.Location.X, Player.Location.Y);
        }
        public String GetClosestWaypointToPlayer(out Single dist)
        {
            return GetClosestWaypointTo(Player.Location.X, Player.Location.Y, out dist);
        }
        public Boolean ForceMoveObjectByPath(String ObjectID, String PathID, Single velocity, Single ApproachDistance = 0.1f)
        {
            if (!TracedPaths.ContainsKey(PathID))
            {
                return false;
            }
            Stack<Waypoint> path = TracedPaths[PathID];
            if (path.Count == 0)
            {
                TracedPaths.Remove(PathID);
                return false;
            }

            Waypoint nextWaypoint = path.Peek();
            Vector2 v = new Vector2(nextWaypoint.X, nextWaypoint.Y) - Objects[ObjectID].Location;
            if (v.Length() != 0)
            {
                v /= v.Length();
                v *= velocity;
            }
            Objects[ObjectID].Velocity = v;
            if (v.Length() <= ApproachDistance)
            {
                path.Pop();
                if (path.Count == 0)
                {
                    TracedPaths.Remove(PathID);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        public Boolean IsEndPoint(String PathID, String waypointID)
        {
            if (!TracedPaths.ContainsKey(PathID))
            {
                return false;
            }
            Waypoint[] wp = TracedPaths[PathID].ToArray();
            if (wp.Length == 0)
            {
                return false;
            }
            return wp[wp.Length - 1] == NavMesh[waypointID];
        }
        public Single DistanceToNextWaypoint(String ObjectID, String PathID)
        {
            if (!TracedPaths.ContainsKey(PathID) || !Objects.ContainsKey(ObjectID))
            {
                return Single.NaN;
            }
            if (TracedPaths[PathID].Count == 0)
            {
                return Single.NaN;
            }
            return TracedPaths[PathID].Peek().GetDistanceTo(Objects[ObjectID].Location.X, Objects[ObjectID].Location.Y);
        }

        public Boolean BuildPath(String PathID, String start, String end)
        {
            if (!NavMesh.ContainsKey(start) || !NavMesh.ContainsKey(end))
            {
                return false;
            }
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
