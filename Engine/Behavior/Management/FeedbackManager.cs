using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DotRPG.Behavior.Management
{
    public class FeedbackManager
    {
        protected Single Vibration;
        protected Single VibrationFadeout;
        protected Single Fac;

        public void SetVibration(Single v)
        {
            Vibration = v;
        }
        public void SetVibration(Single v, Single f)
        {
            Vibration = v;
            VibrationFadeout = f;
        }
        public void SetVibration(Single v, Single f, Single fac)
        {
            Vibration = v;
            VibrationFadeout = f;
            Fac = fac;
        }
        public void SetPanning(Single fac)
        {
            Fac = fac;
        }

        public void Update(GameTime gameTime)
        {
            if (Vibration > 0)
            {
                GamePad.SetVibration(PlayerIndex.One, Math.Clamp(Vibration - (Vibration * Fac), 0, Vibration), Math.Clamp(Vibration + (Vibration * Fac), 0, Vibration));
                Vibration -= (Single)(VibrationFadeout * gameTime.ElapsedGameTime.TotalSeconds);
                if (Vibration < 0)
                {
                    Vibration = 0;
                    VibrationFadeout = 0;
                    Fac = 0;
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }
        }
        public void Reset()
        {
            Vibration = 0;
            VibrationFadeout = 0;
            Fac = 0;
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }
    }
}
