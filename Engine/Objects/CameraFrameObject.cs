using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Objects
{
    public class CameraFrameObject
    {
        public Point Focus
        {
            get
            {
                return FocusRaw + Offset;
            }
            set
            {
                FocusRaw = value;
            }
        }
        public Point FocusRaw;
        public Point TrackTarget;
        public Single CameraVelocity;
        public Int32 DefaultHeight;
        public Point Offset = Point.Zero;
        public Point OffsetTarget = Point.Zero;
        public Single OffsetVelocity;
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

        public static void Move(ref Point pos, Point trackTarget, Single velocity, GameTime gameTime)
        {
            Vector2 Movement = (pos - trackTarget).ToVector2();
            Vector2 MovementDirection = Movement / Movement.Length();
            Vector2 MovementNew = MovementDirection * velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            if (MovementNew.Length() <= Movement.Length())
            {
                pos -= MovementNew.ToPoint();
            }
            else
            {
                pos -= Movement.ToPoint();
            }
        }

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
        public Rectangle GetAOV(Rectangle viewport)
        {
            Int32 height = viewport.Height;
            Single aspectRatio = 1.0f * viewport.Width / viewport.Height;
            Rectangle x = Algebra.SharedRectangleMethods.GetFromOrigin(Focus.ToVector2(), new Vector2(0.5f, 0.5f), new Vector2(height * aspectRatio, height));
            return x;
        }
        public void Update(GameTime gameTime)
        {
            Move(ref FocusRaw, TrackTarget, CameraVelocity, gameTime);
            Move(ref Offset, OffsetTarget, OffsetVelocity, gameTime);
        }
    }
}
