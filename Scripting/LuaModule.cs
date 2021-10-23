﻿using System;
using System.Collections.Generic;
using NLua;
using NLua.Exceptions;

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
        public String LastError = "";
        public Exception LastErrorDetails;

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
                }
                catch (LuaException e)
                {
                    LastError = e.Message;
                    LastErrorDetails = e;
                }
                catch (Exception e)
                {
                    LastError = "Something is creating script errors";
                    LastErrorDetails = e;
                }
            }
        }

        public void Update(String EventSetID, Single elapsedTime, Single totalTime)
        {
            // Event will be ignored if its ID is not referenced in file's eventAmounts table
            foreach (String x in EventAmounts.Keys)
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
                    }
                    catch (Exception e)
                    {
                        LastError = "Something is creating script errors";
                        LastErrorDetails = e;
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
