using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.GameComponents;

namespace DukisCollection.dk_Death
{
    [HarmonyPatch]
    internal class DeathPatcher
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
                float multiplier = 2f;

                if (MCMSettings.Instance != null)
                {
                    multiplier = MCMSettings.Instance.DeathFactor;
                }

                float deathChance = (1f - __result) * multiplier;

                if (MCMSettings.Instance.DeathDebug)
                {
                    Utils.Log($"Death chance: {(int)(MBMath.ClampFloat(deathChance, 0f, 1f) * 100)}%, was {(int)((1f - __result) * 100)}%");
                }

                __result = 1f - MBMath.ClampFloat(deathChance, 0f, 1f);
            }
        }
    }
}
