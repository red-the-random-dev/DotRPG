using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace DotRPG.Objects
{
    public class ResourceHeap : IDisposable
    {
        public ResourceHeap Global = null;
        public readonly Boolean IsGlobal = false;
        // TODO: Add support for more content types
        public Dictionary<String, Texture2D> Textures = new Dictionary<String, Texture2D>();
        public Dictionary<String, SoundEffect> Sounds = new Dictionary<String, SoundEffect>();
        public Dictionary<String, SpriteFont> Fonts = new Dictionary<String, SpriteFont>();
        public Dictionary<String, Song> Music = new Dictionary<String, Song>();

        public ResourceHeap(ResourceHeap globalResourceReference)
        {
            Global = globalResourceReference;
        }

        public ResourceHeap()
        {
            IsGlobal = true;
        }

        public void Dispose()
        {
            Textures.Clear();
            Sounds.Clear();
            Fonts.Clear();
            Music.Clear();
        }
    }
}
