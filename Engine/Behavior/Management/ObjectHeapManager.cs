using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Construct;
using PREFAB_SET = System.Collections.Generic.Dictionary<System.String, DotRPG.Construct.ObjectPrototype>;

namespace DotRPG.Behavior.Management
{
    public delegate void DeploymentMethod(ObjectPrototype op);
    public delegate void FinalizationMethod(String name);

    public class ObjectHeapManager
    {
        protected DeploymentMethod DeployObj;
        protected FinalizationMethod DeleteObj;
        protected PREFAB_SET StaticPrefabs;
        protected PREFAB_SET DynamicPrefabs = new PREFAB_SET();

        public ObjectHeapManager(Dictionary<String, DynamicRectObject> objects, PREFAB_SET staticPrefabs, DeploymentMethod dm, FinalizationMethod fm)
        {
            ObjectHeap = objects;
            StaticPrefabs = staticPrefabs;
            DeployObj = dm;
            DeleteObj = fm;
        }

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

        public void SetAnimationSequence(String name, String sequence, UInt16 continueFrom = 0)
        {
            ObjectHeap[name].Sprite.SetAnimationSequence(sequence, continueFrom);
        }

        public String GetCurrentAnimationSequence(String name)
        {
            return ObjectHeap[name].Sprite.CurrentAnimationSequence;
        }

        public UInt16 GetFramePlayed(String name)
        {
            return ObjectHeap[name].Sprite.SpriteIndex;
        }

        public void SetPlayerAnimationSequence(String sequence, UInt16 continueFrom = 0)
        {
            Player.Sprite.SetAnimationSequence(sequence, continueFrom);
        }

        public String GetCurrentPlayerAnimationSequence()
        {
            return Player.Sprite.CurrentAnimationSequence;
        }

        public UInt16 GetPlayerFramePlayed(String name)
        {
            return Player.Sprite.SpriteIndex;
        }

        public void EnablePlayerControls()
        {
            Player.Controlled = true;
        }

        public void DisablePlayerControls()
        {
            Player.Controlled = false;
        }

        public void SetActive(String name, Boolean active)
        {
            ObjectHeap[name].Active = active;
        }

        public void SetCollidable(String name, Boolean collidable)
        {
            ObjectHeap[name].Collidable = collidable;
        }

        public void SetVisible(String name, Boolean visible)
        {
            ObjectHeap[name].Visible = visible;
        }

        public void SetPlayerVisible(Boolean visible)
        {
            Player.Visible = visible;
        }

        public Single FindAzimuthFromPlayer(String target)
        {
            Vector2 src = ObjectHeap[target].Location - Player.Location;
            Single len = src.Length();
            if (len == 0.0f)
            {
                return 0.0f;
            }
            Vector2 i = src / len;
            Single Angle = (Single)Math.Acos(i.X);
            if (i.Y < 0.0f)
            {
                Angle = MathHelper.TwoPi - Angle;
            }
            return Angle;
        }

        public Single FindAzimuthToPlayer(String target)
        {
            Vector2 src = Player.Location - ObjectHeap[target].Location;
            Single len = src.Length();
            if (len == 0.0f)
            {
                return 0.0f;
            }
            Vector2 i = src / len;
            Single Angle = (Single)Math.Acos(i.X);
            if (i.Y < 0.0f)
            {
                Angle = MathHelper.TwoPi - Angle;
            }
            return Angle;
        }

        public Single FindAzimuth(String origin, String target)
        {
            Vector2 src = ObjectHeap[target].Location - ObjectHeap[origin].Location;
            Single len = src.Length();
            if (len == 0.0f)
            {
                return 0.0f;
            }
            Vector2 i = src / len;
            Single Angle = (Single)Math.Acos(i.X);
            if (i.Y < 0.0f)
            {
                Angle = MathHelper.TwoPi - Angle;
            }
            return Angle;
        }

        public void ApplyForce(String target, Single force, Single angle)
        {
            Vector2 v = new Vector2(force * (Single)Math.Cos(angle), force * (Single)Math.Sin(angle));
            ObjectHeap[target].AppliedForce += v;
        }

        public String GetPlayerSightDirection()
        {
            return Player.SightDirection.ToString().ToLower();
        }

        public void Prefab_CreateNew(String Name, String Tag)
        {
            ObjectPrototype op = new ObjectPrototype();
            op.Name = Tag;
            DynamicPrefabs.Add(Name, op);
        }
        public void Prefab_CreateFromStatic(String Name, String StaticName)
        {
            DynamicPrefabs.Add(Name, ObjectPrototype.CreateCopy(StaticPrefabs[StaticName]));
        }
        public void Prefab_CreateCopy(String Name, String Original)
        {
            DynamicPrefabs.Add(Name, ObjectPrototype.CreateCopy(DynamicPrefabs[Original]));
        }
        public void Prefab_Dispose(String Name)
        {
            if (DynamicPrefabs.ContainsKey(Name))
            {
                DynamicPrefabs.Remove(Name);
            }
        }
        public void Prefab_SetProperty(String Name, String Property, String Value)
        {
            if (DynamicPrefabs.TryGetValue(Name, out ObjectPrototype op))
            {
                if (op.Properties.ContainsKey(Property))
                {
                    op.Properties[Property] = Value;
                }
                else
                {
                    op.Properties.Add(Property, Value);
                }
            }
        }
        public void Prefab_AddSubnode(String Name, String Subnode)
        {
            if (DynamicPrefabs.TryGetValue(Name, out ObjectPrototype op))
            {
                if (DynamicPrefabs.TryGetValue(Subnode, out ObjectPrototype op2))
                {
                    op.Subnodes.Add(ObjectPrototype.CreateCopy(op2));
                }
            }
        }
        public String Prefab_DeployUnderRandomName(String Name)
        {
            Random r = new Random();
            String a = "";
            while (a == "" || ObjectHeap.ContainsKey(a))
            {
                a = r.Next(0, Int32.MaxValue).ToString();
            }
            ObjectPrototype op = ObjectPrototype.CreateCopy(DynamicPrefabs[Name]);
            if (op.Properties.ContainsKey("id"))
            {
                op.Properties["id"] = a;
            }
            else
            {
                op.Properties.Add("id", a);
            }
            DeployObj(op);
            return a;
        }
        public void DestroyObject(String name)
        {
            DeleteObj(name);
        }
        public Boolean Exists(String name)
        {
            return ObjectHeap.ContainsKey(name);
        }
        public void Prefab_Reset()
        {
            DynamicPrefabs.Clear();
        }
    }
}
