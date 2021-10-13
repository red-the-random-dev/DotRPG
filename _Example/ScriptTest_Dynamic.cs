using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DotRPG.Objects;
using DotRPG.Behavior;
using Microsoft.Xna.Framework;
using DotRPG.Objects.Dynamics;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DotRPG.Example
{
    [SceneBuilder("example/topdown", true)]
    public class ScriptTest_Dynamic : Frame, IXMLSceneBuilder
    {
        CameraFrameObject cam = new CameraFrameObject();
        PlayerObject player;
        Int32 _id;
        readonly List<ResourceLoadTask> resourceLoad = new List<ResourceLoadTask>();
        List<XElement> objectPrototypes = new List<XElement>();

        Dictionary<String, DynamicRectObject> props = new Dictionary<string, DynamicRectObject>();

        public override int FrameID
        {
            get
            {
                return _id;
            }
        }

        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            cam.CameraVelocity = 300.0f;
            cam.DefaultHeight = 540;
            foreach (ResourceLoadTask rlt in resourceLoad)
            {
                switch (rlt.Resource)
                {
                    case ResourceType.Texture2D:
                        FrameResources.Textures.Add(rlt.ResourceID, Owner.Content.Load<Texture2D>(rlt.ResourcePath));
                        break;
                    case ResourceType.SoundEffect:
                        FrameResources.Sounds.Add(rlt.ResourceID, Owner.Content.Load<SoundEffect>(rlt.ResourcePath));
                        break;
                    case ResourceType.SpriteFont:
                        FrameResources.Fonts.Add(rlt.ResourceID, Owner.Content.Load<SpriteFont>(rlt.ResourcePath));
                        break;
                    case ResourceType.Song:
                        FrameResources.Music.Add(rlt.ResourceID, Owner.Content.Load<Song>(rlt.ResourcePath));
                        break;
                }
            }
            foreach (XElement xe in objectPrototypes)
            {
                switch (xe.Name.LocalName.ToLower())
                {
                    case "player":
                    {
                        #region Parameters definition
                        Point startPos = XMLSceneLoader.ResolveVector2(xe.Attribute(XName.Get("startPos")).Value).ToPoint();
                        Point colliderSize = XMLSceneLoader.ResolveVector2(xe.Attribute(XName.Get("colliderSize")).Value).ToPoint();
                        Point interactFieldSize = XMLSceneLoader.ResolveVector2(xe.Attribute(XName.Get("interactFieldSize")).Value).ToPoint();
                        Single mass = Single.Parse(xe.Attribute(XName.Get("mass")).Value);
                        player = new PlayerObject(startPos, colliderSize, mass, interactFieldSize);
                        #endregion
                        foreach (XElement xe2 in xe.Elements())
                        {
                            switch (xe2.Name.LocalName.ToLower())
                            {
                                case "sprite":
                                    player.Sprite = XMLSceneLoader.LoadSpriteController(xe2, FrameResources);
                                    break;
                                case "motion":
                                    player.Motion.MovementSpeed = Single.Parse(xe2.Attribute(XName.Get("movementSpeed")).Value);
                                    foreach (XElement xe3 in xe2.Elements())
                                    {
                                        switch (xe3.Name.LocalName.ToLower())
                                        {
                                            case "idleanimation":
                                                player.Motion.IdleAnimation = xe3.Attribute(XName.Get("id")).Value;
                                                break;
                                            case "walkinganimation":
                                                player.Motion.WalkingAnimation = xe3.Attribute(XName.Get("id")).Value;
                                                break;
                                        }
                                    }
                                    break;

                            }
                        }
                        break;
                    }
                    case "prop":
                    {
                        #region Parameters definition
                        String ID = xe.Attribute(XName.Get("id")).Value;
                        Point startPos = XMLSceneLoader.ResolveVector2(xe.Attribute(XName.Get("startPos")).Value).ToPoint();
                        Point colliderSize = XMLSceneLoader.ResolveVector2(xe.Attribute(XName.Get("colliderSize")).Value).ToPoint();
                        Boolean isStatic = Boolean.Parse(xe.Attribute(XName.Get("static")).Value);
                        Single mass = Single.Parse(xe.Attribute(XName.Get("mass")).Value);
                        props.Add(ID, new DynamicRectObject(startPos, colliderSize, mass, isStatic));

                        #endregion
                        foreach (XElement xe2 in xe.Elements())
                        {
                            switch (xe2.Name.LocalName.ToLower())
                            {
                                case "sprite":
                                    props[ID].Sprite = XMLSceneLoader.LoadSpriteController(xe2, FrameResources);
                                    break;
                            }
                        }
                        break;
                    }
                }
            }
            throw new NotImplementedException();
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
                                String loadPath = Path.GetFullPath(Path.Combine(Owner.Content.RootDirectory, xa.Value.ToString()));
                                x = XMLSceneLoader.GetObjectPrototype(xe.Name.LocalName, loadPath, resourceLoad);
                                // Override prefab's attributes with specified attributes' values
                                foreach (XAttribute xx in xe.Attributes())
                                {
                                    x.SetAttributeValue(xx.Name, xx.Value);
                                }
                                break;
                            }
                        }
                        objectPrototypes.Add(x);
                        break;
                }
            }

            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
