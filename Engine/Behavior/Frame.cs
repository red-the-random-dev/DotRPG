using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DotRPG.Objects;

namespace DotRPG.Behavior
{
    public abstract class Frame
    {
        protected ResourceHeap FrameResources;
        protected Game Owner;
        public Boolean Instantiated { get; protected set; }

        public abstract Int32 FrameID
        {
            get;
        }

        // TODO: Add collections for game objects
        protected HashSet<TimedEvent> LocalEventSet = new HashSet<TimedEvent>();
        protected HashSet<TimedEvent> GlobalEventSet;

        public void ResolveIncomingEvent(TimedEvent te)
        {
            LocalEventSet.Add(te);
        }

        public abstract void SetPlayerPosition(Object sender, EventArgs e, GameTime gameTime);

        public Frame(Game owner, ResourceHeap globalGameResources, HashSet<TimedEvent> globalEventSet)
        {
            Owner = owner;
            FrameResources = new ResourceHeap(globalGameResources);
            GlobalEventSet = globalEventSet;
            Instantiated = true;
        }

        public virtual void Update(GameTime gameTime, ControlInput controls)
        {
            if (LocalEventSet.Count > 0)
            {
                TimedEvent[] tea = new TimedEvent[LocalEventSet.Count];
                Int32 c = 0;
                foreach (TimedEvent te in LocalEventSet)
                {
                    tea[c++] = te;
                }
                foreach (TimedEvent te in tea)
                {
                    if (te.TryFire(this, gameTime))
                    {
                        LocalEventSet.Remove(te);
                    }
                }
            }
        }

        public abstract void Draw(GameTime gameTime, GraphicsDevice gd, Rectangle drawZone);
        public void Draw(GameTime gameTime, GraphicsDevice gd)
        {
            Draw(gameTime, gd, new Rectangle(new Point(0, 0), new Point(Owner.Window.ClientBounds.Width, Owner.Window.ClientBounds.Height)));
        }
        public abstract void LoadContent(GraphicsDevice gd);
        public abstract void Initialize();
        public abstract void UnloadContent();
    }
}
