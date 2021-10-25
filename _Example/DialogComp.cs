using System;
using DotRPG.Behavior;
using DotRPG.Behavior.Defaults;
using DotRPG.Scripting;

namespace DotRPG._Example
{
    [BuiltScript("testroom00")]
    public class DialogScript : TopDownFrameScript
    {
        Single PunchCount = 0.0f;
        public override bool RequireRawSceneData => true;
        public override bool RequireResourceHeap => true;
        public override void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            switch (EventID)
            {
                case "default":
                    PunchCount -= ElapsedGameTime;
                    if (PunchCount < 0.0f)
                    {
                        PunchCount = 0.0f;
                    }
                    break;
                case "treeouch":
                    Events.Enqueue(TotalGameTime, 1000.0f, "kickablereset");
                    Audio.PlayLocal("hit");
                    Camera.Shake(10, 10);
                    ObjectHeap.SetPlayerAnimationSequence("red.idle." + Scene.Player.SightDirection.ToString().ToLower());
                    ObjectHeap.EnablePlayerControls();
                    PunchCount += 2500.0f;
                    if (PunchCount > 9000)
                    {
                        Audio.PlayLocal("kaboom");
                        ObjectHeap.DisablePlayerControls();
                        ObjectHeap.SetAnimationSequence("tree", "explodes");
                        Events.Enqueue(TotalGameTime, 1000.0f, "treegone");
                    }
                    break;
                case "treetalk":
                    ObjectHeap.DisablePlayerControls();
                    ObjectHeap.SetPlayerAnimationSequence("red.punch." + Scene.Player.SightDirection.ToString().ToLower());
                    Events.Enqueue(TotalGameTime, 666.0f, "treeouch");
                    break;
                case "treegone":
                    ObjectHeap.SetActive("tree", false);
                    ObjectHeap.EnablePlayerControls();
                    break;
            }
        }
    }
}