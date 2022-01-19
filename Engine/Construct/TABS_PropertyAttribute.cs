using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Construct
{
    public enum PropertyType : Byte
    {
        String = 0,
        Integer = 1,
        FloatPoint = 2,
        Boolean = 4,
        Vector2 = 8,
        Vector3 = 16,
        Vector4 = 32,
        Color = 64,
        Resource = 128,
        Resource_Font = 129,
        Resource_Texture2D = 130,
        Resource_SoundEffect = 132,
        Resource_Effect = 136
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class TABS_PropertyAttribute : Attribute
    {
        public String ID;
        public PropertyType ValueType;

        public TABS_PropertyAttribute(String Name, PropertyType t = PropertyType.String)
        {
            ID = Name;
            ValueType = t;
        }
    }
}
