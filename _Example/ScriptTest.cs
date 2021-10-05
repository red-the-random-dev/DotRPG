using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using DotRPG.Objects;
using DotRPG.Objects.Dynamics;
using DotRPG.Scripting;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    class ScriptTest : Frame
    {
        LuaModule DialogTest1;
        TextObject DialogForm;
        SceneSwitchSet SceneSwitches = new SceneSwitchSet();
        PlayerObject Player;
        DynamicRectObject dro;
        Boolean[] lastInputCollection = new bool[8];
        Boolean ShowingText;
        CameraFrameObject cam = new CameraFrameObject();

        public override int FrameID
        {
            get
            {
                return 2;
            }
        }

        public ScriptTest(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {

        }

        public override void Initialize()
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            spriteBatch.Draw(FrameResources.Textures["backdrop"], Vector2.Zero - cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)).ToVector2(), new Rectangle(0, 0, 960, 540), Color.White, 0, Vector2.Zero, (drawZone.Height / 540), SpriteEffects.None, 1.0f);
            dro.Draw(spriteBatch, gameTime, 540, cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(drawZone.Width, drawZone.Height), (0.3f - (0.1f * (dro.Location.Y / 540))));
            Player.Draw(spriteBatch, gameTime, 540, cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(drawZone.Width, drawZone.Height), (0.3f - (0.1f * (Player.Location.Y / 540))));
            if (ShowingText)
            {
                Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, drawZone.Width, drawZone.Height / 5);
                Color[] data = new Color[drawZone.Width * drawZone.Height / 5];
                for (int i = 0; i < data.Length; i++) data[i] = Color.Black;
                rect.SetData(data);
                Vector2 textBoxLocation = new Vector2(0, drawZone.Height * 4 / 5);
                spriteBatch.Draw(rect, textBoxLocation, Color.White);
                DialogForm.Draw(spriteBatch, Owner.Window);
            }
#if DEBUG
            spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], String.Format("Sight: {0}, Direction: {1}, AnimFrame: {2}", Player.SightArea, Player.SightDirection.ToString().ToLower(), Player.Sprite.SpriteIndex), new Vector2(0, 12), Color.White);
#endif
        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            Single loco_x = 0.0f; Single loco_y = 0.0f;
            if (controls[0]) { loco_y -= 1.0f; }
            if (controls[1]) { loco_y += 1.0f; }
            if (controls[2]) { loco_x -= 1.0f; }
            if (controls[3]) { loco_x += 1.0f; }
            Vector2 Locomotion = new Vector2(loco_x, loco_y);
            if (controls[0] && !(controls[1] || controls[2] || controls[3]))
            {
                Player.SightDirection = Direction.Up;
            }
            if (controls[1] && !(controls[0] || controls[2] || controls[3]))
            {
                Player.SightDirection = Direction.Down;
            }
            if (controls[2] && !(controls[1] || controls[0] || controls[3]))
            {
                Player.SightDirection = Direction.Left;
            }
            if (controls[3] && !(controls[1] || controls[0] || controls[2]))
            {
                Player.SightDirection = Direction.Right;
            }
            String newAnimSequence = String.Format("red.idle.{0}", Player.SightDirection.ToString().ToLower());
            if (Player.Sprite.CurrentAnimationSequence != newAnimSequence)
            {
                Player.Sprite.SetAnimationSequence(newAnimSequence);
            }
            Locomotion /= (Locomotion.Length() != 0 ? Locomotion.Length() : 1.0f);
            Locomotion *= 256f;
            if (ShowingText)
            {
                Locomotion = Vector2.Zero;
            }
            Player.Velocity = Locomotion;
            Player.TryCollideWith(dro);
            if (controls[4] && !lastInputCollection[4] || (ShowingText && DialogForm.ReachedEnd && SceneSwitches.AutoScroll))
            {
                if (Player.SightArea.Intersects(dro.Collider) && !ShowingText)
                {
                    SceneSwitches.AutoScroll = false;
                    SceneSwitches.ExitDialog = false;
                    ShowingText = true;
                    DialogTest1.Update(0, (float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds);
                    DialogForm.ResetToStart();
                }
                else if (DialogForm.ReachedEnd)
                {
                    if (SceneSwitches.ExitDialog)
                    {
                        ShowingText = false;
                    }
                    else
                    {
                        SceneSwitches.AutoScroll = false;
                        SceneSwitches.ExitDialog = false;
                        DialogTest1.Update(0, (float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds);
                        DialogForm.ResetToStart();
                    }
                }
                
            }
            if (ShowingText)
            {
                DialogForm.Update(gameTime);
            }
            Player.Update(gameTime);
            base.Update(gameTime, controls);
            for (int i = 0; i < Math.Min(lastInputCollection.Length, controls.Length); i++)
            {
                lastInputCollection[i] = controls[i];
            }
            cam.Focus = Player.Location.ToPoint();
        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            FrameResources.Textures.Add("backdrop", Owner.Content.Load<Texture2D>("Texture2D/backdrop"));
            DialogForm = new TextObject(FrameResources.Global.Fonts["vcr_large"], "...", 0.01f, 0.80f, Color.White, AlignMode.TopLeft, 1080, scrollPerTick: 1, scrollDelay: 0.04f);
            DialogTest1 = new LuaModule(System.IO.File.ReadAllText(System.IO.Path.Join(Owner.Content.RootDirectory, "Scripts/dialog.lua")));
            DialogTest1.Runtime["dialog"] = DialogForm;
            DialogTest1.Runtime["scene"] = SceneSwitches;
            Player = new PlayerObject(new Point(32, 64), new Point(32, 32), 20.0f, new Point(64, 64));
            dro = new DynamicRectObject(new Point(128, 128), new Point(32, 32), 30.0f, true);
            FrameResources.Textures.Add("red.idle.down", Owner.Content.Load<Texture2D>("Texture2D/red.idle.down"));
            FrameResources.Textures.Add("red.idle.up", Owner.Content.Load<Texture2D>("Texture2D/red.idle.up"));
            FrameResources.Textures.Add("red.idle.right", Owner.Content.Load<Texture2D>("Texture2D/red.idle.right"));
            FrameResources.Textures.Add("red.idle.left", Owner.Content.Load<Texture2D>("Texture2D/red.idle.left"));
            Player.Sprite = new SpriteController(1000 / 6.0f, FrameResources.Textures["red.idle.down"], 12);
            Player.Sprite.AddAnimationSequence("red.idle.down", FrameResources.Textures["red.idle.down"], 12);
            Player.Sprite.AddAnimationSequence("red.idle.up", FrameResources.Textures["red.idle.up"], 1);
            Player.Sprite.AddAnimationSequence("red.idle.left", FrameResources.Textures["red.idle.left"], 12);
            Player.Sprite.AddAnimationSequence("red.idle.right", FrameResources.Textures["red.idle.right"], 12);
            FrameResources.Textures.Add("cube-o", Owner.Content.Load<Texture2D>("Texture2D/cube-o"));
            dro.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-o"]);
        }

        public override void UnloadContent()
        {
            DialogForm = null;
            FrameResources.Dispose();
        }
    }
}
