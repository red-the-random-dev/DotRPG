// #define FORCE_4x3
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using DotRPG.Objects;

namespace DotRPG._Example
{
    public class Game1 : Game
    {
        private SpriteController ScrollMarker;
        private SpriteController Banana;
        private TextObject PressStart;
        private TextObject ScrollText;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private SpriteFont _spriteFontLarge;
        private Boolean GameStarted;
        private Double GameStartMark;
        private Boolean[] IsCtrlKeyDown = new bool[8] { false, false, false, false, false, false, false, false};
        private Boolean FullScreen;
        private Boolean Scrolling;
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
        private Double EscapeTimer = 0.0f;
        #if DEBUG
        private Double LastRegisteredEventTime;
        #endif
        private HashSet<TimedEvent> LogicEventSet = new HashSet<TimedEvent>();
        private Boolean WideScreen
        {
            get
            {
            #if !FORCE_4x3
                return GraphicsAdapter.DefaultAdapter.IsWideScreen;
            #else
            #warning Widescreen setting will be ignored for this build. (FORCE_4x3)
                return false;
            #endif
            }
        }

        private void StartScroll(Object sender, EventArgs e, GameTime gameTime)
        {
            Scrolling = true;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ResetAspectRatio();

            base.Initialize();
        }

        protected void ResetAspectRatio()
        {
            if (WideScreen)
            {
                _graphics.PreferredBackBufferWidth = (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef) && FullScreen ? 1920 : 960);
                _graphics.PreferredBackBufferHeight = (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef) && FullScreen ? 1080 : 540);
            }
            else
            {
                _graphics.PreferredBackBufferWidth = 800;
                _graphics.PreferredBackBufferHeight = 600;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("../GameData/Fonts/MainFont");
            _spriteFontLarge = Content.Load<SpriteFont>("../GameData/Fonts/MainFont_Large");
            SE_1 = Content.Load<SoundEffect>("../GameData/Sounds/text-scroll");
            SE_2 = Content.Load<SoundEffect>("../GameData/Sounds/pixelText");
            SE_Next = Content.Load<SoundEffect>("../GameData/Sounds/clickText");
            PressStart = new TextObject(_spriteFontLarge, "[Press START or ENTER]", 0.5f, 0.5f, Color.White, AlignMode.Center, (WideScreen ? 1080 : 960));
            ScrollText = new TextObject(_spriteFontLarge, /*"* Scroll text test. 1234567890ABCDEFGHIJKLMN\n  OPQRSTUVWXYZ\n* OMFGWTFLMAOSUSCHUNGUS"*/ "* bananas\n* rotat e", 0.01f, 0.80f, Color.White, AlignMode.TopLeft, (WideScreen ? 1080 : 960), scrollPerTick: 1, scrollDelay: 0.16f);
            ScrollText.ScrollingSound = SE_1;
            ScrollMarker = new SpriteController(1000.0f / 60, Content.Load<Texture2D>("../GameData/Texture2D/ScrollMarker"), 27);
            Banana = new SpriteController(1000.0f / 6, Content.Load<Texture2D>("../GameData/Texture2D/Banana"), 28);
            ResetAspectRatio();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            #if DEBUG
            LastRegisteredEventTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            #endif
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                EscapeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (EscapeTimer >= 1000.0f)
                {
                    Exit();
                }
            }
            else
            {
                EscapeTimer = 0.0f;
            }
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) && !GameStarted)
            {
                GameStarted = true;
                GameStartMark = gameTime.TotalGameTime.TotalMilliseconds;
                LogicEventSet.Add(new TimedEvent(gameTime, 1000.0f, StartScroll));
            }
            if (!IsCtrlKeyDown[7] && (Keyboard.GetState().IsKeyDown(Keys.F4) || GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed))
            {
                IsCtrlKeyDown[7] = true;
                FullScreen = !FullScreen;
            }
            else if (IsCtrlKeyDown[7] && (Keyboard.GetState().IsKeyUp(Keys.F4) && GamePad.GetState(PlayerIndex.One).Buttons.RightStick != ButtonState.Pressed))
            {
                IsCtrlKeyDown[7] = false;
            }
            if (DialogIndex == Dialog.Length-1)
            {
                ScrollText.ScrollingSound = SE_1;
                ScrollText.ScrollDelay = 0.08f;
                ScrollText.TextColor = Color.Red;
                if (ScrollText.ReachedEnd)
                    Exit();
            }
            if (DialogIndex == Dialog.Length - 2)
            {
                ScrollText.ScrollingSound = SE_2;
                ScrollText.TextColor = Color.Yellow;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !IsCtrlKeyDown[4] && ScrollText.ReachedEnd)
            {
                DialogIndex++;
                ScrollText.Text = Dialog[DialogIndex];
                ScrollText.ResetToStart();
                SE_Next.Play();
                Banana.PlaybackSpeed = 1.0f + (0.25f * DialogIndex);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && IsCtrlKeyDown[4])
            {
                IsCtrlKeyDown[4] = true;
                // ScrollText.ScrollDelay = 0.16f;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Z))
            {
                IsCtrlKeyDown[4] = false;
                // ScrollText.ScrollDelay = 0.16f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                ScrollText.SkipToEnd();
            }

            if (LogicEventSet.Count > 0)
            {
                try
                {
                    foreach (TimedEvent te in LogicEventSet)
                    {
                        if (te.TryFire(this, new EventArgs(), gameTime))
                        {
                            LogicEventSet.Remove(te);
                        }
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }
            if (Scrolling)
            {
                ScrollText.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (FullScreen != _graphics.IsFullScreen)
            {
                ResetAspectRatio();
                _graphics.ToggleFullScreen();
            }
            Double FrameRate = Math.Round(1000 / gameTime.ElapsedGameTime.TotalMilliseconds);
            if (GameStarted && gameTime.TotalGameTime.TotalMilliseconds - GameStartMark < 1000.0)
            {
                Single t = (float) (0.65-((gameTime.TotalGameTime.TotalMilliseconds - GameStartMark) / 1000 * 0.65f));
                GraphicsDevice.Clear(new Color(new Vector3(t, t, t)));
            }
            else
                GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            #if DEBUG
            _spriteBatch.DrawString(_spriteFont, "FPS: "+FrameRate.ToString()+" || Fullscreen: "+FullScreen.ToString()+" || Aspect ratio: "+(WideScreen?"16:9":"4:3")+" Update rate: "+Math.Round(1000/LastRegisteredEventTime), new Vector2(0, 0), (FrameRate > 50 ? Color.White : (FrameRate > 24 ? Color.Yellow : Color.Red)));
            #endif
            _spriteBatch.DrawString(_spriteFont, "Quitting...", new Vector2(0, 12), new Color(new Vector4((float) EscapeTimer/1000)));
            if (!GameStarted)
            {
                if (Math.Floor(gameTime.TotalGameTime.TotalSeconds) % 2 == 1)
                {
                    // Replaced with scalable method
                    // PressStart.TextColor = Color.White;
                    // _spriteBatch.DrawString(_spriteFont, "[Press START or ENTER]", SharedMethodSet.FindTextAlignment(_spriteFont, "[Press START or ENTER]", Window.ClientBounds), Color.White, 0.0f, new Vector2(0.0f), (FullScreen&&WideScreen?2.0f:1.0f), SpriteEffects.None, 0.0f);
                    PressStart.Draw(_spriteBatch, Window);
                }
            }
            else if (gameTime.TotalGameTime.TotalMilliseconds - GameStartMark < 1000.0)
            {
                if (Math.Floor(gameTime.TotalGameTime.TotalMilliseconds) % 40 > 20)
                {
                    PressStart.TextColor = Color.Yellow;
                    PressStart.Draw(_spriteBatch, Window);
                    // Replaced with scalable method
                    // _spriteBatch.DrawString(_spriteFont, "[Press START or ENTER]", SharedMethodSet.FindTextAlignment(_spriteFont, "[Press START or ENTER]", Window.ClientBounds), Color.Yellow, 0.0f, new Vector2(0.0f), (FullScreen && WideScreen ? 2.0f : 1.0f), SpriteEffects.None, 0.0f);
                }
                Int32 JiggleDeviation = (int) ((gameTime.TotalGameTime.TotalMilliseconds - GameStartMark) / 1000 * 15.0f);
                Window.Position = new Point(Math.Floor(gameTime.TotalGameTime.TotalMilliseconds) % 40 > 20 ? Window.Position.X + JiggleDeviation - 1: Window.Position.X - JiggleDeviation, Math.Floor(gameTime.TotalGameTime.TotalMilliseconds) % 40 > 20 ? Window.Position.Y + JiggleDeviation : Window.Position.Y - JiggleDeviation);
            }
            if (Scrolling)
            {
                ScrollText.Draw(_spriteBatch, Window);
                if (DialogIndex < Dialog.Length - 2)
                    Banana.Draw(_spriteBatch, new Vector2((float)(Window.ClientBounds.Width / 2 - 125.0f), (float)(Window.ClientBounds.Height / 2 - 125.0f)), gameTime);
                if (ScrollText.ReachedEnd)
                {
                    ScrollMarker.Draw(_spriteBatch, new Vector2(Window.ClientBounds.Width-90.0f, Window.ClientBounds.Height-90.0f), gameTime);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
