using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.UI
{
    [Construct.TABS_Deployable("blank", Construct.ObjectType.UserInterfaceElement)]
    public class UIBlank : UserInterfaceElement
    {
        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 positionOverride, float turn)
        {
            
        }

        protected override void UpdateElement(GameTime gameTime)
        {
            
        }
    }
}
