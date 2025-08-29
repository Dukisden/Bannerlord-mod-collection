using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace DK_Collection.dk_Companions
{
    [HarmonyPatch]
    internal class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RemoveCompanionAction), nameof(RemoveCompanionAction.ApplyByDeath))]
        public static void KeepCompanionOnClanPageOnDeath(Clan clan, Hero companion)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableCompanions)
            {
                return;
            }

            InformationManager.ShowInquiry(new InquiryData(
                $"Preserve {companion.Name} on clan page ?",
                $"{companion.Name} died. Do you want to honor their service for {clan.Name} ?",
                true, true,
                "Yes", "No",
                () => {
                    companion.CompanionOf = null;
                    companion.Clan = clan;
                },
                new Action(InformationManager.HideInquiry)
             ), true);
        }
    }
}
