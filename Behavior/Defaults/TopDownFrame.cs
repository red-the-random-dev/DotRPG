using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DotRPG.Objects;
using Microsoft.Xna.Framework;
using DotRPG.Objects.Dynamics;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using DotRPG.Scripting;
using DotRPG.Behavior.Routines;
using Microsoft.Xna.Framework.Input;
using DotRPG.Waypoints;
using DotRPG.Behavior.Management;

namespace DotRPG.Behavior.Defaults
{
    [SceneBuilder("default/topdown", typeof(TopDownFrame), true)]
    public class TopDownFrame : Frame, IXMLSceneBuilder, ILoadable
    {
        public String DebugText = "";
        public CameraFrameObject Camera { get; private set; } = new CameraFrameObject();
        public CameraManager CameraManager { get; private set; }
        public ObjectHeapManager ObjectManager { get; private set; }
        public SoundManager Audio { get; private set; }
        public PaletteManager Palette { get; private set; }
        public PathfindingManager Pathfinder { get; private set; }
        public ScriptEventManager EventTimer { get; private set; }
        public PlayerObject Player;
        Int32 _id;
        readonly List<ResourceLoadTask> resourceLoad = new List<ResourceLoadTask>();
        List<XElement> objectPrototypes = new List<XElement>();
        List<IScriptModule> Scripts = new List<IScriptModule>();
        public List<Backdrop> backdrops { get; private set; } = new List<Backdrop>();
        public Dictionary<String, Waypoint> NavMap { get; private set; }  = new Dictionary<string, Waypoint>();

        public Dictionary<String, DynamicRectObject> Props { get; private set; } = new Dictionary<string, DynamicRectObject>();
        public Dictionary<String, DynamicRectObject> Interactable { get; private set; } = new Dictionary<string, DynamicRectObject>();

        Int32 LastMWheelValue = 0;
        public Boolean[] LastInput { get; private set; } = new bool[8];

        Boolean AllowManualZoom = false;
        Boolean SuppressScriptExceptions = false;

        #region ILoadable implementation
        Int32 content = 0;
        Int32 objects = 0;
        Boolean ready = false;
        Boolean loaded = false;
        Boolean showHitboxes = false;
        public Int32 ContentTasks_Total
        {
            get
            {
                return resourceLoad.Count;
            }
        }
        public Int32 ObjectTasks_Total
        {
            get
            {
                return objectPrototypes.Count;
            }
        }
        public Int32 ContentTasks_Done
        {
            get
            {
                return content;
            }
        }
        public Int32 ObjectTasks_Done
        {
            get
            {
                return objects;
            }
        }
        public Boolean ReadyForLoad
        {
            get
            {
                return ready;
            }
        }
        public Boolean Loaded
        {
            get
            {
                return loaded;
            }
        }

        public void PerformContentTask()
        {
            PerformContentTasks(1);
        }
        public void PerformObjectTask()
        {
            PerformObjectTasks(1);
        }
        public void PerformContentTasks(Int32 step)
        {
            Int32 x = content;
            for (int i = content; i < Math.Min(x+step, resourceLoad.Count); i++)
            {
                ResourceLoadTask rlt = resourceLoad[i];
                LoadResource(rlt);
                content = i+1;
            }
        }
        public void PerformObjectTasks(Int32 step)
        {
            Int32 x = objects;
            for (int i = objects; i < Math.Min(x + step, objectPrototypes.Count); i++)
            {
                XElement xe = objectPrototypes[i];
                LoadObject(xe);
                objects = i+1;
            }
        }
        public void PreloadTask()
        {
            Camera.CameraVelocity = 300.0f;
            Camera.OffsetVelocity = 450.0f;
            Camera.DefaultHeight = 540;
            Palette = new PaletteManager();
            Palette.SetColor("--bg", 255, 255, 255, 255);
            ready = true;
        }
        public void PostLoadTask()
        {
            Camera.Focus = Player.Location.ToPoint();
            Pathfinder = new PathfindingManager(NavMap, Props, Player);
            foreach (IScriptModule x in Scripts)
            {
                x.AddData("obj", ObjectManager);
                x.AddData("camera", CameraManager);
                x.AddData("audio", Audio);
                x.AddData("timer", EventTimer);
                x.AddData("palette", Palette);
                x.AddData("navmap", Pathfinder);
                x.SuppressExceptions = SuppressScriptExceptions;
                if (x is TopDownFrameScript)
                {
                    TopDownFrameScript a = x as TopDownFrameScript;
                    if (a.RequireRawSceneData)
                    {
                        a.AddData("scene", this);
                    }
                    if (a.RequireResourceHeap)
                    {
                        a.AddData("resources", FrameResources);
                    }
                }
                x.Start();
            }
            loaded = true;
            LastMWheelValue = Mouse.GetState().ScrollWheelValue;
            CameraManager.Player = Player;
            CameraManager.TrackToPlayer();
            ObjectManager.Player = Player;
        }
        public Boolean SupportsMultiLoading
        {
            get
            {
                return true;
            }
        }
        #endregion
        public override int FrameID
        {
            get
            {
                return _id;
            }
        }
        public override void SetPlayerPosition(object sender, EventArgs e, GameTime gameTime)
        {
            if (e is FrameShiftEventArgs fs)
            {
                if (fs.IncludesPlayerLocation && Player != null)
                {
                    Player.Location = fs.PlayerLocation.ToVector2();
                }
            }
        }
        #region Loader
        void LoadResource(ResourceLoadTask rlt)
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
        void LoadObject(XElement xe)
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
                        Player = new PlayerObject(startPos, colliderSize, mass, interactFieldSize);
                        String channel = "global";
                        #endregion
                        foreach (XElement xe2 in xe.Elements())
                        {
                            switch (xe2.Name.LocalName.ToLower())
                            {
                                case "sprite":
                                    Player.Sprite = XMLSceneLoader.LoadSpriteController(xe2, FrameResources);
                                    break;
                                case "colorchannel":
                                    channel = xe2.Attribute(XName.Get("channel")).Value;
                                    break;
                                case "motion":
                                    Player.Motion.MovementSpeed = Single.Parse(xe2.Attribute(XName.Get("movementSpeed")).Value);
                                    foreach (XElement xe3 in xe2.Elements())
                                    {
                                        switch (xe3.Name.LocalName.ToLower())
                                        {
                                            case "idleanimation":
                                                Player.Motion.IdleAnimation = xe3.Attribute(XName.Get("id")).Value;
                                                break;
                                            case "walkinganimation":
                                                Player.Motion.WalkingAnimation = xe3.Attribute(XName.Get("id")).Value;
                                                break;
                                        }
                                    }
                                    break;

                            }
                        }
                        Palette.SetObjectChannel("--player", channel);
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
                        String channel = "global";
                        #endregion
                        Props.Add(ID, new DynamicRectObject(startPos, colliderSize, mass, isStatic));
                        foreach (XElement xe2 in xe.Elements())
                        {
                            switch (xe2.Name.LocalName.ToLower())
                            {
                                case "sprite":
                                    Props[ID].Sprite = XMLSceneLoader.LoadSpriteController(xe2, FrameResources);
                                    break;
                                case "colorchannel":
                                    channel = xe2.Attribute(XName.Get("channel")).Value;
                                    break;
                                case "interactable":
                                    Interactable.Add(xe2.Attribute(XName.Get("interactAction")).Value, Props[ID]);
                                    break;
                                case "ignorecollisions":
                                    Props[ID].Collidable = false;
                                    break;
                                case "objectscript":
                                {
                                    String scriptContent = File.ReadAllText(Path.Combine(Owner.Content.RootDirectory, xe2.Attribute(XName.Get("location")).Value));
                                    LuaModule lm = new LuaModule(scriptContent, xe2.Attribute(XName.Get("location")).Value+":"+ID);
                                    lm.Runtime["this"] = ID;
                                    Scripts.Add(lm);
                                    break;
                                }
                            }
                        }
                        Palette.SetObjectChannel(ID, channel);
                        break;
                    }
                case "colorchannel":
                    {
                        String ID = xe.Attribute(XName.Get("id")).Value;
                        Vector4 color = XMLSceneLoader.ResolveColorVector4(xe.Attribute(XName.Get("color")).Value);
                        Palette.SetColor(ID, color);
                        if (xe.Attribute(XName.Get("mixWithGlobal")) != null)
                        {
                            Palette.SetMixWithGlobal(ID, bool.Parse(xe.Attribute(XName.Get("mixWithGlobal")).Value));
                        }
                        break;
                    }
                case "script":
                    {
                        String scriptContent = File.ReadAllText(Path.Combine(Owner.Content.RootDirectory, xe.Attribute(XName.Get("location")).Value));
                        Scripts.Add(new LuaModule(scriptContent, xe.Attribute(XName.Get("location")).Value));
                        break;
                    }
                case "csiscript":
                    {
                        /*
                        List<String> log = new List<string>();
                        CSIModule csim = ClassBuilder.ComplileFromFile(Path.Combine(Owner.Content.RootDirectory, xe.Attribute(XName.Get("location")).Value), log);
                        if (csim == null)
                        {
                            String ErrorText = "Errors have occured while trying to build a script!";
                            foreach (String t in log)
                            {
                                ErrorText += "\n" + t;
                            }
                            throw new SerializationException(ErrorText);
                        }
                        Scripts.Add(csim);
                        */
                        break;
                    }
                case "prebuiltscript":
                    {
                        Scripts.Add(ClassBuilder.GetPrebuiltScript(xe.Attribute(XName.Get("name")).Value));
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
                case "ruleset":
                    {
                        foreach (XAttribute xa in xe.Attributes())
                        {
                            switch (xa.Name.LocalName)
                            {
                                case "allowManualZoom":
                                    AllowManualZoom = Boolean.Parse(xa.Value);
                                    break;
                                case "suppressScriptExceptions":
                                    SuppressScriptExceptions = Boolean.Parse(xa.Value);
                                    break;
                            }
                        }

                        break;
                    }
                case "navmesh":
                    {
                        WaypointGraph.LoadNavMapFromXML(xe, NavMap);
                        break;
                    }
            }
        }
        public override void LoadContent()
        {
            PreloadTask();
            foreach (ResourceLoadTask rlt in resourceLoad)
            {
                LoadResource(rlt);
            }
            foreach (XElement xe in objectPrototypes)
            {
                LoadObject(xe);
            }
            PostLoadTask();
            objects = objectPrototypes.Count;
            content = resourceLoad.Count;
        }

        public Frame BuildFromXML(XDocument Document, Object[] parameters)
        {
            XElement root = Document.Root;
            if (root.Name.LocalName.ToString() != "Scene" || root.Attribute(XName.Get("behaviorType")).Value != "default/topdown")
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
        public TopDownFrame(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {
            CameraManager = new CameraManager(Camera, Props);
            ObjectManager = new ObjectHeapManager(Props);
            Audio = new SoundManager(FrameResources);
            EventTimer = new ScriptEventManager(Execute);
        }

        public void Execute(Object sender, String e, GameTime g)
        {
            foreach (IScriptModule lm in Scripts)
            {
                lm.Update(e, (Single)g.ElapsedGameTime.TotalMilliseconds, (Single)g.TotalGameTime.TotalMilliseconds);
            }
        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            Single loco_x = 0.0f; Single loco_y = 0.0f;
            if (Player.Controlled)
            {
                if (controls[0]) { loco_y -= 1.0f; }
                if (controls[1]) { loco_y += 1.0f; }
                if (controls[2]) { loco_x -= 1.0f; }
                if (controls[3]) { loco_x += 1.0f; }
            }
            Vector2 Locomotion = new Vector2(loco_x, loco_y);
            if (Player.Controlled)
            {
                if (controls[0] && !(controls[1] || controls[2] || controls[3]))
                {
                    Player.SightDirection = Direction.Up;
                }
                if (controls[1] && !(controls[0] || controls[2] || controls[3]))
                {
                    Player.SightDirection = Direction.Down;
                }
                if (controls[2] && !(controls[1] || controls[0] || controls[3]))
                {
                    Player.SightDirection = Direction.Left;
                }
                if (controls[3] && !(controls[1] || controls[0] || controls[2]))
                {
                    Player.SightDirection = Direction.Right;
                }
                String newAnimSequence = Player.Motion.FetchAnimationSequenceID(Locomotion.Length(), Player.SightDirection);
                if (Player.Sprite.CurrentAnimationSequence != newAnimSequence)
                {
                    Player.Sprite.SetAnimationSequence(newAnimSequence);
                }
            }
            
            Locomotion /= (Locomotion.Length() != 0 ? Locomotion.Length() : 1.0f);
            Locomotion *= Player.Motion.MovementSpeed;
            Player.Velocity = Locomotion;
            Player.Update(gameTime);
            foreach (String i in Props.Keys)
            {
                Props[i].Update(gameTime);
                Player.TryCollideWith(Props[i]);
                if (!Props[i].Static)
                {
                    foreach (String x in Props.Keys)
                    {
                        if (i == x)
                        {
                            continue;
                        }
                        Props[i].TryCollideWith(Props[x]);
                    }
                }
            }
            if (controls[4] && !LastInput[4] && Player.Controlled)
            {
                foreach (String i in Interactable.Keys)
                {
                    if (Player.SightArea.Intersects(Interactable[i].Collider) && Interactable[i].Active)
                    {
                        foreach (IScriptModule lm in Scripts)
                        {
                            lm.Update(i, (Single)gameTime.ElapsedGameTime.TotalMilliseconds, (Single)gameTime.TotalGameTime.TotalMilliseconds);
                        }
                    }
                }
            }
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                showHitboxes = true;
            }
#endif            
            CameraManager.Update(gameTime);
            Camera.TrackTarget = CameraManager.TrackPoint;
            Camera.OffsetTarget = CameraManager.Offset;
            Camera.Update(gameTime);
            EventTimer.Update(gameTime);
            foreach (IScriptModule x in Scripts)
            {
                x.Update("default", (Single)gameTime.ElapsedGameTime.TotalMilliseconds, (Single)gameTime.TotalGameTime.TotalMilliseconds);
            }
            if (AllowManualZoom)
            {
                Int32 mwheel = Mouse.GetState().ScrollWheelValue - LastMWheelValue;
                if (mwheel != 0)
                {
                    Camera.Zoom = Math.Max(0.3f, Math.Min(2.5f, Camera.Zoom + (0.1f * mwheel / 120)));
                }
            }
            base.Update(gameTime, controls);
            LastMWheelValue = Mouse.GetState().ScrollWheelValue;
            for (int i = 0; i < Math.Min(controls.Length, LastInput.Length); i++)
            {
                LastInput[i] = controls[i];
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone)
        {
            Rectangle dynDrawZone = Camera.GetDrawArea(drawZone);
            Texture2D t2d = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            t2d.SetData(new Color[] { Color.White });
            foreach (Backdrop b in backdrops)
            {
                b.Draw(spriteBatch, 540, Camera.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(dynDrawZone.Width, dynDrawZone.Height), Palette.GetColor("--bg"));
            }
            foreach (String i in Props.Keys)
            {
                Single depth = (0.3f - (0.1f * (Props[i].Location.Y / 540)));
                Props[i].Draw(spriteBatch, gameTime, 540, Camera.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(dynDrawZone.Width, dynDrawZone.Height), Palette.GetObjectColor(i), depth);
#if DEBUG
                if (showHitboxes)
                {
                    spriteBatch.Draw(t2d, Props[i].Collider, Interactable.ContainsValue(Props[i]) ? Color.Yellow : Color.Red);
                }
#endif
            }
            Single p_depth = 0.3f - (0.1f * (Player.Location.Y / 540));
            Player.Draw(spriteBatch, gameTime, 540, Camera.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(dynDrawZone.Width, dynDrawZone.Height), Palette.GetObjectColor("--player"), p_depth);
#if DEBUG
            if (showHitboxes)
            {
                spriteBatch.Draw(t2d, Player.Collider, Color.Green);
                spriteBatch.Draw(t2d, Player.SightArea, new Color(0, 255, 0, 128));
            }
            spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], DebugText, new Vector2(0, 36), Color.Yellow);
            Single y = 48.0f;
            Int32 c = Scripts.Count;
            for (int i = 0; i < c; i++)
            {
                if (Scripts[i].LastError != "")
                {
                    spriteBatch.DrawString(FrameResources.Global.Fonts["vcr"], i + ": " + Scripts[i].LastError, new Vector2(0, y), Color.Yellow);
                    y += 12;
                }
            }
#endif
        }

        public override void Initialize()
        {
            
        }

        public override void UnloadContent()
        {
            Palette.Dispose();
            Palette = null;
            NavMap.Clear();
            Pathfinder = null;
            showHitboxes = false;
            Audio.Dispose();
            FrameResources.Dispose();
            Props.Clear();
            Interactable.Clear();
            CameraManager.Player = null;
            AllowManualZoom = false;
            SuppressScriptExceptions = false;
            Camera.Zoom = 1.0f;
            ObjectManager.Player = null;
            Player = null;
            Scripts.Clear();
            backdrops.Clear();
            content = 0;
            objects = 0;
            ready = false;
            loaded = false;
        }
    }
}
