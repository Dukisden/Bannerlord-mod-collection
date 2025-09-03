using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace DukisCollection.dk_Damage
{
    [HarmonyPatch]
    internal class BodyguardFormationPatch
    {
        public static bool Prepare()
        {
            Type? targetType = AccessTools.TypeByName("Bodyguards.AddBodyguardsMissionBehavior");

            return targetType != null;
        }
        public static MethodBase? TargetMethod()
        {
            Type? targetType = AccessTools.TypeByName("Bodyguards.AddBodyguardsMissionBehavior");

            return AccessTools.Method(targetType, "TransferUnits");
        }

        [HarmonyPostfix]
        public static void RetrieveBodyguardFormation(Formation newFormation, bool defaultFormations)
        {
            if (defaultFormations || newFormation.Team != Mission.Current.MainAgent.Team)
                return;

            DamagePatcher.BodyguardFormation = newFormation;
        }
    }
}
