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
    }
}
