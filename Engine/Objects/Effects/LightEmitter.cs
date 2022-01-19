using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects.Effects
{
    public class LightEmitter
    {
        public String AssociatedObject;
        public Color EmitterColor;
        public Effect EmitterShader;
        public Single Range;

        public virtual Single GetBrightnessIn(Vector2 destination, Vector2 source)
        {
            return Math.Clamp((Range - (destination - source).Length()) / Range, 0.0f, 1.0f);    
        }

        public Color Retrieve(Vector2 destination, Vector2 source, out Single bright)
        {
            bright = GetBrightnessIn(destination, source);
            return EmitterColor * bright;
        }
    }
}
