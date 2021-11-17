using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Algebra
{
    public class SharedRectangleMethods
    {
        public static Rectangle GetFromOrigin(Vector2 position, Vector2 origin, Vector2 size)
        {
            Point boxSize = size.ToPoint();
            Point topLeft = (position - new Vector2(size.X * origin.X, size.Y * origin.Y)).ToPoint();
            return new Rectangle
            (
                topLeft.X, topLeft.Y, boxSize.X, boxSize.Y
            );
        }

        public static Rectangle CutInto(Rectangle one, Rectangle two)
        {
            Int32 corner_x = one.X;
            Int32 corner_y = one.Y;
            Int32 w = one.Width;
            Int32 h = one.Height;
            if (!one.Intersects(two))
            {
                return new Rectangle(0, 0, 0, 0);
            }
            if (one.Left < two.Left)
            {
                corner_x += Math.Abs(two.Left - one.Left);
                w -= Math.Abs(two.Left - one.Left);
            }
            if (one.Right > two.Right)
            {
                w -= Math.Abs(one.Right - two.Right);
            }
            if (one.Top < two.Top)
            {
                corner_y += Math.Abs(two.Top - one.Top);
                h -= Math.Abs(two.Top - one.Top);
            }
            if (one.Bottom > two.Bottom)
            {
                h -= Math.Abs(one.Top - two.Top);
            }
            return new Rectangle(corner_x, corner_y, w, h);
        }
        public static void GetSizeDifference(Rectangle one, Rectangle two, out Int32 dx, out Int32 dy, out Int32 dw, out Int32 dh)
        {
            dx = two.X - one.X; dy = two.Y - one.Y; dw = two.Width - one.Width; dh = two.Height - one.Height;
        }
    }
}
