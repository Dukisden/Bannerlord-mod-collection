using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace DK_Collection.dk_Tourney
{
    [HarmonyPatch]
    internal class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerEliminated")]
        public static void RoundReward(TournamentBehavior __instance)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableTourney)
            {
                return;
            }

            int baseCost = 100;
            int partialBetReward = (int)(0.15f * __instance.CurrentRoundIndex * __instance.OverallExpectedDenars);
            int roundReward = 50 * (__instance.CurrentRoundIndex * __instance.CurrentRoundIndex);

            int total = partialBetReward + roundReward - baseCost;

            if (total < 0)
            {
                GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, total, false);
            }
            else
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, total);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
        public static void TourneyReward(TournamentBehavior __instance)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableTourney)
            {
                return;
            }

            int roundReward = 50 * (__instance.CurrentRoundIndex * __instance.CurrentRoundIndex);

            GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, roundReward);
        }
    }
}