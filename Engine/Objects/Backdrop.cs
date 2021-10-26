using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Objects
{
    public class Backdrop
    {
        public Texture2D Sprite;
        public Vector2 Position;

        public void Draw(SpriteBatch sb, Int32 VirtualVSize, Point drawOffset, Point drawSize, Color drawColor)
        {
            Single transform = 1.0f * drawSize.Y / VirtualVSize;
            sb.Draw
            (
                Sprite, new Vector2
                (
                    Position.X * transform - drawOffset.X,
                    Position.Y * transform - drawOffset.Y
                ),
                new Rectangle(0, 0, Sprite.Width, Sprite.Height),
                // null,
                drawColor,
                0,
                new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f),
                transform,
                SpriteEffects.None,
                1.0f
            );
        }

        public Backdrop(Texture2D sprite, Vector2 topLeft)
        {
            Sprite = sprite;
            Position = topLeft;
        }
    }
}
