using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Construct
{
    public enum ObjectType
    {
        SceneObject = 0,
        Frame = 1,
        UserInterfaceElement = 2
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TABS_DeployableAttribute : Attribute
    {
        public String ID;
        public ObjectType Category;

        public TABS_DeployableAttribute(String name, ObjectType cl)
        {
            ID = name;
            Category = cl;
        }
    }
}
