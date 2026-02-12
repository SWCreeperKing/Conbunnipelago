using Conbunnipelago.Archipelago;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Conbunnipelago.Patches;

[PatchAll]
public static class CollectablePatch
{
    [HarmonyPatch(typeof(PorteriaPP), "PorteriaPPEnterFile"), HarmonyPostfix]
    public static void RemoveBlockers(PorteriaPP __instance)
    {
        __instance.cintasHolder.SetActive(false);
    }
    
    // most of this is just cleaning the save data for duplicates for 'just in case'
    [HarmonyPatch(typeof(CoinController), "OnTriggerEnter"), HarmonyPrefix]
    public static bool CoinsCollected(CoinController __instance, Collider other)
    {
        try
        {
            if (other.name != "Player") return false;
            var coinList = SaveLoadManager.Instance.GetCoinList();
            var controllerList = coinList.Select(gobj => gobj.GetComponent<CoinController>()).ToArray();

            SaveLoadManager.Instance.monedasTotales[SaveLoadManager.Instance.saveFileID]
                = Refresh(coinList, controllerList);

            CleanCollectedCount();
            try { BnuyClient.Client.SendLocation(BnuyClient.IdToLocation[__instance.GetUniqueId()]); }
            catch (KeyNotFoundException)
            {
                Core.Log.Error($"Key not found!!?? REPORT IMMEDIATELY [{__instance.GetUniqueId()}]");
            }
            // Core.Log.Msg($"Collecting: [{__instance.GetUniqueId()}], [{ApShenanigans.IdsHave.Contains(__instance.GetUniqueId())}]");

            if (!coinList.Contains(__instance.gameObject)) return true;
            __instance.gameObject.SetActive(false);
            return false;
        }
        catch (Exception e) { Core.Log.Error(e); }
        return true;
    }
    
    // most of this is just cleaning the save data for duplicates for 'just in case'
    [HarmonyPatch(typeof(CDController), "OnTriggerEnter"), HarmonyPrefix]
    public static bool CdCollected(CDController __instance, Collider other)
    {
        try
        {
            if (other.name != "Player") return false;
            var cdList = SaveLoadManager.Instance.GetCdList();
            var controllerList = cdList.Select(gobj => gobj.GetComponent<CDController>()).ToArray();

            SaveLoadManager.Instance.monedasTotales[SaveLoadManager.Instance.saveFileID]
                = Refresh(cdList, controllerList);

            CleanCollectedCount();
            try { BnuyClient.Client.SendLocation(BnuyClient.IdToLocation[__instance.GetUniqueId()]); }
            catch (KeyNotFoundException)
            {
                Core.Log.Error($"Key not found!!?? REPORT IMMEDIATELY [{__instance.GetUniqueId()}]");
            }
            // Core.Log.Msg($"Collecting: [{__instance.GetUniqueId()}], [{ApShenanigans.IdsHave.Contains(__instance.GetUniqueId())}], [{!cdList.Contains(__instance.gameObject)}]");

            if (!cdList.Contains(__instance.gameObject)) return true;
            __instance.gameObject.SetActive(false);
            return false;
        }
        catch (Exception e) { Core.Log.Error(e); }
        return true;
    }
    
    [HarmonyPatch(typeof(PauseMenu), "ProvideSkins"), HarmonyPrefix]
    public static bool GetSkin(int i)
    {
        try
        {
            var loc = BnuyClient.SkinData[i - 1].Item1;
            if (!BnuyClient.Client.MissingLocations.Contains(loc)) return false;
            BnuyClient.Client.SendLocation(loc);

            var pauseMenu = Object.FindObjectOfType<PauseMenu>();
            pauseMenu.skinArray[i].skinUnlocked = true;
            SaveLoadManager.Instance.oneSkinUnlocked[SaveLoadManager.Instance.saveFileID] = true;
            SaveLoadManager.Instance.skinsTotales[SaveLoadManager.Instance.saveFileID]++;
            pauseMenu.skinChangeIcon.SetActive(true);
            CleanCollectedCount();
        }
        catch (Exception e)
        {
            Core.Log.Error(e);
        }
        return false;
    }

    [HarmonyPatch(typeof(SaveLoadManager), "LoadLists"), HarmonyPostfix]
    public static void UnHideCoins(SaveLoadManager __instance)
    {
        // Core.Log.Msg($"Ids: [{ApShenanigans.IdsHave.Length}]:\n\t- {string.Join("\n\t- ", ApShenanigans.IdsHave)}");
        // Core.Log.Msg($"Ids: [{ApShenanigans.IdsHave.Length}]");
        
        foreach (var collected in __instance.GetCoinList())
        {
            var id = collected.GetComponent<CoinController>().GetUniqueId();
            collected.SetActive(BnuyClient.Client.MissingLocations.Contains(BnuyClient.IdToLocation[id]));
        }

        foreach (var collected in __instance.GetCdList())
        {
            var id = collected.GetComponent<CDController>().GetUniqueId();
            collected.SetActive(BnuyClient.Client.MissingLocations.Contains(BnuyClient.IdToLocation[id]));
        }

        var skins = __instance.GetSkinList();
        var pauseMenu = Object.FindObjectOfType<PauseMenu>();
        for (var i = 0; i < 5; i++)
        {
            var (skin, _) = BnuyClient.SkinData[i];
            var has = !BnuyClient.Client.MissingLocations.Contains(skin);
            skins[i + 1] = has;
            
            if (!has) continue;
            pauseMenu.skinArray[i + 1].skinUnlocked = true;
            SaveLoadManager.Instance.oneSkinUnlocked[SaveLoadManager.Instance.saveFileID] = true;
            pauseMenu.skinChangeIcon.SetActive(true);
        }
        
        CleanCollectedCount();
    }

    public static void CleanCollectedCount()
    {
        var collectableListArea = SaveLoadManager.Instance.GetCoinListArea();
        var collectableListGlobal = SaveLoadManager.Instance.GetCoinListGlobal();
        SaveLoadManager.Instance.CDsTotales[SaveLoadManager.Instance.saveFileID] = 0;
        SaveLoadManager.Instance.monedasTotales[SaveLoadManager.Instance.saveFileID] = 0;
        SaveLoadManager.Instance.skinsTotales[SaveLoadManager.Instance.saveFileID] = 0;
        Array.Clear(collectableListArea, 0, collectableListArea.Length);
        Array.Clear(collectableListGlobal, 0, collectableListGlobal.Length);

        foreach (var controller in SaveLoadManager.Instance.GetCoinList()
                                                  .Select(gobj => gobj.GetComponent<CoinController>()).ToArray())
        {
            collectableListArea[(int)controller.coinLocation]++;
            collectableListGlobal[(int)controller.coinGlobal]++;
            SaveLoadManager.Instance.monedasTotales[SaveLoadManager.Instance.saveFileID]++;
        }

        foreach (var cd in SaveLoadManager.Instance.GetCdList().Select(gobj => gobj.GetComponent<CDController>()))
        {
            collectableListArea[(int)cd.coinLocation]++;
            collectableListGlobal[(int)cd.coinGlobal]++;
            SaveLoadManager.Instance.CDsTotales[SaveLoadManager.Instance.saveFileID]++;
        }

        var skins = SaveLoadManager.Instance.GetSkinList();
        for (var i = 0; i < 5; i++)
        {
            if (!skins[i + 1]) continue;
            var (_, ids) = BnuyClient.SkinData[i];
            collectableListArea[ids[0]]++;
            collectableListGlobal[ids[1]]++;
            SaveLoadManager.Instance.skinsTotales[SaveLoadManager.Instance.saveFileID]++;
        }
    }

    private static int Refresh(List<GameObject> list, CoinController[] controllers)
    {
        var group = controllers.GroupBy(gobj => gobj.GetUniqueId()).ToArray();
        var hashList = group.Select(g => g.First().gameObject).ToArray();
        list.Clear();
        list.AddRange(hashList);
        return list.Count;
    }
    
    private static int Refresh(List<GameObject> list, CDController[] controllers)
    {
        var group = controllers.GroupBy(gobj => gobj.GetUniqueId()).ToArray();
        var hashList = group.Select(g => g.First().gameObject).ToArray();
        list.Clear();
        list.AddRange(hashList);
        return list.Count;
    }

    [HarmonyPatch(typeof(PauseMenu), "Update"), HarmonyPostfix]
    private static void TrackCSs(PauseMenu __instance)
    {
        __instance.cDUIText.text =  __instance.cDUIText2.text = $"{BnuyClient.Cds}/{BnuyClient.CdsToGoal}";
    }
}