using HarmonyLib;
using UnityEngine;

namespace Conbunnipelago.Patches;

[PatchAll]
public static class FreeCursorPatch
{
    [HarmonyPatch(typeof(MainMenuManager), "Start"), HarmonyPostfix]
    public static void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}