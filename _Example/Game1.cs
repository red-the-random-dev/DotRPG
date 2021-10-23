// #define FORCE_4x3
// #define MUTE
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DotRPG.Objects;
using DotRPG.Behavior;
using System.Xml.Linq;
using System.IO;
using DotRPG.Behavior.Routines;
using DotRPG.Behavior.Defaults;
using DotRPG.UI;
using System.Reflection;

namespace DotRPG._Example
{
    public class Game1 : Game
    {
        private StageSelectFrame stageSelect;
        private ResourceHeap ResourceHGlobal = new ResourceHeap();
        private List<String> FrameNames = new List<string>();
        private List<Frame> Frames = new List<Frame>();
        private Frame ActiveFrame;
        private Frame ActiveSubframe;
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
        private Double TimeSinceError = 0.0f;
        private HashSet<TimedEvent> LogicEventSet = new HashSet<TimedEvent>();
        private Boolean ContinuityError = false;
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

        public static void SetFrameNumber(Object sender, EventArgs e, GameTime gameTime)
        {
            Game1 game = (Game1)sender;
            if (e is FrameShiftEventArgs)
            {
                FrameShiftEventArgs fse = (FrameShiftEventArgs)e;
                switch (fse.FrameID)
                {
                    case -1:
                        game.ActiveFrame.UnloadContent();
                        game.ActiveFrame = null;
                        break;
                    case -2:
                        game.LogicEventSet.Clear();
                        game.ActiveFrame = null;
                        game.ContinuityError = true;
                        game._spriteBatch.End();
                        break;
                    case Int32.MinValue:
                        game.Exit();
                        break;
                    default:
                        try
                        {
                            game.ActiveFrame.UnloadContent();
                            game.ActiveFrame = game.Frames[fse.FrameID];
                            game.ActiveFrame.LoadContent();
                        }
                        catch (NullReferenceException)
                        {
                            game.ActiveFrame = null;
                            game.ContinuityError = true;
                            game._spriteBatch.End();
                            game.LogicEventSet.Clear();
                        }
                        break;
                }
            }
        }

        private void StartScroll(Object sender, EventArgs e, GameTime gameTime)
        {
            ActiveFrame = stageSelect;
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
            foreach (String i in Directory.EnumerateFiles(Path.Combine(Content.RootDirectory, "Maps/")))
            {
                Frame f = XMLSceneLoader.LoadFrame(XDocument.Parse(File.ReadAllText(i)), new Type[] {typeof(TopDownFrame)}, new Object[] { this, ResourceHGlobal, LogicEventSet }, out String Name);
                FrameNames.Add(Name);
                Frames.Add(f);
                f.Initialize();
            }
            
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
                _graphics.IsFullScreen = FullScreen;
                _graphics.PreferredBackBufferWidth = 800;
                _graphics.PreferredBackBufferHeight = 600;
            }
            Window.IsBorderless = FullScreen;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceHGlobal.Fonts.Add("vcr", Content.Load<SpriteFont>("Fonts/MainFont"));
            ResourceHGlobal.Fonts.Add("vcr_large", Content.Load<SpriteFont>("Fonts/MainFont_Large"));
            _spriteFontLarge = ResourceHGlobal.Fonts["vcr_large"];
            _spriteFont = ResourceHGlobal.Fonts["vcr"];
            stageSelect = new StageSelectFrame("Please select the map to continue.\n[UP/DOWN] Scroll\n[Z] Select\n[F2] Quit to stage select", FrameNames.ToArray(), _spriteFont, this, ResourceHGlobal, LogicEventSet);
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
            if (ActiveFrame != null)
            {
                if (ActiveSubframe is LoadingScreen l)
                {
                    if (l.LoadedFrame.Loaded)
                    {
                        ActiveFrame = l.LoadedFrame as Frame;
                        ActiveSubframe = null;
                    }
                    if (ActiveFrame == null)
                    {
                        ActiveFrame = stageSelect;
                    }
                }
                else if (ActiveFrame.FrameID == -128 && stageSelect.SelectedOption != -1)
                {
                    Frame toLoad = Frames[stageSelect.SelectedOption];
                    if (toLoad is ILoadable load)
                    {
                        ActiveSubframe = new LoadingScreen(load, this, ResourceHGlobal, LogicEventSet);
                    }
                    else
                    {
                        ActiveFrame = toLoad;
                        ActiveFrame.LoadContent();
                    }
                }
                else if (ActiveFrame.FrameID != -128 && Keyboard.GetState().IsKeyDown(Keys.F2))
                {
                    ActiveFrame.UnloadContent();
                    stageSelect.SelectedOption = -1;
                    ActiveFrame = stageSelect;
                    GC.Collect();
                }
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
            IsCtrlKeyDown[4] = Keyboard.GetState().IsKeyDown(Keys.Z) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
            IsCtrlKeyDown[5] = Keyboard.GetState().IsKeyDown(Keys.X) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B);

            IsCtrlKeyDown[0] = Keyboard.GetState().IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp);
            IsCtrlKeyDown[1] = Keyboard.GetState().IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown);
            IsCtrlKeyDown[2] = Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft);
            IsCtrlKeyDown[3] = Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight);

            if (LogicEventSet.Count > 0)
            {
                try
                {
                    foreach (TimedEvent te in LogicEventSet)
                    {
                        if (te.TryFire(this, gameTime))
                        {
                            LogicEventSet.Remove(te);
                        }
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }
            if (ActiveSubframe != null)
            {
                ActiveSubframe.Update(gameTime, IsCtrlKeyDown);
            }
            else if (ActiveFrame != null)
            {
                ActiveFrame.Update(gameTime, IsCtrlKeyDown);
            }
            if (ContinuityError)
            {
                TimeSinceError += gameTime.ElapsedGameTime.TotalSeconds;
            }
            PressStart.Rotation = (Single)Math.Sin(gameTime.TotalGameTime.TotalSeconds * Math.PI / 2) / 8;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ResetAspectRatio();
            Rectangle win = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            Double FrameRate = Math.Round(1000 / gameTime.ElapsedGameTime.TotalMilliseconds);
            if (GameStarted && gameTime.TotalGameTime.TotalMilliseconds - GameStartMark < 1000.0)
            {
                Single t = (float) (0.65-((gameTime.TotalGameTime.TotalMilliseconds - GameStartMark) / 1000 * 0.65f));
                GraphicsDevice.Clear(new Color(new Vector3(t, t, t)));
            }
            else
                GraphicsDevice.Clear(Color.Black);
            if (FrameRate < 24)
            {
                _spriteBatch.Begin();
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront);
            }
            
            if (ContinuityError)
            {
                _spriteBatch.DrawString(_spriteFontLarge, "/!\\ CONTINUITY ERROR /!\\", SharedGraphicsMethods.FindTextAlignment(_spriteFontLarge, "/!\\ CONTINUITY ERROR /!\\", Window.ClientBounds, 0.5f, 0.5f), Color.Red);
                _spriteBatch.DrawString(_spriteFont, "This is not an easter egg. Something is wrong with the game.", SharedGraphicsMethods.FindTextAlignment(_spriteFont, "This is not an easter egg. Something is wrong with the game.", Window.ClientBounds, 0.5f, 0.6f, AlignMode.TopCenter), new Color(new Vector4(Math.Min((Single)TimeSinceError*0.02f, 255.0f))));
                _spriteBatch.End();
                base.Draw(gameTime);
                return;
            }
            if (!GameStarted)
            {
                // Replaced with scalable method
                // PressStart.TextColor = Color.White;
                // _spriteBatch.DrawString(_spriteFont, "[Press START or ENTER]", SharedMethodSet.FindTextAlignment(_spriteFont, "[Press START or ENTER]", Window.ClientBounds), Color.White, 0.0f, new Vector2(0.0f), (FullScreen&&WideScreen?2.0f:1.0f), SpriteEffects.None, 0.0f);
                PressStart.Draw(gameTime, _spriteBatch, win);
            }
            else if (gameTime.TotalGameTime.TotalMilliseconds - GameStartMark < 1000.0)
            {
                if (Math.Floor(gameTime.TotalGameTime.TotalMilliseconds) % 40 > 20)
                {
                    PressStart.TextColor = Color.Yellow;
                    PressStart.Draw(gameTime, _spriteBatch, win);
                    // Replaced with scalable method
                    // _spriteBatch.DrawString(_spriteFont, "[Press START or ENTER]", SharedMethodSet.FindTextAlignment(_spriteFont, "[Press START or ENTER]", Window.ClientBounds), Color.Yellow, 0.0f, new Vector2(0.0f), (FullScreen && WideScreen ? 2.0f : 1.0f), SpriteEffects.None, 0.0f);
                }
                if (gameTime.TotalGameTime.TotalMilliseconds - GameStartMark > 900.0)
                {
                    _spriteBatch.DrawString(_spriteFont, "Loading...", new Vector2(0, 36), Color.White);
                }
            }
            if (ActiveFrame != null)
            {
                ActiveFrame.Draw(gameTime, _spriteBatch);
            }
            if (ActiveSubframe != null)
            {
                ActiveSubframe.Draw(gameTime, _spriteBatch);
            }
#if DEBUG
            _spriteBatch.DrawString(_spriteFont, "FPS: " + FrameRate.ToString() + " || Fullscreen: " + FullScreen.ToString() + String.Format(" || Resolution: {0}x{1}", Window.ClientBounds.Width, Window.ClientBounds.Height) + " || Frame active: " + (ActiveFrame != null ? ActiveFrame.FrameID.ToString() : "-1") + " || Update rate: " + Math.Round(1000 / LastRegisteredEventTime), new Vector2(0, 0), (FrameRate > 50 ? Color.White : (FrameRate > 24 ? Color.Yellow : Color.Red)));
#endif
            _spriteBatch.DrawString(_spriteFont, "Quitting...", new Vector2(0, 12), new Color(new Vector4((float)EscapeTimer / 1000)));
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
