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

        public Vector2 RelativeSize { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 RotationOrigin { get; set; } = new Vector2(0.5f, 0.5f);
        public abstract void DrawElement(SpriteBatch spriteBatch, Rectangle drawArea);
        public void Draw(SpriteBatch spriteBatch, Rectangle drawArea)
        {
            DrawElement(spriteBatch, drawArea);

            foreach (UserInterfaceElement ui in Subnodes)
            {
                
            }
        }
    }
}
