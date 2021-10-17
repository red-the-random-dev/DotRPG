using System;
using System.Collections.Generic;

namespace DotRPG.Behavior
{
    /// <summary>
    /// Attribute used to declare XML-based frame constructors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SceneBuilderAttribute : Attribute
    {
        public static List<Type> SceneBuilderRegistry = new List<Type>();

        public String BehaviorType;
        public Boolean SelfBuilt;
        /// <summary>
        /// Labels class as DotRPG scene builder for cetrain behavior.
        /// </summary>
        /// <param name="behaviorType">Name of behavior type. Preferred: &lt;game-name&gt;/&lt;behavior-name&gt;</param>
        /// <param name="selfBuilt">Defines whether attribute belongs to built frame type itself. Useful for self-building frames with many private fields.</param>
        public SceneBuilderAttribute(String behaviorType, Boolean selfBuilt = false) : base()
        {
            BehaviorType = behaviorType;
            SelfBuilt = selfBuilt;
        }

        public SceneBuilderAttribute(String behaviorType, Type register, Boolean selfBuilt = false) : base()
        {
            BehaviorType = behaviorType;
            SelfBuilt = selfBuilt;
            SceneBuilderAttribute.SceneBuilderRegistry.Add(register);
        }
    }
}
