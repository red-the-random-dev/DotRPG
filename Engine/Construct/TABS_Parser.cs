using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DotRPG.Construct
{
    /// <summary>
    /// TABS (Tabulation-Aligned Block aSsembly) is markup language used for declaration of DotRPG object prototypes.
    /// This markup language is similar to XML and designed as its counterpart with less syntax complexity.
    /// </summary>
    public class TABS_Parser
    {
        protected String GetStringWithoutTabulation(String intake, out Int32 tabAmount)
        {
            tabAmount = 0;
            String a = "";
            foreach (Char i in intake)
            {
                if (i == '\t' && a.Length == 0)
                {
                    tabAmount++;
                }
                else
                {
                    a += i;
                }
            }
            return a;
        }
        protected String GetStringCut(Int32 x, String intake)
        {
            String a = "";
            for (int i = x; i < intake.Length; i++)
            {
                a += intake[i];
            }
            return a;
        }
        protected void GetPropertyValue(String intake, out String property, out String value)
        {
            if (intake.Length == 0 || intake.Length == 1 || !intake.Contains(' '))
            {
                throw new ArgumentException("Invalid property format.");
            }
            intake = GetStringCut(1, intake);
            String[] tagValue = intake.Split(' ');
            property = tagValue[0];
            value = "";
            for (int i = 1; i < tagValue.Length; i++)
            {
                value += tagValue[i];
                if (i < tagValue.Length-1)
                {
                    value += ' ';
                }
            }
        }

        public ObjectPrototype[] FromStream(Stream s)
        {
            List<ObjectPrototype> opl = new List<ObjectPrototype>();
            StreamReader sr = new StreamReader(s);
            ObjectPrototype[] ActiveChain = new ObjectPrototype[0];

            while (!sr.EndOfStream)
            {
                String dataChunk = GetStringWithoutTabulation(sr.ReadLine(), out Int32 level);
                if (dataChunk.Length == 0)
                {
                    continue;
                }
                Array.Resize<ObjectPrototype>(ref ActiveChain, level + 1);
                switch (dataChunk[0])
                {
                    case '@':
                        {
                            Array.Resize<ObjectPrototype>(ref ActiveChain, level);
                            Array.Resize<ObjectPrototype>(ref ActiveChain, level + 1);
                            ObjectPrototype op = new ObjectPrototype();
                            op.Name = GetStringCut(1, dataChunk);
                            if (level == 0)
                            {
                                opl.Add(op);
                            }
                            else
                            {
                                ActiveChain[level - 1].Subnodes.Add(op);
                            }
                            ActiveChain[level] = op;
                            break;
                        }
                    case '#':
                        {
                            GetPropertyValue(dataChunk, out String prop, out String val);
                            if (ActiveChain[level].Properties.ContainsKey(prop))
                            {
                                ActiveChain[level].Properties[prop] = val;
                            }
                            else
                            {
                                ActiveChain[level].Properties.Add(prop, val);
                            }
                            break;
                        }
                    case '+':
                        {
                            if (ActiveChain[level].InternalData.Length > 0)
                            {
                                ActiveChain[level].InternalData += '\n';
                            }
                            ActiveChain[level].InternalData += GetStringCut(1, dataChunk);
                            break;
                        }
                    case '>':
                        {
                            ActiveChain[level].PrefabName = GetStringCut(1, dataChunk);
                            break;
                        }
                }
            }
            return opl.ToArray();
        }
    }
}
