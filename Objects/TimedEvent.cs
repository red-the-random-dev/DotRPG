using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Objects
{
    public delegate void LogicEventHandler(Object sender, EventArgs e, GameTime g);

    public class TimedEvent
    {
        public readonly Double EnqueueStart;
        public Single Countdown;
        public EventArgs Arguments;
        public LogicEventHandler FireEvent { get; private set; }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler leh)
        {
            EnqueueStart = gt.TotalGameTime.TotalMilliseconds;
            Countdown = countdown;
            FireEvent = leh;
            Arguments = new EventArgs();
        }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler leh, EventArgs embed)
        {
            EnqueueStart = gt.TotalGameTime.TotalMilliseconds;
            Countdown = countdown;
            FireEvent = leh;
            Arguments = embed;
        }

        public override int GetHashCode()
        {
            return FireEvent.GetHashCode() ^ ((int) Math.Round(EnqueueStart));
        }

        public Boolean TryFire(Object sender, GameTime g)
        {
            if (g.TotalGameTime.TotalMilliseconds-EnqueueStart > Countdown)
            {
                FireEvent(sender, Arguments, g);
                return true;
            }
            return false;
        }
    }
}
