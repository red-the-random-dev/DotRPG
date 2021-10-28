// #define SPEEN
using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Behavior;
using DotRPG.Behavior.Routines;
using DotRPG.Objects;
using DotRPG.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Algebra;

namespace DotRPG._Example
{
    class LoadingScreen : Frame
    {
        public FrameLoader Loader;
        TextObject percentage;
        ProgressBar progress;
        ColorBox cb;
        Single timeCountdown;
        
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
            percentage = new TextObject(globalGameResources.Fonts["vcr_large"], "0%", 0.5f, 0.35f, Color.White, AlignMode.BottomCenter, 540);
            progress = new ProgressBar(Color.Gray, Color.Black, new Vector2(0.75f, 0.1f), new Vector2(0.5f, 1.0f));
            progress.RotationOrigin = new Vector2(0.5f, 1.0f);
            cb = new ColorBox(new Color(0, 0, 75), new Vector2(0.75f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            cb.SubnodePadding = new Vector4(4.0f);
            cb.Subnodes.Add(percentage);
            cb.Subnodes.Add(progress);
        }
        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime, bool[] controls)
        {
            timeCountdown += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            cb.RelativeSize = new Vector2(0.75f, Math.Max(0.01f, Math.Min(0.5f, timeCountdown*4)));
#if SPEEN
            cb.Rotation = (Single)gameTime.TotalGameTime.TotalSeconds * MathHelper.TwoPi;
#endif
            if (timeCountdown > 0.125f)
            {
                Loader.Update();
            }
            progress.Progress_Percentage = Loader.LoadPercentage;
            percentage.Text = Loader.LoadPercentage.ToString() + "%";
            base.Update(gameTime, controls);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            cb.Draw(gameTime, spriteBatch, drawZone);
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
