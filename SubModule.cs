using HarmonyLib;
using TaleWorlds.MountAndBlade;


namespace DukisCollection
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("DukisCollection");
            harmony.PatchAll();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            if (mission.CombatType.ToString() == "Combat")
            {
                mission.AddMissionBehavior(new dk_Bleed.BleedMissionBehavior());
            }
        }

    }
}