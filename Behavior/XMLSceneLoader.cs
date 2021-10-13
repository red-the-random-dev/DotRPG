using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using DotRPG.Objects;

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
            if (rt.Name.LocalName.ToString().ToLower() != "scene")
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

        public static IEnumerable<ResourceLoadTask> FetchLoadTasks(XElement loadRoot)
        {
            foreach (XElement xe2 in loadRoot.Elements())
            {
                ResourceLoadTask rlt = new ResourceLoadTask();
                switch (xe2.Name.LocalName.ToString().ToLower())
                {
                    case "texture2d":
                        rlt.Resource = ResourceType.Texture2D;
                        break;
                    case "soundeffect":
                        rlt.Resource = ResourceType.SoundEffect;
                        break;
                    case "spritefont":
                        rlt.Resource = ResourceType.SpriteFont;
                        break;
                    case "song":
                        rlt.Resource = ResourceType.Song;
                        break;
                    default:
                        throw new SerializationException("No such resource type: " + xe2.Name.ToString());
                }
                rlt.ResourceID = xe2.Attribute(XName.Get("id")).Value;
                rlt.ResourcePath = xe2.Attribute(XName.Get("location")).Value;
                rlt.LoadFrom = ResourceLocation.ContentFolder;
                yield return rlt;
            }
        }

        public static XElement GetObjectPrototype(String elementName, String prefabFileName, List<ResourceLoadTask> addResources = null)
        {
            XDocument Document = XDocument.Parse(File.ReadAllText(prefabFileName));

            if (Document.DocumentType.Name != "dotrpg-prefab")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load prototype data from document of following type: " + Document.DocumentType.Name + " (type \"dotrpg-prefab\" expected).");
            }
            XElement rt = Document.Root;
            if (rt.Name.LocalName.ToString().ToLower() != "objectprefab")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from root tag of following type: <" + rt.Name + "> (type <ObjectPrefab> expected).");
            }
            foreach (XElement xe in Document.Root.Elements())
            {
                if (xe.Name.LocalName.ToLower() == "require" && addResources != null)
                {
                    foreach (ResourceLoadTask rlt in FetchLoadTasks(xe))
                    {
                        addResources.Add(rlt);
                    }
                }
                else if (xe.Name.LocalName.ToLower() == elementName.ToLower())
                {
                    return xe;
                }
            }
            throw new SerializationException("No data found for following tag: " + elementName + ".");
        }
    }
}
