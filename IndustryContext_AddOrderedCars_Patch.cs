using HarmonyLib;
using Model.OpsNew;
using Serilog;
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
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase b)
        {

            var validInts = new HashSet<int>(b.GetMethodBody().LocalVariables.Where(i => i.LocalType == typeof(int)).Select(s => s.LocalIndex));            
            var logger = Log.ForContext(typeof(InterchangeUnchained));
            var codes = new List<CodeInstruction>(instructions);
            int index = -1;
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_S && validInts.Contains((int)codes[i].operand))
                {
                    // third check
                    if (codes[i + 1].opcode == OpCodes.Ldloc_S && validInts.Contains((int)codes[i+1].operand))
                    {
                        if (codes[i + 2].opcode == OpCodes.Blt)
                        {
                            logger.Information("Updating conditional jump");
                            codes.RemoveRange(i,2);
                            codes[i].opcode = OpCodes.Jmp;
                            break;
                        }
                    }
                }
            }

            if (index != -1)
            {
                
                codes[index].opcode = OpCodes.Jmp;
            }

            return codes;
        }
    }
}
