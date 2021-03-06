using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Construct;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.UI
{
    [TABS_Deployable("ColorBox", ObjectType.UserInterfaceElement)]
    public class ColorBox : UserInterfaceElement
    {
        [TABS_Property("color", PropertyType.Color)]
        public Color DrawColor = Color.Transparent;

        public ColorBox()
        {

        }

        public ColorBox(Color color, Vector2 relativeSize, Vector2 relativePos, Vector2 origin, Int32 defaultScreenHeight = 540)
        {
            DrawColor = color;
            RelativeSize = relativeSize;
            RelativePosition = relativePos;
            RotationOrigin = origin;
            DefaultDrawAreaHeight = defaultScreenHeight;
        }
        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 offset, float turn)
        {
            Texture2D t2d = new Texture2D(spriteBatch.GraphicsDevice, Math.Max(1,(Int32)(drawArea.Width * RelativeSize.X)), Math.Max(1, (Int32)(drawArea.Height * RelativeSize.Y)));
            Color[] data = new Color[t2d.Width * t2d.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = DrawColor;
            }
            t2d.SetData(data);
            Vector2 loc = new Vector2(drawArea.X + (drawArea.Width * (RelativePosition.X + offset.X)), drawArea.Y + (drawArea.Height * (RelativePosition.Y + offset.Y)));

            spriteBatch.Draw(
               t2d, loc,
                new Rectangle(0, 0, t2d.Width, t2d.Height),
                // null,
                Color.White,
                Rotation+turn,
                new Vector2(t2d.Width * RotationOrigin.X, t2d.Height * RotationOrigin.Y),
                1.0f,
                SpriteEffects.None,
                1.0f
            );
        }

        protected override void UpdateElement(GameTime gameTime)
        {
            
        }
    }
}
