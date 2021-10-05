// #define MUTE
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
        DynamicRectObject Obstacle1;
        DynamicRectObject Obstacle2;
        SoundEffectInstance Sliding;

        DynamicRectObject WallTop = new DynamicRectObject(new Point(480, -60), new Point(960, 120), 100.0f, true);
        DynamicRectObject WallBottom = new DynamicRectObject(new Point(480, 600), new Point(960, 120), 100.0f, true);
        DynamicRectObject WallLeft = new DynamicRectObject(new Point(-60, 270), new Point(120, 540), 100.0f, true);
        DynamicRectObject WallRight = new DynamicRectObject(new Point(1020, 270), new Point(120, 540), 100.0f, true);

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
            FrameResources.Textures.Add("cube-o2", Owner.Content.Load<Texture2D>("Texture2D/cube-o2"));
            Obstacle1.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-o"]);
            Obstacle2.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-o2"]);
            FrameResources.Sounds.Add("slide", Owner.Content.Load<SoundEffect>("Sounds/wallcling"));
            FrameResources.Sounds.Add("impact", Owner.Content.Load<SoundEffect>("Sounds/impact"));
            Sliding = FrameResources.Sounds["slide"].CreateInstance();
            Sliding.IsLooped = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            Player.Draw(spriteBatch, gameTime, 540, Point.Zero, new Point(drawZone.Width, drawZone.Height));
            Obstacle1.Draw(spriteBatch, gameTime, 540, Point.Zero, new Point(drawZone.Width, drawZone.Height));
            Obstacle2.Draw(spriteBatch, gameTime, 540, Point.Zero, new Point(drawZone.Width, drawZone.Height));
            #if DEBUG
            spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], String.Format("Player: ({0}, {1}), Velocity=({2}, {3})", Player.Collider.X, Player.Collider.Y, Player.Velocity.X, Player.Velocity.Y), new Vector2(0, 12), Color.White);
            #endif
        }

        public override void Initialize()
        {
            Player = new DynamicRectObject(new Point(48, 48), new Point(32, 32), 20.0f);
            Obstacle1 = new DynamicRectObject(new Point(128, 128), new Point(32, 32), 10.0f);
            Obstacle2 = new DynamicRectObject(new Point(192, 128), new Point(64, 64), 10.0f);
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
            Locomotion *= 256f;
            Vector2 FrictionVector1 = new Vector2(0.0f - (Obstacle1.Velocity.X / (Obstacle1.Velocity.Length() != 0 ? Obstacle1.Velocity.Length() : 1.0f)), 0.0f - (Obstacle1.Velocity.Y / (Obstacle1.Velocity.Length() != 0 ? Obstacle1.Velocity.Length() : 1.0f)));
            FrictionVector1 *= Obstacle1.Mass * 0.5f;
            Vector2 FrictionVector2 = new Vector2(0.0f - (Obstacle2.Velocity.X / (Obstacle2.Velocity.Length() != 0 ? Obstacle2.Velocity.Length() : 1.0f)), 0.0f - (Obstacle2.Velocity.Y / (Obstacle2.Velocity.Length() != 0 ? Obstacle2.Velocity.Length() : 1.0f)));
            FrictionVector2 *= Obstacle2.Mass * 0.05f;
            if (Obstacle1.Velocity.Length() >= 0.001f)
            {
                #if !MUTE
                Sliding.Play();
                #endif
            }
            else
            {
                Sliding.Stop();
            }
            Obstacle1.AppliedForce += FrictionVector1;
            Obstacle2.AppliedForce += FrictionVector2;
            if (Obstacle1.Collider.Y <= 0 || Obstacle1.Collider.Y >= 540)
            {
                Obstacle1.Location = new Vector2(Obstacle1.Location.X, (Obstacle1.Collider.Y <= 0 ? 16.0f : 524.0f));
                Obstacle1.Velocity = new Vector2(Obstacle1.Velocity.X, 0 - Obstacle1.Velocity.Y);
                #if !MUTE
                FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Obstacle1.Collider.X <= 0 || Obstacle1.Collider.X >= 960)
            {
                Obstacle1.Location = new Vector2((Obstacle1.Collider.X <= 0 ? 16.0f : 944.0f), Obstacle1.Location.Y);
                Obstacle1.Velocity = new Vector2(0 - Obstacle1.Velocity.X, Obstacle1.Velocity.Y);
                #if !MUTE
                FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Obstacle2.Collider.Y <= 0 || Obstacle2.Collider.Y >= 540)
            {
                Obstacle2.Location = new Vector2(Obstacle2.Location.X, (Obstacle2.Collider.Y <= 0 ? 32.0f : 508.0f));
                Obstacle2.Velocity = new Vector2(Obstacle2.Velocity.X, 0 - Obstacle2.Velocity.Y);
                #if !MUTE
                // FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Obstacle2.Collider.X <= 0 || Obstacle2.Collider.X >= 960)
            {
                Obstacle2.Location = new Vector2((Obstacle2.Collider.X <= 0 ? 32.0f : 928.0f), Obstacle2.Location.Y);
                Obstacle2.Velocity = new Vector2(0 - Obstacle2.Velocity.X, Obstacle2.Velocity.Y);
                #if !MUTE
                // FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Player.TryCollideWith(Obstacle1) && Math.Max(Player.Momentum.Length(), Obstacle1.Momentum.Length()) > 1.5f)
            {
                Player.FullStop();
                #if !MUTE
                FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Player.TryCollideWith(Obstacle2))
            {
                Player.FullStop();
                #if !MUTE
                // FrameResources.Sounds["impact"].Play();
                #endif
            }
            if (Obstacle1.TryCollideWith(Obstacle2) && Math.Max(Obstacle1.Momentum.Length(), Obstacle2.Momentum.Length()) > 1.5f)
            {
                #if !MUTE
                FrameResources.Sounds["impact"].Play();
                #endif
            }
            Player.Velocity = Locomotion;
            Player.Update(gameTime);
            Obstacle1.Update(gameTime);
            Player.TryCollideWith(WallTop);
            Player.TryCollideWith(WallBottom);
            Player.TryCollideWith(WallLeft);
            Player.TryCollideWith(WallRight);
            if (Player.Collider.Y >= 540)
            {
                Sliding.Stop();
                GlobalEventSet.Add(
                    new TimedEvent(
                        gameTime,
                        0.0f,
                        Game1.SetFrameNumber,
                        new FrameShiftEventArgs(
                            -2, null
                        )
                    )
                );
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
            Obstacle1 = null;
            Obstacle2 = null;
            Sliding = null;
        }
    }
}
