using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Behavior;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    internal class SettingsFrame : Frame
    {
        Int32 Index = 0;
        SpriteFont Font;
        public SByte FramerateOption = 1;
        public Boolean TurnOnVSync = true;

        public override int FrameID
        {
            get
            {
                return -128;
            }
        }

        public SettingsFrame(SpriteFont font, Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {
            Font = font;
        }

        public override void Update(GameTime gameTime, ControlInput controls)
        {
            if (controls.KeyPressed(0))
            {
                Index--;
                Index = Math.Clamp(Index, 0, 1);
            }
            else if (controls.KeyPressed(1))
            {
                Index++;
                Index = Math.Clamp(Index, 0, 1);
            }
            else if (controls.KeyPressed(2))
            {
                switch (Index)
                {
                    case 0:
                        FramerateOption = (SByte)Math.Clamp(FramerateOption - 1, 0, 2);
                        break;
                    case 1:
                        TurnOnVSync = false;
                        break;
                }
            }
            else if (controls.KeyPressed(3))
            {
                switch (Index)
                {
                    case 0:
                        FramerateOption = (SByte)Math.Clamp(FramerateOption + 1, 0, 2);
                        break;
                    case 1:
                        TurnOnVSync = true;
                        break;
                }
            }
            base.Update(gameTime, controls);
        }

        public override void Initialize()
        {

        }

        public override void Draw(GameTime gameTime, GraphicsDevice gd, Rectangle drawZone)
        {
            Vector2 Measure = Font.MeasureString("@");
            SpriteBatch spriteBatch = new SpriteBatch(gd);
            spriteBatch.Begin();
            Texture2D t = new Texture2D(spriteBatch.GraphicsDevice, drawZone.Width, Measure.ToPoint().Y);
            Color[] data = new Color[drawZone.Width * Measure.ToPoint().Y];
            for (int i = 0; i < data.Length; i++) data[i] = Color.White;
            t.SetData(data);
            spriteBatch.Draw(t, drawZone, new Color(0, 0, 0, 100));
            spriteBatch.DrawString(Font, "Settings menu\nPress X to exit", Measure, Color.White);
            Vector2 Measure_h = new Vector2(Measure.X, 0);
            Vector2 Measure_v = new Vector2(0, Measure.Y);
            Vector2 ListOffset = new Vector2(0, (Measure_v * 4).Y);
            
            spriteBatch.Draw(t, ListOffset + (Index != 0 ? Measure_v : Vector2.Zero), new Rectangle(0, 0, t.Width, t.Height), Color.Blue, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(Font, String.Format("  Max framerate        {0} {1} {2}", Index == 0 && FramerateOption > 0 ? "<" : " ", 30 << FramerateOption, Index == 0 && FramerateOption < 2 ? ">" : " "), ListOffset, Color.White);
            spriteBatch.DrawString(Font, String.Format("  Sync to monitor rate {0} {1} {2}", Index == 1 && TurnOnVSync ? "<" : " ", TurnOnVSync ? "Yes" : "No", Index == 1 && !TurnOnVSync ? ">" : " "), ListOffset + Measure_v, Color.White);
            spriteBatch.End();
        }

        public override void LoadContent(GraphicsDevice gd)
        {

        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void UnloadContent()
        {

        }
    }
}
