using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DotRPG.Objects;
using DotRPG.Behavior;
using Microsoft.Xna.Framework;
using DotRPG.Objects.Dynamics;
using System.Runtime.Serialization;
using System.IO;

namespace DotRPG.Example
{
    [SceneBuilder("example/topdown", true)]
    public class ScriptTest_Dynamic : Frame, IXMLSceneBuilder
    {
        PlayerObject player;
        Int32 _id;
        List<ResourceLoadTask> resourceLoad = new List<ResourceLoadTask>();
        List<XElement> objectPrototypes = new List<XElement>();

        public override int FrameID
        {
            get
            {
                return _id;
            }
        }

        public ScriptTest_Dynamic(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {

        }

        public Frame BuildFromXML(XDocument Document, Object[] parameters)
        {
            XElement root = Document.Root;
            if (root.Name.LocalName.ToString() != "Scene" || root.Attribute(XName.Get("behaviorType")).Value != "example/topdown")
            {
                throw new SerializationException("Attempted to load scene with mismatching behavior.");
            }
            _id = Int32.Parse(root.Attribute(XName.Get("index")).Value);

            foreach (XElement xe in root.Elements())
            {
                switch (xe.Name.LocalName.ToString().ToLower())
                {
                    case "require":
                    {
                        foreach (ResourceLoadTask rlt in XMLSceneLoader.FetchLoadTasks(xe))
                        {
                            resourceLoad.Add(rlt);
                        }
                        break;
                    }
                    default:
                        XElement x = xe;
                        foreach (XAttribute xa in xe.Attributes())
                        {
                            if (xa.Name.LocalName.ToString() == "_usePrefab")
                            {
                                String loadPath = Path.Combine(Owner.Content.RootDirectory, xa.Value.ToString());

                            }
                        }
                        break;
                }
            }

            return this;
        }
    }
}
