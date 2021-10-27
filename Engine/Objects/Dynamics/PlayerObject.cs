using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects.Dynamics
{
    public enum Direction
    {
        Up=0, Down=1, Left=2, Right=3
    };

    public class PlayerObject : DynamicObject
    {
        protected Point SightAreaSize;
        public Direction SightDirection;
        public MotionParameters Motion = new MotionParameters();
        public Boolean Controlled = true;

        public virtual Rectangle SightArea
        {
            get
            {
                Rectangle c = SquareForm;
                switch (SightDirection)
                {
                    case Direction.Up:
                    {
                            return new Rectangle
                            (
                                c.X + (c.Width / 2) - (SightAreaSize.X / 2),
                                c.Y - SightAreaSize.Y,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    case Direction.Down:
                    {
                            return new Rectangle
                            (
                                c.X + (c.Width / 2) - (SightAreaSize.X / 2),
                                c.Y + c.Height,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    case Direction.Left:
                    {
                            return new Rectangle
                            (
                                c.X - SightAreaSize.X,
                                c.Y + (c.Height / 2) - (SightAreaSize.Y / 2),
                                SightAreaSize.Y,
                                SightAreaSize.X
                            );
                    }
                    case Direction.Right:
                    {
                            return new Rectangle
                            (
                                c.X + c.Width,
                                c.Y + (c.Height / 2) - (SightAreaSize.Y / 2),
                                SightAreaSize.Y,
                                SightAreaSize.X
                            );
                    }
                    default:
                    {
                            return new Rectangle
                            (
                                c.X + c.Width / 2 - SightAreaSize.X / 2,
                                c.Y + c.Height / 2 - SightAreaSize.Y / 2,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                }
            }
        }

        public PlayerObject(Point StartLocation, Point colliderSize, Single mass, Vector2 origin, Point sightAreaSize) : base(StartLocation, colliderSize, mass, origin, false)
        {
            SightAreaSize = sightAreaSize;
        }

        public void Update(GameTime gameTime, Direction lookTowards)
        {
            base.Update(gameTime);
            SightDirection = lookTowards;
        }
    }
}
