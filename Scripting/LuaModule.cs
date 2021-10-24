using System;
using System.Collections.Generic;
using NLua;
using NLua.Exceptions;

namespace DotRPG.Scripting
{
    /// <summary>
    /// Embedded Lua module which features Update() function done every frame.
    /// </summary>
    public class LuaModule : IScriptModule
    {
        public Lua Runtime;
        public Boolean IsUp = true;
        public readonly String Name;
        public LuaTable EventAmounts { get; private set; }
        public Dictionary<String, Int64> EventIDs = new Dictionary<string, long>();
        public Boolean HasDefaultAction = false;
        public String LastError { get; set; } = "";
        public Exception LastErrorDetails { get; set; }
        public Boolean SuppressExceptions { get; set; } = false;

        public LuaModule(String initFile, LuaTable eventAmounts, String initName = "dotrpgmodule")
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile, initName);
            EventAmounts = eventAmounts;
            foreach (String i in EventAmounts.Keys)
            {
                EventIDs.Add(i, 0);
            }
            Name = initName;
        }
        public void AddData(Dictionary<String, Object> data)
        {
            foreach (String i in data.Keys)
            {
                Runtime[i] = data[i];
            }
        }
        public void AddData(String key, Object value)
        {
            Runtime[key] = value;
        }
        public LuaModule(String initFile, String initName = "dotrpgmodule")
        {
            Runtime = new Lua();
            Runtime.LoadCLRPackage();
            Runtime.DoString(initFile, initName);
            EventAmounts = (LuaTable) Runtime["event_counts"];
            foreach (String i in EventAmounts.Keys)
            {
                EventIDs.Add(i, 0);
            }
            Name = initName;
        }

        public void Start()
        {
            if (Runtime["start"] != null)
            {
                LuaFunction startFunction = Runtime["start"] as LuaFunction;
                try
                {
                    startFunction.Call();
                    LastError = "";
                    LastErrorDetails = null;
                    EventAmounts = (LuaTable)Runtime["event_counts"];
                    EventIDs.Clear();
                    foreach (String i in EventAmounts.Keys)
                    {
                        EventIDs.Add(i, 0);
                    }
                }
                catch (LuaException e)
                {
                    LastError = e.Message;
                    LastErrorDetails = e;
                    if (!SuppressExceptions)
                    {
                        throw e;
                    }
                }
                catch (Exception e)
                {
                    LastError = "Something is creating script errors";
                    LastErrorDetails = e;
                    if (!SuppressExceptions)
                    {
                        throw e;
                    }
                }
            }
        }

        public void Update(String EventSetID, Single elapsedTime, Single totalTime)
        {
            // Event will be ignored if its ID is not referenced in file's eventAmounts table
            foreach (String x in EventIDs.Keys)
            {
                if (EventSetID == x)
                {
                    try
                    {
                        LuaFunction loopFunction = Runtime["update"] as LuaFunction;
                        loopFunction.Call(EventSetID, EventIDs[EventSetID], elapsedTime, totalTime);
                        
                        LastError = "";
                        LastErrorDetails = null;
                    }
                    catch (LuaException e)
                    {
                        LastError = e.Message;
                        LastErrorDetails = e;
                        if (!SuppressExceptions)
                        {
                            throw e;
                        }
                    }
                    catch (Exception e)
                    {
                        LastError = "Something is creating script errors";
                        LastErrorDetails = e;
                        if (!SuppressExceptions)
                        {
                            throw e;
                        }
                    }
                    finally
                    {
                        EventIDs[EventSetID] = (Int64)EventIDs[EventSetID] + 1;
                        if (EventIDs[EventSetID] >= (Int64)EventAmounts[EventSetID])
                        {
                            EventIDs[EventSetID] = 0;
                        }
                    }
                    break;
                }
            }
        }
    }
}
