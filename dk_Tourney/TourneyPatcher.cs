using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace DukisCollection.dk_Tourney
{
    [HarmonyPatch]
    internal class TourneyPatcher
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerEliminated")]
        public static void RoundReward(TournamentBehavior __instance)
        {
            DistributeRewards(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
        public static void TourneyReward(TournamentBehavior __instance)
        {
            DistributeRewards(__instance);
        }

        private static void DistributeRewards(TournamentBehavior tourney)
        {
            if (!MCM.EnableTourney)
            {
                return;
            }

            int baseCost = MCM.EntryFee;
            Utils.Log($"You expend {baseCost}g for the tournament's participation fee");
            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, baseCost);

            int roundRewardBase = MCM.RoundRewardBase;
            int roundIndex = tourney.CurrentRoundIndex;
            int reward = roundRewardBase * (roundIndex * roundIndex);

            if (reward > 0)
            {
                Utils.Log($"You are rewarded {reward}g for reaching round {roundIndex + 1} in the tournament");
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, reward);
            }
        }
    }
}