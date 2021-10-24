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

        public abstract void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime);

        public override void Update(string EventID, float ElapsedGameTime, float TotalGameTime)
        {
            try
            {
                UpdateInternal(EventID, ElapsedGameTime, TotalGameTime);
                LastError = "";
                LastErrorDetails = null;
            }
            catch (Exception e)
            {
                LastError = e.Message;
                LastErrorDetails = e;
            }
        }

        public override void Start()
        {
            
        }
    }
}
