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
        }

        public virtual void Update(GameTime gameTime, Boolean[] controls)
        {
            if (LocalEventSet.Count > 0)
            {
                try
                {
                    foreach (TimedEvent te in LocalEventSet)
                    {
                        if (te.TryFire(this, gameTime))
                        {
                            LocalEventSet.Remove(te);
                        }
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle drawZone);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Draw(gameTime, spriteBatch, new Rectangle(new Point(0, 0), new Point(Owner.Window.ClientBounds.Width, Owner.Window.ClientBounds.Height)));
        }
        public abstract void LoadContent();
        public abstract void Initialize();
        public abstract void UnloadContent();
    }
}
