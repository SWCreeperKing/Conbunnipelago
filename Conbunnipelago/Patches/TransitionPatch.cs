using Conbunnipelago.Archipelago;
using HarmonyLib;

namespace Conbunnipelago.Patches;

[PatchAll]
public static class TransitionPatch
{
    [HarmonyPatch(typeof(TransitionTrigger), "OnTriggerEnter"), HarmonyPrefix]
    public static bool TransitionEnter(CloseAreaMusicChangeTrigger __instance)
    {
        // Core.Log.Msg($"key exists: [{BnuyClient.IdToTransition.ContainsKey(__instance.name)}], [{__instance.name}]");
        if (!BnuyClient.IdToTransition.TryGetValue(__instance.name, out var value)) return true;
        // Core.Log.Msg($"has: [{BnuyClient.Items.Contains(BnuyClient.IdToTransition[__instance.name])}]");
        return BnuyClient.Items.Contains(value);
    }
}