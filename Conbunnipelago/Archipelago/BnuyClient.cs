using Archipelago.MultiClient.Net.Enums;
using Conbunnipelago.Patches;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Werepelago.Archipelago;

namespace Conbunnipelago.Archipelago;

public static class BnuyClient
{
    public static Dictionary<string, string> IdToLocation;
    public static Dictionary<string, string> IdToTransition;
    public static Dictionary<string, string> LocationDoors;
    public static (string, int[])[] SkinData;
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static long CdsToGoal = 0;
    public static string GameUUID = "";
    public static List<string> Items = [];
    
    public static long Cds => Items.Count(item => item is "CD");
    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () =>
        {
            // if (Core.Scene is "Game") GameUI.Instance.IngameMenuReturnToTitle();
            Core.Log.Error("Lost Connection to Ap");
            Items.Clear();
        };

        Client.OnConnectionEvent += _ =>
        {
            try
            {
                Items.Clear();
                CdsToGoal = (long)Client.SlotData["cds_required_to_goal"];
                GameUUID = (string)Client.SlotData["uuid"];
                
                SaveAndLoadPatch.Path = $"{GameUUID}.es3";
                APGui.Manager.CallPrivateMethod("Start");
            }
            catch (Exception e) { Core.Log.Error(e); }
        };

        Client.OnConnectionErrorReceived += (e, _) => Core.Log.Error(e);
        Client.OnErrorReceived += e => Core.Log.Error(e);
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "Conbunn Cardboard", ItemsHandlingFlags.AllItems);
    }

    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));

    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;

            var items = Client.GetOutstandingItems();
            if (items is null || items.Length == 0) return;
            Items.AddRange(items.Select(item => item.ItemName).Where(item => item is not "Cardboard Coin"));
            
            if (Core.Trampolines is null || Core.Trampolines.Count == 0) return;
            if (!items.Any(item => item.ItemName is "Bounce Pads")) return;
            
            foreach (var trampoline in Core.Trampolines)
            {
                trampoline.SetActive(true);
            }
        }
        catch (Exception e) { Core.Log.Error(e); }
    }
}