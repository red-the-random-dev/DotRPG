using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Scripting;
using DotRPG.Behavior.Management;
using DotRPG.Objects;

namespace DotRPG.Behavior.Defaults
{
    public abstract class TopDownFrameScript : CSIModule
    {
        protected String ObjectName = "";
        protected CameraManager Camera;
        protected ScriptEventManager Events;
        protected ObjectHeapManager ObjectHeap;
        protected SoundManager Audio;
        protected TopDownFrame Scene;
        protected ResourceHeap FrameResources;
        protected PaletteManager Palette;
        protected PathfindingManager Pathfinder;
        protected FeedbackManager Feedback;
        protected DialogueManager Dialogue;

        public virtual Boolean RequireRawSceneData => false;
        public virtual Boolean RequireResourceHeap => false;

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
                case "scene":
                    Scene = value as TopDownFrame;
                    break;
                case "resources":
                    FrameResources = value as ResourceHeap;
                    break;
                case "palette":
                    Palette = value as PaletteManager;
                    break;
                case "navmap":
                    Pathfinder = value as PathfindingManager;
                    break;
                case "feedback":
                    Feedback = value as FeedbackManager;
                    break;
                case "dialogue":
                    Dialogue = value as DialogueManager;
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
#if !DEBUG
            catch (Exception e)
            {
                LastError = e.Message;
                LastErrorDetails = e;
                if (!SuppressExceptions)
                {
                    throw e;
                }
            }
#else
            finally
            {

            }
#endif
        }

        public override void Start()
        {
            
        }
    }
}
