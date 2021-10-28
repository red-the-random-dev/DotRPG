using DotRPG.Objects;
using DotRPG.Behavior;
using System;
using System.Collections.Generic;
using System.Text;
using T_EVENT_HANDLER = DotRPG.Behavior.LogicEventHandler<System.String>;
using Microsoft.Xna.Framework;

namespace DotRPG.Behavior.Management
{
    public class ScriptEventManager
    {
        HashSet<TimedEvent<String>> events = new HashSet<TimedEvent<String>>();
        T_EVENT_HANDLER execMethod;
        
        public ScriptEventManager(T_EVENT_HANDLER method)
        {
            execMethod = method;
        }

        public void Enqueue(Single startTime, Single countdown, String arg)
        {
            events.Add(new TimedEvent<string>(startTime, countdown, execMethod, arg));
        }

        public void Update(GameTime gameTime)
        {
            if (events.Count > 0)
            {
                TimedEvent<String>[] tea = new TimedEvent<String>[events.Count];
                Int32 c = 0;
                foreach (TimedEvent<String> te in events)
                {
                    tea[c++] = te;
                }
                foreach (TimedEvent<String> te in tea)
                {
                    if (te.TryFire(this, gameTime))
                    {
                        events.Remove(te);
                    }
                }
            }
        }
        public void Reset()
        {
            events.Clear();
        }
    }
}
