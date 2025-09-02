using HarmonyLib;
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
            if (MCMSettings.Instance.EnableDamage)
            {
                AmpliflyArmorEffect(attackInformation, attackerWeapon, damageType, ref inflictedDamage, ref absorbedByArmor);
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
                int extraDamage = (int)(inflictedDamage * 0.75f);
                inflictedDamage += extraDamage;
                if (true && attackInformation.AttackerAgent.IsMainAgent || attackInformation.VictimAgent.IsMainAgent)
                {
                    Utils.Log($"Shield damage increased by {extraDamage}");
                }
            }
        }

        public static void AmpliflyArmorEffect(in AttackInformation attackInformation, WeaponComponentData attackerWeapon, DamageTypes damageType, ref int inflictedDamage, ref int absorbedByArmor)
        {
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

            if (damageType == DamageTypes.Cut && !attackerWeapon.WeaponClass.ToString().Contains("Axe") && !attackerWeapon.WeaponClass.ToString().Contains("Dagger"))
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
    }
}