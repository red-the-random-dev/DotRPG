using System;
using System.Collections.Generic;
using System.Text;
using DotRPG.Construct;
using System.Reflection;
using TYPE_INDEX = System.Collections.Generic.Dictionary<System.String, System.Type>;
using DotRPG.Objects;
using System.Globalization;

namespace DotRPG.UI
{
    public static class UIBuilder
    {
        public static Boolean TABS_Initialize(out UserInterfaceElement uie, ObjectPrototype op_sub, ResourceHeap resources, TYPE_INDEX typeref, Dictionary<String, UserInterfaceElement> namedElements)
        {
            Type deploy = typeref[op_sub.Name];
            uie = Activator.CreateInstance(deploy) as UserInterfaceElement;
            if (uie == null)
            {
                return false;
            }

            foreach (PropertyInfo pi in deploy.GetProperties())
            {
                var tp = pi.GetCustomAttribute(typeof(TABS_PropertyAttribute));
                var tp2 = pi.GetCustomAttribute(typeof(TABS_InternalTextAttribute));
                if (tp is TABS_PropertyAttribute tpa)
                {
                    if (!op_sub.Properties.ContainsKey(tpa.ID))
                    {
                        continue;
                    }
                    switch (tpa.ValueType)
                    {
                        case PropertyType.String:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID]);
                            break;
                        case PropertyType.Integer:
                            pi.SetValue(uie, Int32.Parse(op_sub.Properties[tpa.ID]));
                            break;
                        case PropertyType.FloatPoint:
                            pi.SetValue(uie, Single.Parse(op_sub.Properties[tpa.ID], NumberStyles.Any, CultureInfo.InvariantCulture));
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
                        case PropertyType.Resource_Font:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Fonts[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Fonts[op_sub.Properties[tpa.ID]]);
                            break;
                        case PropertyType.Resource_Texture2D:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Textures[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Textures[op_sub.Properties[tpa.ID]]);
                            break;
                        case PropertyType.Resource_SoundEffect:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Sounds[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Sounds[op_sub.Properties[tpa.ID]]);
                            break;
                    }
                }
                if (tp2 is TABS_InternalTextAttribute)
                {
                    pi.SetValue(uie, op_sub.InternalData);
                }
            }
            foreach (FieldInfo pi in deploy.GetFields())
            {
                var tp2 = pi.GetCustomAttribute(typeof(TABS_InternalTextAttribute));
                if (pi.GetCustomAttribute(typeof(TABS_PropertyAttribute)) is TABS_PropertyAttribute tpa)
                {
                    if (!op_sub.Properties.ContainsKey(tpa.ID))
                    {
                        continue;
                    }
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
                            pi.SetValue(uie, new Microsoft.Xna.Framework.Color(Behavior.XMLSceneLoader.ResolveColorVector4(op_sub.Properties[tpa.ID])));
                            break;
                        case PropertyType.Resource_Font:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Fonts[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Fonts[op_sub.Properties[tpa.ID]]);
                            break;
                        case PropertyType.Resource_Texture2D:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Textures[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Textures[op_sub.Properties[tpa.ID]]);
                            break;
                        case PropertyType.Resource_SoundEffect:
                            pi.SetValue(uie, op_sub.Properties[tpa.ID].Split(':')[0] == "g" ? resources.Global.Sounds[op_sub.Properties[tpa.ID].Split(':')[1]] : resources.Sounds[op_sub.Properties[tpa.ID]]);
                            break;
                    }
                }
                if (tp2 is TABS_InternalTextAttribute)
                {
                    pi.SetValue(uie, op_sub.InternalData);
                }
            }
            foreach (ObjectPrototype op_sub2 in op_sub.Subnodes)
            {
                if (TABS_Initialize(out UserInterfaceElement uie_s, op_sub2, resources, typeref, namedElements))
                {
                    uie.Subnodes.Add(uie_s);
                    if (op_sub2.Properties.ContainsKey("id"))
                    {
                        namedElements.Add(op_sub2.Properties["id"], uie_s);
                    }
                }
            }
            return true;
        }

        public static UserInterfaceElement[] BuildFromTABS(ObjectPrototype root, Dictionary<String, UserInterfaceElement> namedElements, ResourceHeap resources, TYPE_INDEX lookIn = null)
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
                if (TABS_Initialize(out UserInterfaceElement uie, op_sub, resources, lookIn, namedElements))
                {
                    uiel.Add(uie);
                    if (op_sub.Properties.ContainsKey("id"))
                    {
                        namedElements.Add(op_sub.Properties["id"], uie);
                    }
                }
            }

            return uiel.ToArray();
        }
    }
}
