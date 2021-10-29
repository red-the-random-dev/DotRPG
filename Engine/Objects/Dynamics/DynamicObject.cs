#define FORCE_USE_LEGACY
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Objects;
using DotRPG.Objects.Complexity;
using DotRPG.Algebra;

namespace DotRPG.Objects.Dynamics
{
    public delegate Boolean TryCollideWithMethod(DynamicObject another, out int contacts, int sampleAmount, GameTime gameTime);

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
#if !FORCE_USE_LEGACY
        // don't forget.
        public Boolean UseSimplifiedCollision = true;
#endif
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
        public Boolean TryCollideWith(DynamicObject another, out Int32 contactAmount, UInt16 sampleAmount = 1, GameTime gameTime = null)
        {
            if
            (
#if FORCE_USE_LEGACY
                true
#else
                UseSimplifiedCollision
#endif
            )
            {
                return _legacy_TryCollideWith(another, out contactAmount, sampleAmount, gameTime);
            }
            else
            {
                return _poly_TryCollideWith(another, out contactAmount, sampleAmount, gameTime);
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
        protected void _legacy_CollideWith(DynamicObject another, Boolean hitVertically, Boolean splitVector)
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
                        this.Location = new Vector2(this.Location.X, this.Location.Y + Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
                    }
                    else
                    {
                        this.Location = new Vector2(this.Location.X, this.Location.Y - Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
                    }
                }
                else
                {
                    if (this.Location.X >= another.Location.X)
                    {
                        this.Location = new Vector2(this.Location.X + Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), this.Location.Y);
                    }
                    else
                    {
                        this.Location = new Vector2(this.Location.X - Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), this.Location.Y);
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
                        another.Location = new Vector2(another.Location.X, another.Location.Y - Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
                    }
                    else
                    {
                        another.Location = new Vector2(another.Location.X, another.Location.Y + Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
                    }
                }
                else
                {
                    if (this.Location.Y >= another.Location.Y)
                    {
                        this.Location = new Vector2(this.Location.X, this.Location.Y - Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
                    }
                    else
                    {
                        this.Location = new Vector2(this.Location.X, this.Location.Y + Math.Max((this.BodySize.Y / 2 + another.BodySize.Y / 2) - Math.Abs(this.Location.Y - another.Location.Y), 0));
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
                        another.Location = new Vector2(another.Location.X - Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), another.Location.Y);
                    }
                    else
                    {
                        another.Location = new Vector2(another.Location.X + Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), another.Location.Y);
                    }
                }
                else
                {
                    if (this.Location.X >= another.Location.X)
                    {
                        this.Location = new Vector2(this.Location.X + Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), this.Location.Y);
                    }
                    else
                    {
                        this.Location = new Vector2(this.Location.X - Math.Max((this.BodySize.X / 2 + another.BodySize.X / 2) - Math.Abs(this.Location.X - another.Location.X), 0), this.Location.Y);
                    }
                }
            }
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
                Single a = (i.MedianPoint - center1).Length();
                if (a < distFromCenter)
                {
                    distFromCenter = a;
                    interEdge = i;
                }
            }
            Vector2 farthestEndPoint = (interEdge.End1 - center1).Length() > (interEdge.End2 - center1).Length() ? interEdge.End1 : interEdge.End2;
            List<LineFragment> containingEdges = new List<LineFragment>();
            foreach (LineFragment lf in e2)
            {
                if (lf.Intersects(farthestEndPoint))
                {
                    LineFragment ien = new LineFragment((lf.End1 - center1).Length() > (lf.End2 - center1).Length() ? lf.End2 : lf.End1, farthestEndPoint);
                    if (ien.FullLine.Tangent != interEdge.FullLine.Tangent)
                    {
                        interEdge = ien;
                    }
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
            Vector2 add = (center1 - interEdge.FullLine.GetPointProjection(center1));
            add /= (add.Length() != 0.0f ? add.Length() : 1.0f);
            one.Location = one.Location + shift + add;
            normal = shift / (shift.Length() != 0.0f ? shift.Length() : 1.0f);
            /*
            Vector2[] v1_new = one.Collider.TurnedVertices;
            Vector2[] v2_new = two.Collider.TurnedVertices;
            if (Polygon.Overlaps(v1_new, v2_new))
            {
                Vector2 eviction = SharedVectorMethods.ToLengthAngle(one.Location - two.Location + new Vector2(1,0));
                eviction = new Vector2(Polygon.FindMedianRadius(v1_new)+Polygon.FindMedianRadius(v2_new), eviction.Y);
                one.Location -= SharedVectorMethods.FromLengthAngle(eviction);
                normal = (shift + eviction);
                normal = normal / (normal.Length() != 0.0f ? normal.Length() : 1.0f);
            }
            */
        }

        protected void _poly_CollideWith(DynamicObject another, Vector2[] v1, Vector2[] v2, Vector2[] c, LineFragment[] e1, LineFragment[] e2)
        {
            Shift(this, another, v1, v2, c, e1, e2, out Vector2 normal);
            Vector2 m1 = Momentum;
            Vector2 m2 = another.Momentum;
            Vector2 sacrificedMomentum1 = m1 * Math.Abs((m1.X * normal.X + m1.Y * normal.Y) / (m1.Length() != 0.0f ? m1.Length() : 1.0f));
            Vector2 sacrificedMomentum2 = m2 * Math.Abs((m2.X * normal.X + m2.Y * normal.Y) / (m2.Length() != 0.0f ? m2.Length() : 1.0f));
            Velocity -= sacrificedMomentum1 / Mass;
            if (another.Static)
            {
                // Velocity += normal * 2 * Velocity.Length();
                FullStop();
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

        protected Boolean _poly_TryCollideWith(DynamicObject another, out Int32 contactAmount, UInt16 sampleAmount = 1, GameTime gameTime = null)
        {
            contactAmount = 0;
            if (!this.Collidable || !another.Collidable || !this.Active || !another.Active || this.Static)
            {
                return false;
            }
            Vector2 ogPos1 = Location;
            Vector2 ogPos2 = another.Location;
            
            if (sampleAmount >= 2 && gameTime != null)
            {
                Polygon p1 = Polygon.Copy(Collider);
                Polygon p2 = Polygon.Copy(another.Collider);
                Vector2 loc1 = Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 loc2 = another.Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

                for (UInt16 i = 0; i < sampleAmount; i++)
                {
                    p1.Location = ogPos1 + loc1 * 1.0f * i / (sampleAmount + 1);
                    p2.Location = ogPos2 + loc2 * 1.0f * i / (sampleAmount + 1);

                    Vector2[] v1 = p1.TurnedVertices;
                    Vector2[] v2 = p2.TurnedVertices;
                    LineFragment[] e1 = Polygon.FindEdges(v1);
                    LineFragment[] e2 = Polygon.FindEdges(v2);
                    Vector2[] c = Polygon.FindContactsOf(e1, e2, v1, v2);
                    if ((contactAmount = c.Length) >= 2)
                    {
                        Location = p1.Location;
                        another.Location = p2.Location;
                        _poly_CollideWith(another, v1, v2, c, e1, e2);
                        return true;
                    }
                }
            }
            else
            {
                Vector2[] v1 = Collider.TurnedVertices;
                Vector2[] v2 = another.Collider.TurnedVertices;
                LineFragment[] e1 = Polygon.FindEdges(v1);
                LineFragment[] e2 = Polygon.FindEdges(v2);
                Vector2[] c = Polygon.FindContactsOf(e1, e2, v1, v2);
                if ((contactAmount = c.Length) >= 2)
                {
                    _poly_CollideWith(another, v1, v2, c, e1, e2);
                    return true;
                }
            }
            return false;
        }
        protected Boolean _legacy_TryCollideWith(DynamicObject another, out Int32 contactAmount, UInt16 sampleAmount = 1, GameTime gameTime = null)
        {
            if (!this.Collidable || !another.Collidable || !this.Active || !another.Active)
            {
                contactAmount = 0;
                return false;
            }
            if (this.SquareForm.Intersects(another.SquareForm))
            {
                if (1.0 * Math.Abs(this.Location.X - another.Location.X) / (this.BodySize.X / 2 + another.BodySize.X / 2) >= 1.0 * Math.Abs(this.Location.Y - another.Location.Y) / (this.BodySize.Y / 2 + another.BodySize.Y / 2))
                {
                    _legacy_CollideWith(another, false, false);
                }
                else
                {
                    _legacy_CollideWith(another, true, false);
                }
                contactAmount = 2;
                return true;
            }
            contactAmount = 0;
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
