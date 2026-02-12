using Conbunnipelago.Archipelago;
using HarmonyLib;

namespace Conbunnipelago.Patches;

[PatchAll]
public class DashPatch
{
    [HarmonyPatch(typeof(PlayerDash), "Update"), HarmonyPrefix]
    public static bool DashControl() => BnuyClient.Items.Contains("Dash");
}