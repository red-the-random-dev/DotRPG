using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotRPG.Behavior
{
    public static class XMLSceneLoader
    {
        /// <summary>
        /// Gets IXMLSceneBuilder instance appropriate for building scene of specified behavior.
        /// </summary>
        /// <param name="assembly">Assembly to search scene builder in.</param>
        /// <param name="behaviorType">Behavior type.</param>
        /// <returns></returns>
        public static IXMLSceneBuilder FetchSceneBuilder(Assembly assembly, String behaviorType)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (Attribute.IsDefined(type, typeof(XMLSceneBuilderAttribute)))
                {
                    if (type.BaseType == typeof(IXMLSceneBuilder))
                    {
                        Object[] attrs = type.GetCustomAttributes(false);
                        
                        foreach (Object x in attrs)
                        {
                            if (x is XMLSceneBuilderAttribute)
                            {
                                if (((XMLSceneBuilderAttribute)x).BehaviorType == behaviorType)
                                {
                                    return (IXMLSceneBuilder)Activator.CreateInstance(type);
                                }
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("Unable to fetch appropriate scene builder instance for behavior type \""+behaviorType+"\" among registered types.");
        }
    }
}
