using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DotRPG.UI
{
    public abstract class UserInterfaceElement
    {
        public readonly HashSet<UserInterfaceElement> Subnodes = new HashSet<UserInterfaceElement>();
        Single Rotation { get; set; } = 0.0f;
        public Int32 DefaultDrawAreaHeight { get; set; }
        public Vector2 RelativeSize { get; set; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 RotationOrigin { get; set; } = new Vector2(0.5f, 0.5f);
        public abstract void Update(GameTime gameTime);
        protected abstract void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Single turn);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Single turn = 0)
        {
            DrawElement(gameTime, spriteBatch, drawArea, turn);
            Rectangle newDrawRect = SharedGraphicsMethods.FindEmbedDrawArea(drawArea, RelativePosition, RelativeSize);

            foreach (UserInterfaceElement ui in Subnodes)
            {
                ui.Draw(gameTime, spriteBatch, newDrawRect, turn + Rotation);
            }
        }
    }
}
