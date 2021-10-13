using System;
using System.Collections.Generic;
using NLua;

namespace DotRPG.Scripting
{
    /// <summary>
    /// Embedded Lua module which features Update() function done every frame.
    /// </summary>
    public class LuaModule
    {
        public Lua Runtime;
        public Boolean IsUp = true;
        public readonly LuaTable EventAmounts;
        public Dictionary<String, Int64> EventIDs = new Dictionary<string, long>();
        public Boolean HasDefaultAction = false;

        public LuaModule(String initFile, LuaTable eventAmounts)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = eventAmounts;
            foreach (String i in EventAmounts.Keys)
            {
                EventIDs.Add(i, 0);
            }
        }
        public LuaModule(String initFile)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = (LuaTable) Runtime["event_counts"];
            foreach (String i in EventAmounts.Keys)
            {
                EventIDs.Add(i, 0);
            }
        }

        public void Update(String EventSetID, Single elapsedTime, Single totalTime)
        {
            LuaFunction loopFunction = Runtime["loop"] as LuaFunction;
            loopFunction.Call(EventSetID, EventIDs[EventSetID], elapsedTime, totalTime);
            EventIDs[EventSetID] = (Int64)EventIDs[EventSetID] + 1;
            if (EventIDs[EventSetID] >= (Int64)EventAmounts[EventSetID])
            {
                EventIDs[EventSetID] = 0;
            }
        }
    }
}
