using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace DukisCollection.dk_Tourney
{
    internal class PartialTourneyReward
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        public static void OnPlayerEliminated(int roundIndex, Town town)
        {
            if (!MCM.EnableTourney)
            {
                return;
            }

            int roundRewardBase = MCM.RoundRewardBase;
            int reward = roundRewardBase * (roundIndex * roundIndex);

            if (reward > 0)
            {
                Utils.Log($"You are rewarded {reward}g for reaching round {roundIndex + 1} in the tournament");

                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, reward);
            }
        }

        internal static void OnPlayerJoined(Town town, bool isParticipating)
        {
            if (!MCM.EnableTourney || !isParticipating || MCM.EntryFee == 0)
            {
                return;
            }

            Utils.Log($"You expend {MCM.EntryFee}g for the tournament's participation fee");

            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, town.Settlement, MCM.EntryFee);
        }
    }
}