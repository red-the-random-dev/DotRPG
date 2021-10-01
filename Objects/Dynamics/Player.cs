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

    public class Player : DynamicRectObject
    {
        protected Point SightAreaSize;
        public Direction SightDirection; 

        public virtual Rectangle SightArea
        {
            get
            {
                switch (SightDirection)
                {
                    case Direction.Up:
                    {
                            return new Rectangle
                            (
                                Collider.X + Collider.Width / 2 - SightAreaSize.X,
                                Collider.Y - SightAreaSize.Y,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    case Direction.Down:
                    {
                            return new Rectangle
                            (
                                Collider.X + Collider.Width / 2 - SightAreaSize.X,
                                Collider.Y + Collider.Height + SightAreaSize.Y,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    case Direction.Left:
                    {
                            return new Rectangle
                            (
                                Collider.X - SightArea.X,
                                Collider.Y + Collider.Height / 2 - SightAreaSize.Y,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    case Direction.Right:
                    {
                            return new Rectangle
                            (
                                Collider.X + Collider.Width + SightArea.X,
                                Collider.Y + Collider.Height / 2 - SightAreaSize.Y,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                    default:
                    {
                            return new Rectangle
                            (
                                Collider.X + Collider.Width / 2 - SightAreaSize.X / 2,
                                Collider.Y + Collider.Height / 2 - SightAreaSize.Y / 2,
                                SightAreaSize.X,
                                SightAreaSize.Y
                            );
                    }
                }
            }
        }

        public Player(Point StartLocation, Point colliderSize, Single mass, Point sightAreaSize) : base(StartLocation, colliderSize, mass, false)
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
