using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace DukisCollection.dk_Death
{
    [HarmonyPatch]
    internal class DeathPatcher
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetSurvivalChance")]
        public static void DieDieDie(CharacterObject character, ref float __result)
        {
            if (!MCM.EnableDeath)
            {
                return;
            }

            if (character.IsHero)
            {
                float multiplier = 1f;
                Hero hero = character.HeroObject;

                if (hero == Hero.MainHero)
                {
                    multiplier = MCM.DeathFactorPlayer;
                }
                else if (Utils.IsFamily(hero))
                {
                    multiplier = MCM.DeathFactorFamily;
                }
                else if (hero.Clan == Hero.MainHero.Clan)
                {
                    multiplier = MCM.DeathFactorClan;
                }
                else if (hero.Clan != null && Hero.MainHero.Clan != null && hero.Clan.Kingdom == Hero.MainHero.Clan.Kingdom)
                {
                    multiplier = MCM.DeathFactorKingdom;
                }
                else
                {
                    multiplier = MCM.DeathFactorAi;
                }

                float deathChance = (1f - __result) * multiplier;

                if (MCM.DeathDebug)
                {
                    Utils.Log($"Death chance: {(int)(MBMath.ClampFloat(deathChance, 0f, 1f) * 100)}%, was {(int)((1f - __result) * 100)}%");
                }

                // result is survival chance
                __result = 1f - MBMath.ClampFloat(deathChance, 0f, 1f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SandboxAgentDecideKilledOrUnconsciousModel), "GetAgentStateProbability")]
        public static void DieDieInCombat(Agent affectorAgent, Agent effectedAgent, ref float __result)
        {
            if (!MCM.EnableDeath || !effectedAgent.IsHero || effectedAgent.Formation == null)
            {
                return;
            }

            float multiplier = 1f;

            if (effectedAgent.Formation.CountOfUnits > 15 && effectedAgent.Team == Mission.Current.MainAgent.Team)
            {
                multiplier = MCM.DeathFactorFormation;
            }

            float deathChance = __result * multiplier;

            if (MCM.DeathDebug)
            {
                Utils.Log($"Death chance with Formation: {(int)(MBMath.ClampFloat(deathChance, 0f, 1f) * 100)}%, was {(int)((__result) * 100)}%");
            }

            // result is death chance
            __result = MBMath.ClampFloat(deathChance, 0f, 1f);
        }
    }
}
