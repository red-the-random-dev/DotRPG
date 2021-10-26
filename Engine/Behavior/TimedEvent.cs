using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DotRPG.Behavior
{
    public delegate void LogicEventHandler(Object sender, EventArgs e, GameTime g);
    public delegate void LogicEventHandler<Args>(Object sender, Args e, GameTime g); 

    public class TimedEvent
    {
        public readonly Single EnqueueStart;
        public Single Countdown;
        public EventArgs Arguments;
        public LogicEventHandler FireEvent { get; private set; }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler leh) : this(gt, countdown, leh, new EventArgs())
        {
            
        }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler leh, EventArgs embed) : this((Single)gt.TotalGameTime.TotalMilliseconds, countdown, leh, embed)
        {
            
        }
        public TimedEvent(Single mils, Single countdown, LogicEventHandler leh, EventArgs embed)
        {
            EnqueueStart = mils;
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

    public class TimedEvent<T>
    {
        public readonly Single EnqueueStart;
        public Single Countdown;
        public T Arguments;
        public LogicEventHandler<T> FireEvent { get; private set; }
        public TimedEvent(GameTime gt, Single countdown, LogicEventHandler<T> leh, T embed)
        {
            EnqueueStart = (Single)gt.TotalGameTime.TotalMilliseconds;
            Countdown = countdown;
            FireEvent = leh;
            Arguments = embed;
        }
        public TimedEvent(Single mils, Single countdown, LogicEventHandler<T> leh, T embed)
        {
            EnqueueStart = mils;
            Countdown = countdown;
            FireEvent = leh;
            Arguments = embed;
        }

        public override int GetHashCode()
        {
            return Arguments.GetHashCode() ^ ((int)Math.Round(EnqueueStart));
        }

        public Boolean TryFire(Object sender, GameTime g)
        {
            if (g.TotalGameTime.TotalMilliseconds - EnqueueStart > Countdown)
            {
                FireEvent(sender, Arguments, g);
                return true;
            }
            return false;
        }
    }
}
