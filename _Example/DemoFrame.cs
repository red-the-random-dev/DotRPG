using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace DotRPG._Example
{
    class DemoFrame : Frame
    {
        public DemoFrame(Game owner, ResourceHeap rh, HashSet<TimedEvent> eventSet) : base(owner, rh, eventSet)
        {

        }

        private SpriteController Banana;
        private TextObject ScrollText;
        private SpriteController ScrollMarker;
        private SoundEffect SE_1;
        private SoundEffect SE_2;
        private SoundEffect SE_Next;
        private Int32 DialogIndex = 0;
        private String[] Dialog =
        {
            "* bananas\n* rotat e",
            "* banan   a",
            "* rotato faster",
            "* banana",
            "* go",
            "* g    O",
            "* can  u fEEl it",
            "* b an      \n* an  ba?",
            "* yES fEEl tHE SPED",
            "* WE HAVE REAHCED MXAIMUN VLELOCIPY",
            "-> Are you OK?",
            "* Who are you to accuse me?"
        };

        private Boolean WideScreen
        {
            get
            {
            #if !FORCE_4x3
                return GraphicsAdapter.DefaultAdapter.IsWideScreen;
            #else
                return false;
            #endif
            }
        }

        public override int FrameID
        {
            get
            {
                return 0;
            }
        }

        public override void Initialize()
        {
            
        }

        public override void LoadContent()
        {
            FrameResources.Sounds.Add("scroll_01", Owner.Content.Load<SoundEffect>("Sounds/text-scroll"));
            FrameResources.Sounds.Add("scroll_02", Owner.Content.Load<SoundEffect>("Sounds/pixelText"));
            FrameResources.Sounds.Add("scroll_next", Owner.Content.Load<SoundEffect>("Sounds/clickText"));
            ScrollMarker = new SpriteController(1000.0f / 60, Owner.Content.Load<Texture2D>("Texture2D/ScrollMarker"), 27);
            Banana = new SpriteController(1000.0f / 6, Owner.Content.Load<Texture2D>("Texture2D/Banana"), 28);
            SE_1 = FrameResources.Sounds["scroll_01"];
            SE_2 = FrameResources.Sounds["scroll_02"];
            SE_Next = FrameResources.Sounds["scroll_next"];
            ScrollText = new TextObject(FrameResources.Global.Fonts["vcr_large"], "* bananas\n* rotat e", 0.01f, 0.80f, Color.White, AlignMode.TopLeft, (WideScreen ? 1080 : 960), scrollPerTick: 1, scrollDelay: 0.16f);
            ScrollText.ScrollingSound = SE_1;
        }
        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            if (DialogIndex == Dialog.Length - 1)
            {
                ScrollText.ScrollingSound = SE_1;
                ScrollText.ScrollDelay = 0.08f;
                ScrollText.TextColor = Color.Red;
                if (ScrollText.ReachedEnd)
                    GlobalEventSet.Add(
                        new TimedEvent(
                            gameTime,
                            0.0f,
                            Game1.SetFrameNumber,
                            new FrameShiftEventArgs(
                                1, null
                            )
                        )
                    );
            }
            if (DialogIndex == Dialog.Length - 2)
            {
                ScrollText.ScrollingSound = SE_2;
                ScrollText.TextColor = Color.Yellow;
            }
            if (controls[4] && ScrollText.ReachedEnd)
            {
                DialogIndex++;
                ScrollText.Text = Dialog[DialogIndex];
                ScrollText.ResetToStart();
                SE_Next.Play();
                Banana.PlaybackSpeed = 1.0f + (0.25f * DialogIndex);
            }
            if (controls[5])
            {
                ScrollText.SkipToEnd();
            }
            ScrollText.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            ScrollText.Draw(spriteBatch, Owner.Window);
            if (DialogIndex < Dialog.Length - 2)
                Banana.Draw(spriteBatch, new Vector2((float)(Owner.Window.ClientBounds.Width / 2 - 125.0f), (float)(Owner.Window.ClientBounds.Height / 2 - 125.0f)), gameTime);
            if (ScrollText.ReachedEnd)
            {
                ScrollMarker.Draw(spriteBatch, new Vector2(Owner.Window.ClientBounds.Width - 90.0f, Owner.Window.ClientBounds.Height - 90.0f), gameTime);
            }
        }
        public override void UnloadContent()
        {
            Banana = null;
            DialogIndex = 0;
            ScrollText = null;
            ScrollMarker = null;
            SE_1 = null;
            SE_2 = null;
            SE_Next = null;
            FrameResources.Dispose();
        }
    }
}
