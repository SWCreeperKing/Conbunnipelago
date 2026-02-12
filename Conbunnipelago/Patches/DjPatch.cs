using Conbunnipelago.Archipelago;
using HarmonyLib;

namespace Conbunnipelago.Patches;

[PatchAll]
public static class DjPatch
{
    [HarmonyPatch(typeof(NPCTextManager), "TriggerDialogue"), HarmonyPrefix]
    public static bool Goal(NPCTextManager __instance)
    {
        if (__instance.gameObject.name is not "MesaDJ") return true;
        Core.Log.Msg($"You have [{BnuyClient.Cds}] CDs out of [{BnuyClient.CdsToGoal}]");
        BnuyClient.Client.Goal();
        return true;
    }

    [HarmonyPatch(typeof(DialogueManager), "IsExceptionSentences"), HarmonyPostfix]
    public static void DialoguePatch(Dialogue dialogue)
    {
        switch (dialogue.englishName)
        {
            case "Taladra":
                CollectablePatch.GetSkin(3);
                break;
            case "Fuya":
                CollectablePatch.GetSkin(5);
                break;
        }
        
        Core.Log.Msg($"Getting dialogue from: [{dialogue.englishName}]");
    }
}