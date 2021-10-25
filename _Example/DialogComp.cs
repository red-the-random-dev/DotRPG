using System;
using DotRPG.Waypoints;
using DotRPG.Behavior.Defaults;
using DotRPG.Scripting;

namespace DotRPG._Example
{
    [BuiltScript("testroom00")]
    public class DialogScript : TopDownFrameScript
    {
        Single PunchCount = 0.0f;
        Waypoint w1;
        Waypoint w5;
        public override bool RequireRawSceneData => true;
        public override bool RequireResourceHeap => true;

        public override void Start()
        {
            // Palette.SetMixWithGlobal("treecolor", false);
            w1 = new Waypoint(0, 0);
            Waypoint w2 = new Waypoint(2, 0);
            Waypoint w3 = new Waypoint(3, 0);
            Waypoint w4 = new Waypoint(3, 1);
            w5 = new Waypoint(4, 0);
            w1.SetNeighbor(w2);
            w2.SetNeighbor(w3);
            w2.SetNeighbor(w4);
            w3.SetNeighbor(w5);
            w4.SetNeighbor(w5);
        }
        public override void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            switch (EventID)
            {
                case "default":
                    WaypointGraph.BuildPath(w1, w5, out Single x);
                    if (ObjectHeap.GetCurrentAnimationSequence("tree") == "default")
                        Palette.SetColor("treecolor", 255, (byte)Math.Max(1, 255 - (255 * PunchCount / 9000)), (byte)Math.Max(1, 255 - (255 * PunchCount / 9000)), 255);
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
                        Palette.SetColor("treecolor", 255, 255, 255, 255);
                        // Palette.SetColor("global", 255, 0, 0, 255);
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
                case "set_r":
                    Palette.SetColor("global", 255, 0, 0, 255);
                    Audio.PlayLocal("sam_r");
                    break;
                case "set_g":
                    Palette.SetColor("global", 0, 255, 0, 255);
                    Audio.PlayLocal("sam_g");
                    break;
                case "set_b":
                    Palette.SetColor("global", 0, 0, 255, 255);
                    Audio.PlayLocal("sam_b");
                    break;
                case "set_w":
                    Audio.PlayLocal("sam_reset");
                    Palette.SetColor("global", 255, 255, 255, 255);
                    break;
            }
        }
    }
}