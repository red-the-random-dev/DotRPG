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
        public Int32 SwitchCount { get; private set; } = 8;
        public Int32 StateCount { get; private set; } = 1;

        protected Byte[] SwitchSet = new byte[1];
        protected Byte[] StateSet = new byte[1];

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
            UInt64 c = Algebra.Packman.u64_Read(InputBuffer);
            if (!(c == DOTRPGSV))
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
        public void Push()
        {
            Push(Location);
        }
        public void Push(String location)
        {
            Stream s = File.OpenWrite(location);
            Push(s);
            s.Close();
        }
        public void Push(Stream s)
        {
            Byte[] header = new byte[16];
            Algebra.Packman.u64_Write(DOTRPGSV, header, 0);
            Algebra.Packman.u32_Write((UInt32)SwitchSet.Length, header, 8);
            Algebra.Packman.u32_Write((UInt32)StateSet.Length, header, 12);
            s.Write(header, 0, 16);
            s.Write(SwitchSet, 0, SwitchSet.Length);
            s.Write(StateSet, 0, StateSet.Length);
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
                SwitchCount = switchCount;
            }
        }
        public void Inflate_States(Int32 stateCount)
        {
            if (StateCount < stateCount)
            {
                Array.Resize<Byte>(ref StateSet, stateCount);
                StateCount = stateCount;
            }
        }
        public void ExportTo(Object origin, PropertyInfo property)
        {
            if (property.GetCustomAttribute(typeof(StateFilledAttribute)) is StateFilledAttribute sfa)
            {
                switch (sfa.DataType)
                {
                    case StateType.Int8:
                        Inflate_States(sfa.FileIndex + 1);
                        property.SetValue(origin, StateSet[sfa.FileIndex]);
                        break;
                    case StateType.Int16:
                        Inflate_States(sfa.FileIndex+2);
                        property.SetValue(origin, Algebra.Packman.u16_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Int32:
                        Inflate_States(sfa.FileIndex+4);
                        property.SetValue(origin, Algebra.Packman.u32_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Int64:
                        Inflate_States(sfa.FileIndex+8);
                        property.SetValue(origin, Algebra.Packman.u64_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Float32:
                        Inflate_States(sfa.FileIndex+4);
                        property.SetValue(origin, Algebra.Packman.f32_Read(StateSet, sfa.FileIndex));
                        break;
                    case StateType.Float64:
                        Inflate_States(sfa.FileIndex+8);
                        property.SetValue(origin, Algebra.Packman.f64_Read(StateSet, sfa.FileIndex));
                        break;
                }
            }
            if (property.GetCustomAttribute(typeof(SwitchedAttribute)) is SwitchedAttribute sa)
            {
                Inflate_Switches(sa.FileIndex + 1);
                property.SetValue(origin, Algebra.Packman.Flag_Retrieve(SwitchSet[sa.FileIndex / 8], (Byte)(sa.FileIndex % 8)));
            }
        }
        public void ImportFrom(Object origin, PropertyInfo property)
        {
            if (property.GetCustomAttribute(typeof(StateFilledAttribute)) is StateFilledAttribute sfa)
            {
                switch (sfa.DataType)
                {
                    case StateType.Int8:
                        Inflate_States(sfa.FileIndex+1);
                        StateSet[sfa.FileIndex] = (Byte)property.GetValue(origin);
                        break;
                    case StateType.Int16:
                        Inflate_States(sfa.FileIndex+2);
                        Algebra.Packman.u16_Write((UInt16)property.GetValue(origin), StateSet, sfa.FileIndex);
                        break;
                    case StateType.Int32:
                        Inflate_States(sfa.FileIndex + 4);
                        Algebra.Packman.u32_Write((UInt32)property.GetValue(origin), StateSet, sfa.FileIndex);
                        break;
                    case StateType.Int64:
                        Inflate_States(sfa.FileIndex + 8);
                        Algebra.Packman.u64_Write((UInt64)property.GetValue(origin), StateSet, sfa.FileIndex);
                        break;
                    case StateType.Float32:
                        Inflate_States(sfa.FileIndex + 4);
                        Algebra.Packman.f32_Write((Single)property.GetValue(origin), StateSet, sfa.FileIndex);
                        break;
                    case StateType.Float64:
                        Inflate_States(sfa.FileIndex + 8);
                        Algebra.Packman.f64_Write((Double)property.GetValue(origin), StateSet, sfa.FileIndex);
                        break;
                }
            }
            if (property.GetCustomAttribute(typeof(SwitchedAttribute)) is SwitchedAttribute sa)
            {
                Inflate_Switches(sa.FileIndex + 1);
                Byte x = SwitchSet[sa.FileIndex / 8];
                Algebra.Packman.Flag_Mutate(ref x, (Byte)(sa.FileIndex % 8), (Boolean)property.GetValue(origin));
                SwitchSet[sa.FileIndex / 8] = x;
            }
        }
        
    }
}
