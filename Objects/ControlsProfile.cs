using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DotRPG.Objects
{
    class ControlsProfile
    {
        public Keys[] KeyboardBindings;
        public Buttons[,] GamepadBindings;
        public ControlsProfile(Int16 keyAmount, Int16 playerAmount)
        {
            KeyboardBindings = new Keys[keyAmount];
            GamepadBindings = new Buttons[playerAmount, keyAmount];
        }
    }
}
