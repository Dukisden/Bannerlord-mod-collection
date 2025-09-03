using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DukisCollection.dk_Damage
{
    [HarmonyPatch]
    internal class DamagePatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamage")]
        public static void BlowDamagePatch(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, DamageTypes damageType, float magnitude, int speedBonus, bool cancelDamage, ref int inflictedDamage, ref int absorbedByArmor)
        {
            if (MCMSettings.Instance.EnableArmorAmplify)
            {
                AmpliflyArmorEffect(attackInformation, attackerWeapon, damageType, ref inflictedDamage, ref absorbedByArmor);
            }

            if (MCMSettings.Instance.EnableDamageMults)
            {
                ApplyDamageMults(attackInformation, ref inflictedDamage, ref absorbedByArmor);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamageOnShield")]
        public static void IncreaseAxeVsShield(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, float blowMagnitude, ref int inflictedDamage)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableArmorAmplify)
            {
                return;
            }

            float shieldMult = MCMSettings.Instance.ShieldMult;

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
            float bluntMult = MCMSettings.Instance.BluntMult;
            float cutMult = MCMSettings.Instance.CutMult;

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

            if (!victim.IsActive() || victim.IsMount)
            {
                return;
            }

            float multiplier = 1f;

            if (victim.IsMainAgent)
            {
                multiplier = MCMSettings.Instance.DamageMultiplierPlayer;
            }
            else if (isFamily(victim))
            {
                multiplier = MCMSettings.Instance.DamageMultiplierFamily;
            }
            else if (isClan(victim))
            {
                multiplier = MCMSettings.Instance.DamageMultiplierClan;
            }
            else if (isPlayerTroop(victim))
            {
                multiplier = MCMSettings.Instance.DamageMultiplierPlayerTroops;
            }
            else if (isAiHero(victim))
            {
                multiplier = MCMSettings.Instance.DamageMultiplierAILords;
            }
            else
            {
                multiplier = MCMSettings.Instance.DamageMultiplierAITroops;
            }

            if (victim.IsHero && victim.Formation != null && victim.Formation.CountOfUnits > 15)
            {
                if (isPlayerTroop(victim))
                {
                    multiplier *= MCMSettings.Instance.DamageMultiplierFormation;
                }
                else if (isAiHero(victim))
                {
                    multiplier *= MCMSettings.Instance.DamageMultiplierAiFormation;
                }
            }

            if (isBodyguard(victim))
            {
                multiplier *= MCMSettings.Instance.DamageMultiplierBodyguard;
            }

            inflictedDamage = (int)(inflictedDamage * multiplier);
            absorbedByArmor = (int)(absorbedByArmor * multiplier);
        }

        public static bool isFamily(Agent agent)
        {
            if (!agent.IsHero) return false;

            CharacterObject? characterObject = agent.Character as CharacterObject;
            Hero? hero = characterObject?.HeroObject;

            if (hero == null) return false;

            bool check1 = hero.Siblings.Contains(Hero.MainHero);
            bool check2 = hero.Father == Hero.MainHero || Hero.MainHero.Father == hero;
            bool check3 = hero.Mother == Hero.MainHero || Hero.MainHero.Mother == hero;

            return (check1 || check2 || check3);
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

        public static bool isPlayerTroop(Agent agent)
        {
            return agent.IsPlayerTroop;
        }

        public static bool isBodyguard(Agent agent)
        {
            return agent.Formation != null && agent.Formation.FormationIndex == FormationClass.Bodyguard;
        }
    }
}