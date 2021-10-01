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
        public Int32 EventID;
        public readonly Int32 EventAmount;

        public LuaModule(String initFile, Int32 eventAmount)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmount = eventAmount;
        }
        public LuaModule(String initFile)
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile);
            EventAmount = (Int32) Runtime["event_count"];
        }

        public void Update(Single elapsedTime, Single totalTime)
        {
            LuaFunction loopFunction = Runtime["loop"] as LuaFunction;
            loopFunction.Call(EventID, elapsedTime, totalTime);
            EventID++;
            if (EventID >= EventAmount)
            {
                EventID = 0;
            }
        }
    }
}
