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
using DotRPG.Scripting;

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
        List<LuaModule> Scripts = new List<LuaModule>();
        List<Backdrop> backdrops = new List<Backdrop>();

        Dictionary<String, DynamicRectObject> props = new Dictionary<string, DynamicRectObject>();
        Dictionary<String, DynamicRectObject> interactable = new Dictionary<string, DynamicRectObject>();

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
        #region Loader
        public override void LoadContent()
        {
            cam.CameraVelocity = 300.0f;
            cam.DefaultHeight = 540;
            foreach (ResourceLoadTask rlt in resourceLoad)
            {
                switch (rlt.Resource)
                {
                    case ResourceType.Texture2D:
                        foreach (String x in FrameResources.Textures.Keys)
                        {
                            if (x == rlt.ResourceID)
                            {
                                goto end_t;
                            }
                        }
                        FrameResources.Textures.Add(rlt.ResourceID, Owner.Content.Load<Texture2D>(rlt.ResourcePath));
                        end_t: break;
                    case ResourceType.SoundEffect:
                        foreach (String x in FrameResources.Sounds.Keys)
                        {
                            if (x == rlt.ResourceID)
                            {
                                goto end_s;
                            }
                        }
                        FrameResources.Sounds.Add(rlt.ResourceID, Owner.Content.Load<SoundEffect>(rlt.ResourcePath));
                        end_s: break;
                    case ResourceType.SpriteFont:
                        foreach (String x in FrameResources.Textures.Keys)
                        {
                            if (x == rlt.ResourceID)
                            {
                                goto end_f;
                            }
                        }
                        FrameResources.Fonts.Add(rlt.ResourceID, Owner.Content.Load<SpriteFont>(rlt.ResourcePath));
                        end_f: break;
                    case ResourceType.Song:
                        foreach (String x in FrameResources.Textures.Keys)
                        {
                            if (x == rlt.ResourceID)
                            {
                                goto end_m;
                            }
                        }
                        FrameResources.Music.Add(rlt.ResourceID, Owner.Content.Load<Song>(rlt.ResourcePath));
                        end_m: break;
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
                        #endregion
                        props.Add(ID, new DynamicRectObject(startPos, colliderSize, mass, isStatic));
                        foreach (XElement xe2 in xe.Elements())
                        {
                            switch (xe2.Name.LocalName.ToLower())
                            {
                                case "sprite":
                                    props[ID].Sprite = XMLSceneLoader.LoadSpriteController(xe2, FrameResources);
                                    break;
                                case "interactable":
                                    interactable.Add(xe2.Attribute(XName.Get("interactAction")).Value, props[ID]);
                                    break;
                            }
                        }
                        break;
                    }
                    case "script":
                    {
                        String scriptContent = File.ReadAllText(Path.Combine(Owner.Content.RootDirectory, xe.Attribute(XName.Get("location")).Value));
                        Scripts.Add(new LuaModule(scriptContent));
                        break;
                    }
                    case "backdrop":
                    {
                        Texture2D t = null;
                        Vector2 p = Vector2.Zero;
                        foreach (XAttribute xa in xe.Attributes())
                        {
                            switch (xa.Name.LocalName)
                            {
                                case "local":
                                    t = FrameResources.Textures[xa.Value];
                                    break;
                                case "global":
                                    t = FrameResources.Global.Textures[xa.Value];
                                    break;
                                case "offset":
                                    p = XMLSceneLoader.ResolveVector2(xa.Value);
                                    break;
                            }
                        }
                        if (t != null)
                        {
                            backdrops.Add(new Backdrop(t, p));
                        }
                        break;
                    }
                }
            }
            cam.Focus = player.Location.ToPoint();
            foreach (LuaModule x in Scripts)
            {
                x.Runtime["obj"] = props;
            }
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
                                foreach (XElement xx in xe.Elements())
                                {
                                    x.Add(xx);
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
        #endregion
        public ScriptTest_Dynamic(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {

        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            Single loco_x = 0.0f; Single loco_y = 0.0f;
            if (controls[0]) { loco_y -= 1.0f; }
            if (controls[1]) { loco_y += 1.0f; }
            if (controls[2]) { loco_x -= 1.0f; }
            if (controls[3]) { loco_x += 1.0f; }
            Vector2 Locomotion = new Vector2(loco_x, loco_y);
            if (controls[0] && !(controls[1] || controls[2] || controls[3]))
            {
                player.SightDirection = Direction.Up;
            }
            if (controls[1] && !(controls[0] || controls[2] || controls[3]))
            {
                player.SightDirection = Direction.Down;
            }
            if (controls[2] && !(controls[1] || controls[0] || controls[3]))
            {
                player.SightDirection = Direction.Left;
            }
            if (controls[3] && !(controls[1] || controls[0] || controls[2]))
            {
                player.SightDirection = Direction.Right;
            }
            String newAnimSequence = player.Motion.FetchAnimationSequenceID(Locomotion.Length(), player.SightDirection);
            if (player.Sprite.CurrentAnimationSequence != newAnimSequence)
            {
                player.Sprite.SetAnimationSequence(newAnimSequence);
            }
            Locomotion /= (Locomotion.Length() != 0 ? Locomotion.Length() : 1.0f);
            Locomotion *= player.Motion.MovementSpeed;
            player.Velocity = Locomotion;
            player.Update(gameTime);
            foreach (String i in props.Keys)
            {
                props[i].Update(gameTime);
                player.TryCollideWith(props[i]);
                if (!props[i].Static)
                {
                    foreach (String x in props.Keys)
                    {
                        if (i == x)
                        {
                            continue;
                        }
                        props[i].TryCollideWith(props[x]);
                    }
                }
            }
            cam.TrackTarget = player.Location.ToPoint();
            cam.Update(gameTime);
            base.Update(gameTime, controls);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            foreach (Backdrop b in backdrops)
            {
                b.Draw(spriteBatch, cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), drawZone.Height / 540);
            }
            foreach (String i in props.Keys)
            {
                props[i].Draw(spriteBatch, gameTime, 540, cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(drawZone.Width, drawZone.Height), (0.3f - (0.1f * (props[i].Location.Y / 540))));
            }
            player.Draw(spriteBatch, gameTime, 540, cam.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(drawZone.Width, drawZone.Height), (0.3f - (0.1f * (player.Location.Y / 540))));
        }

        public override void Initialize()
        {
            
        }

        public override void UnloadContent()
        {
            FrameResources.Dispose();
            props.Clear();
            interactable.Clear();
            player = null;
            Scripts.Clear();
            backdrops.Clear();
        }
    }
}
