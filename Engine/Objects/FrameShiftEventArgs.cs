using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects
{
    public class FrameShiftEventArgs : EventArgs
    {
        public Int32 FrameID;
        public Point PlayerLocation;
        public Boolean IncludesPlayerLocation = true;

        public FrameShiftEventArgs(Int32 id, Point? playerLocation)
        {
            FrameID = id;
            if (playerLocation != null)
            {
                PlayerLocation = (Point)playerLocation;
            }
            else
            {
                IncludesPlayerLocation = false;
            }
        }
    }
}
