using HarmonyLib;
using Model.OpsNew;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Track;
using UnityEngine;

namespace InterchangeUnchained
{
    [HarmonyPatch(typeof(Interchange))]
    [HarmonyPatch("CalculateCapacity")]
    internal class Interchange_CalculateCapacity_Patch
    {
        private static void Postfix(ref int __result)
        {
            // x * .7 = val
            // val / .7 = x
            float newCapcacity = __result / 0.7f;

            // potentially make configurable
            newCapcacity *= 1.0f;
            
            __result = Mathf.RoundToInt(newCapcacity);
        }
    }

}