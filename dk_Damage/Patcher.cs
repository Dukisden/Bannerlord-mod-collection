using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DK_Collection.dk_Damage
{
    [HarmonyPatch]
    internal class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamage")]
        public static void IncreaseBluntVsArmor(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, DamageTypes damageType, float magnitude, int speedBonus, bool cancelDamage, ref int inflictedDamage, ref int absorbedByArmor)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableDamage)
            {
                return;
            }

            if (damageType == DamageTypes.Blunt)
            {
                int extraDamage = (int)(absorbedByArmor * 0.2f);
                inflictedDamage += extraDamage;
                absorbedByArmor -= extraDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Blunt damage increased by {extraDamage}");
                }
                return;
            }

            if (damageType == DamageTypes.Cut && !attackerWeapon.WeaponClass.ToString().Contains("Axe"))
            {
                int reducedDamage = (int)(absorbedByArmor * 0.2f);
                inflictedDamage -= reducedDamage;
                absorbedByArmor += reducedDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Cut damage reduced by {reducedDamage}");
                }
                return;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamageOnShield")]
        public static void IncreaseAxeVsShield(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, float blowMagnitude, ref int inflictedDamage)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableDamage)
            {
                return;
            }

            if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
            {
                int extraDamage = (int)(inflictedDamage * 1.5f);
                inflictedDamage += extraDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Shield damage increased by {extraDamage}");
                }
            }
        }
    }
}