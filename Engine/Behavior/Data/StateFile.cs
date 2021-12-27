using System;
using System.IO;
using System.Reflection;

namespace DotRPG.Behavior.Data
{
    public class StateFile
    {
        /// <summary>
        /// Save file header. Corresponds to code "DOTRPGSV".
        /// </summary>
        public const UInt64 DOTRPGSV = 0x444f545250475356;

        public String Location;
        public Int32 SwitchCount { get; private set; }
        public Int32 StateCount { get; private set; }

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
        public void Reset(Int32 switchCount, Int32 stateCount)
        {
            if (switchCount % 8 != 0)
            {
                switchCount += 8 - (switchCount % 8);
            }

            SwitchSet = new byte[switchCount / 8];
            StateSet = new byte[stateCount];

            SwitchCount = switchCount;
            StateCount = stateCount;
        }
        public void Inflate_Switches(Int32 switchCount)
        {
            if (switchCount % 8 != 0)
            {
                switchCount += 8 - (switchCount % 8);
            }
            if (SwitchCount < switchCount)
            {
                Array.Resize<Byte>(ref SwitchSet, switchCount / 8);
            }
        }
        public void Inflate_States(Int32 stateCount)
        {
            if (StateCount < stateCount)
            {
                Array.Resize<Byte>(ref SwitchSet, stateCount / 8);
            }
        }
        public void Export(Object origin, PropertyInfo property)
        {
            if (property.GetCustomAttribute(typeof(StateFilledAttribute)) is StateFilledAttribute sfa)
            {
                switch (sfa.DataType)
                {
                    case StateType.Int8:
                        property.SetValue(origin, StateSet[sfa.FileIndex]);
                        break;
                    case StateType.Int16:
                        property.SetValue(origin, Algebra.Packman.u16_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Int32:
                        property.SetValue(origin, Algebra.Packman.u32_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Int64:
                        property.SetValue(origin, Algebra.Packman.u64_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Float32:
                        property.SetValue(origin, Algebra.Packman.f32_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Float64:
                        property.SetValue(origin, Algebra.Packman.f64_Read(StateSet, sfa.FileIndex));
                        break;
                }
            }
        }
    }
}
