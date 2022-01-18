using System;
using DotRPG.Behavior.Data;
using DotRPG.Behavior.Defaults;
using DotRPG.Scripting;
using DotRPG.UI;
using Microsoft.Xna.Framework;

namespace DotRPG._Example
{
    [BuiltScript("testroom_01")]
    public class GlitchRoomScript : TopDownFrameScript
    {
        public override void Start()
        {
            Audio.BufferLocal("50hz", "scare");
        }
        public override void UpdateInternal(String EventID, Single ElapsedGameTime, Single TotalGameTime)
        {
            switch (EventID)
            {
                case "default":
                    PostProcess.LineSkip = (Byte)Math.Truncate(Math.Clamp(512.0f / ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 255));
                    PostProcess.LinePaint = (Byte)Math.Truncate(Math.Clamp(128.0f / ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 255));
                    PostProcess.LineShift = (Byte)Math.Truncate(Math.Clamp(256.0f / ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 255));
                    Audio.Play("scare");
                    Audio.SetParameters("scare", Math.Clamp(8.0f / ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 1), ObjectHeap.GetDistanceToPlayer("whiterect") < 96 ? (Single)PostProcess.GlitchGen.NextDouble() : 0, ObjectHeap.GetSoundPanning("whiterect"));
                    PostProcess.TintG = (Byte)Math.Truncate(Math.Clamp(ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 255));
                    PostProcess.TintB = (Byte)Math.Truncate(Math.Clamp(ObjectHeap.GetDistanceToPlayer("whiterect"), 0, 255));
                    break;
            }
        }
    }
}