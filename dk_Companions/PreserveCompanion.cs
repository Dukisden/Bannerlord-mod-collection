using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace DukisCollection.dk_Companions
{
    internal class PreserveCompanion
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        public static void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
        {
            if (!MCM.EnableCompanions)
            {
                return;
            }

            if (victim.CompanionOf != Clan.PlayerClan || victim.Occupation != Occupation.Wanderer)
            {
                return;
            }

            Hero companion = victim;
            Clan clan = Clan.PlayerClan;

            InformationManager.ShowInquiry(new InquiryData(
                $"Preserve {companion.Name} on clan page ?",
                $"{companion.Name} died. Do you want to honor their service for {clan.Name} ?",
                true, true,
                "Yes", "No",
                () => {
                    companion.Clan = clan;
                    companion.SetNewOccupation(Occupation.Lord);
                },
                new Action(InformationManager.HideInquiry)
             ), true);
        }
    }
}
