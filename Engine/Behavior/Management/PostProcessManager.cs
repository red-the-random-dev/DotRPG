using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Behavior.Management
{
    public class PostProcessManager
    {
        public Random GlitchGen = new Random();

        public Byte LineSkip = 0;
        public Byte LinePaint = 0;
        public Byte LineShift = 0;
        public Byte TintR = 255;
        public Byte TintG = 255;
        public Byte TintB = 255;
        public Byte TintA = 255;

        public Color Tint
        {
            get
            {
                return new Color(TintR, TintG, TintB, TintA);
            }
        }

        public Color RandomMonoColor
        {
            get
            {
                Int32 c = GlitchGen.Next(6);
                switch (c)
                {
                    case 0: return Color.Red;
                    case 1: return Color.Yellow;
                    case 2: return Color.Green;
                    case 3: return Color.Cyan;
                    case 4: return Color.Blue;
                    case 5: return Color.Purple;
                    default: return Color.White;
                }
            }
        }
    }
}
