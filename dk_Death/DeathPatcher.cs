using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

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
                Hero hero = character.HeroObject;

                float deathChance = 1f - __result;
                float _deathChance = deathChance;

                if (MCM.DeathBias != 50f)
                {
                    deathChance = CalculateDeathChanceWithNewBias(deathChance, MCM.DeathBias);
                    _deathChance = deathChance;
                }

                float multiplier = 1f;

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

                deathChance *= multiplier;

                if (MCM.DeathDebug)
                {
                    Utils.Log($"Death chance: {stringify(deathChance)} (was {stringify(_deathChance)} before multipliers and {stringify(1f - __result)} natively)");
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

            if (effectedAgent.Formation.CountOfUnits > 15 && effectedAgent.Team == Mission.Current.PlayerTeam)
            {
                multiplier = MCM.DeathFactorFormation;
            }

            float deathChance = __result * multiplier;

            if (MCM.DeathDebug)
            {
                Utils.Log($"Death chance with Formation: {stringify(deathChance)} (was {stringify(__result)})");
            }

            // result is death chance
            __result = MBMath.ClampFloat(deathChance, 0f, 1f);
        }

        public static string CalculateEstimate(float bias)
        {
            return (CalculateDeathChanceWithNewBias(0.0085f, bias) * 100f).ToString("F2") + "%";
        }

        private static float CalculateDeathChanceWithNewBias(float oldDeathChance, float newDeathBias)
        {
            float nativeBias = 50f;
            float fudgeValue = 2.5f; // normally, this is the result of calculation with armor, age, medecine, ..

            // approximately reverse engineering the death chance calculation, see DefaultPartyHealingModel.GetSurvivalChance()
            float valueBeforeBias = ((1f / oldDeathChance) - fudgeValue) / nativeBias;

            float newBiasedValue = valueBeforeBias * newDeathBias;

            // recalculate the death chance with our new bias
            return 1f / (newBiasedValue + valueBeforeBias);
        }

        private static string stringify(float deathChance)
        {
            return (MBMath.ClampFloat(deathChance, 0f, 1f) * 100).ToString("F2") + "%";
        }
    }
}
