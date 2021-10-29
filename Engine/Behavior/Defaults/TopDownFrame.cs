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
using DotRPG.Construct;

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
        List<ObjectPrototype> objectPrototypes = new List<ObjectPrototype>();
        Dictionary<String, ObjectPrototype> prefabs = new Dictionary<string, ObjectPrototype>();
        List<IScriptModule> Scripts = new List<IScriptModule>();
        Dictionary<String, List<IScriptModule>> ObjectBoundScripts = new Dictionary<String, List<IScriptModule>>();
        public List<Backdrop> backdrops { get; private set; } = new List<Backdrop>();
        public Dictionary<String, Waypoint> NavMap { get; private set; } = new Dictionary<string, Waypoint>();

        public Dictionary<String, DynamicObject> Props { get; private set; } = new Dictionary<string, DynamicObject>();
        public Dictionary<String, DynamicObject> Interactable { get; private set; } = new Dictionary<string, DynamicObject>();

        Int32 LastMWheelValue = 0;
        public Boolean[] LastInput { get; private set; } = new bool[8];

        Boolean AllowManualZoom = false;
        Boolean SuppressScriptExceptions = false;
        Boolean Running = false;

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

        protected void FinalizeObject(String name)
        {
            ObjectBoundScripts[name].Clear();
            ObjectBoundScripts.Remove(name);
            if (Interactable.ContainsValue(Props[name]))
            {
                foreach (String k in Interactable.Keys)
                {
                    if (Interactable[k] == Props[name])
                    {
                        Interactable.Remove(k);
                        break;
                    }
                }
                
            }
            if (Palette.ObjectColors.ContainsKey(name))
            {
                Palette.ObjectColors.Remove(name);
            }
            Props.Remove(name);
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
            for (int i = content; i < Math.Min(x + step, resourceLoad.Count); i++)
            {
                ResourceLoadTask rlt = resourceLoad[i];
                LoadResource(rlt);
                content = i + 1;
            }
        }
        public void PerformObjectTasks(Int32 step)
        {
            Int32 x = objects;
            for (int i = objects; i < Math.Min(x + step, objectPrototypes.Count); i++)
            {
                ObjectPrototype op = objectPrototypes[i];
                LoadObject(op);
                objects = i + 1;
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
                StartScript(x);
            }
            foreach (List<IScriptModule> scripts in ObjectBoundScripts.Values)
            {
                foreach (IScriptModule x in scripts)
                {
                    StartScript(x);
                }
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
        protected void LoadResource(ResourceLoadTask rlt)
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
        protected void StartScript(IScriptModule x)
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
        protected void LoadObject(ObjectPrototype op)
        {
            if (op.PrefabName != null)
            {
                op = ObjectPrototype.Forge(prefabs[op.PrefabName], op);
            }
            switch (op.Name.ToLower())
            {
                case "player":
                    {
                        #region Parameters definition
                        Point startPos = XMLSceneLoader.ResolveVector2(op.Properties["startPos"]).ToPoint();
                        Point colliderSize = XMLSceneLoader.ResolveVector2(op.Properties["colliderSize"]).ToPoint();
                        Point interactFieldSize = XMLSceneLoader.ResolveVector2(op.Properties["interactFieldSize"]).ToPoint();
                        Single mass = Single.Parse(op.Properties["mass"]);
                        Player = new PlayerObject(startPos, colliderSize, mass, new Vector2(0.5f, 0.5f), interactFieldSize);
                        String channel = "global";
                        #endregion
                        foreach (ObjectPrototype op2 in op.Subnodes)
                        {
                            switch (op2.Name.ToLower())
                            {
                                case "sprite":
                                    Player.Sprite = XMLSceneLoader.LoadSpriteController(op2, FrameResources);
                                    break;
                                case "colorchannel":
                                    channel = op2.Properties["channel"];
                                    break;
                                case "motion":
                                    Player.Motion.MovementSpeed = Single.Parse(op2.Properties["movementSpeed"]);
                                    foreach (ObjectPrototype op3 in op2.Subnodes)
                                    {
                                        switch (op3.Name.ToLower())
                                        {
                                            case "idleanimation":
                                                Player.Motion.IdleAnimation = op3.Properties["id"];
                                                break;
                                            case "walkinganimation":
                                                Player.Motion.WalkingAnimation = op3.Properties["id"];
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
                        String ID = op.Properties["id"];
                        Point startPos = XMLSceneLoader.ResolveVector2(op.Properties["startPos"]).ToPoint();
                        Point colliderSize = XMLSceneLoader.ResolveVector2(op.Properties["colliderSize"]).ToPoint();
                        Boolean isStatic = Boolean.Parse(op.Properties["static"]);
                        Single mass = Single.Parse(op.Properties["mass"]);
                        String channel = "global";
                        #endregion
                        Props.Add(ID, new DynamicObject(startPos, colliderSize, mass, new Vector2(0.5f, 0.5f), isStatic));
                        ObjectBoundScripts.Add(ID, new List<IScriptModule>());
                        foreach (ObjectPrototype op2 in op.Subnodes)
                        {
                            switch (op2.Name.ToLower())
                            {
                                case "sprite":
                                    Props[ID].Sprite = XMLSceneLoader.LoadSpriteController(op2, FrameResources);
                                    break;
                                case "colorchannel":
                                    channel = op2.Properties["channel"];
                                    break;
                                case "interactable":
                                    Interactable.Add(op2.Properties["interactAction"], Props[ID]);
                                    break;
                                case "ignorecollisions":
                                    Props[ID].Collidable = false;
                                    break;
                                case "objectscript":
                                    {
                                        String scriptContent = File.ReadAllText(Path.Combine(Owner.Content.RootDirectory, op2.Properties["location"]));
                                        LuaModule lm = new LuaModule(scriptContent, op2.Properties["location"] + ":" + ID);
                                        lm.Runtime["this"] = ID;
                                        ObjectBoundScripts[ID].Add(lm);
                                        if (Running)
                                        {
                                            StartScript(lm);
                                        }
                                        break;
                                    }
                            }
                        }
                        Palette.SetObjectChannel(ID, channel);
                        break;
                    }
                case "colorchannel":
                    {
                        String ID = op.Properties["id"];
                        Vector4 color = XMLSceneLoader.ResolveColorVector4(op.Properties["color"]);
                        Palette.SetColor(ID, color);
                        if (op.Properties.ContainsKey("mixWithGlobal"))
                        {
                            Palette.SetMixWithGlobal(ID, bool.Parse(op.Properties["mixWithGlobal"]));
                        }
                        break;
                    }
                case "script":
                    {
                        String scriptContent = File.ReadAllText(Path.Combine(Owner.Content.RootDirectory, op.Properties["location"]));
                        Scripts.Add(new LuaModule(scriptContent, op.Properties["location"]));
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
                        Scripts.Add(ClassBuilder.GetPrebuiltScript(op.Properties["name"]));
                        break;
                    }
                case "backdrop":
                    {
                        Texture2D t = null;
                        Vector2 p = Vector2.Zero;
                        foreach (String opa in op.Properties.Keys)
                        {
                            switch (opa)
                            {
                                case "local":
                                    t = FrameResources.Textures[op.Properties[opa]];
                                    break;
                                case "global":
                                    t = FrameResources.Global.Textures[op.Properties[opa]];
                                    break;
                                case "offset":
                                    p = XMLSceneLoader.ResolveVector2(op.Properties[opa]);
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
                        foreach (String opa in op.Properties.Keys)
                        {
                            switch (opa)
                            {
                                case "allowManualZoom":
                                    AllowManualZoom = Boolean.Parse(op.Properties[opa]);
                                    break;
                                case "suppressScriptExceptions":
                                    SuppressScriptExceptions = Boolean.Parse(op.Properties[opa]);
                                    break;
                            }
                        }

                        break;
                    }
                case "navmesh":
                    {
                        WaypointGraph.LoadNavMap(op, NavMap);
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
            foreach (ObjectPrototype op in objectPrototypes)
            {
                LoadObject(op);
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
                    case "prefab":
                        {
                            XMLSceneLoader.GetPrefab(xe.Attribute(XName.Get("objType")).Value, Path.GetFullPath(Path.Combine(Owner.Content.RootDirectory, xe.Attribute(XName.Get("location")).Value)), prefabs, out String newID, resourceLoad);
                            break;
                        }
                    default:
                        ObjectPrototype x = ObjectPrototype.FromXML(xe);
                        foreach (XAttribute xa in xe.Attributes())
                        {
                            if (xa.Name.LocalName.ToString() == "_usePrefab")
                            {
                                String loadPath = Path.GetFullPath(Path.Combine(Owner.Content.RootDirectory, xa.Value.ToString()));
                                XMLSceneLoader.GetPrefab(xe.Name.LocalName, loadPath, prefabs, out String newID, resourceLoad);
                                x.PrefabName = newID;
                                break;
                            }
                            if (xa.Name.LocalName.ToString() == "_usePrefabID")
                            {
                                x.PrefabName = xa.Value.ToString();
                            }
                        }
                        objectPrototypes.Add(x);
                        break;
                }
            }
            List<ResourceLoadTask> rlt_new = new List<ResourceLoadTask>();
            HashSet<String> textureIDs = new HashSet<string>();
            HashSet<String> fontIDs = new HashSet<string>();
            HashSet<String> soundIDs = new HashSet<string>();
            HashSet<String> songIDs = new HashSet<string>();

            HashSet<String> currentCollection = null;
            foreach (ResourceLoadTask rlt in resourceLoad)
            {
                switch (rlt.Resource)
                {
                    case ResourceType.Texture2D:
                        currentCollection = textureIDs;
                        break;
                    case ResourceType.SpriteFont:
                        currentCollection = fontIDs;
                        break;
                    case ResourceType.SoundEffect:
                        currentCollection = soundIDs;
                        break;
                    case ResourceType.Song:
                        currentCollection = songIDs;
                        break;
                }
                if (!currentCollection.Contains(rlt.ResourceID))
                {
                    rlt_new.Add(rlt);
                    currentCollection.Add(rlt.ResourceID);
                }
            }
            resourceLoad.Clear();
            foreach (ResourceLoadTask rlt in rlt_new)
            {
                resourceLoad.Add(rlt);
            }
            rlt_new.Clear();
            textureIDs.Clear();
            fontIDs.Clear();
            soundIDs.Clear();
            songIDs.Clear();
            return this;
        }
        #endregion
        public TopDownFrame(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet) : base(owner, globalGameResources, globalEventSet)
        {
            CameraManager = new CameraManager(Camera, Props);
            ObjectManager = new ObjectHeapManager(Props, prefabs, LoadObject, FinalizeObject);
            Audio = new SoundManager(FrameResources);
            EventTimer = new ScriptEventManager(Execute);
        }

        public void Execute(Object sender, String e, GameTime g)
        {
            foreach (IScriptModule lm in Scripts)
            {
                lm.Update(e, (Single)g.ElapsedGameTime.TotalMilliseconds, (Single)g.TotalGameTime.TotalMilliseconds);
            }
            foreach (String ism in ObjectBoundScripts.Keys)
            {
                foreach (IScriptModule lm in ObjectBoundScripts[ism].ToArray())
                {
                    lm.Update(e, (Single)g.ElapsedGameTime.TotalMilliseconds, (Single)g.TotalGameTime.TotalMilliseconds);
                    if (!ObjectManager.Exists(ism))
                    {
                        break;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime, bool[] controls)
        {
            Running = true;
            DebugText = "";
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
            if (Locomotion.Length() == 0.0f)
            {
                Player.AppliedForce += Player.Velocity * -Player.Motion.MovementSpeed;
            }
            Player.Update(gameTime);
            foreach (String i in Props.Keys)
            {
                Props[i].Update(gameTime);
                if (Player.TryCollideWith(Props[i], out Int32 hits_p, 0, gameTime))
                {
                    DebugText += String.Format("Player hits {0} with {1} contact(s)\n", i, hits_p);
                }
                if (!Props[i].Static)
                {
                    foreach (String x in Props.Keys)
                    {
                        if (i == x)
                        {
                            continue;
                        }
                        if (Props[i].TryCollideWith(Props[x], out Int32 hits, 0, gameTime))
                        {
                            DebugText += String.Format("{0} hits {1} with {2} contact(s)\n", i, x, hits);
                        }
                    }
                }
            }
            if (controls[4] && !LastInput[4] && Player.Controlled)
            {
                foreach (String i in Interactable.Keys)
                {
                    if (Player.SightArea.Intersects(Interactable[i].SquareForm) && Interactable[i].Active)
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
            Execute(this, "default", gameTime);
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
            Running = false;
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
                    Vector2[] ppts = Props[i].Collider.TurnedVertices;
                    foreach (Vector2 pt in ppts)
                    {
                        spriteBatch.Draw(t2d, new Rectangle((int)pt.X, (int)pt.Y, 4, 4), Interactable.ContainsValue(Props[i]) ? Color.Yellow : Color.Red);
                    }
                }
#endif
            }
            Single p_depth = 0.3f - (0.1f * (Player.Location.Y / 540));
            Player.Draw(spriteBatch, gameTime, 540, Camera.GetTopLeftAngle(new Point(drawZone.Width, drawZone.Height)), new Point(dynDrawZone.Width, dynDrawZone.Height), Palette.GetObjectColor("--player"), p_depth);
#if DEBUG
            if (showHitboxes)
            {
                Vector2[] ppts = Player.Collider.TurnedVertices;
                foreach (Vector2 pt in ppts)
                {
                    spriteBatch.Draw(t2d, new Rectangle((int)pt.X, (int)pt.Y, 4, 4), Color.Green);
                }
                
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
            foreach (String x in Props.Keys)
            {
                FinalizeObject(x);
            }
            Palette.Dispose();
            ObjectManager.Prefab_Reset();
            CameraManager.Reset();
            EventTimer.Reset();
            Palette = null;
            NavMap.Clear();
            Pathfinder = null;
            showHitboxes = false;
            Audio.Dispose();
            FrameResources.Dispose();
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
