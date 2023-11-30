using BrutalEvent.Models;
using HarmonyLib;

namespace BrutalEvent
{
    [HarmonyPatch(typeof(TimeOfDay), "Start")]
    public class QuotaAjuster
    {
        private static void Prefix(TimeOfDay __instance)
        {
            Configuration.mls.LogWarning("Changing quota variables in patch!");
            __instance.quotaVariables.startingQuota = Plugin._config.StartingQuota.Value;
            __instance.quotaVariables.startingCredits = Plugin._config.StartingCredits.Value;
            __instance.quotaVariables.baseIncrease = Plugin._config.BaseIncrease.Value;
            __instance.quotaVariables.deadlineDaysAmount = Plugin._config.DeadlineDaysAmount.Value;
        }
    }
}
