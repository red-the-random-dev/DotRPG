using System;
using System.IO;

namespace DotRPG.Behavior.Data
{
    public class StateFile
    {
        /// <summary>
        /// Save file header. Corresponds to code "DOTRPGSV".
        /// </summary>
        public const UInt64 DOTRPGSV = 0x444f545250475356;

        public String Location;
        public Int32 SwitchCount { get; protected set; }
        public Int32 StateCount { get; protected set; }

        protected Byte[] SwitchSet;
        protected Byte[] StateSet;

        public Boolean Fetch()
        {
            return Fetch(Location);
        }
        public Boolean Fetch(String location)
        {
            if (File.Exists(location))
                return Fetch(File.OpenRead(location));
            else return false;
        }
        public Boolean Fetch(Stream s)
        {
            Byte[] InputBuffer = new byte[8];
            s.Read(InputBuffer, 0, 8);
            if (!(Algebra.Packman.u64_Read(InputBuffer) == DOTRPGSV))
                return false;
            s.Read(InputBuffer, 0, 8);
            SwitchSet = new byte[Algebra.Packman.u32_Read(InputBuffer)];
            StateSet = new byte[Algebra.Packman.u32_Read(InputBuffer, 4)];

            s.Read(SwitchSet, 0, SwitchSet.Length);
            s.Read(StateSet, 0, StateSet.Length);

            SwitchCount = SwitchSet.Length * 8;
            StateCount = StateSet.Length;

            return true;
        }
    }
}
