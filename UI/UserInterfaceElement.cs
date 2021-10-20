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
        public Single Rotation { get; set; } = 0.0f;
        public Int32 DefaultDrawAreaHeight { get; set; }
        public Vector4 ElementPadding { get; set; } = Vector4.Zero;
        public Vector4 SubnodePadding { get; set; } = Vector4.Zero;
        public Vector2 RelativeSize { get; set; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 RotationOrigin { get; set; } = new Vector2(0.5f, 0.5f);
        public abstract void Update(GameTime gameTime);
        protected abstract void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Single turn);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Single turn = 0)
        {
            Single sizeMorph = 1.0f * drawArea.Height / DefaultDrawAreaHeight;
            drawArea = SharedGraphicsMethods.ApplyPadding(drawArea, ElementPadding, sizeMorph);
            DrawElement(gameTime, spriteBatch, drawArea, turn);
            Rectangle newDrawRect = SharedGraphicsMethods.ApplyPadding(SharedGraphicsMethods.FindEmbedDrawArea(drawArea, RelativePosition, RelativeSize), SubnodePadding, sizeMorph);

            foreach (UserInterfaceElement ui in Subnodes)
            {
                ui.Draw(gameTime, spriteBatch, newDrawRect, turn + Rotation);
            }
        }
    }
}
