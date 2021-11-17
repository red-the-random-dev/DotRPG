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

        public void Draw(SpriteBatch sb, Int32 VirtualVSize, Point drawOffset, Point drawSize, Rectangle aov, Color drawColor)
        {
            Single transform = 1.0f * drawSize.Y / VirtualVSize;
            Rectangle sproot = new Rectangle((int)(Position.X-Sprite.Width/2), (int)(Position.Y - Sprite.Height / 2), Sprite.Width, Sprite.Height);
            Vector2 origin = new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f);
            Rectangle spront = Algebra.SharedRectangleMethods.CutInto(sproot, aov);
            Algebra.SharedRectangleMethods.GetSizeDifference(sproot, spront, out int dx, out int dy, out int dw, out int dh);
            origin = new Vector2(origin.X - dx, origin.Y - dy);
            sb.Draw
            (
                Sprite, new Vector2
                (
                    Position.X * transform - drawOffset.X,
                    Position.Y * transform - drawOffset.Y
                ),
                new Rectangle(dx, dy, Sprite.Width + dw, Sprite.Height + dh),
                // null,
                drawColor,
                0,
                origin,
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
