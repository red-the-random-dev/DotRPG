using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Scripting
{
    public static class ClassBuilder
    {
        public static CSIModule ComplileFromFile(String fullPath, List<String> CompilerLog = null)
        {
            Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
            List<String> names = new List<string>();
            foreach (Assembly a in ass)
            {
                names.Add(a.FullName);
            }
            CompilerParameters cp = new CompilerParameters(names.ToArray());
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            var results = provider.CompileAssemblyFromFile(cp, new string[]{fullPath});
            if (results.Errors.HasErrors)
            {
                if (CompilerLog != null)
                {
                    foreach (CompilerError ce in results.Errors)
                    {
                        CompilerLog.Add(ce.ErrorNumber + ": " + ce.ErrorText);
                    }
                }
                return null;
            }
            else
            {
                foreach (Type x in results.CompiledAssembly.GetTypes())
                {
                    if (typeof(CSIModule).IsAssignableFrom(x))
                    {
                        return (CSIModule)Activator.CreateInstance(x);
                    }
                }
                return null;
            }
        }
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
