using System;
using DotRPG.Waypoints;
using DotRPG.Behavior.Defaults;
using DotRPG.Scripting;

namespace DotRPG._Example
{
    [BuiltScript("testroom_00")]
    public class TestRoomScript : TopDownFrameScript
    {
        Single PunchCount = 0.0f;
        public override bool RequireRawSceneData => true;
        public override bool RequireResourceHeap => true;

        public override void Start()
        {
            ObjectHeap.Prefab_CreateFromStatic("whiterectbullet", "whiterect");
            ObjectHeap.Prefab_CreateNew("bulletscript", "ObjectScript");
            ObjectHeap.Prefab_CreateNew("empty", "IgnoreCollisions");
            ObjectHeap.Prefab_SetProperty("bulletscript", "location", "Scripts/bullet.lua");
            ObjectHeap.Prefab_AddSubnode("whiterectbullet", "bulletscript");
            ObjectHeap.Prefab_AddSubnode("whiterectbullet", "empty");
            // ObjectHeap.Prefab_Dispose("bulletscript");
            // ObjectHeap.Prefab_Dispose("empty");
            ObjectHeap.Prefab_SetProperty("whiterectbullet", "startPos", "128,160");
        }
        public override void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            switch (EventID)
            {
                case "default":
                    Pathfinder.BuildPath("whiterectchase", Pathfinder.GetClosestWaypointToObject("whiterect2"), Pathfinder.GetClosestWaypointToPlayer());
                    Pathfinder.ForceMoveObjectByPath("whiterect2", "whiterectchase", 256, 16);
                    if (ObjectHeap.Exists("tree"))
                    {
                        if (ObjectHeap.GetCurrentAnimationSequence("tree") == "default")
                            Palette.SetColor("treecolor", 255, (byte)Math.Max(1, 255 - (255 * PunchCount / 9000)), (byte)Math.Max(1, 255 - (255 * PunchCount / 9000)), 255);
                        else
                            ObjectHeap.Prefab_DeployUnderRandomName("whiterectbullet");
                    }
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
                    ObjectHeap.DestroyObject("tree");
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