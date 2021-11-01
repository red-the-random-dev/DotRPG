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
        [Construct.TABS_Property("aspectRatio", Construct.PropertyType.FloatPoint)]
        public Single AspectRatio { get; set; } = 0.0f;

        public UIBlank()
        {

        }
        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawArea, Vector2 positionOverride, float turn)
        {
            if (AspectRatio != 0.0f)
            {
                Single newWidth = drawArea.Height * AspectRatio * RelativeSize.Y;
                Single newRelativeWidth = newWidth / drawArea.Width;
                RelativeSize = new Vector2(newRelativeWidth, RelativeSize.Y);
            }
        }

        protected override void UpdateElement(GameTime gameTime)
        {
            
        }
    }
}
