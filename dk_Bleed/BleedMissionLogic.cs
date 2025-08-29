using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace DukisCollection.dk_Bleed
{
    public class BleedMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private readonly BleedLogic BleedManager = new();

        public override void OnRegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow blow, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
        {
            if (MCMSettings.Instance != null && !MCMSettings.Instance.EnableBleed)
            {
                return;
            }

            if (victim == null || blow.InflictedDamage <= 10 || victim.Health <= 0)
            {
                return;
            }

            float bleedChance = BleedLogic.BleedProc(victim, blow, attackerWeapon, attacker);


            if (MBRandom.RandomFloat >= bleedChance)
            {
                return;
            }

            int bleedAmount = BleedLogic.CalculateBleed(blow);

            if (bleedAmount > 0)
            {
                if (attacker.IsMainAgent || victim.IsMainAgent)
                {
                    string info = $"Bleeding for {bleedAmount}";
                    if (MCMSettings.Instance.BleedDebug)
                    {
                        info += $", victim hp: {victim.Health}, procced at {(int)(bleedChance * 100)}%";
                    }
                    Utils.Log(info);
                }
                BleedManager.AddBleed(victim, attacker, bleedAmount);
            }
        }

        public override void OnMissionTick(float dt)
        {
            List<int> toRemove = new ();

            foreach (KeyValuePair<int, BleedStatus> kvp in BleedManager.BleedingAgents)
            {
                int index = kvp.Key;
                BleedStatus bleedStatus = kvp.Value;

                if (bleedStatus.NextTick.IsPast)
                {
                    BleedLogic.BleedTick(bleedStatus);

                    if (bleedStatus.Bled >= bleedStatus.BleedAmount || !bleedStatus.Agent.IsActive())
                    {
                        toRemove.Add(index);
                    }
                }
            }

            foreach (var index in toRemove)
            {
                BleedManager.BleedingAgents.Remove(index);
            }

        }
    }
}