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
        public static void Draw(GraphicsDevice gd, CameraFrameObject camera, OBJ_SET objs, DYNL_SET lights, Effect finalize, Single resize, Point screenScale, Texture2D source, RenderTarget2D destination, RenderTarget2D fallback = null)
        {
            Vector2 topLeft = camera.GetTopLeftAngle(screenScale).ToVector2();
            SpriteBatch sb = new SpriteBatch(gd);

            /*
             * gd.SetRenderTarget(destination);
             * sb.Begin();
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
            */
            RenderTarget2D intermediate = new RenderTarget2D(gd, destination.Width, destination.Height);
            RenderTarget2D lightMask = new RenderTarget2D(gd, destination.Width, destination.Height);
            foreach (LightEmitter le in lights)
            {
                Effect sh = le.EmitterShader;
                if (sh == null)
                {
                    continue;
                }
                gd.SetRenderTarget(intermediate);
                sb.Begin();
                sh.Parameters["source"].SetValue(objs[le.AssociatedObject].Location);
                sh.Parameters["topLeft"].SetValue(topLeft);
                sh.Parameters["distResize"].SetValue(resize);
                sh.Parameters["range"].SetValue(le.Range);
                sh.Parameters["source_c"].SetValue(le.EmitterColor.ToVector4());
                sh.CurrentTechnique.Passes[0].Apply();
                sb.Draw(lightMask, Vector2.Zero, new Rectangle(0, 0, intermediate.Width, intermediate.Height), Color.White);
                sb.End();
                gd.SetRenderTarget(lightMask);
                sb.Begin();
                sb.Draw(intermediate, Vector2.Zero, new Rectangle(0, 0, intermediate.Width, intermediate.Height), Color.White);
                sb.End();
            }
            gd.SetRenderTarget(destination);
            finalize.Parameters["level"].SetValue(source);
            sb.Begin();
            finalize.CurrentTechnique.Passes[0].Apply();
            sb.Draw(lightMask, Vector2.Zero, new Rectangle(0, 0, intermediate.Width, intermediate.Height), Color.White);
            sb.End();
            lightMask.Dispose();
            intermediate.Dispose();
            gd.SetRenderTarget(fallback);
        }
    }
}
