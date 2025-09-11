using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace DukisCollection.dk_Bleed
{
    public class BleedMissionBehavior : MissionBehavior
    {
        private static MCMSettings MCM = MCMSettings.Instance;
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
        private readonly BleedLogic BleedManager = new();

        public override void OnRegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow blow, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
        {
            if (!MCM.EnableBleed)
            {
                return;
            }

            if (victim == null || victim.Health <= 0)
            {
                return;
            }

            float bleedChance = BleedLogic.BleedProc(victim, blow, attackerWeapon, attacker);

            if (MBRandom.RandomFloat >= bleedChance)
            {
                BleedManager.ApplySlow(victim);
                return;
            }

            int bleedAmount = BleedLogic.CalculateBleed(blow);

            if (bleedAmount > 0)
            {
                BleedManager.AddBleed(victim, attacker, bleedAmount);
            }

            BleedManager.ApplySlow(victim);
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
                        BleedManager.ApplySlow(bleedStatus.Agent, true);
                    }
                    else
                    {
                        BleedManager.ApplySlow(bleedStatus.Agent);
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