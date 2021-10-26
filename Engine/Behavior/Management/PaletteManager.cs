using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class PaletteManager : IDisposable
    {
        public readonly Dictionary<String, String> ObjectColors = new Dictionary<string, string>();
        public readonly Dictionary<String, Color> Channels = new Dictionary<string, Color>();
        public readonly Dictionary<String, Boolean> MixWithGlobal = new Dictionary<string, bool>();
        
        public void Dispose()
        {
            ObjectColors.Clear();
            Channels.Clear();
            MixWithGlobal.Clear();
        }

        public PaletteManager()
        {
            Channels.Add("global", Color.White);
            MixWithGlobal.Add("global", false);
        }

        public Color GetColor(String channel)
        {
            Color x = Channels[channel];
            if (MixWithGlobal[channel])
            {
                GetColorMask(GetColor("global"), out Single r2, out Single g2, out Single b2, out Single a2);
                GetColorMask(x, out Single r1, out Single g1, out Single b1, out Single a1);
                x = new Color(new Vector4(r1 * r2, g1 * g2, b1 * b2, a1 * a2));
            }
            return x;
        }

        public void SetColor(String channel, Byte r, Byte g, Byte b, Byte a = 255)
        {
            if (!Channels.ContainsKey(channel)) Channels.Add(channel, Color.White);
            if (!MixWithGlobal.ContainsKey(channel)) MixWithGlobal.Add(channel, true);
            Channels[channel] = new Color(r, g, b, a);
        }

        public void SetColor(String channel, Vector4 color)
        {
            if (!Channels.ContainsKey(channel)) Channels.Add(channel, Color.White);
            if (!MixWithGlobal.ContainsKey(channel)) MixWithGlobal.Add(channel, true);
            Channels[channel] = new Color(color);
        }

        public void SetObjectChannel(String obj, String ch)
        {
            if (!ObjectColors.ContainsKey(obj)) ObjectColors.Add(obj, "global");
            ObjectColors[obj] = ch;
        }

        public void GetColorMask(Color color, out Single r, out Single g, out Single b, out Single a)
        {
            r = 1.0f * color.R / 255.0f;
            g = 1.0f * color.G / 255.0f;
            b = 1.0f * color.B / 255.0f;
            a = 1.0f * color.A / 255.0f;
        }

        public void SetMixWithGlobal(String channel, Boolean mix)
        {
            MixWithGlobal[channel] = mix;
        }

        public Color GetObjectColor(String obj)
        {
            return GetColor(ObjectColors[obj]);
        }

        public String GetObjectColorChannel(String obj)
        {
            return ObjectColors[obj];
        }

        public Byte R(String channel)
        {
            return GetColor(channel).R;
        }

        public Byte G(String channel)
        {
            return GetColor(channel).G;
        }

        public Byte B(String channel)
        {
            return GetColor(channel).B;
        }

        public Byte A(String channel)
        {
            return GetColor(channel).A;
        }
    }
}
