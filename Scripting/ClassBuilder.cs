using System;
using System.Reflection;

namespace DotRPG.Scripting
{
    public static class ClassBuilder
    {
        public static CSIModule GetPrebuiltScript(String name)
        {
            foreach (Assembly x in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in x.GetTypes())
                {
                    if (Attribute.IsDefined(t, typeof(BuiltScriptAttribute)) && typeof(CSIModule).IsAssignableFrom(t))
                    {
                        Object[] attrs = t.GetCustomAttributes(false);

                        foreach (Object attr in attrs)
                        {
                            if (attr is BuiltScriptAttribute y)
                            {
                                if (y.Name == name)
                                {
                                    return (CSIModule)Activator.CreateInstance(t);
                                }
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("Unable to load script " + name + ".");
        }
    }
}
