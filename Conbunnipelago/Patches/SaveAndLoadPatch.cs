using HarmonyLib;

namespace Conbunnipelago.Patches;

[PatchAll]
public static class SaveAndLoadPatch
{
    public static string Path = "null";
    
    [HarmonyPatch(typeof(ES3Settings), MethodType.Constructor, typeof(string), typeof(ES3Settings)), HarmonyPrefix]
    public static void ConstructorPatch(ref string path, ref ES3Settings settings)
    {
        if (Path is "null") return;
        path = Path;
    }
    
    // [HarmonyPatch(typeof(SaveLoadManager), "LoadFile"), HarmonyPostfix]
    // public static void LoadNewFile()
    // {
    //     
    // }
}