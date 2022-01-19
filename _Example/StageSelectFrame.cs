using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Behavior;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    class StageSelectFrame : Frame
    {
        String[] OptionsList;
        String WelcomeMessage;
        Int32 Index = 0;
        Boolean[] lastInput = new bool[8];
        SpriteFont Font;

        public Int32 SelectedOption = -1;

        public override int FrameID
        {
            get
            {
                return -128;
            }
        }

        public StageSelectFrame(String welcomeMsg, String[] optionsList, SpriteFont font, Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {
            Font = font;
            OptionsList = optionsList;
            WelcomeMessage = welcomeMsg;
        }

        public override void Update(GameTime gameTime, ControlInput controls)
        {
            if (controls.KeyPressed(0))
            {
                Index--;
            }
            else if (controls.KeyPressed(1))
            {
                Index++;
            }
            else if (controls.KeyPressed(4))
            {
                SelectedOption = Index;
            }
            Index = Math.Max(0, Index);
            Index = Math.Min(OptionsList.Length-1, Index);
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
            for (int i = 0; i < data.Length; i++) data[i] = Color.Blue;
            t.SetData(data);
            spriteBatch.DrawString(Font, WelcomeMessage, Measure, Color.White);
            Vector2 Measure_h = new Vector2(Measure.X, 0);
            Vector2 Measure_v = new Vector2(0, Measure.Y);
            Vector2 ListOffset = new Vector2(0, (Font.MeasureString(WelcomeMessage) + (Measure_v * 3)).Y);
            for (int i = 0; i < OptionsList.Length; i++)
            {
                if (i == Index)
                {
                    spriteBatch.Draw(t, ListOffset + (Measure_v * i), new Rectangle(0, 0, t.Width, t.Height), Color.Blue, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                }
                spriteBatch.DrawString(Font, OptionsList[i], ListOffset + (Measure_v * i) + (Measure_h * 2), i == Index ? Color.Yellow : Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
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
