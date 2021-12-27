using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Algebra
{
    /// <summary>
    /// Static class for compact data packing.
    /// </summary>
    public static class Packman
    {
        /// <summary>
        /// Inserts boolean value into a byte.
        /// </summary>
        /// <param name="i">Byte to pack boolean into.</param>
        /// <param name="index">Number of bit (enumerated from oldest to youngest).</param>
        /// <param name="value">Boolean value to pack.</param>
        public static void Flag_Mutate(ref Byte i, Byte index, Boolean value)
        {
            index %= 8;
            Int32 v = 1 << (7 - index);
            if (value)
            {
                i = (Byte) (i | v);
            }
            else
            {
                i = (Byte)(i & (255 - v));
            }
        }
        public static Boolean Flag_Retrieve(Byte i, Byte index)
        {
            index %= 8;
            i = (Byte)(i >> (7 - index));
            i %= 2;
            return (i == 1);
        }
        public static Byte Flag_RImplies(Byte i1, Byte i2)
        {
            return (Byte)(i1 & (~i2 % 256));
        }

        public static UInt32 u32_Read(Byte[] num, Int32 start = 0)
        {
            return (UInt32)num[start] << 24 + num[start+1] << 16 + num[start+2] << 8 + num[start+3];
        }
        public static UInt16 u16_Read(Byte[] num, Int32 start = 0)
        {
            return (UInt16)(num[start] << 8 + num[start + 1]);
        }
        public static UInt64 u64_Read(Byte[] num, Int32 start = 0)
        {
            return (UInt64)num[start] << 56 + num[start + 1] << 48 + num[start + 2] << 40 + num[start + 3] << 32 + num[start + 4] << 24 + num[start + 5] << 16 + num[start + 6] << 8 + num[start + 7];
        }
        public static Single f32_Read(Byte[] num, Int32 start = 0)
        {
            UInt32 a = u32_Read(num, start);
            Byte[] n = BitConverter.GetBytes(a);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
            return BitConverter.ToSingle(n);
        }
        public static Double f64_Read(Byte[] num, Int32 start = 0)
        {
            UInt64 a = u64_Read(num, start);
            Byte[] n = BitConverter.GetBytes(a);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
            return BitConverter.ToDouble(n);
        }
        public static void u32_Write(UInt32 value, Byte[] num, Int32 start = 0)
        {
            num[start] = (Byte)(value >> 24);
            num[start + 1] = (Byte)(value >> 16 % 256);
            num[start + 2] = (Byte)(value >> 8 % 256);
            num[start + 3] = (Byte)(value % 256);
        }
        public static void u64_Write(UInt64 value, Byte[] num, Int32 start = 0)
        {
            num[start] = (Byte)(value >> 56);
            num[start + 1] = (Byte)(value >> 48 % 256);
            num[start + 2] = (Byte)(value >> 40 % 256);
            num[start + 3] = (Byte)(value >> 32 % 256);
            num[start + 4] = (Byte)(value >> 24 % 256);
            num[start + 5] = (Byte)(value >> 16 % 256);
            num[start + 6] = (Byte)(value >> 8 % 256);
            num[start + 7] = (Byte)(value % 256);
        }
        public static void u16_Write(UInt16 value, Byte[] num, Int32 start = 0)
        {
            num[start] = (Byte)(value >> 8);
            num[start + 1] = (Byte)(value % 256);
        }
        public static void f32_Write(Single value, Byte[] num, Int32 start = 0)
        {
            Byte[] n = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
            UInt32 a = BitConverter.ToUInt32(n);
            u32_Write(a, num, start);
        }
        public static void f64_Write(Single value, Byte[] num, Int32 start = 0)
        {
            Byte[] n = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
            UInt64 a = BitConverter.ToUInt64(n);
            u64_Write(a, num, start);
        }
    }
}
