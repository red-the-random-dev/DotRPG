using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Objects.Dynamics
{
    public class MotionParameters
    {
        public Single MovementSpeed;
        public String IdleAnimation;
        public String WalkingAnimation;

        public MotionParameters(Single velocity = 256.0f, String idleAnim = "default", String walkingAnim = "default")
        {
            MovementSpeed = velocity;
            IdleAnimation = idleAnim;
            WalkingAnimation = walkingAnim;
        }

        public String FetchAnimationSequenceID(Single velocity, Direction direction)
        {
            if (velocity == 0.0f)
            {
                return String.Format(IdleAnimation, direction.ToString().ToLower());
            }
            else
            {
                return String.Format(WalkingAnimation, direction.ToString().ToLower());
            }
        }
    }
}
