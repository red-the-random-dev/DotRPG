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
        protected static void Shift(DynamicObject one, DynamicObject two, Vector2[] v1, Vector2[] v2, Vector2[] c, LineFragment[] e1, LineFragment[] e2, out Vector2 normal, Boolean inverted = false)
        {
            if (one.Static || (one.Mass >= two.Mass && !inverted))
            {
                Shift(two, one, v2, v1, c, e2, e1, out normal, true);
                return;
            }
            LineFragment[] inter = Polygon.FindEdges(c);
            LineFragment interEdge = inter[0];
            Vector2 center1 = Polygon.FindActualCenter(v1);
            Single distFromCenter = Single.PositiveInfinity;
            foreach (LineFragment i in inter)
            {
                Single a = i.FullLine.GetDistanceTo(center1);
                if (a < distFromCenter)
                {
                    distFromCenter = a;
                    interEdge = i;
                }
            }
            Vector2 shiftOrigin1 = Vector2.Zero;
            Single dist1 = 0.0f;
            Vector2 shiftDestination1 = Vector2.Zero;
            foreach (Vector2 i in v1)
            {
                Vector2 x = i - center1;
                if (x.Length() > dist1 && interEdge.Intersects(new LineFragment(center1, i), out Vector2 dummy))
                {
                    dist1 = x.Length();
                    shiftOrigin1 = i;
                    shiftDestination1 = interEdge.FullLine.GetPointProjection(shiftOrigin1);
                }
            }
            Vector2 shift = shiftDestination1 - shiftOrigin1;
            Vector2 center2 = Polygon.FindActualCenter(v2);
            Vector2 shiftOrigin2 = Vector2.Zero;
            Single dist2 = 0.0f;
            Vector2 shiftDestination2 = Vector2.Zero;
            foreach (Vector2 i in v2)
            {
                Vector2 x = i - center2;
                if (x.Length() > dist2 && interEdge.Intersects(new LineFragment(center2, i), out Vector2 dummy))
                {
                    dist2 = x.Length();
                    shiftOrigin2 = i;
                    shiftDestination2 = interEdge.FullLine.GetPointProjection(shiftOrigin2);
                }
            }
            shift += (shiftOrigin2 - shiftDestination2);
            one.Location = one.Location + shift;
            normal = shift / (shift.Length() != 0.0f ? shift.Length() : 1.0f);
            /*
            Vector2[] v1_new = one.Collider.TurnedVertices;
            Vector2[] v2_new = two.Collider.TurnedVertices;
            if (Polygon.Overlaps(v1_new, v2_new))
            {
                Vector2 eviction = SharedVectorMethods.ToLengthAngle(one.Location - two.Location);
                eviction = new Vector2(Polygon.FindMedianRadius(v1_new)+Polygon.FindMedianRadius(v2_new), eviction.Y);
                one.Location -= SharedVectorMethods.FromLengthAngle(eviction);
            }
            */
        }

        protected void CollideWith(DynamicObject another, Vector2[] v1, Vector2[] v2, Vector2[] c, LineFragment[] e1, LineFragment[] e2)
        {
            Shift(this, another, v1, v2, c, e1, e2, out Vector2 normal);
            Vector2 m1 = Momentum;
            Vector2 m2 = another.Momentum;
            Vector2 sacrificedMomentum1 = m1 * Math.Abs((m1.X * normal.X + m1.Y * normal.Y) / (m1.Length() != 0.0f ? m1.Length() : 1.0f));
            Vector2 sacrificedMomentum2 = m2 * Math.Abs((m2.X * normal.X + m2.Y * normal.Y) / (m2.Length() != 0.0f ? m2.Length() : 1.0f));
            Velocity -= sacrificedMomentum1 / Mass;
            if (another.Static)
            {
                return;
            }
            another.Velocity -= sacrificedMomentum2 / Mass;
            Vector2 summaryMomentum = sacrificedMomentum1 + sacrificedMomentum2;
            Velocity += summaryMomentum / Mass;
            another.Velocity += summaryMomentum / another.Mass;
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
