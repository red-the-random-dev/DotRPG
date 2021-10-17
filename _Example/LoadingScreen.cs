using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Behavior;
using DotRPG.Behavior.Routines;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    class LoadingScreen : Frame
    {
        public FrameLoader Loader;
        TextObject percentage;
        public ILoadable LoadedFrame
        {
            get
            {
                return Loader.Loaded;
            }
            set
            {
                Loader = new FrameLoader(value);
            }
        }
        public override int FrameID
        {
            get
            {
                return -129;
            }
        }
        public LoadingScreen(ILoadable il, Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {
            Loader = new FrameLoader(il);
            percentage = new TextObject(globalGameResources.Fonts["vcr_large"], "00.0%", 0.5f, 0.35f, Color.White, AlignMode.BottomCenter, 1080);
        }
        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime, bool[] controls)
        {
            Loader.Update();
            percentage.Text = Loader.LoadPercentage.ToString() + "%";
            base.Update(gameTime, controls);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            Vector2 v = new Vector2(drawZone.X + (drawZone.Width / 8), drawZone.Y + (drawZone.Height / 2));
            Texture2D t1 = new Texture2D(spriteBatch.GraphicsDevice, drawZone.Width * 3 / 4, drawZone.Height / 20);
            Texture2D t2 = new Texture2D(spriteBatch.GraphicsDevice, Math.Max(drawZone.Width * 3 / 4 * (Loader.Loaded.ContentTasks_Done / Loader.Loaded.ContentTasks_Total), 1), drawZone.Height / 20);
            Texture2D t3 = new Texture2D(spriteBatch.GraphicsDevice, Math.Max(drawZone.Width * 3 / 4 * (Int32)Loader.LoadPercentage/100, 1), drawZone.Height / 20);

            Color[] d1 = new Color[drawZone.Width * 3 / 4 * drawZone.Height / 20];
            Color[] d2 = new Color[Math.Max(drawZone.Width * 3 / 4 * (Loader.Loaded.ContentTasks_Done / Loader.Loaded.ContentTasks_Total), 1) * drawZone.Height / 20];
            Color[] d3 = new Color[Math.Max(drawZone.Width * 3 / 4 * (Int32)Loader.LoadPercentage / 100, 1) * drawZone.Height / 20];

            for (int i = 0; i < d1.Length; i++) d1[i] = new Color(75, 75, 75);
            for (int i = 0; i < d2.Length; i++) d2[i] = Color.Gray;
            for (int i = 0; i < d3.Length; i++) d3[i] = Color.White;

            t1.SetData(d1);
            t2.SetData(d2);
            t3.SetData(d3);

            spriteBatch.Draw(t1, v, Color.White);
            spriteBatch.Draw(t2, v, Color.White);
            spriteBatch.Draw(t3, v, Color.White);

            percentage.Draw(spriteBatch, Owner.Window);
        }
        public override void Initialize()
        {
            
        }
        public override void LoadContent()
        {
            
        }
        public override void UnloadContent()
        {
            
        }
    }
}
