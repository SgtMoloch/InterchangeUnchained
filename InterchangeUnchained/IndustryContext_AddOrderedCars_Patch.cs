using HarmonyLib;
using Model.OpsNew;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace InterchangeUnchained
{
    [HarmonyPatch(typeof(IndustryContext))]
    [HarmonyPatch(nameof(IndustryContext.AddOrderedCars))]
    internal class IndustryContext_AddOrderedCars_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var validInts = original.GetMethodBody().LocalVariables.Where(i => i.LocalType == typeof(int)).Select(s => s.LocalIndex).ToArray();
            var logger = Log.ForContext(typeof(IndustryContext_AddOrderedCars_Patch));
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_S && codes[i].operand is LocalBuilder lb1 && validInts.Contains(lb1.LocalIndex)
                    && codes[i + 1].opcode == OpCodes.Ldloc_S && codes[i + 1].operand is LocalBuilder lb2 && validInts.Contains(lb2.LocalIndex)
                    && codes[i + 2].opcode == OpCodes.Blt)
                {
                    logger.Information("Found & replaced the code");
                    codes.RemoveRange(i, 2);
                    logger.Information("Replacing {OpCode} {Operand} with a jump", codes[i].opcode, codes[i].operand);
                    codes[i] = new CodeInstruction(OpCodes.Br, codes[i].operand);
                    return codes;
                }
            }

            throw new Exception("Could not find instructions to patch.");
        }
    }
}
