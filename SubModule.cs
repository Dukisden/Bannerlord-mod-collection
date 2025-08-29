using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace DK_Collection
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            Harmony harmony = new Harmony("dk_collection");
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