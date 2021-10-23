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

        public Vector2 GetScalarVelocity(String name)
        {
            return ObjectHeap[name].Velocity;
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

        public ObjectHeapManager(Dictionary<String, DynamicRectObject> objects)
        {
            ObjectHeap = objects;
        }
    }
}
