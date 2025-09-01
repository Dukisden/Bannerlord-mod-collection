using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;

namespace DukisCollection.dk_Companions
{
    [HarmonyPatch]
    internal class CompanionPatcher
    {
        private static HashSet<string> DeathList = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RemoveCompanionAction), nameof(RemoveCompanionAction.ApplyByDeath))]
        public static void KeepCompanionOnClanPageOnDeath(Clan clan, Hero companion)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableCompanions)
            {
                return;
            }

            if (DeathList.Contains(companion.Name.ToString()))
            {
                return;
            }

            DeathList.Add(companion.Name.ToString());

            InformationManager.ShowInquiry(new InquiryData(
                $"Preserve {companion.Name} on clan page ?",
                $"{companion.Name} died. Do you want to honor their service for {clan.Name} ?",
                true, true,
                "Yes", "No",
                () =>
                {
                    DeathList.Remove(companion.Name.ToString());
                    companion.CompanionOf = null;
                    companion.Clan = clan;
                },
                () => {
                    DeathList.Remove(companion.Name.ToString());
                }
            ), true);
        }
    }
}
