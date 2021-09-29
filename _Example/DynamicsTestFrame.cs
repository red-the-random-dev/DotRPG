using System;
using System.Collections.Generic;
using DotRPG.Objects;
using DotRPG.Objects.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    class DynamicsTestFrame : Frame
    {
        DynamicRectObject Player;
        DynamicRectObject Obstacle;
        SoundEffectInstance Sliding;

        public override int FrameID
        {
            get
            {
                return 1;
            }
        }

        public DynamicsTestFrame(Game owner, ResourceHeap rh, HashSet<TimedEvent> eventSet) : base(owner, rh, eventSet)
        {

        }

        public override void LoadContent()
        {
            FrameResources.Textures.Add("cube-p", Owner.Content.Load<Texture2D>("Texture2D/cube-p"));
            Player.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-p"]);
            FrameResources.Textures.Add("cube-o", Owner.Content.Load<Texture2D>("Texture2D/cube-o"));
            Obstacle.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-o"]);
            FrameResources.Sounds.Add("slide", Owner.Content.Load<SoundEffect>("Sounds/wallcling"));
            FrameResources.Sounds.Add("impact", Owner.Content.Load<SoundEffect>("Sounds/impact"));
            Sliding = FrameResources.Sounds["slide"].CreateInstance();
            Sliding.IsLooped = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            Player.Draw(spriteBatch, gameTime, 540, Point.Zero, new Point(drawZone.Width, drawZone.Height));
            Obstacle.Draw(spriteBatch, gameTime, 540, Point.Zero, new Point(drawZone.Width, drawZone.Height));
            spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], String.Format("Player: ({0}, {1}), Velocity=({2}, {3})", Player.Collider.X, Player.Collider.Y, Player.Velocity.X, Player.Velocity.Y), new Vector2(0, 12), Color.White);
        }

        public override void Initialize()
        {
            Player = new DynamicRectObject(new Point(16, 16), new Point(32, 32), 65.0f);
            Obstacle = new DynamicRectObject(new Point(128, 128), new Point(32, 32), 10.0f);
        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            Single loco_x = 0.0f; Single loco_y = 0.0f;
            if (controls[0]) { loco_y -= 1.0f; }
            if (controls[1]) { loco_y += 1.0f; }
            if (controls[2]) { loco_x -= 1.0f; }
            if (controls[3]) { loco_x += 1.0f; }
            Vector2 Locomotion = new Vector2(loco_x, loco_y);
            Locomotion /= (Locomotion.Length() != 0 ? Locomotion.Length() : 1.0f);
            Locomotion *= Player.Mass * 0.001f;
            Vector2 FrictionVector = new Vector2(0.0f - (Player.Velocity.X / (Player.Velocity.Length() != 0 ? Player.Velocity.Length() : 1.0f)), 0.0f - (Player.Velocity.Y / (Player.Velocity.Length() != 0 ? Player.Velocity.Length() : 1.0f)));
            FrictionVector *= Player.Mass * 0.001f;
            Vector2 FrictionVectorO = new Vector2(0.0f - (Obstacle.Velocity.X / (Obstacle.Velocity.Length() != 0 ? Obstacle.Velocity.Length() : 1.0f)), 0.0f - (Obstacle.Velocity.Y / (Obstacle.Velocity.Length() != 0 ? Obstacle.Velocity.Length() : 1.0f)));
            FrictionVectorO *= Obstacle.Mass * 0.00001f;
            if (Obstacle.Velocity.Length() >= 0.001f)
            {
                Sliding.Play();
            }
            else
            {
                Sliding.Stop();
            }
            Player.AppliedForce += FrictionVector;
            Player.AppliedForce += Locomotion;
            Obstacle.AppliedForce += FrictionVectorO;
            Player.Update(gameTime);
            Obstacle.Update(gameTime);
            if (Player.Collider.Y <= 0 || Player.Collider.Y >= 540)
            {
                Player.Location = new Vector2(Player.Location.X, (Player.Collider.Y <= 0 ? 16.0f : 524.0f));
                Player.FullStop();
            }
            if (Player.Collider.X <= 0 || Player.Collider.X >= 960)
            {
                Player.Location = new Vector2((Player.Collider.X <= 0 ? 16.0f : 944.0f), Player.Location.Y);
                Player.FullStop();
            }
            if (Obstacle.Collider.Y <= 0 || Obstacle.Collider.Y >= 540)
            {
                Obstacle.Location = new Vector2(Obstacle.Location.X, (Obstacle.Collider.Y <= 0 ? 16.0f : 524.0f));
                Obstacle.Velocity = new Vector2(Obstacle.Velocity.X, 0 - Obstacle.Velocity.Y);
                FrameResources.Sounds["impact"].Play();
            }
            if (Obstacle.Collider.X <= 0 || Obstacle.Collider.X >= 960)
            {
                Obstacle.Location = new Vector2((Obstacle.Collider.X <= 0 ? 16.0f : 944.0f), Obstacle.Location.Y);
                Obstacle.Velocity = new Vector2(0 - Obstacle.Velocity.X, Obstacle.Velocity.Y);
                FrameResources.Sounds["impact"].Play();
            }
            if (Player.TryCollideWith(Obstacle))
            {
                Player.FullStop();
                FrameResources.Sounds["impact"].Play();
            }
            base.Update(gameTime, controls);
        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void UnloadContent()
        {
            FrameResources.Dispose();
            Player = null;
            Obstacle = null;
            Sliding = null;
        }
    }
}
