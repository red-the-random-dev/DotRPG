using DotRPG.Objects;
using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DotRPG.Behavior.Management
{
    public class CameraManager
    {
        public CameraFrameObject ManagedCamera;
        public Dictionary<String, DynamicRectObject> ObjectHeap;
        public PlayerObject Player;
        protected Random ShakeRandomizer = new Random();

        protected DynamicRectObject TrackedObject;
        protected Point _o = Point.Zero;
        protected Point ShakeOffset = Point.Zero;
        public Point Offset
        {
            get
            {
                return _o + ShakeOffset;
            }
            set
            {
                _o = value;
            }
        }

        protected Single ShakeStrength = 0;
        protected Single ShakeFadeout = 0;
        protected Single MultiplyX = 1.0f;
        protected Single MultiplyY = 1.0f;
        protected Single Frequency = 30.0f;

        public CameraManager(CameraFrameObject camera, Dictionary<String, DynamicRectObject> objectHeap)
        {
            ManagedCamera = camera;
            ObjectHeap = objectHeap;
        }

        public void TrackToPlayer()
        {
            TrackedObject = Player;
        }

        public void TrackTo(String ID)
        {
            TrackedObject = ObjectHeap[ID];
        }

        public void SetOffset(Int32 x, Int32 y)
        {
            Offset = new Point(x, y);
        }

        public void Reset()
        {
            ShakeFadeout = 0;
            ShakeStrength = 0;
            Offset = Point.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (ShakeStrength > 0)
            {
                ShakeOffset = new Point(ShakeRandomizer.Next(-(int)ShakeStrength,(int)ShakeStrength), ShakeRandomizer.Next(-(int)ShakeStrength, (int)ShakeStrength));
                ShakeStrength -= (Single)(ShakeFadeout * gameTime.ElapsedGameTime.TotalSeconds);
                if (ShakeStrength < 0)
                {
                    ShakeStrength = 0;
                    ShakeFadeout = 0;
                }
            }
            else
            {
                ShakeOffset = Point.Zero;
            }
        }

        public void Shake(Single strength, Single fadeout)
        {
            ShakeStrength = strength;
            ShakeFadeout = fadeout;
        }

        public Point TrackPoint_PlusOffset
        {
            get
            {
                return TrackedObject.Location.ToPoint() + Offset;
            }
        }

        public Point TrackPoint
        {
            get
            {
                return TrackedObject.Location.ToPoint();
            }
        }
    }
}
