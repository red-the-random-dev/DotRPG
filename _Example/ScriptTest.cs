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
            Player.Draw(spriteBatch, gameTime, 540, new Point(0, 0), new Point(drawZone.Width, drawZone.Height));
            dro.Draw(spriteBatch, gameTime, 540, new Point(0, 0), new Point(drawZone.Width, drawZone.Height));
            if (ShowingText)
            {
                DialogForm.Draw(spriteBatch, Owner.Window);
            }
#if DEBUG
            spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], String.Format("Sight: {0}, Z key: {1}", Player.SightArea, lastInputCollection[4]), new Vector2(0, 12), Color.White);
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
            Locomotion /= (Locomotion.Length() != 0 ? Locomotion.Length() : 1.0f);
            Locomotion *= 0.1f;
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
                    DialogTest1.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds);
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
                        DialogTest1.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds);
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
        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            DialogForm = new TextObject(FrameResources.Global.Fonts["vcr_large"], "...", 0.01f, 0.80f, Color.White, AlignMode.TopLeft, 1080, scrollPerTick: 1, scrollDelay: 0.04f);
            DialogTest1 = new LuaModule(System.IO.File.ReadAllText(System.IO.Path.Join(Owner.Content.RootDirectory, "Scripts/dialog.lua")));
            DialogTest1.Runtime["dialog"] = DialogForm;
            DialogTest1.Runtime["scene"] = SceneSwitches;
            Player = new PlayerObject(new Point(32, 32), new Point(32, 32), 20.0f, new Point(32, 8));
            dro = new DynamicRectObject(new Point(32, 128), new Point(32, 32), 30.0f, true);
            FrameResources.Textures.Add("cube-p", Owner.Content.Load<Texture2D>("Texture2D/cube-p"));
            Player.Sprite = new SpriteController(1000 / 60.0f, FrameResources.Textures["cube-p"]);
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
