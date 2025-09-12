using DukisCollection.dk_Companions;
using DukisCollection.dk_Tourney;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;


namespace DukisCollection
{
    public class SubModule : MBSubModuleBase
    {
        public static string Version = ModuleHelper.GetModuleInfo("DukisCollection").Version.ToString();
        public static string SettingsVersion = "1";

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
        public override void OnConfigChanged()
        {
            base.OnConfigChanged();

            MCMSettings.Instance.ToggleSettingsArmor = false;
            MCMSettings.Instance.ToggleSettingsDamage = false;
            MCMSettings.Instance.ToggleSettingsDeath = false;
            MCMSettings.Instance.ToggleSettingsBleed = false;
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);

            if (game.GameType is Campaign)
            {
                MCMSettings.Instance.CheckVersion();

                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)starterObject;
                campaignGameStarter.AddBehavior(new DK_CollectionBehavior());
            }
        }
    }

    internal class DK_CollectionBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, PreserveCompanion.OnHeroKilled);
            CampaignEvents.OnPlayerJoinedTournamentEvent.AddNonSerializedListener(this, PartialTourneyReward.OnPlayerJoined);
            CampaignEvents.PlayerEliminatedFromTournament.AddNonSerializedListener(this, PartialTourneyReward.OnPlayerEliminated);
        }

        public override void SyncData(IDataStore dataStore) {}
    }
}