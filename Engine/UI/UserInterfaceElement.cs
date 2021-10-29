using System;
using DotRPG.Algebra;
using System.Collections.Generic;
using System.Text;
using DotRPG.Construct;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DotRPG.UI
{
    public abstract class UserInterfaceElement
    {
        public readonly HashSet<UserInterfaceElement> Subnodes = new HashSet<UserInterfaceElement>();
        [TABS_Property("rotation", PropertyType.FloatPoint)]
        public Single Rotation { get; set; } = 0.0f;
        [TABS_Property("defaultHeight", PropertyType.Integer)]
        public Int32 DefaultDrawAreaHeight { get; set; }
        [TABS_Property("elementPadding", PropertyType.Vector4)]
        public Vector4 ElementPadding { get; set; } = Vector4.Zero;
        [TABS_Property("subnodePadding", PropertyType.Vector4)]
        public Vector4 SubnodePadding { get; set; } = Vector4.Zero;
        [TABS_Property("relativeSize", PropertyType.Vector2)]
        public Vector2 RelativeSize { get; set; }
        [TABS_Property("relativePos", PropertyType.Vector2)]
        public Vector2 RelativePosition { get; set; }
        [TABS_Property("origin", PropertyType.Vector2)]
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
            drawArea = SharedVectorMethods.ApplyPadding(drawArea, ElementPadding, sizeMorph);
            DrawElement(gameTime, spriteBatch, drawArea, positionOverride, turn);
            Rectangle newDrawRect = SharedVectorMethods.ApplyPadding(SharedVectorMethods.FindEmbedDrawArea(drawArea, RelativePosition, RelativeSize), SubnodePadding, sizeMorph);

            foreach (UserInterfaceElement ui in Subnodes)
            {
                Vector2 ogPos = ui.RelativePosition - RotationOrigin;
                Vector2 pos = SharedVectorMethods.ToLengthAngle(ogPos);
                pos = new Vector2(pos.X, pos.Y + Rotation + turn);
                pos = SharedVectorMethods.FromLengthAngle(pos);
                Single newAngle = turn + Rotation;
                // Doing some fucking vector magic over here
                pos -= (pos * (Single)((RelativeSize.Y / RelativeSize.X) * Math.Abs(Math.Sin(newAngle))));
                Vector2 vector = pos + positionOverride - ogPos;
                ui.Draw(gameTime, spriteBatch, newDrawRect, vector, newAngle);
            }
        }
    }
}
