using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.UI
{
    public enum AlignMode
    {
        Center = 0,
        TopCenter = 1,
        BottomCenter = 2,

        CenterLeft = 3,
        TopLeft = 4,
        BottomLeft = 5,

        CenterRight = 6,
        TopRight = 7,
        BottomRight = 8,
    }
    public static class SharedGraphicsMethods
    {
        public static Vector2 FindTextAlignment(SpriteFont sf, String line, Rectangle scr, Single AlignX = 0.5f, Single AlignY = 0.5f, AlignMode anchor = AlignMode.Center, Single rescale = 1.0f)
        {
            return FindTextAlignment(sf, line, scr, AlignX, AlignY, Vector2.Zero, anchor, rescale);
        }

        public static Vector2 FindRelativeOrigin(AlignMode alignMode)
        {
            Single x = 0.5f;
            Single y = 0.5f;
            Int32 IntegerAnchorValue = (int)alignMode;

            switch (IntegerAnchorValue / 3)
            {
                case 1:
                    x = 0.0f;
                    break;
                case 2:
                    x = 1.0f;
                    break;
            }
            switch (IntegerAnchorValue % 3)
            {
                case 1:
                    y = 0.0f;
                    break;
                case 2:
                    y = 1.0f;
                    break;
            }
            return new Vector2(x, y);
        }

        public static Rectangle FindEmbedDrawArea(Rectangle drawArea, Vector2 offset, Vector2 resize)
        {
            Int32 newWidth = (Int32)Math.Ceiling(drawArea.Width * resize.X);
            Int32 newHeight = (Int32)Math.Ceiling(drawArea.Height * resize.Y);

            Int32 shrinkWidth = drawArea.Width - newWidth;
            Int32 shrinkHeight = drawArea.Height - newHeight;

            return new Rectangle
            (
                drawArea.X + (Int32)Math.Ceiling(shrinkWidth * offset.X),
                drawArea.Y + (Int32)Math.Ceiling(shrinkHeight * offset.Y),
                newWidth, newHeight
            );
        }

        public static Vector2 FindTextAlignment(SpriteFont sf, String line, Rectangle scr, Single AlignX, Single AlignY, Vector2 InitialFieldOffset, AlignMode anchor = AlignMode.Center, Single rescale = 1.0f)
        {
            Vector2 str = sf.MeasureString(line) * rescale;
            Int32 IntegerAnchorValue = (int)anchor;
            Single x = InitialFieldOffset.X + (scr.Width * AlignX);
            Single y = InitialFieldOffset.Y + (scr.Height * AlignY);
            switch (IntegerAnchorValue / 3)
            {
                case 0:
                    x -= (str.X / 2);
                    break;
                case 2:
                    x += (str.X / 2);
                    break;
            }
            switch (IntegerAnchorValue % 3)
            {
                case 0:
                    y -= (str.Y / 2);
                    break;
                case 2:
                    y += (str.Y / 2);
                    break;
            }
            return new Vector2(x, y);
        }
    }
}
