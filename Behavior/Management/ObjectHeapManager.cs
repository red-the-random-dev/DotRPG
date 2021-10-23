using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class ObjectHeapManager
    {
        protected Dictionary<String, DynamicRectObject> ObjectHeap;
        public PlayerObject Player;

        public Single GetScalarVelocity(String name)
        {
            return ObjectHeap[name].Velocity.Length();
        }

        public Single GetDistance(String one, String other)
        {
            return (ObjectHeap[one].Location - ObjectHeap[other].Location).Length();
        }

        public Single GetDistanceToPlayer(String obj)
        {
            return (Player.Location - ObjectHeap[obj].Location).Length();
        }

        public Single GetVelocityDerivative(String name)
        {
            return ObjectHeap[name].VelocityDerivative;
        }

        public Single Player_X()
        {
            return Player.Location.X;
        }

        public Single Player_Y()
        {
            return Player.Location.Y;
        }

        public Single Pos_X(String name)
        {
            return ObjectHeap[name].Location.X;
        }

        public Single Pos_Y(String name)
        {
            return ObjectHeap[name].Location.Y;
        }

        public ObjectHeapManager(Dictionary<String, DynamicRectObject> objects)
        {
            ObjectHeap = objects;
        }

        public Single GetSoundPanning(String target)
        {
            Vector2 v = ObjectHeap[target].Location - Player.Location;
            v /= v.Length();
            return v.X;
        }
    }
}
