using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.UI
{
    public class ProgressBar : UserInterfaceElement
    {
        [Construct.TABS_Property("fgColor", Construct.PropertyType.Color)]
        public Color ForegroundColor;
        [Construct.TABS_Property("bgColor", Construct.PropertyType.Color)]
        public Color BackgroundColor;
        Single _p = 0.0f;
        public Single Progress_Percentage
        {
            get
            {
                return 100.0f * _p;
            }
            set
            {
                _p = Math.Max(0.0f, Math.Min(1.0f, value / 100.0f));
            }
        }

        public ProgressBar()
        {

        }

        public ProgressBar(Color fgColor, Color ? bgColor, Vector2 relativeSize, Vector2 relativePosition, Int32 DefaultScreenHeight = 540)
        {
            ForegroundColor = fgColor;
            BackgroundColor = bgColor ?? Color.Transparent;
            RelativeSize = relativeSize;
            RelativePosition = relativePosition;
            DefaultDrawAreaHeight = DefaultScreenHeight;
        }

        protected override void UpdateElement(GameTime gameTime)
        {
            
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 offset, float turn)
        {
            Vector2 v = new Vector2(drawArea.X + (drawArea.Width * (RelativePosition.X + offset.X)), drawArea.Y + (drawArea.Height * (RelativePosition.Y + offset.Y)));
            Texture2D t1 = new Texture2D(spriteBatch.GraphicsDevice, (Int32)(drawArea.Width * RelativeSize.X), (Int32)Math.Ceiling(drawArea.Height * RelativeSize.Y));
            Texture2D t2 = new Texture2D(spriteBatch.GraphicsDevice, Math.Max((Int32)(t1.Width * _p), 1), t1.Height);
            
            Color[] d1 = new Color[t1.Width * t1.Height];
            Color[] d2 = new Color[t2.Width * t2.Height];
            
            for (int i = 0; i < d1.Length; i++) d1[i] = BackgroundColor;
            for (int i = 0; i < d2.Length; i++) d2[i] = ForegroundColor;

            t1.SetData(d1);
            t2.SetData(d2);

            spriteBatch.Draw(t1, v, new Rectangle(0, 0, t1.Width, t1.Height), Color.White, Rotation + turn, new Vector2(t1.Width * RotationOrigin.X, t1.Height * RotationOrigin.Y), 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(t2, v, new Rectangle(0, 0, t2.Width, t2.Height), Color.White, Rotation + turn, new Vector2(t1.Width * RotationOrigin.X, t1.Height * RotationOrigin.Y), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
