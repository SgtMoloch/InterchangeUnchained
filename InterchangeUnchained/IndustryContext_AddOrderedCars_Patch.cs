using HarmonyLib;
using Model.Database;
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
    [HarmonyPatch(typeof(IndustryContext))]
    [HarmonyPatch(nameof(IndustryContext.AddOrderedCars))]
    internal class IndustryContext_AddOrderedCars_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var validInts = original.GetMethodBody().LocalVariables.Where(i => i.LocalType == typeof(int)).Select(s => s.LocalIndex).ToArray();
            var validTrackSpans = original.GetMethodBody().LocalVariables.Where(i => i.LocalType == typeof(TrackSpan[])).Select(s => s.LocalIndex).ToArray();
            var validPrefabStores = original.GetMethodBody().LocalVariables.Where(i => i.LocalType == typeof(IPrefabStore)).Select(s => s.LocalIndex).ToArray();

            if(validTrackSpans.Length != 1 && validPrefabStores.Length != 1)
            {
                 throw new Exception("Could not find instructions to patch.");
            }

            var logger = Log.ForContext(typeof(IndustryContext_AddOrderedCars_Patch));
            var codes = new List<CodeInstruction>(instructions);


            for(int i = 0; i < codes.Count; i++)
            {
                if(//prefab
                    codes[i].opcode == OpCodes.Stloc_S && codes[i].operand is LocalBuilder lb1 && validPrefabStores[0] == lb1.LocalIndex
                    //num2
                    && codes[i + 1].opcode == OpCodes.Ldc_I4_2
                    && codes[i + 2].opcode  == OpCodes.Stloc_S && codes[i + 2].operand is LocalBuilder lb2 && validInts.Contains(lb2.LocalIndex)
                    //num3
                    && codes[i + 3].opcode == OpCodes.Ldc_I4_0
                    && codes[i + 4].opcode == OpCodes.Stloc_S && codes[i + 4].operand is LocalBuilder lb3 && validInts.Contains(lb3.LocalIndex)
                    //while
                    && codes[i+5].opcode == OpCodes.Br)
                {
                    logger.Information("Found Code Instruction");
                    int index = i + 1;
                    
                    logger.Information("Removing variable assignment");
                    codes.RemoveAt(index);
                    
                    logger.Information("Adding instructions to get number of Tracks to spawn");
                    codes.Insert(index++, new CodeInstruction(OpCodes.Ldloc_S, validTrackSpans[0]));
                    codes.Insert(index++, new CodeInstruction(OpCodes.Ldlen));
                    codes.Insert(index++, new CodeInstruction(OpCodes.Conv_I4));

                    return codes.AsEnumerable();
                }
            }

            throw new Exception("Could not find instructions to patch.");
        }      
    }
}
