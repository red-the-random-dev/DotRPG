using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior
{
    public enum ResourceType
    {
        Texture2D, SoundEffect, SpriteFont, Song, Effect, MGFX
    }

    public enum ResourceLocation
    {
        ContentFolder, AssemblyResources
    }

    public struct ResourceLoadTask
    {
        public String ResourceID;
        public String ResourcePath;
        public ResourceType Resource;
        public ResourceLocation LoadFrom;
    }
}
