using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Algebra
{
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
    }
}
