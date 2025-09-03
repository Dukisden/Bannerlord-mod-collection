using HarmonyLib;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DukisCollection.dk_Damage
{
    [HarmonyPatch]
    internal class DamagePatcher
    {
        private static MCMSettings MCM = MCMSettings.Instance;
        public static Formation? BodyguardFormation = null;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamage")]
        public static void BlowDamagePatch(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, DamageTypes damageType, float magnitude, int speedBonus, bool cancelDamage, ref int inflictedDamage, ref int absorbedByArmor)
        {
            if (MCM.EnableArmorAmplify)
            {
                AmpliflyArmorEffect(attackInformation, attackerWeapon, damageType, ref inflictedDamage, ref absorbedByArmor);
            }

            if (MCM.EnableDamageMults)
            {
                ApplyDamageMults(attackInformation, ref inflictedDamage, ref absorbedByArmor);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamageOnShield")]
        public static void IncreaseAxeVsShield(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, float blowMagnitude, ref int inflictedDamage)
        {
            if (!MCM.EnableArmorAmplify)
            {
                return;
            }

            float shieldMult = MCM.ShieldMult / 100f;

            if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
            {
                int extraDamage = (int)(inflictedDamage * shieldMult);
                inflictedDamage += extraDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Shield damage increased by {extraDamage}");
                }
            }
        }

        public static void AmpliflyArmorEffect(in AttackInformation attackInformation, WeaponComponentData attackerWeapon, DamageTypes damageType, ref int inflictedDamage, ref int absorbedByArmor)
        {
            float bluntMult = MCM.BluntMult / 100f;
            float cutMult = MCM.CutMult / 100f;

            if (damageType == DamageTypes.Blunt)
            {
                int extraDamage = (int)(absorbedByArmor * bluntMult);
                inflictedDamage += extraDamage;
                absorbedByArmor -= extraDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Blunt damage increased by {extraDamage}");
                }
                return;
            }

            if (damageType == DamageTypes.Cut && !attackerWeapon.WeaponClass.ToString().Contains("Axe") && !attackerWeapon.WeaponClass.ToString().Contains("Dagger"))
            {
                int reducedDamage = (int)(absorbedByArmor * cutMult);
                inflictedDamage -= reducedDamage;
                absorbedByArmor += reducedDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Cut damage reduced by {reducedDamage}");
                }
                return;
            }
        }

        public static void ApplyDamageMults(in AttackInformation attackInformation, ref int inflictedDamage, ref int absorbedByArmor)
        {
            Agent victim = attackInformation.VictimAgent;

            if (!victim.IsActive() || victim.IsMount || Mission.Current.MainAgent == null)
            {
                return;
            }

            float multiplier = 1f;

            if (victim.IsMainAgent)
            {
                multiplier = MCM.DamageMultiplierPlayer;
            }
            else if (isFamily(victim))
            {
                multiplier = MCM.DamageMultiplierFamily;
            }
            else if (isClan(victim))
            {
                multiplier = MCM.DamageMultiplierClan;
            }
            else if (isPlayerTeam(victim))
            {
                multiplier = MCM.DamageMultiplierPlayerTroops;
            }
            else if (isAiHero(victim))
            {
                multiplier = MCM.DamageMultiplierAILords;
            }
            else
            {
                multiplier = MCM.DamageMultiplierAITroops;
            }

            if (victim.IsHero && !isBodyguard(victim) && victim.Formation != null && victim.Formation.CountOfUnits > 15)
            {
                if (isPlayerTeam(victim))
                {
                    multiplier *= (MCM.DamageMultiplierFormation / 100f);
                }
                else if (isAiHero(victim))
                {
                    multiplier *= (MCM.DamageMultiplierAiFormation / 100f);
                }
            }

            if (isBodyguard(victim))
            {
                multiplier *= (MCM.DamageMultiplierBodyguard / 100f);
            }

            multiplier /= 100f; // convert MCM's int percent for player convenience to %float

            inflictedDamage = (int)(inflictedDamage * multiplier);
            absorbedByArmor = (int)(absorbedByArmor * multiplier);
        }

        public static bool isFamily(Agent agent)
        {
            if (!agent.IsHero) return false;

            CharacterObject? characterObject = agent.Character as CharacterObject;
            Hero? hero = characterObject?.HeroObject;

            if (hero == null) return false;

            return Utils.IsFamily(hero);
        }

        public static bool isClan(Agent agent)
        {
            if (!agent.IsHero) return false;

            CharacterObject? characterObject = agent.Character as CharacterObject;
            Hero? hero = characterObject?.HeroObject;

            if (hero == null) return false;
            return hero.Clan == Hero.MainHero.Clan;
        }

        public static bool isAlly(Agent agent)
        {
            return agent.Team == Mission.Current.PlayerAllyTeam;
        }

        public static bool isAiHero(Agent agent)
        {
            if (!agent.IsHero) return false;

            return agent.IsAIControlled;
        }

        public static bool isPlayerTeam(Agent agent)
        {
            return agent.Team == Mission.Current.MainAgent.Team;
        }

        public static bool isBodyguard(Agent agent)
        {
            if (agent.Formation == null)
            {
                return false;
            }

            if (isPlayerTeam(agent) && BodyguardFormation != null)
            {
                // BodyguardFormation is retrieved from the Bodyguards mod, if it's present.
                return agent.Formation == BodyguardFormation;
            }

            return agent.Formation.FormationIndex == FormationClass.Bodyguard;
        }
    }
}