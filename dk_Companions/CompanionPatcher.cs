using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace DukisCollection.dk_Companions
{
    [HarmonyPatch]
    internal class CompanionPatcher
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
                    companion.SetNewOccupation(Occupation.Lord);
                },
                new Action(InformationManager.HideInquiry)
             ), true);
        }
    }
}
