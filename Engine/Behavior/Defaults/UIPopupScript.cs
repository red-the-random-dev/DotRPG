using DotRPG.Scripting;
using DotRPG.UI;
using System;
using System.Collections.Generic;

namespace DotRPG.Behavior.Defaults
{
    [BuiltScript("default/popup")]
    public class UIPopupScript : TopDownFrameScript
    {
        HashSet<String> Raise = new HashSet<string>();
        HashSet<String> Lower = new HashSet<string>();
        Int32 speed = 100;
        public override bool RequireRawSceneData => true;
        public override bool RequireResourceHeap => false;

        public override void Start()
        {
            
        }
        public override void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            if (EventID.Length == 0)
            {
                return;
            }
            if (EventID == "default")
            {
                foreach (String x in Raise)
                {
                    UserInterfaceElement uie = Scene.UI_NamedList[x];
                    uie.RotationOrigin = new Microsoft.Xna.Framework.Vector2(uie.RotationOrigin.X, Math.Clamp(uie.RotationOrigin.Y + ElapsedGameTime * speed / 100000, 0.0f, 1.0f));
                }
                foreach (String x in Lower)
                {
                    UserInterfaceElement uie = Scene.UI_NamedList[x];
                    uie.RotationOrigin = new Microsoft.Xna.Framework.Vector2(uie.RotationOrigin.X, Math.Clamp(uie.RotationOrigin.Y - ElapsedGameTime * speed / 100000, 0.0f, 1.0f));
                }
                return;
            }
            String[] pars = EventID.Split(':');
            if (pars.Length < 3)
            {
                return;
            }
            if (pars[0] != "popup")
            {
                return;
            }
            switch (pars[1])
            {
                case "raise":
                    if (Lower.Contains(pars[2]))
                    {
                        Lower.Remove(pars[2]);
                    }
                    if (Raise.Contains(pars[2]))
                    {
                        Raise.Remove(pars[2]);
                    }
                    Raise.Add(pars[2]);
                    break;
                case "lower":
                    if (Lower.Contains(pars[2]))
                    {
                        Lower.Remove(pars[2]);
                    }
                    if (Raise.Contains(pars[2]))
                    {
                        Raise.Remove(pars[2]);
                    }
                    Lower.Add(pars[2]);
                    break;
                case "pause":
                    if (Lower.Contains(pars[2]))
                    {
                        Lower.Remove(pars[2]);
                    }
                    if (Raise.Contains(pars[2]))
                    {
                        Raise.Remove(pars[2]);
                    }
                    break;
                case "setspeed":
                    speed = Int32.Parse(pars[2]);
                    break;
            }
        }
    }
}
