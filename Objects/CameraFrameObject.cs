using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Objects
{
    public class CameraFrameObject
    {
        public Point Focus;

        public Point GetTopLeftAngle(Point screenSize)
        {
            return new Point(Focus.X - screenSize.X/2, Focus.Y - screenSize.Y/2);
        }
    }
}
