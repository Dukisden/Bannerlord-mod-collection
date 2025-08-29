using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.GameComponents;

namespace DK_Collection.dk_Death
{
    internal class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetSurvivalChance")]
        public static void DieDieDIe(CharacterObject character, ref float __result)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableDeath)
            {
                return;
            }

            if (character.IsHero)
            {
                float factor = 0.5f;

                if (MCMSettings.Instance != null)
                {
                    factor = 1f - (MCMSettings.Instance.DeathFactor / 100f);
                }

                __result *= factor;
            }
        }
    }
}
