using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class ObjectHeapManager
    {
        protected Single VectorAngleCosine(Vector2 _1, Vector2 _2)
        {
            return (_1.X * _2.X + _1.Y * _2.Y) / (_1.Length() * _2.Length());
        }

        public Single VectorAngleCosine(Single _1x, Single _1y, Single _2x, Single _2y)
        {
            return VectorAngleCosine(new Vector2(_1x, _1y), new Vector2(_2x, _2y));
        }
        protected Dictionary<String, DynamicRectObject> ObjectHeap;
        public PlayerObject Player;

        public Single GetScalarVelocity(String name)
        {
            return ObjectHeap[name].Velocity.Length();
        }

        public Single GetPlayerScalarVelocity()
        {
            return Player.Velocity.Length();
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

        public void SetObjectPosition(String name, Single x, Single y)
        {
            ObjectHeap[name].Location = new Vector2(x, y);
        }

        public void SetPlayerPosition(Single x, Single y)
        {
            Player.Location = new Vector2(x, y);
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

        public Single GetRelativeScalarVelocity(String center, String other)
        {
            return (ObjectHeap[other].Velocity - ObjectHeap[center].Velocity).Length();
        }

        public Single GetRelativeScalarVelocityFromPlayer(String other)
        {
            return (ObjectHeap[other].Velocity - Player.Velocity).Length();
        }

        public Single GetApproachScalarVelocity(String center, String other)
        {
            Vector2 SightLine = ObjectHeap[other].Location - ObjectHeap[center].Location;
            Single V1 = GetScalarVelocity(other) * VectorAngleCosine(ObjectHeap[other].Velocity, SightLine);
            Single V2 = GetScalarVelocity(center) * VectorAngleCosine(ObjectHeap[center].Velocity, -SightLine);
            return V1 + V2;
        }

        public Single GetApproachScalarVelocityFromPlayer(String other)
        {
            Vector2 SightLine = ObjectHeap[other].Location - Player.Location;
            Single V1 = GetScalarVelocity(other) * VectorAngleCosine(ObjectHeap[other].Velocity, -SightLine);
            Single V2 = GetPlayerScalarVelocity() * VectorAngleCosine(Player.Velocity, SightLine);
            return V1 + V2;
        }
        /// <summary>
        /// Gets pitch shift which depends on approach velocity (https://en.wikipedia.org/wiki/Doppler_effect)
        /// </summary>
        /// <param name="target">Emitter object</param>
        /// <param name="sonicSpeed">Sound propagation speed. Default: 12160 dots per second (equivalent to 380 m/s)</param>
        /// <returns></returns>
        public Single GetDopplerShift(String target, Single sonicSpeed = 12160.0f)
        {
            // deltaf = (deltaV/c) * f0, where deltaV is approach velocity, c is sonic speed and f0 equals 1
            return Math.Max(Math.Min(GetApproachScalarVelocityFromPlayer(target) / sonicSpeed, 1.0f), -1.0f);
        }
    }
}
