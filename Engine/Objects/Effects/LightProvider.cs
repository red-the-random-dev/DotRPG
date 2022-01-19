using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OBJ_SET = System.Collections.Generic.Dictionary<string, DotRPG.Objects.Dynamics.DynamicObject>;
using DYNL_SET = System.Collections.Generic.HashSet<DotRPG.Objects.Effects.LightEmitter>;

namespace DotRPG.Objects.Effects
{
    public static class LightProvider
    {
        public static void Draw(GraphicsDevice gd, CameraFrameObject camera, OBJ_SET objs, DYNL_SET lights, Byte step, Single resize, Point screenScale, Texture2D source, RenderTarget2D destination, RenderTarget2D fallback = null)
        {
            Vector2 topLeft = camera.GetTopLeftAngle(screenScale).ToVector2();
            gd.SetRenderTarget(destination);
            SpriteBatch sb = new SpriteBatch(gd);
            sb.Begin();

            for (int x = 0; x < destination.Width; x += step)
            {
                for (int y = 0; y < destination.Height; y += step)
                {
                    Color c = Color.Black;
                    Vector2 drawPoint = topLeft + (new Vector2(x, y) / resize);
                    foreach (LightEmitter le in lights)
                    {
                        Color n = le.Retrieve(drawPoint, objs[le.AssociatedObject].Location, out Single br);
                        if (br > 0)
                        {
                            c.Deconstruct(out Byte r1, out Byte g1, out Byte b1);
                            n.Deconstruct(out Byte r2, out Byte g2, out Byte b2);
                            c = new Color(Math.Max(r1, r2), Math.Max(g1, g2), Math.Max(b1, b2));
                        }
                    }
                    sb.Draw(source, new Vector2(x, y), new Rectangle(x, y, step, step), c);
                }
            }
            sb.End();
            gd.SetRenderTarget(fallback);
        }
    }
}
