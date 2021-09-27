using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Objects
{
    public abstract class Frame
    {
        protected ResourceHeap FrameResources;
        protected Game Owner;

        // TODO: Add collections for game objects
        protected HashSet<TimedEvent> LogicEventSet = new HashSet<TimedEvent>();

        public Frame(Game owner, ResourceHeap globalGameResources)
        {
            Owner = owner;
            FrameResources = new ResourceHeap(globalGameResources);
        }

        public virtual void Update(GameTime gameTime, Boolean[] controls)
        {
            if (LogicEventSet.Count > 0)
            {
                try
                {
                    foreach (TimedEvent te in LogicEventSet)
                    {
                        if (te.TryFire(this, new EventArgs(), gameTime))
                        {
                            LogicEventSet.Remove(te);
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
    }
}
