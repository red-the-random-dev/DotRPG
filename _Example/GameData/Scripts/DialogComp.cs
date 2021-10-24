using System;
using DotRPG.Behavior;
using DotRPG.Behavior.Defaults;
using DotRPG.Scripting;

namespace DotRPG._Example
{
    [BuiltScript("tree")]
    public class DialogScript : TopDownFrameScript
    {
        public override void Update(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            Single PunchCount = 0.0f;
            switch (EventID)
            {
                case "default":
                    PunchCount -= ElapsedGameTime;
                    if (PunchCount < 0.0f)
                    {
                        PunchCount = 0.0f;
                    }
                    break;
                case "treetalk":
                    Events.Enqueue(TotalGameTime, 1000.0f, "kickablereset");
                    Audio.PlayLocal("hit");
                    Camera.Shake(10, 10);
                    PunchCount = PunchCount + 1000.0f;
                    if (PunchCount > 9000) {
                        Audio.PlayLocal("kaboom");
                        ObjectHeap.DisablePlayerControls();
                        ObjectHeap.SetAnimationSequence("tree", "explodes");
                        Events.Enqueue(TotalGameTime, 1000.0f, "treegone");
                    }
                    break;
                case "treegone":
                    ObjectHeap.SetActive("tree", false);
                    ObjectHeap.EnablePlayerControls();
                    break;
            }
        }
    }
}