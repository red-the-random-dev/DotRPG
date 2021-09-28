using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using DotRPG.Objects;

namespace DotRPG.Objects.Dynamics
{
    public class DynamicObject
    {
        public Boolean Fixed;
        protected Point Location;
        protected Point BodySize;
        public Rectangle Collider
        {
            get
            {
                return new Rectangle
                (
                    Location.X - BodySize.X / 2,
                    Location.Y - BodySize.Y / 2,
                    BodySize.X,
                    BodySize.Y
                );
            }
        }
        public Vector2 Velocity { get; private set; }
        public Single Mass;

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

        public void CollideWith(DynamicObject another)
        {
            if (another.Fixed)
            {
                this.Velocity += Vector2.Zero;
            }
            Vector2 SummaryMomentum = (this.Momentum + another.Momentum) / 2;
            this.Velocity = SummaryMomentum / this.Mass;
            another.Velocity = SummaryMomentum / another.Mass;
        }
    }
}
