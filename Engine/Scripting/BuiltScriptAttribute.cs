using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Scripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BuiltScriptAttribute : Attribute
    {
        public String Name;
        public BuiltScriptAttribute(String name)
        {
            Name = name;
        }
    }
}
