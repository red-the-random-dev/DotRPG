﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace DotRPG.Objects
{
    /// <summary>
    /// Instance of drawable object that represents scrollable text.
    /// </summary>
    public class TextObject
    {
        public String Text;
        public Color TextColor;
        public Single RelativeAlignmentX;
        public Single RelativeAlignmentY;
        public AlignMode AlignAnchor;
        public SpriteFont Font;
        protected Int32 DefaultScreenScale;
        public Single Rotation;
        public Single ScrollDelay;
        protected Double ScrollTimer;
        protected Int32 DrawnText;
        public Int32 ScrollPerTick;
        public Single Depth;
        protected String WrittenString = String.Empty;
        public SoundEffect ScrollingSound = null;
        protected Int32 LastDrawnText = 0;
        /// <summary>
        /// Creates an instance of Text Object.
        /// </summary>
        /// <param name="sf">Font that will be used to render text.</param>
        /// <param name="StartText">Initial line of text.</param>
        /// <param name="PosX">Relative X alignment. 0.5F is a center.</param>
        /// <param name="PosY">Relative Y alignment. 0.5F is a center.</param>
        /// <param name="color">Text color.</param>
        /// <param name="anchor">Determines which edge of text box will be used as anchor point.</param>
        /// <param name="absoluteScreenScale">Screen height that is set as default. Used for dynamic rescaling.</param>
        /// <param name="scrollPerTick">How many letters should be added to written string per one iteration. Set to -1 for static text.</param>
        /// <param name="scrollDelay">Basically a frame time for scrolling. 0.04F -- add letters 25 times per second.</param>
        /// <param name="initialRotation">Rotate text around anchor point.</param>
        /// <param name="depth">Sprite depth. Sprites will greater values classify as backdrops.</param>
        public TextObject(SpriteFont sf, String StartText, Single PosX, Single PosY, Color color, AlignMode anchor, Int32 absoluteScreenScale, Int32 scrollPerTick = -1, Single scrollDelay = 0.04f, Single initialRotation = 0.0f, Single depth = 0.0f)
        {
            TextColor = color;
            Font = sf;
            Text = StartText;
            RelativeAlignmentX = PosX;
            RelativeAlignmentY = PosY;
            AlignAnchor = anchor;
            DefaultScreenScale = absoluteScreenScale;
            ScrollPerTick = scrollPerTick;
            ScrollDelay = scrollDelay;
            Rotation = initialRotation;
            Depth = depth;
        }
        /// <summary>
        /// Display text with use of SpriteBatch
        /// </summary>
        /// <param name="_sb">SpriteBatch object used for rendering.</param>
        /// <param name="w">Game window. Used for aligning.</param>
        public void Draw(SpriteBatch _sb, GameWindow w)
        {
            if (LastDrawnText < DrawnText && WrittenString[DrawnText-1] != ' ' && WrittenString[DrawnText - 1] != '*' && WrittenString[DrawnText-1] != '\n' && WrittenString[DrawnText - 1] != '-' && WrittenString[DrawnText - 1] != '>' && ScrollingSound != null)
            {
                ScrollingSound.Play();
            }
            // Alighning text according to set anchor position and client bounds
            Vector2 position = SharedMethodSet.FindTextAlignment(Font, (ScrollPerTick > 0 ? WrittenString : Text), w.ClientBounds, RelativeAlignmentX, RelativeAlignmentY, AlignAnchor, w.ClientBounds.Height / DefaultScreenScale);
            _sb.DrawString(Font, (ScrollPerTick > 0 ? WrittenString : Text), position, TextColor, Rotation, new Vector2(0.0f), 1.0f*(w.ClientBounds.Height)/DefaultScreenScale, SpriteEffects.None, Depth);
            LastDrawnText = DrawnText;
        }
        public void ResetToStart()
        {
            DrawnText = 0;
        }
        /// <summary>
        /// Skip to end of text, for example, with [X] button.
        /// </summary>
        public void SkipToEnd()
        {
            DrawnText = Text.Length;
        }
        /// <summary>
        /// Property that indicates if text scrolling has reached its end.
        /// </summary>
        public Boolean ReachedEnd
        {
            get
            {
                return DrawnText >= Text.Length;
            }
        }
        /// <summary>
        /// Prepare a line of text according to elapsed gametime.
        /// </summary>
        /// <param name="gameTime">Gametime retrieved via game's Update() method parameter.</param>
        public void Update(GameTime gameTime)
        {
            LastDrawnText = DrawnText;
            WrittenString = "";
            if (ScrollPerTick > 0)
            {
                if (ScrollTimer >= ScrollDelay && DrawnText < Text.Length)
                {
                    ScrollTimer -= ScrollDelay;
                    DrawnText += ScrollPerTick;
                }
                for (int i = 0; i < Math.Min(DrawnText, Text.Length); i++)
                {
                    WrittenString += Text[i];
                }
                ScrollTimer += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                WrittenString = Text;
            }
        }
    }
}