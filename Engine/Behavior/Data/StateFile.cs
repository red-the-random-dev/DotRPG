using System;
using System.IO;

namespace DotRPG.Behavior.Data
{
    public class StateFile
    {
        public String Location;
        public Int32 SwitchCount { get; protected set; }
        public Int32 StateCount { get; protected set; }

        protected Byte[] SwitchSet;
        protected Byte[] StateSet;

        public void Fetch()
        {

        }
    }
}
