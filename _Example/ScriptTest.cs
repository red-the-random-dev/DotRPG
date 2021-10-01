using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using DotRPG.Objects;
using DotRPG.Objects.Dynamics;
using DotRPG.Scripting;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG._Example
{
    class ScriptTest : Frame
    {
        LuaModule DialogTest1;
        TextObject DialogForm;
        Player P;
        Boolean[] lastInputCollection = new bool[8];
        Boolean ShowingText;

        public override int FrameID
        {
            get
            {
                return 2;
            }
        }

        public ScriptTest(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {

        }

        public override void Initialize()
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            
        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            DialogForm = new TextObject(FrameResources.Global.Fonts["vcr_large"], "...", 0.01f, 0.80f, Color.White, AlignMode.TopLeft, 540, scrollPerTick: 1, scrollDelay: 0.16f);
        }

        public override void UnloadContent()
        {
            DialogForm = null;
            FrameResources.Dispose();
        }
    }
}
