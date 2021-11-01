using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Construct;
using System.Reflection;
using TYPE_INDEX = System.Collections.Generic.Dictionary<System.String, System.Type>;

namespace DotRPG.UI
{
    public static class UIBuilder
    {
        public static UserInterfaceElement[] BuildFromTABS(ObjectPrototype root, Dictionary<String, ObjectPrototype> namedElements, TYPE_INDEX lookIn = null)
        {
            if (lookIn == null)
            {
                lookIn = new TYPE_INDEX();
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type t in ass.GetTypes())
                    {
                        var attr = t.GetCustomAttribute(typeof(TABS_DeployableAttribute));
                        if (attr is TABS_DeployableAttribute x)
                        {
                            if (x.Category == ObjectType.UserInterfaceElement && typeof(UserInterfaceElement).IsAssignableFrom(t))
                            {
                                lookIn.Add(x.ID, t);
                            }
                        }
                    }
                }
            }
            List<UserInterfaceElement> uiel = new List<UserInterfaceElement>();
            
            foreach (ObjectPrototype op_sub in root.Subnodes)
            {
                Type deploy = lookIn[op_sub.Name];
                UserInterfaceElement uie = Activator.CreateInstance(deploy) as UserInterfaceElement;
                if (uie == null)
                {
                    continue;
                }

                foreach (PropertyInfo pi in deploy.GetProperties())
                {
                    TABS_PropertyAttribute tpa = pi.GetCustomAttribute(typeof(TABS_PropertyAttribute)) as TABS_PropertyAttribute;
                    if (tpa != null)
                    {
                        switch (tpa.ValueType)
                        {
                            case PropertyType.String:
                                pi.SetValue(uie, op_sub.Properties[tpa.ID]);
                                break;
                            case PropertyType.Integer:
                                pi.SetValue(uie, Int32.Parse(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.FloatPoint:
                                pi.SetValue(uie, Single.Parse(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.Boolean:
                                pi.SetValue(uie, Boolean.Parse(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.Vector2:
                                pi.SetValue(uie, Behavior.XMLSceneLoader.ResolveVector2(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.Vector3:
                                pi.SetValue(uie, Behavior.XMLSceneLoader.ResolveVector3(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.Vector4:
                                pi.SetValue(uie, Behavior.XMLSceneLoader.ResolveVector4(op_sub.Properties[tpa.ID]));
                                break;
                            case PropertyType.Color:
                                pi.SetValue(uie, Behavior.XMLSceneLoader.ResolveColorVector4(op_sub.Properties[tpa.ID]));
                                break;
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}
