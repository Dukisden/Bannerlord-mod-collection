using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DukisCollection.dk_Bleed
{
    internal class BleedLogic
    {
        private static readonly Dictionary<BoneBodyPartType, float> BodyPartBleedChanceFactor = new ()
        {
            { BoneBodyPartType.Head, 1.5f },
            { BoneBodyPartType.Neck, 2.0f },
            { BoneBodyPartType.Chest, 1.0f },
            { BoneBodyPartType.Abdomen, 0.8f },
            { BoneBodyPartType.ShoulderLeft, 0.4f },
            { BoneBodyPartType.ShoulderRight, 0.4f },
            { BoneBodyPartType.ArmLeft, 0.1f },
            { BoneBodyPartType.ArmRight, 0.1f },
            { BoneBodyPartType.Legs, 0.3f }
        };

        public Dictionary<int, BleedStatus> BleedingAgents = new();

        public static float BleedProc(Agent victim, Blow blow, MissionWeapon attackerWeapon, Agent attacker)
        {
            if (victim.IsMount)
            {
                return 1f;
            }

            float damagePercent = blow.InflictedDamage / victim.HealthLimit;
            float armorRatio = blow.AbsorbedByArmor / blow.InflictedDamage;
            float healthPercent = (victim.Health + blow.InflictedDamage) / victim.HealthLimit;

            float procChance = !victim.IsAIControlled ? 20.0f : victim.IsHero ? 60.0f : 100.0f;
            int baseChance = (int)(procChance);
            procChance += damagePercent * 50.0f;
            procChance -= armorRatio * 20.0f; // armorRatio can be > 1
            procChance -= healthPercent * 30.0f;
            procChance = Math.Max(procChance, 20.0f);

            if (!BodyPartBleedChanceFactor.TryGetValue(blow.VictimBodyPart, out float bodyPartFactor))
            {
                bodyPartFactor = 1.0f; // Default factor if not found
            }
            procChance *= bodyPartFactor;

            if (attackerWeapon.CurrentUsageItem != null && attackerWeapon.CurrentUsageItem.WeaponClass == WeaponClass.Dagger)
            {
                procChance = Math.Max(50, procChance);
            }

            procChance = Math.Min(procChance, 100.0f);
            if (MCMSettings.Instance.BleedDebug && (attacker.IsMainAgent || victim.IsMainAgent))
            {
                Utils.Log($"Proc chance: {(int)(procChance)}(base:{baseChance}, dmg:+{(int)(damagePercent * 50.0f)}, arm:-{(int)(armorRatio * 20.0f)}, hp:-{(int)(healthPercent * 30.0f)}, body:*{bodyPartFactor})");
            }
            return procChance / 100.0f;
        }

        public static int CalculateBleed(Blow blow)
        {
            int bleed = (int)(blow.InflictedDamage / 1.5);

            if (blow.DamageType == DamageTypes.Cut)
            {
                bleed = (int)(bleed - Math.Min(30, blow.AbsorbedByArmor / 2));
            }

            if (blow.IsMissile)
            {
                bleed = bleed / 2;
            }

            return blow.DamageType switch
            {
                DamageTypes.Cut => bleed,
                DamageTypes.Pierce => (int)(bleed / 1.5),
                DamageTypes.Blunt => (int)(bleed / 3),
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

            float secondsTilNextTick = 1.1f - Math.Min(0.6f, bleedStatus.BleedStack / 10);
            bleedStatus.NextTick = MissionTime.SecondsFromNow(secondsTilNextTick);
        }

        public void AddBleed(Agent victim, Agent attacker, int bleedAmount)
        {
            if (BleedingAgents.TryGetValue(victim.Index, out BleedStatus? bleedStatus))
            {
                int ticksLeft = Math.Max(0, (bleedStatus.BleedAmount - bleedStatus.Bled) / bleedStatus.BleedTick);
                bleedStatus.BleedAmount += (int)(ticksLeft + bleedAmount * 0.2f);
                bleedStatus.BleedTick++;
                bleedStatus.BleedStack++;
            }
            else
            {
                BleedingAgents[victim.Index] = new BleedStatus 
                {
                    Agent = victim,
                    Attacker = attacker,
                    BleedAmount = bleedAmount,
                    BleedTick = (int)(Math.Ceiling(bleedAmount / 20f)),
                    NextTick = MissionTime.SecondsFromNow(1)
                };
            }
        }

        private static void KillAgent(Agent victim, Agent attacker, int bled)
        {
            Blow blow = new Blow(attacker.Index);
            blow.DamageType = DamageTypes.Blunt;
            blow.BoneIndex = victim.Monster.HeadLookDirectionBoneIndex;
            blow.GlobalPosition = victim.Position;
            blow.GlobalPosition.z += victim.GetEyeGlobalHeight();
            blow.BaseMagnitude = 1f;
            blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
            blow.InflictedDamage = bled;
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
    }
}
