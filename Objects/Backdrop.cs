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
        public Vector2 TopLeftPosition;

        public void Draw(SpriteBatch sb, Point drawOffset, Single transform)
        {
            sb.Draw(Sprite, TopLeftPosition * transform - drawOffset.ToVector2(), new Rectangle(0, 0, Sprite.Width, Sprite.Height), Color.White, 0, Vector2.Zero, transform, SpriteEffects.None, 1.0f);
        }

        public Backdrop(Texture2D sprite, Vector2 topLeft)
        {
            Sprite = sprite;
            TopLeftPosition = topLeft;
        }
    }
}
