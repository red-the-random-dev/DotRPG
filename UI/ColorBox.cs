using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.UI
{
    public class ColorBox : UserInterfaceElement
    {
        public Color DrawColor;

        public ColorBox(Color color, Vector2 relativeSize, Vector2 relativePos, Vector2 origin, Int32 defaultScreenHeight = 540)
        {
            DrawColor = color;
            RelativeSize = relativeSize;
            RelativePosition = relativePos;
            RotationOrigin = origin;
            DefaultDrawAreaHeight = defaultScreenHeight;
        }
        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, float turn)
        {
            Texture2D t2d = new Texture2D(spriteBatch.GraphicsDevice, Math.Max(1,(Int32)(drawArea.Width * RelativeSize.X)), Math.Max(1, (Int32)(drawArea.Height * RelativeSize.Y)));
            Color[] data = new Color[t2d.Width * t2d.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = DrawColor;
            }
            t2d.SetData(data);
            Vector2 loc = new Vector2(drawArea.X + (drawArea.Width * RelativePosition.X), drawArea.Y + (drawArea.Height * RelativePosition.Y));

            spriteBatch.Draw(
               t2d, loc,
                new Rectangle(0, 0, t2d.Width, t2d.Height),
                // null,
                Color.White,
                0,
                new Vector2(t2d.Width * RotationOrigin.X, t2d.Height * RotationOrigin.Y),
                1.0f,
                SpriteEffects.None,
                1.0f
            );
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
