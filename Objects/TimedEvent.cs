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
        public LogicEventHandler FireEvent { get; private set; }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler leh)
        {
            EnqueueStart = gt.TotalGameTime.TotalMilliseconds;
            Countdown = countdown;
            FireEvent = leh;
        }

        public override int GetHashCode()
        {
            return FireEvent.GetHashCode() ^ ((int) Math.Round(EnqueueStart));
        }

        public Boolean TryFire(Object sender, EventArgs e, GameTime g)
        {
            if (g.TotalGameTime.TotalMilliseconds-EnqueueStart > Countdown)
            {
                FireEvent(sender, e, g);
                return true;
            }
            return false;
        }
    }
}
