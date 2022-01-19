using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Construct;

namespace DotRPG.Behavior
{
    public static class XMLSceneLoader
    {
        public static IXMLSceneBuilder FetchSceneBuilder(Assembly assembly, String behaviorType, out Boolean selfBuilt, out Type builder)
        {
            return FetchSceneBuilder(assembly.GetTypes(), behaviorType, out selfBuilt, out builder);
        }

        /// <summary>
        /// Gets IXMLSceneBuilder instance appropriate for building scene of specified behavior.
        /// </summary>
        /// <param name="types">List of types to look for appropriate builder in.</param>
        /// <param name="behaviorType">Behavior type.</param>
        /// <param name="selfBuilt">Set to True if corresponding builder is declared as self-built frame.</param>
        /// <param name="builder">Builder type. Used for later instantiation of frame with given parameters.</param>
        /// <returns>IXMLSceneBuilder instance. If scene builder is declared as self-built frame, a NULL value is returned.</returns>
        public static IXMLSceneBuilder FetchSceneBuilder(Type[] types, String behaviorType, out Boolean selfBuilt, out Type builder)
        {
            foreach (Type type in types)
            {
                if (Attribute.IsDefined(type, typeof(SceneBuilderAttribute)))
                {
                    if (typeof(IXMLSceneBuilder).IsAssignableFrom(type))
                    {
                        Object[] attrs = type.GetCustomAttributes(false);

                        foreach (Object x in attrs)
                        {
                            if (x is SceneBuilderAttribute y)
                            {
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
            throw new InvalidOperationException("Unable to fetch appropriate scene builder instance for behavior type \"" + behaviorType + "\" among registered types.");
        }

        public static Frame LoadFrame(XDocument Document, Assembly[] lookIn, object[] buildData, out String literalName)
        {
            foreach (Assembly a in lookIn)
            {
                try
                {
                    return LoadFrame(Document, a, buildData, out literalName);
                }
                catch (InvalidOperationException)
                {
                    Console.Error.WriteLine("Unable to find reference in " + a.FullName);
                }
                catch (SerializationException e)
                {
                    throw e;
                }
            }
            throw new InvalidOperationException("Unable to fetch appropriate scene builder instance for referenced document.");
        }

        public static Frame LoadFrame(XDocument Document, Object[] buildData, out String literalName)
        {
            return LoadFrame(Document, SceneBuilderAttribute.SceneBuilderRegistry.ToArray(), buildData, out literalName);
        }
        public static Frame LoadFrame(XDocument Document, Type[] typeSet, Object[] buildData, out String literalName)
        {
            if (Document.DocumentType.Name != "dotrpg-frame")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from document of following type: " + Document.DocumentType.Name + " (type \"dotrpg-frame\" expected).");
            }
            XElement rt = Document.Root;
            if (rt.Name.LocalName.ToString().ToLower() != "scene")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from root tag of following type: <" + rt.Name + "> (type <Scene> expected).");
            }
            literalName = rt.Attribute(XName.Get("id")).Value;
            String behaviorType = rt.Attribute(XName.Get("behaviorType")).Value;
            IXMLSceneBuilder ixsb = FetchSceneBuilder(typeSet, behaviorType, out bool selfBuilt, out Type t);

            if (ixsb == null && selfBuilt)
            {
                ixsb = (IXMLSceneBuilder)Activator.CreateInstance(t, buildData);
            }

            return ixsb.BuildFromXML(Document, buildData);
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
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from root tag of following type: <" + rt.Name + "> (type <Scene> expected).");
            }
            literalName = rt.Attribute(XName.Get("id")).Value;
            String behaviorType = rt.Attribute(XName.Get("behaviorType")).Value;
            IXMLSceneBuilder ixsb = FetchSceneBuilder(lookIn, behaviorType, out bool selfBuilt, out Type t);

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
                ResourceLoadTask rlt = new ResourceLoadTask
                {
                    Resource = xe2.Name.LocalName.ToString().ToLower() switch
                    {
                        "texture2d" => ResourceType.Texture2D,
                        "soundeffect" => ResourceType.SoundEffect,
                        "spritefont" => ResourceType.SpriteFont,
                        "song" => ResourceType.Song,
                        "effect" => ResourceType.Effect,
                        _ => throw new SerializationException("No such resource type: " + xe2.Name.ToString()),
                    },
                    ResourceID = xe2.Attribute(XName.Get("id")).Value,
                    ResourcePath = xe2.Attribute(XName.Get("location")).Value,
                    LoadFrom = ResourceLocation.ContentFolder
                };
                yield return rlt;
            }
        }

        public static void GetPrefab(String elementName, String prefabFileName, Dictionary<String, ObjectPrototype> prefabs, out String ID, out ObjectPrototype[] include, List<ResourceLoadTask> addResources = null)
        {
            XDocument Document = XDocument.Parse(File.ReadAllText(prefabFileName));
            List<ObjectPrototype> includes = new List<ObjectPrototype>();
            if (Document.DocumentType.Name != "dotrpg-prefab")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load prototype data from document of following type: " + Document.DocumentType.Name + " (type \"dotrpg-prefab\" expected).");
            }
            XElement rt = Document.Root;
            if (rt.Name.LocalName.ToString().ToLower() != "objectprefab")
            {
                throw new System.Runtime.Serialization.SerializationException("Unable to load data from root tag of following type: <" + rt.Name + "> (type <ObjectPrefab> expected).");
            }
            ID = rt.Attribute(XName.Get("id")).Value;
            if (prefabs.ContainsKey(ID))
            {
                include = includes.ToArray();
                return;
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
                else if (xe.Name.LocalName.ToLower() == "include")
                {
                    foreach (XElement xe2 in xe.Elements())
                    {
                        includes.Add(ObjectPrototype.FromXML(xe2));
                    }
                }
                else if (xe.Name.LocalName.ToLower() == elementName.ToLower())
                {
                    prefabs.Add(ID, ObjectPrototype.FromXML(xe));
                    include = includes.ToArray();
                    return;
                }
            }
            throw new SerializationException("No data found for following tag: " + elementName + ".");
        }

        public static Vector2 ResolveVector2(String intake)
        {
            String a = "";
            foreach (Char x in intake)
            {
                if (x != ' ')
                {
                    a += x;
                }
            }
            String[] dims = a.Split(',');
            if (dims.Length < 2)
            {
                throw new SerializationException(String.Format("Unable to load Vector2 from {0}-dimensional string vector.", dims.Length));
            }
            return new Vector2(Single.Parse(dims[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
        }

        public static Vector3 ResolveVector3(String intake)
        {
            String a = "";
            foreach (Char x in intake)
            {
                if (x != ' ')
                {
                    a += x;
                }
            }
            String[] dims = a.Split(',');
            if (dims.Length < 3)
            {
                throw new SerializationException(String.Format("Unable to load Vector3 from {0}-dimensional string vector.", dims.Length));
            }
            return new Vector3(Single.Parse(dims[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
        }

        public static Vector4 ResolveVector4(String intake)
        {
            String a = "";
            foreach (Char x in intake)
            {
                if (x != ' ')
                {
                    a += x;
                }
            }
            String[] dims = a.Split(',');
            if (dims.Length < 4)
            {
                throw new SerializationException(String.Format("Unable to load Vector4 from {0}-dimensional string vector.", dims.Length));
            }
            return new Vector4(Single.Parse(dims[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), Single.Parse(dims[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
        }
        public static Vector4 ResolveColorVector4(String intake)
        {
            String a = "";
            foreach (Char x in intake)
            {
                if (x != ' ')
                {
                    a += x;
                }
            }
            String[] dims = a.Split(',');
            if (dims.Length < 4)
            {
                throw new SerializationException(String.Format("Unable to load Vector4 from {0}-dimensional string vector.", dims.Length));
            }
            return new Vector4(Single.Parse(dims[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture) / 255.0f, Single.Parse(dims[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture) / 255.0f, Single.Parse(dims[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture) / 255.0f, Single.Parse(dims[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture) / 255.0f);
        }

        public static SpriteController LoadSpriteController(XElement xe2, ResourceHeap FrameResources)
        {
            SpriteController sc = null;
            Single frameTime = 1000.0f / Single.Parse(xe2.Attribute(XName.Get("frameRate")).Value);
            foreach (XElement xe3 in xe2.Elements())
            {
                switch (xe3.Name.LocalName.ToLower())
                {
                    case "defaultanimationsequence":
                        {
                            Texture2D defAnim = null;
                            UInt16 frameAmount = 1;
                            foreach (XAttribute xa in xe3.Attributes())
                            {
                                switch (xa.Name.LocalName)
                                {
                                    case "local":
                                        defAnim = FrameResources.Textures[xa.Value];
                                        break;
                                    case "global":
                                        defAnim = FrameResources.Global.Textures[xa.Value];
                                        break;
                                    case "frameAmount":
                                        frameAmount = UInt16.Parse(xa.Value);
                                        break;
                                }
                            }
                            if (defAnim == null)
                            {
                                throw new SerializationException("Unable to load animation sequence data for: default.");
                            }
                            sc = new SpriteController(frameTime, defAnim, frameAmount);
                            break;
                        }
                    case "animation":
                        {
                            String ID = xe3.Attribute(XName.Get("id")).Value;
                            Texture2D Anim = null;
                            UInt16 frameAmount = 1;
                            foreach (XAttribute xa in xe3.Attributes())
                            {
                                switch (xa.Name.LocalName)
                                {
                                    case "local":
                                        Anim = FrameResources.Textures[xa.Value];
                                        break;
                                    case "global":
                                        Anim = FrameResources.Global.Textures[xa.Value];
                                        break;
                                    case "frameAmount":
                                        frameAmount = UInt16.Parse(xa.Value);
                                        break;
                                }
                            }
                            if (Anim == null)
                            {
                                throw new SerializationException("Unable to load animation sequence data for: default.");
                            }
                            sc.AddAnimationSequence(ID, Anim, frameAmount);
                            break;
                        }
                }
            }
            return sc;
        }
        public static SpriteController LoadSpriteController(ObjectPrototype op2, ResourceHeap FrameResources)
        {
            SpriteController sc = null;
            Single frameTime = 1000.0f / Single.Parse(op2.Properties["frameRate"]);
            foreach (ObjectPrototype op3 in op2.Subnodes)
            {
                switch (op3.Name.ToLower())
                {
                    case "defaultanimationsequence":
                        {
                            Texture2D defAnim = null;
                            UInt16 frameAmount = 1;
                            foreach (String opa in op3.Properties.Keys)
                            {
                                switch (opa)
                                {
                                    case "local":
                                        defAnim = FrameResources.Textures[op3.Properties[opa]];
                                        break;
                                    case "global":
                                        defAnim = FrameResources.Global.Textures[op3.Properties[opa]];
                                        break;
                                    case "frameAmount":
                                        frameAmount = UInt16.Parse(op3.Properties[opa]);
                                        break;
                                }
                            }
                            if (defAnim == null)
                            {
                                throw new SerializationException("Unable to load animation sequence data for: default.");
                            }
                            sc = new SpriteController(frameTime, defAnim, frameAmount);
                            break;
                        }
                    case "animation":
                        {
                            String ID = op3.Properties["id"];
                            Texture2D Anim = null;
                            UInt16 frameAmount = 1;
                            foreach (String opa in op3.Properties.Keys)
                            {
                                switch (opa)
                                {
                                    case "local":
                                        Anim = FrameResources.Textures[op3.Properties[opa]];
                                        break;
                                    case "global":
                                        Anim = FrameResources.Global.Textures[op3.Properties[opa]];
                                        break;
                                    case "frameAmount":
                                        frameAmount = UInt16.Parse(op3.Properties[opa]);
                                        break;
                                }
                            }
                            if (Anim == null)
                            {
                                throw new SerializationException("Unable to load animation sequence data for: default.");
                            }
                            sc.AddAnimationSequence(ID, Anim, frameAmount);
                            break;
                        }
                }
            }
            return sc;
        }
    }
}
