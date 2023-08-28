using MoonSharp.Interpreter;
using OpenTS2.Common;
using OpenTS2.Content;
using OpenTS2.Files;
using OpenTS2.Files.Formats.DBPF;
using OpenTS2.Lua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OpenTS2.Engine.Tests
{
    public class LuaTestController : MonoBehaviour
    {
        void Start()
        {
            // Initialize lua engine, register a test API so that we can use UnityLog and hook GetSimulatorGlobal to return 27 as day of the month, 8 as month and 2023 as year.
            var luaManager = LuaManager.Get();
            luaManager.RegisterAPI(new LuaTestAPI());
            luaManager.InitializeObjectScripts();

            // nTime is defined in the game's "Time" global Lua script.
            try
            {
                LuaManager.Get().RunScript(@"local time = nTime.Now()
                                            UnityLog('Lua Year is ' .. time.mYears)
                                            UnityLog('Lua Month is ' .. time.mMonths)
                                            UnityLog('Lua Day is ' .. time.mDays)");
            }
            catch (Exception e)
            {
                var msg = e.ToString();
                if (e is InterpreterException)
                    msg = (e as InterpreterException).DecoratedMessage;
                Debug.LogError($"Problem running:{msg}");
            }
}
    }
}