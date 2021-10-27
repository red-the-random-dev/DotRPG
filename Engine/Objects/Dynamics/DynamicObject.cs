using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Objects;
using DotRPG.Objects.Complexity;
using DotRPG.Algebra;

namespace DotRPG.Objects.Dynamics
{
    public class DynamicObject
    {
        public Boolean Static;
        public Vector2 Location
        {
            get
            {
                return Collider.Location;
            }
            set
            {
                Collider.Location = value;
            }
        }
        public Rectangle SquareForm
        {
            get
            {
                Vector2[] vrt = Collider.Vertices;
                Single Max_X = Single.NegativeInfinity;
                Single Min_X = Single.PositiveInfinity;
                Single Max_Y = Single.NegativeInfinity;
                Single Min_Y = Single.PositiveInfinity;
                foreach (Vector2 v in vrt)
                {
                    Max_X = Math.Max(Max_X, v.X);
                    Min_X = Math.Min(Min_X, v.X);
                    Max_Y = Math.Max(Max_Y, v.Y);
                    Min_Y = Math.Min(Min_Y, v.Y);
                }
                return new Rectangle
                (
                    (int)Min_X, (int)Min_Y,
                    (int)(Max_X - Min_X), (int)(Max_Y - Min_Y)
                );
            }
        }
        public SpriteController Sprite;
        public Single Rotation
        {
            get
            {
                return Collider.Turn;
            }
            set
            {
                Collider.Turn = value;
            }
        }
        public Vector2 BodySize;
        public Boolean Collidable = true;
        public Boolean Active = true;
        public Boolean Visible = true;
        public Vector2 ColliderOrigin = new Vector2(0.5f, 0.5f);
        public Vector2 SpriteOffset = Vector2.Zero;
        public Vector2 SpriteOrigin = new Vector2(0.5f, 0.5f);
        public Polygon Collider;
        /// <summary>
        /// Vector value which describes how much body will travel (pts/s)
        /// </summary>
        public Vector2 Velocity;
        protected Single LastScalarVelocity = 0.0f;
        public Single Mass;
        public Vector2 AppliedForce = Vector2.Zero;

        public Single VelocityDerivative { get; protected set; }

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
        public DynamicObject(Point StartLocation, Point colliderSize, Single mass, Vector2 origin, Boolean isStatic = false)
        {
            Rectangle r = new Rectangle(
                (int)(StartLocation.X - colliderSize.X * origin.X),
                (int)(StartLocation.Y - colliderSize.Y * origin.Y),
                colliderSize.X,
                colliderSize.Y
            );
            Collider = PolygonBuilder.BuildFromRect(r, origin);
            Location = StartLocation.ToVector2();
            BodySize = colliderSize.ToVector2();
            Mass = mass;
            Static = isStatic;
        }
        protected static void Shift(DynamicObject one, DynamicObject two, Vector2[] v1, Vector2[] v2, Vector2[] c, LineFragment[] e1, LineFragment[] e2, Boolean inverted = false)
        {
            if (one.Static || (one.Mass >= two.Mass && !inverted))
            {
                Shift(two, one, v2, v1, c, e2, e1, true);
                return;
            }
            LineFragment[] inter = Polygon.FindEdges(c);
            LineFragment interEdge = inter[0];
            Vector2 center = Polygon.FindActualCenter(v1);
            Single maxDist = Single.PositiveInfinity;
            foreach (LineFragment i in inter)
            {
                Single a = i.FullLine.GetDistanceTo(center);
                if (a < maxDist)
                {
                    maxDist = a;
                    interEdge = i;
                }
            }
            Single minDist = Single.PositiveInfinity;
            Vector2 shift = Vector2.Zero;
            foreach (Vector2 i in v1)
            {
                Single a = interEdge.FullLine.GetDistanceTo(i, out Vector2 dist);
                if (a < minDist || minDist == Single.PositiveInfinity)
                {
                    minDist = a;
                    shift = dist;
                }
            }
            one.Location = one.Location + shift;
            Vector2[] v1_new = one.Collider.TurnedVertices;
            Vector2[] v2_new = two.Collider.TurnedVertices;
            if (Polygon.Overlaps(v1_new, v2_new))
            {
                Vector2 eviction = SharedVectorMethods.ToLengthAngle(one.Location - two.Location);
                eviction = new Vector2(Polygon.FindMedianRadius(v1_new)+Polygon.FindMedianRadius(v2_new), eviction.Y);
                one.Location -= SharedVectorMethods.FromLengthAngle(eviction);
            }
        }

        protected void CollideWith(DynamicObject another, Vector2[] v1, Vector2[] v2, Vector2[] c, LineFragment[] e1, LineFragment[] e2)
        {
            Shift(this, another, v1, v2, c, e1, e2);
            if (another.Static)
            {
                FullStop();
                return;
            }
            Single Summary_X_Momentum = (this.Momentum.X + another.Momentum.X) / 2;
            Single Summary_Y_Momentum = (this.Momentum.Y + another.Momentum.Y) / 2;

            Velocity = new Vector2(Summary_X_Momentum / this.Mass, Summary_Y_Momentum / this.Mass);
            another.Velocity = new Vector2(Summary_X_Momentum / another.Mass, Summary_Y_Momentum / another.Mass);
        }

        public void Draw(SpriteBatch _sb, GameTime gameTime, Int32 VirtualVSize, Point scrollOffset, Point scrollSize, Color DrawColor, Single ZIndex = 0.0f)
        {
            if (Sprite == null || !Active  || !Visible)
            {
                return;
            }
            Single sizeMorph = 1.0f * scrollSize.Y / VirtualVSize;
            Vector2 location = new Vector2
            (
                Location.X * sizeMorph - scrollOffset.X,
                Location.Y * sizeMorph + (BodySize.Y * sizeMorph / 2) - scrollOffset.Y
            );
            Sprite.Draw(_sb, location, gameTime, new Vector2(0.5f, 1.0f), DrawColor, sizeMorph, ZIndex);
        }

        public Boolean TryCollideWith(DynamicObject another)
        {
            if (!this.Collidable || !another.Collidable || !this.Active || !another.Active || this.Static)
            {
                return false;
            }
            Vector2[] v1 = Collider.TurnedVertices;
            Vector2[] v2 = another.Collider.TurnedVertices;
            LineFragment[] e1 = Polygon.FindEdges(v1);
            LineFragment[] e2 = Polygon.FindEdges(v2);
            Vector2[] c = Polygon.FindContactsOf(e1, e2);
            if (c.Length >= 2)
            {
                CollideWith(another, v1, v2, c, e1, e2);
                return true;
            }
            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
            Velocity += AppliedForce * ((Single)gameTime.ElapsedGameTime.TotalSeconds * this.Mass);
            Location += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            AppliedForce = Vector2.Zero;
            Single v = Velocity.Length();
            VelocityDerivative = (v - LastScalarVelocity) / (Single)gameTime.ElapsedGameTime.TotalSeconds;
            LastScalarVelocity = v;
        }

        public void FullStop()
        {
            Velocity = Vector2.Zero;
        }
    }
}
