using HarmonyLib;
using Railloader;


namespace InterchangeUnchained
{
    public class InterchangeUnchained : PluginBase
    {
        public InterchangeUnchained()
        {
            new Harmony("Moloch.InterchangeUnchained").PatchAll(GetType().Assembly);
        }
    }
}
