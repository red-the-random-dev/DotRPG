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
        private ResourceHeap ResourceHGlobal = new ResourceHeap();
        private List<Frame> Frames = new List<Frame>();
        private Frame ActiveFrame;
        private TextObject PressStart;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private SpriteFont _spriteFontLarge;
        private Boolean GameStarted;
        private Double GameStartMark;
        private Boolean[] IsCtrlKeyDown = new bool[8] { false, false, false, false, false, false, false, false};
        private Boolean FullScreen;
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
            ActiveFrame = Frames[0];
            ActiveFrame.LoadContent();
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "GameData";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ResetAspectRatio();
            Frames.Add(new DemoFrame(this, ResourceHGlobal));
            Frames[0].Initialize();
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
            ResourceHGlobal.Fonts.Add("vcr", Content.Load<SpriteFont>("Fonts/MainFont"));
            ResourceHGlobal.Fonts.Add("vcr_large", Content.Load<SpriteFont>("Fonts/MainFont_Large"));
            _spriteFontLarge = ResourceHGlobal.Fonts["vcr_large"];
            _spriteFont = ResourceHGlobal.Fonts["vcr"];

            PressStart = new TextObject(_spriteFontLarge, "[Press START or ENTER]", 0.5f, 0.5f, Color.White, AlignMode.Center, (WideScreen ? 1080 : 960));
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
            IsCtrlKeyDown[4] = Keyboard.GetState().IsKeyDown(Keys.Z);
            IsCtrlKeyDown[5] = Keyboard.GetState().IsKeyDown(Keys.X);

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
            if (ActiveFrame != null)
            {
                ActiveFrame.Update(gameTime, IsCtrlKeyDown);
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
            }
            if (ActiveFrame != null)
            {
                ActiveFrame.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
