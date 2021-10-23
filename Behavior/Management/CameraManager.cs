using DotRPG.Objects;
using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Management
{
    public class CameraManager
    {
        public CameraFrameObject ManagedCamera;
        public Dictionary<String, DynamicRectObject> ObjectHeap;
        public PlayerObject Player;

        protected DynamicRectObject TrackedObject;
        protected Point Offset = Point.Zero;

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

        public Point TrackPoint
        {
            get
            {
                return TrackedObject.Location.ToPoint() + Offset;
            }
        }
    }
}
