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
        protected abstract void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 positionOverride, Single turn);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Single turn = 0)
        {
            Draw(gameTime, spriteBatch, drawArea, Vector2.Zero, turn);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 positionOverride, Single turn = 0)
        {
            Single sizeMorph = 1.0f * drawArea.Height / DefaultDrawAreaHeight;
            drawArea = SharedGraphicsMethods.ApplyPadding(drawArea, ElementPadding, sizeMorph);
            DrawElement(gameTime, spriteBatch, drawArea, positionOverride, turn);
            Rectangle newDrawRect = SharedGraphicsMethods.ApplyPadding(SharedGraphicsMethods.FindEmbedDrawArea(drawArea, RelativePosition, RelativeSize), SubnodePadding, sizeMorph);

            foreach (UserInterfaceElement ui in Subnodes)
            {
                Vector2 ogPos = ui.RelativePosition - RotationOrigin;
                Vector2 pos = SharedGraphicsMethods.ToLengthAngle(ogPos);
                pos = new Vector2(pos.X, pos.Y + Rotation + turn);
                pos = SharedGraphicsMethods.FromLengthAngle(pos);
                Single newAngle = turn + Rotation;
                pos -= (pos * (Single)((RelativeSize.Y / RelativeSize.X) * Math.Abs(Math.Sin(newAngle))));
                // Doing some fucking vector magic over here
                Vector2 vector = pos + positionOverride - ogPos;
                ui.Draw(gameTime, spriteBatch, newDrawRect, vector, newAngle);
            }
        }
    }
}
