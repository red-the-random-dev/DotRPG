using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Algebra;

namespace DotRPG.Behavior
{
    public class ControlInput
    {
        protected Byte[] InputBuffer;
        protected Byte[] LastBuffer;

        public Boolean this[Byte index]
        {
            get
            {
                return KeyDown(index);
            }
            set
            {
                SetKeyDown(index, value);
            }
        }

        public void NextInput()
        {
            for (Int32 i = 0; i < Math.Min(InputBuffer.Length, LastBuffer.Length); i++)
            {
                LastBuffer[i] = InputBuffer[i];
                InputBuffer[i] = 0;
            }
        }
        public Boolean KeyDown(Byte index)
        {
            Int32 a = index / 8;
            index = (Byte)(index % 8);
            return Packman.Flag_Retrieve(InputBuffer[a], index);
        }
        public Boolean KeyPressed(Byte index)
        {
            Int32 a = index / 8;
            index = (Byte)(index % 8);
            return Packman.Flag_Retrieve(Packman.Flag_RImplies(InputBuffer[a], LastBuffer[a]), index);
        }
        public void SetKeyDown(Byte index, Boolean value)
        {
            Int32 a = index / 8;
            index = (Byte)(index % 8);
            Byte x = InputBuffer[a];
            Packman.Flag_Mutate(ref x, index, value);
            InputBuffer[a] = x;
        }

        public Int32 Length
        {
            get
            {
                return InputBuffer.Length;
            }
        }

        public ControlInput(Int32 size)
        {
            size += (size % 8 != 0 ? 8 - (size % 8) : 0);
            size /= 8;
            InputBuffer = new byte[size];
            LastBuffer = new byte[size];
        }
    }
}
