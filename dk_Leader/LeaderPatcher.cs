using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DukisCollection.dk_Leader
{
    [HarmonyPatch]
    internal class LeaderPatcher
    {
        private static MCMSettings MCM = MCMSettings.Instance;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AssignPlayerRoleInTeamMissionController), MethodType.Constructor, new Type[] { typeof(bool), typeof(bool), typeof(bool), typeof(List<string>), typeof(FormationClass) })]
        public static void ForceGeneral(ref bool isPlayerGeneral, ref bool isPlayerSergeant, ref bool isPlayerInArmy)
        {
            if (MCM.LeadAttackToggle && isPlayerInArmy && isPlayerSergeant)
            {
                isPlayerGeneral = !isPlayerGeneral;
                isPlayerSergeant = !isPlayerSergeant;

                MCM.LeadAttackToggle = false;
            }
        }
    }
}
