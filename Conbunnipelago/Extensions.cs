using UnityEngine;
using UnityEngine.SceneManagement;

namespace Conbunnipelago;

public static class Extensions
{
    public static string GetUniqueId(this CoinController controller)
        => $"Coin_{SceneManager.GetActiveScene().name},{controller.gameObject.GetFullPath()}";

    public static string GetUniqueId(this CDController controller)
        => $"Cd_{SceneManager.GetActiveScene().name},{controller.gameObject.GetFullPath()}";

    public static List<GameObject> GetCoinList(this SaveLoadManager manager) => manager.saveFileID switch
    {
        0 => manager.coinsCollected0, 1 => manager.coinsCollected1, 2 => manager.coinsCollected2
    };

    public static int[] GetCoinListArea(this SaveLoadManager manager) => manager.saveFileID switch
    {
        0 => manager.collectablesAreas0, 1 => manager.collectablesAreas1, 2 => manager.collectablesAreas2
    };

    public static int[] GetCoinListGlobal(this SaveLoadManager manager) => manager.saveFileID switch
    {
        0 => manager.collectablesGlobal0, 1 => manager.collectablesGlobal1, 2 => manager.collectablesGlobal2
    };

    public static List<GameObject> GetCdList(this SaveLoadManager manager) => manager.saveFileID switch
    {
        0 => manager.cDsCollected0, 1 => manager.cDsCollected1, 2 => manager.cDsCollected2
    };
    
    public static bool[] GetSkinList(this SaveLoadManager manager) => manager.saveFileID switch
    {
        0 => manager.skinUnlocked0, 1 => manager.skinUnlocked1, 2 => manager.skinUnlocked2
    };

}