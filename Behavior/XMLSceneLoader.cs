using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
        /// <param name="selfBuilt">Set to True if corresponding builder is declared as self-built frame.</param>
        /// <param name="builder">Builder type. Used for later instantiation of frame with given parameters.</param>
        /// <returns>IXMLSceneBuilder instance. If scene builder is declared as self-built frame, a NULL value is returned.</returns>
        public static IXMLSceneBuilder FetchSceneBuilder(Assembly assembly, String behaviorType, out Boolean selfBuilt, out Type builder)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (Attribute.IsDefined(type, typeof(SceneBuilderAttribute)))
                {
                    if (type.BaseType == typeof(IXMLSceneBuilder))
                    {
                        Object[] attrs = type.GetCustomAttributes(false);
                        
                        foreach (Object x in attrs)
                        {
                            if (x is SceneBuilderAttribute)
                            {
                                SceneBuilderAttribute y = (SceneBuilderAttribute)x;
                                if (y.BehaviorType == behaviorType && !y.SelfBuilt)
                                {
                                    builder = type;
                                    selfBuilt = y.SelfBuilt;
                                    return (IXMLSceneBuilder)Activator.CreateInstance(type);
                                }
                                else if (y.BehaviorType == behaviorType)
                                {
                                    builder = type;
                                    selfBuilt = y.SelfBuilt;
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("Unable to fetch appropriate scene builder instance for behavior type \""+behaviorType+"\" among registered types.");
        }

        public static Frame LoadFrame(XDocument Document, Assembly lookIn, Object[] buildData, out String literalName)
        {
            if (Document.DocumentType.Name != "dotrpg-frame")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from document of following type: " + Document.DocumentType.Name + " (type \"dotrpg-frame\" expected).");
            }
            XElement rt = Document.Root;
            if (rt.Name.ToString().ToLower() != "scene")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from root tag of following type: <"+ rt.Name+"> (type <Scene> expected).");
            }
            literalName = rt.Attribute(XName.Get("id")).Value;
            String behaviorType = rt.Attribute(XName.Get("behaviorType")).Value;
            Boolean selfBuilt = false;
            Type t;
            IXMLSceneBuilder ixsb = FetchSceneBuilder(lookIn, behaviorType, out selfBuilt, out t);

            if (ixsb == null && selfBuilt)
            {
                ixsb = (IXMLSceneBuilder)Activator.CreateInstance(t, buildData);
            }

            return ixsb.BuildFromXML(Document, buildData);
        }
    }
}
