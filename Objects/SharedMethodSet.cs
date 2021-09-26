using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects
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
    public static class SharedMethodSet
    {
        public static Vector2 FindTextAlignment(SpriteFont sf, String line, Rectangle scr, Single AlignX = 0.5f, Single AlignY = 0.5f, AlignMode anchor = AlignMode.Center, Single rescale = 1.0f)
        {
            return FindTextAlignment(sf, line, scr, AlignX, AlignY, Vector2.Zero, anchor, rescale);
        }

        public static Vector2 FindTextAlignment(SpriteFont sf, String line, Rectangle scr, Single AlignX, Single AlignY, Vector2 InitialFieldOffset, AlignMode anchor = AlignMode.Center, Single rescale = 1.0f)
        {
            Vector2 str = sf.MeasureString(line);
            Int32 IntegerAnchorValue = (int)anchor;
            Single x = InitialFieldOffset.X + scr.Width * AlignX;
            Single y = InitialFieldOffset.Y + scr.Height * AlignY;
            switch (IntegerAnchorValue / 3)
            {
                case 0:
                    x -= (str.X / 2) * rescale;
                    break;
                case 2:
                    x += (str.X / 2) * rescale;
                    break;
            }
            switch (IntegerAnchorValue % 3)
            {
                case 0:
                    y -= (str.Y / 2) * rescale;
                    break;
                case 2:
                    y += (str.Y / 2) * rescale;
                    break;
            }
            return new Vector2(x, y);
        }
    }
}
