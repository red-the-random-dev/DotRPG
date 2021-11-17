using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace DotRPG.Construct
{
    public class ObjectPrototype
    {
        public String Name;
        public readonly Dictionary<String, String> Properties = new Dictionary<string, string>();
        public readonly List<ObjectPrototype> Subnodes = new List<ObjectPrototype>();
        public String PrefabName = null;
        public String InternalData = "";

        public static ObjectPrototype FromXML(XElement xe)
        {
            ObjectPrototype op = new ObjectPrototype();
            op.Name = xe.Name.LocalName;
            op.InternalData = xe.Value;

            foreach (XAttribute xa in xe.Attributes())
            {
                op.Properties.Add(xa.Name.LocalName, xa.Value);
            }
            foreach (XElement xe2 in xe.Elements())
            {
                op.Subnodes.Add(FromXML(xe2));
            }
            return op;
        }
        public static ObjectPrototype CreateCopy(ObjectPrototype target)
        {
            ObjectPrototype op = new ObjectPrototype();
            op.Name = target.Name;
            foreach (String x in target.Properties.Keys)
            {
                op.Properties.Add(x, target.Properties[x]);
            }
            foreach (ObjectPrototype op2 in target.Subnodes)
            {
                op.Subnodes.Add(CreateCopy(op2));
            }
            return op;
        }
        public static ObjectPrototype Forge(ObjectPrototype bottom, ObjectPrototype top)
        {
            ObjectPrototype op1 = CreateCopy(bottom);
            ObjectPrototype op2 = CreateCopy(top);

            foreach (String x in op2.Properties.Keys)
            {
                if (op1.Properties.ContainsKey(x))
                {
                    op1.Properties[x] = op2.Properties[x];
                }
                else
                {
                    op1.Properties.Add(x, op2.Properties[x]);
                }
            }
            foreach (ObjectPrototype op_sub in op2.Subnodes)
            {
                op1.Subnodes.Add(op_sub);
            }
            return op1;
        }
    }
}
