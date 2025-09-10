using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.CampaignSystem.CharacterDevelopment.DefaultPerks;

namespace DukisCollection.dk_Bleed
{
    internal class BleedLogic
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        public Dictionary<int, BleedStatus> BleedingAgents = new();

        public static float BleedProc(Agent victim, Blow blow, MissionWeapon attackerWeapon, Agent attacker)
        {
            if (victim.IsMount)
            {
                return 1f;
            }
            if (blow.InflictedDamage <= MCM.MinDamageForBleed) 
            {
                return 0f;
            }
            if ((!attacker.IsHero && !MCM.canTroopBleed) || (attacker.IsHero && !MCM.canHeroesBleed))
            {
                return 0f;
            }

            // Config values
            float procBasePlayer    = MCM.BC_Player;
            float procBaseHero      = MCM.BC_Hero;
            float procBaseTroop     = MCM.BC_Troop;

            float armorFactor       = MCM.BF_Armor;
            float damageFactor      = MCM.BF_Damage;
            float healthFactor      = MCM.BF_Health;

            float minChance         = MCM.BleedMinChance;
            float minChanceDagger   = MCM.BleedMinChanceDagger;

            // Core calculations
            float damagePercent = blow.InflictedDamage / victim.HealthLimit;
            float armorRatio = blow.AbsorbedByArmor / blow.InflictedDamage;
            float healthPercent = (victim.Health + blow.InflictedDamage) / victim.HealthLimit;
            float bodyPartMultiplier = GetBodyPartFactor(blow.VictimBodyPart);

            float baseChance = !victim.IsAIControlled ? procBasePlayer : victim.IsHero ? procBaseHero : procBaseTroop;
            float damageMod = damagePercent * damageFactor;
            float armorMod = armorRatio * armorFactor;
            float healthMod = healthPercent * healthFactor;

            float procChance = baseChance + damageMod - armorMod - healthMod;

            procChance = Math.Max(procChance, minChance);

            procChance *= bodyPartMultiplier;

            procChance = Utils.Clamp(procChance, 0f, 100f);

            // Dagger special case
            if (attackerWeapon.CurrentUsageItem != null && attackerWeapon.CurrentUsageItem.WeaponClass == WeaponClass.Dagger)
            {
                procChance = Math.Max(procChance, minChanceDagger);
            }

            if (!attacker.IsHero)
            {
                procChance *= MCM.BC_TroopPenalty / 100f;
            }

            // Logging
            if (MCM.BleedDebug && (attacker.IsMainAgent || victim.IsMainAgent))
            {
                Utils.Log($"Proc chance: {(int)(procChance)}% (base:{baseChance}, dmg: +{(int)(damageMod)}, armor: -{(int)(armorMod)}, hp: -{(int)(healthMod)}, bodypart: x{bodyPartMultiplier})");
            }

            return procChance / 100f;
        }

        public static int CalculateBleed(Blow blow)
        {
            float baseMult          = MCM.BD_Base / 100f;
            float cutFlatReduction  = MCM.BD_ArmorFlat / 100f;
            float cutArmorMult      = MCM.BD_ArmorCut / 100f;
            float missileMult       = MCM.BD_Missile / 100f;
            float pierceMult        = MCM.BD_Pierce / 100f;
            float bluntMult         = MCM.BD_Blunt / 100f;

            int bleed = (int)(blow.InflictedDamage * baseMult);

            if (blow.DamageType == DamageTypes.Cut)
            {
                bleed -= (int)Math.Min(cutFlatReduction, blow.AbsorbedByArmor * cutArmorMult);
            }

            if (blow.IsMissile)
            {
                bleed = (int)(bleed * missileMult);
            }

            return blow.DamageType switch
            {
                DamageTypes.Cut => bleed,
                DamageTypes.Pierce => (int)(bleed * pierceMult),
                DamageTypes.Blunt => (int)(bleed * bluntMult),
                _ => 0,
            };
        }

        public static void BleedTick(BleedStatus bleedStatus)
        {
            int minHealth = bleedStatus.Agent.IsMainAgent ? 1 : 0;

            bleedStatus.Agent.Health = Math.Max(minHealth, bleedStatus.Agent.Health - bleedStatus.BleedTick);
            bleedStatus.Bled += bleedStatus.BleedTick;

            //Utils.Log("Bleeding! -" + bleedStatus.BleedTick + " (" + bleedStatus.Bled + "/" + bleedStatus.BleedAmount + ")" + " Remaining Health: " + bleedStatus.Agent.Health);

            if (bleedStatus.Agent.Health < 1f && bleedStatus.Agent.IsActive())
            {
                KillAgent(bleedStatus.Agent, bleedStatus.Attacker, bleedStatus.Bled);
                return;
            }

            float secondsTilNextTick = 1.1f - Math.Min(0.5f, bleedStatus.BleedStack / 10);
            bleedStatus.NextTick = MissionTime.SecondsFromNow(secondsTilNextTick);
        }

        public void AddBleed(Agent victim, Agent attacker, int bleedAmount)
        {
            float maxDuration = MCM.MaxBleedDuration;

            if (BleedingAgents.TryGetValue(victim.Index, out BleedStatus? bleedStatus))
            {
                int extraPerTickDamage = MCM.BleedStackFlat;
                float extraPercentDamage = MCM.BleedStackPercent / 100f;

                int extraDamageFromHit = (int)(bleedAmount * extraPercentDamage);
                int extraDamageFromStack = extraPerTickDamage * bleedStatus.TicksLeft;

                int extraBleedAmount = extraDamageFromHit + extraDamageFromStack;

                bleedStatus.BleedAmount += extraBleedAmount;
                bleedStatus.BleedTick += (int)(extraBleedAmount / maxDuration);
                bleedStatus.BleedStack++;

                if (attacker.IsMainAgent || victim.IsMainAgent)
                {
                    Utils.Log("Bleed stack increased !");
                    Utils.Log($"Bleeding for {bleedStatus.Bled + extraBleedAmount} ({extraBleedAmount} extra from stack)");
                }
            }
            else
            {
                BleedingAgents[victim.Index] = new BleedStatus
                {
                    Agent = victim,
                    Attacker = attacker,
                    BleedAmount = bleedAmount,
                    BleedTick = (int)(Math.Ceiling(bleedAmount / maxDuration)),
                    NextTick = MissionTime.SecondsFromNow(1)
                };

                if (attacker.IsMainAgent || victim.IsMainAgent)
                {
                    Utils.Log($"Bleeding for {bleedAmount}hp");
                }
            }
        }

        public void ApplySlow(Agent agent, bool skipBleedSlow = false)
        {
            if (!agent.IsActive() || agent.Health <= 0 || MCM.SlowAmount + MCM.BleedSlowAmount == 200)
            {
                return;
            }

            float slowPercent = MCM.SlowAmount / 100f;
            float bleedSlowPercent = MCM.BleedSlowAmount / 100f;

            float healthPercent = agent.Health / agent.HealthLimit;
            float healthSlow = slowPercent + ((1f - slowPercent) * healthPercent);
            float bleedSlow = 1f;

            if (!skipBleedSlow && BleedingAgents.ContainsKey(agent.Index))
            {
                bleedSlow = bleedSlowPercent + ((1f - bleedSlowPercent) * healthPercent);
            }

            float finalSlow = healthSlow * bleedSlow;

            Utils.Log($"{agent.GetCurrentSpeedLimit()}, finalSlow");
            agent.SetMaximumSpeedLimit(finalSlow, true);
            Utils.Log($"{agent.GetCurrentSpeedLimit()}");
            Utils.Log("---");
        }

        private static void KillAgent(Agent victim, Agent attacker, int bled)
        {
            // based on Mission.KillAgentCheat

            Blow blow = new Blow(attacker.Index);
            blow.DamageType = DamageTypes.Blunt;
            blow.BoneIndex = victim.Monster.HeadLookDirectionBoneIndex;
            blow.GlobalPosition = victim.Position;
            blow.GlobalPosition.z += victim.GetEyeGlobalHeight();
            blow.BaseMagnitude = 1f; // modified
            blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
            blow.InflictedDamage = bled; // modified
            blow.SwingDirection = victim.LookDirection;
            if (Mission.Current.InputManager.IsGameKeyDown(2))
            {
                blow.SwingDirection = victim.Frame.rotation.TransformToParent(new Vec3(-1f, 0));
                blow.SwingDirection.Normalize();
            }
            else if (Mission.Current.InputManager.IsGameKeyDown(3))
            {
                blow.SwingDirection = victim.Frame.rotation.TransformToParent(new Vec3(1f, 0));
                blow.SwingDirection.Normalize();
            }
            else if (Mission.Current.InputManager.IsGameKeyDown(1))
            {
                blow.SwingDirection = victim.Frame.rotation.TransformToParent(new Vec3(0f, -1f));
                blow.SwingDirection.Normalize();
            }
            else if (Mission.Current.InputManager.IsGameKeyDown(0))
            {
                blow.SwingDirection = victim.Frame.rotation.TransformToParent(new Vec3(0f, 1f));
                blow.SwingDirection.Normalize();
            }

            blow.Direction = blow.SwingDirection;
            blow.DamageCalculated = true;
            sbyte mainHandItemBoneIndex = attacker.Monster.MainHandItemBoneIndex;
            AttackCollisionData collisionData = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(_attackBlockedWithShield: false, _correctSideShieldBlock: false, _isAlternativeAttack: false, _isColliderAgent: true, _collidedWithShieldOnBack: false, _isMissile: false, _isMissileBlockedWithWeapon: false, _missileHasPhysics: false, _entityExists: false, _thrustTipHit: false, _missileGoneUnderWater: false, _missileGoneOutOfBorder: false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, mainHandItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, victim.Velocity, Vec3.Up);
            victim.RegisterBlow(blow, in collisionData);
        }

        private static float GetBodyPartFactor(BoneBodyPartType part)
        {
            return part switch
            {
                BoneBodyPartType.Head => MCM.BC_Head,
                BoneBodyPartType.Neck => MCM.BC_Neck,
                BoneBodyPartType.Chest => MCM.BC_Chest,
                BoneBodyPartType.Abdomen => MCM.BC_Chest,
                BoneBodyPartType.ShoulderLeft => MCM.BC_Arms,
                BoneBodyPartType.ShoulderRight => MCM.BC_Arms,
                BoneBodyPartType.ArmLeft => MCM.BC_Arms,
                BoneBodyPartType.ArmRight => MCM.BC_Arms,
                BoneBodyPartType.Legs => MCM.BC_Legs,
                _ => 1f
            };
        }
    }

    public class BleedStatus
    {
        public Agent Agent { get; set; }
        public Agent Attacker { get; set; }
        public int BleedAmount { get; set; }
        public int BleedTick { get; set; }
        public int BleedStack { get; set; } = 1;
        public int Bled { get; set; } = 0;
        public MissionTime NextTick { get; set; }
        public int TicksLeft => Math.Max(0, (BleedAmount - Bled) / BleedTick);
    }
}
