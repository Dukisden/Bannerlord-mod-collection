using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;

namespace DukisCollection.dk_Siege
{
    [HarmonyPatch]
    internal class SiegePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MobilePartyAi), "GetBesiegeBehavior")]
        public static void ForceAssault(ref AiBehavior shortTermBehavior, ref Settlement shortTermTargetSettlement, MobileParty ____mobileParty)
        {
            if (!MCMSettings.Instance.ForceAssaultToggle)
            {
                return;
            }

            Settlement? settlement = Campaign.Current.MainParty.CurrentSettlement;
            SiegeEvent? siegeEvent = Campaign.Current.MainParty.SiegeEvent;

            if (settlement == null && siegeEvent != null)
            {
                // player is attacking
                settlement = siegeEvent.BesiegedSettlement;
            }

            if (siegeEvent == null && settlement != null)
            {
                // player is defending
                siegeEvent = settlement.SiegeEvent;
            }

            if (siegeEvent != null && siegeEvent.BesiegerCamp.LeaderParty == Campaign.Current.MainParty)
            {
                Utils.Log("Error: Player is leading the siege.");
                MCMSettings.Instance.ForceAssaultToggle = false;
                return;
            }

            if (settlement == null || !settlement.IsUnderSiege)
            {
                Utils.Log("Error: Player not involved in a siege.");
                MCMSettings.Instance.ForceAssaultToggle = false;
                return;
            }

            if (____mobileParty.TargetSettlement == settlement)
            {
                shortTermBehavior = AiBehavior.AssaultSettlement;
                shortTermTargetSettlement = settlement;
                MCMSettings.Instance.ForceAssaultToggle = false;
            }
        }
    }
}