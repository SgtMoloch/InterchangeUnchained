using GalaSoft.MvvmLight.Messaging;
using Game.Events;
using HarmonyLib;
using Railloader;
using Serilog;


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
