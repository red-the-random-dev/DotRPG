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
        public Int32[] EventIDs;

        public LuaModule(String initFile, LuaTable eventAmounts)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = eventAmounts;
        }
        public LuaModule(String initFile)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmounts = (LuaTable) Runtime["event_counts"];
            EventIDs = new int[EventAmounts.Values.Count];
        }

        public void Update(Int32 ObjectID, Single elapsedTime, Single totalTime)
        {
            LuaFunction loopFunction = Runtime["loop"] as LuaFunction;
            loopFunction.Call(ObjectID, EventIDs[ObjectID], elapsedTime, totalTime);
            EventIDs[ObjectID]++;
            if (EventIDs[ObjectID] >= (Int64)EventAmounts[ObjectID+1])
            {
                EventIDs[ObjectID] = 0;
            }
        }
    }
}
