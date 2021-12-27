using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Data
{
    public enum StateType : Byte
    {
        Int8 = 0,
        Int16 = 1,
        Int32 = 2,
        Int64 = 4,
        Float32 = 8,

        SInt8 = 128,
        SInt16 = 129,
        SInt32 = 130,
        SInt64 = 132,
        Float64 = 136
    }
    /// <summary>
    /// Indicates that property is filled from save file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StateFilledAttribute : Attribute
    {
        public Int32 FileIndex;
        public StateType DataType;

        public StateFilledAttribute(Int32 index, StateType stateType)
        {
            FileIndex = index;
            DataType = stateType;
        }
    }
    /// <summary>
    /// Indicates that boolean property is filled from switch field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchedAttribute : Attribute
    {
        public Int32 FileIndex;

        public SwitchedAttribute(Int32 index)
        {
            FileIndex = index;
        }
    }
}
