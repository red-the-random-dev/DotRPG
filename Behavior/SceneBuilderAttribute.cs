using System;

namespace DotRPG.Behavior
{
    /// <summary>
    /// Attribute used to declare XML-based frame constructors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SceneBuilderAttribute : Attribute
    {
        public String BehaviorType;
        public Boolean SelfBuilt;
        /// <summary>
        /// Labels class as DotRPG scene builder for cetrain behavior.
        /// </summary>
        /// <param name="behaviorType">Name of behavior type. Preferred: &lt;game-name&gt;/&lt;behavior-name&gt;</param>
        /// <param name="selfBuilt">Defines whether attribute belongs to built frame type itself. Useful for self-building frames with many private fields.</param>
        public SceneBuilderAttribute(String behaviorType, Boolean selfBuilt = false)
        {
            BehaviorType = behaviorType;
            SelfBuilt = selfBuilt;
        }
    }
}
