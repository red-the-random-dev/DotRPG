using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Scripting;
using DotRPG.Behavior.Management;

namespace DotRPG.Behavior.Defaults
{
    public abstract class TopDownFrameScript : CSIModule
    {
        protected String ObjectName = "";
        protected CameraManager Camera;
        protected ScriptEventManager Events;
        protected ObjectHeapManager ObjectHeap;
        protected SoundManager Audio;

        public override void AddData(string key, object value)
        {
            switch (key)
            {
                case "obj":
                    ObjectHeap = value as ObjectHeapManager;
                    break;
                case "audio":
                    Audio = value as SoundManager;
                    break;
                case "timer":
                    Events = value as ScriptEventManager;
                    break;
                case "camera":
                    Camera = value as CameraManager;
                    break;
                case "this":
                    ObjectName = value as String;
                    break;
            }
        }
    }
}
