using System;

namespace DotRPG.Behavior
{
    /// <summary>
    /// Attribute used to declare XML-based frame constructors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XMLSceneBuilderAttribute : Attribute
    {
        public String BehaviorType;

        public XMLSceneBuilderAttribute(String behaviorType)
        {
            BehaviorType = behaviorType;
        }
    }
}
