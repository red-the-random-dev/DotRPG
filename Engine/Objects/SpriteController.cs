using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects
{
    public class SpriteController
    {
        public String CurrentAnimationSequence { get; protected set; } = "default";
        public Single PlaybackSpeed = 1.0f;
        protected Dictionary<String, Texture2D> AnimationSequenceCollection = new Dictionary<string, Texture2D>();
        protected Dictionary<String, UInt16> FrameAmount = new Dictionary<string, ushort>();
        public UInt16 SpriteIndex
        {
            get
            {
                return (ushort)Math.Round(_i);
            }
            set
            {
                ScrollTimer = 0.0f;
                _i = value;
            }
        }
        protected Dictionary<String, UInt16> LoopTo = new Dictionary<string, ushort>();

        protected Single ScrollTimer;
        protected Single FrameTime;
        private Single _i = 0;

        public SpriteController(Single frameTime, Texture2D defaultSprite, UInt16 frameAmount=1, UInt16 loopDefaultTo=0)
        {
            FrameTime = frameTime;
            AnimationSequenceCollection.Add("default", defaultSprite);
            FrameAmount.Add("default", frameAmount);
            LoopTo.Add("default", loopDefaultTo);
        }

        public void SetAnimationSequence(String sequence, UInt16 initialPosition = 0)
        {
            if (initialPosition > FrameAmount[sequence])
            {
                initialPosition = (UInt16)(initialPosition % FrameAmount[sequence] + LoopTo[sequence]);
            }
            CurrentAnimationSequence = sequence;
            SpriteIndex = initialPosition;
        }

        public void AddAnimationSequence(String sequenceName, Texture2D frames, UInt16 frameAmount, UInt16 loopTo=0)
        {
            AnimationSequenceCollection.Add(sequenceName, frames);
            FrameAmount.Add(sequenceName, frameAmount);
            LoopTo.Add(sequenceName, loopTo);
        }

        public Point SpriteSize
        {
            get
            {
                return new Point(AnimationSequenceCollection[CurrentAnimationSequence].Width / FrameAmount[CurrentAnimationSequence], AnimationSequenceCollection[CurrentAnimationSequence].Height);
            }
        }

        public void Draw(SpriteBatch _sb, Vector2 drawLocation, GameTime gameTime, Vector2? Origin, Color? drawColor = null, Rectangle? aov = null, Vector2? aovl = null, Single drawSize = 1.0f, Single ZIndex = 0.0f)
        {
            Color DrawColor = drawColor ?? Color.White;
            Vector2 origin = Origin ?? Vector2.Zero;
            Vector2 _aov_l = aovl ?? drawLocation;
            
            Single addFrames = 0.0f;
            if (PlaybackSpeed > 0.0f)
            {
                ScrollTimer += (Single)gameTime.ElapsedGameTime.TotalMilliseconds;
                addFrames = ScrollTimer / FrameTime;
                addFrames *= PlaybackSpeed;
                ScrollTimer -= (FrameTime / PlaybackSpeed) * (addFrames / PlaybackSpeed);
            }
            Single newPos = _i + addFrames;
            if (newPos >= FrameAmount[CurrentAnimationSequence])
            {
                newPos = LoopTo[CurrentAnimationSequence];
            }
            Texture2D toDraw = AnimationSequenceCollection[CurrentAnimationSequence];
            Single widthPerFrame = toDraw.Width / FrameAmount[CurrentAnimationSequence];
            UInt16 newPosI = (ushort)Math.Floor(newPos);
            Rectangle lololol = new Rectangle
            (
                (int)(newPosI * widthPerFrame),
                0,
                (int)widthPerFrame,
                toDraw.Height
            );
            Vector2 a_origin = new Vector2(origin.X * lololol.Width, origin.Y * lololol.Height);
            if (aov is Rectangle _aov)
            {
                if (_aov == null)
                {
                    goto skip;
                }
                Rectangle visible = SharedRectangleMethods.GetFromOrigin(_aov_l, origin, new Vector2(lololol.Width, lololol.Height));
                if (!visible.Intersects(_aov))
                {
                    return;
                }
                Rectangle visible2 = SharedRectangleMethods.CutInto(visible, _aov);
                SharedRectangleMethods.GetSizeDifference(visible, visible2, out int dx, out int dy, out int dw, out int dh);
                lololol = new Rectangle(
                    lololol.X + dx, lololol.Y + dy, lololol.Width + dw, lololol.Height + dh
                );
                a_origin = new Vector2(a_origin.X - dx, a_origin.Y - dy);
            }
            skip:
            _sb.Draw
            (
                toDraw, drawLocation,
                lololol,
                // null,
                DrawColor,
                0,
                a_origin,
                drawSize,
                SpriteEffects.None,
                ZIndex
            );
            _i = newPos;
        }
    }
}
