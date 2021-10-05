using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Objects;

namespace DotRPG.Objects.Dynamics
{
    public class DynamicRectObject
    {
        public Boolean Static;
        public Vector2 Location;
        protected Point BodySize;
        public SpriteController Sprite;
        public Rectangle Collider
        {
            get
            {
                return new Rectangle
                (
                    (int)Location.X - BodySize.X / 2,
                    (int)Location.Y - BodySize.Y / 2,
                    BodySize.X,
                    BodySize.Y
                );
            }
        }
        /// <summary>
        /// Vector value which describes how much body will travel (pts/s)
        /// </summary>
        public Vector2 Velocity;
        public Single Mass;
        public Vector2 AppliedForce = Vector2.Zero;

        public Single KineticEnergy
        {
            get
            {
                return Mass * (Single)Math.Pow(Velocity.Length(), 2) / 2.0f;
            }
        }
        public Vector2 Momentum
        {
            get
            {
                return new Vector2(Velocity.X * Mass, Velocity.Y * Mass);
            }
        }
        public DynamicRectObject(Point StartLocation, Point colliderSize, Single mass, Boolean isStatic = false)
        {
            Location = StartLocation.ToVector2();
            BodySize = colliderSize;
            Mass = mass;
            Static = isStatic;
        }

        public void CollideWith(DynamicRectObject another, Boolean hitVertically, Boolean splitVector)
        {
            if (another.Static)
            {
                if (splitVector)
                {
                    this.Velocity = new Vector2(!hitVertically ? 0.0f : this.Velocity.X, hitVertically ? 0.0f : this.Velocity.Y);
                }
                else
                {
                    this.Velocity = Vector2.Zero;
                }
                
                if (hitVertically)
                {
                    if (this.Location.Y >= another.Location.Y)
                    {
                        this.Location.Y += Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                    else
                    {
                        this.Location.Y -= Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                }
                else
                {
                    if (this.Location.X >= another.Location.X)
                    {
                        this.Location.X += Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                    else
                    {
                        this.Location.X -= Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                }
                return;
            }
            Single Summary_X_Momentum = (this.Momentum.X + another.Momentum.X) / 2;
            Single Summary_Y_Momentum = (this.Momentum.Y + another.Momentum.Y) / 2;

            if (hitVertically)
            {
                if (splitVector)
                {
                    this.Velocity = new Vector2(this.Velocity.X, Summary_Y_Momentum / this.Mass);
                    another.Velocity = new Vector2(another.Velocity.X, Summary_Y_Momentum / another.Mass);
                }
                else
                {
                    this.Velocity = new Vector2(Summary_X_Momentum / this.Mass, Summary_Y_Momentum / this.Mass);
                    another.Velocity = new Vector2(Summary_X_Momentum / another.Mass, Summary_Y_Momentum / another.Mass);
                }
                
                if (this.Mass > another.Mass)
                {
                    if (this.Location.Y >= another.Location.Y)
                    {
                        another.Location.Y -= Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                    else
                    {
                        another.Location.Y += Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                }
                else
                {
                    if (this.Location.Y >= another.Location.Y)
                    {
                        this.Location.Y += Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                    else
                    {
                        this.Location.Y -= Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0);
                    }
                }
            }
            else
            {
                if (splitVector)
                {
                    this.Velocity = new Vector2(Summary_X_Momentum / this.Mass, this.Velocity.Y);
                    another.Velocity = new Vector2(Summary_X_Momentum / another.Mass, another.Velocity.Y);
                }
                else
                {
                    this.Velocity = new Vector2(Summary_X_Momentum / this.Mass, Summary_Y_Momentum / this.Mass);
                    another.Velocity = new Vector2(Summary_X_Momentum / another.Mass, Summary_Y_Momentum / another.Mass);
                }
                if (this.Mass > another.Mass)
                {
                    if (this.Location.X >= another.Location.X)
                    {
                        another.Location.X -= Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                    else
                    {
                        another.Location.X += Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                }
                else
                {
                    if (this.Location.X >= another.Location.X)
                    {
                        this.Location.X += Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                    else
                    {
                        this.Location.X -= Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0);
                    }
                }
            }
        }

        public void Draw(SpriteBatch _sb, GameTime gameTime, Int32 VirtualVSize, Point scrollOffset, Point scrollSize, Single ZIndex = 0.0f)
        {
            Single sizeMorph = 1.0f * scrollSize.Y / VirtualVSize;
            Vector2 location = new Vector2
            (
                Location.X * sizeMorph - (Sprite.SpriteSize.X * sizeMorph / 2) - scrollOffset.X * sizeMorph,
                Location.Y * sizeMorph + (BodySize.Y * sizeMorph / 2) - (Sprite.SpriteSize.Y * sizeMorph) - scrollOffset.Y * sizeMorph
            );
            Sprite.Draw(_sb, location, gameTime, sizeMorph, ZIndex);
        }

        public Boolean TryCollideWith(DynamicRectObject another, Boolean splitVector = false)
        {
            if (this.Collider.Intersects(another.Collider))
            {
                if (1.0 * Math.Abs(this.Location.X - another.Location.X) / (this.BodySize.X / 2 + another.BodySize.X / 2) >= 1.0 * Math.Abs(this.Location.Y - another.Location.Y) / (this.BodySize.Y / 2 + another.BodySize.Y / 2))
                {
                    CollideWith(another, false, splitVector);
                }
                else
                {
                    CollideWith(another, true, splitVector);
                }
                return true;
            }
            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
            Velocity += AppliedForce * ((Single)gameTime.ElapsedGameTime.TotalSeconds * this.Mass);
            Location += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            AppliedForce = Vector2.Zero;
        }

        public void FullStop()
        {
            Velocity = Vector2.Zero;
        }
    }
}
