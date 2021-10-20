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
        public Single Zoom
        {
            get
            {
                return _z;
            }
            set
            {
                _z = Math.Max(value, 0.1f);
            }
        }
        Single _z = 1.0f;

        public Point GetTopLeftAngle(Point screenSize)
        {
            return new Vector2(Focus.X * Zoom * (screenSize.Y / DefaultHeight) - (Int32)(screenSize.X/2), Focus.Y * Zoom * (screenSize.Y / DefaultHeight) - (Int32)(screenSize.Y/2)).ToPoint();
        }
        public Rectangle GetDrawArea(Rectangle drawArea)
        {
            Int32 newWidth = (Int32)(drawArea.Width * Zoom);
            Int32 newHeight = (Int32)(drawArea.Height * Zoom);

            Int32 offsetX = drawArea.X - newWidth / 2;
            Int32 offsetY = drawArea.Y - newHeight / 2;

            return new Rectangle(offsetX, offsetY, newWidth, newHeight);
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
