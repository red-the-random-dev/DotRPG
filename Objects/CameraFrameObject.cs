using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Objects
{
    public class CameraFrameObject
    {
        public Point Focus;
        public Point TrackTarget;
        public Single CameraVelocity;
        public Int32 DefaultHeight;

        public Point GetTopLeftAngle(Point screenSize)
        {
            return new Point(Focus.X * (screenSize.Y / DefaultHeight) - screenSize.X/2, Focus.Y - screenSize.Y/2);
        }
        public void Update(GameTime gameTime)
        {
            Vector2 cameraMovement = (Focus - TrackTarget).ToVector2();
            Vector2 cameraMovementDirection = cameraMovement / cameraMovement.Length();
            Vector2 cameraMovementNew = cameraMovementDirection * CameraVelocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            if (cameraMovementNew.Length() <= cameraMovement.Length())
            {
                Focus -= cameraMovementNew.ToPoint();
            }
            else
            {
                Focus -= cameraMovement.ToPoint();
            }
        }
    }
}
