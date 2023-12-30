using HarmonyLib;
using Railloader;


namespace InterchangeUnchained
{
    public class InterchangeUnchained : PluginBase, IUpdateHandler
    {
        public InterchangeUnchained()
        {
            new Harmony("Moloch.InterchangeUnchained").PatchAll(GetType().Assembly);
        }

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        public void Update()
        {
        }
    }
}
