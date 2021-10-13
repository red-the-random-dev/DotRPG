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
        public LuaTable EventIDs;
        public Boolean HasDefaultAction = false;

        public LuaModule(String initFile, LuaTable eventAmounts)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = eventAmounts;
            EventIDs = new LuaTable(0, Runtime);
            foreach (Object i in EventAmounts.Keys)
            {
                EventIDs[i] = 0;
            }
        }
        public LuaModule(String initFile)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = (LuaTable) Runtime["event_counts"];
            EventIDs = new LuaTable(0, Runtime);
            foreach (Object i in EventAmounts.Keys)
            {
                EventIDs[i] = 0;
            }
        }

        public void Update(Object ObjectID, Single elapsedTime, Single totalTime)
        {
            LuaFunction loopFunction = Runtime["loop"] as LuaFunction;
            loopFunction.Call(ObjectID, EventIDs[ObjectID], elapsedTime, totalTime);
            EventIDs[ObjectID] = (Int64)EventIDs[ObjectID] + 1;
            if ((Int64)EventIDs[ObjectID] >= (Int64)EventAmounts[ObjectID])
            {
                EventIDs[ObjectID] = 0;
            }
        }
    }
}
