using System;
using DotRPG.Behavior;
using DotRPG.Behavior.Defaults;

namespace ScriptTest
{
    class DialogScript
    {
        public override void Update(String EventID, String ElapsedGameTime, String TotalGameTime)
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
                    Events.Enqueue(totalTime, 1000, "kickablereset")
                    Audio.PlayLocal("hit")
                    Camera.Shake(10, 10)
                    punch_count = punch_count + 1000.0f;
                    if (punch_count > 9000) {
                        Audio.PlayLocal("kaboom");
                        ObjectHeap.DisablePlayerControls();
                        ObjectHeap.SetAnimationSequence("tree", "explodes")
                        Events.Enqueue(totalTime, 1000, "treegone")
                    }
                    break;
                case "treegone":
                    ObjectHeap.SetActive("tree", false);
                    ObjectHeap.EnabledPlayerControls();
                    break;
            }
        }
    }
}