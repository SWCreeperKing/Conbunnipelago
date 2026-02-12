using Conbunnipelago;
using Conbunnipelago.Archipelago;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(Core), "Conbunnipelago", Core.VersionNumber, "SW_CreeperKing", null)]
[assembly: MelonGame("Keiby", "ConbunnCardboard")]

namespace Conbunnipelago;

public class Core : MelonMod
{
    public const string VersionNumber = "0.1.0";
    public static MelonLogger.Instance Log;
    public static List<GameObject> Trampolines = [];
    public const string DataFolder = "Mods/SW_CreeperKing.Conbunnipelago/Data";

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        var classesToPatch = MelonAssembly.Assembly.GetTypes()
                                          .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                          .ToArray();

        Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch)
        {
            HarmonyInstance.PatchAll(patch);

            Log.Msg($"Loaded: [{patch.Name}]");
        }

        Log.Msg("Loading Data");

        BnuyClient.IdToLocation = File.ReadAllLines($"{DataFolder}/LocationIds.txt").Select(s => s.Split(':'))
                                      .ToDictionary(arr => arr[0], arr => arr[1]);

        BnuyClient.IdToTransition = File.ReadAllLines($"{DataFolder}/TransitionIds.txt").Select(s => s.Split(':'))
                                        .ToDictionary(arr => arr[0], arr => $"Transition Unlock: {arr[1]}");

        BnuyClient.LocationDoors = File.ReadAllLines($"{DataFolder}/LocationDoors.txt").Select(s => s.Split(','))
                                       .ToDictionary(arr => arr[0], arr => arr[1]);

        BnuyClient.SkinData = File.ReadAllLines($"{DataFolder}/SkinData.txt").Select(s =>
            {
                var arr = s.Split(',');
                return (arr[0], (int[])[int.Parse(arr[1]), int.Parse(arr[2])]);
            }
        ).ToArray();

        LoggerInstance.Msg("Setting up Client");

        BnuyClient.Init();

        LoggerInstance.Msg("Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Log.Msg($"Scene was loaded: [{sceneName}]");
        Trampolines.Clear();

        switch (sceneName)
        {
            case "ConbunnTitle":
                var menu = new GameObject();
                var gui = menu.AddComponent<APGui>();
                break;
            case "ConbunnGame":
                GameObject.Find(
                    "Subzonas/PX Subzona/Subzona Concierto Cardbun/ConciertoCardbun Terreno/ConciertoCardbun Terreno 6 (Puerta)"
                ).AddComponent<Hiderinator>().Condition = () => false;

                GameObject.Find(
                    "Subzonas/PX Subzona/Subzona Concierto Cardbun/ConciertoCardbun Prompts/DetallesGrandes/MesaDJ"
                ).AddComponent<Hiderinator>().Condition = () => BnuyClient.Cds >= BnuyClient.CdsToGoal;

                foreach (var doorData in BnuyClient.LocationDoors)
                {
                    GameObject.Find(doorData.Value).AddComponent<Hiderinator>().Condition
                        = () => BnuyClient.Items.Contains($"Transition Unlock: {doorData.Key}");
                }
                break;
        }

        Trampolines.AddRange(
            Object.FindObjectsOfType<TrampolinTrigger>().Select(trigger => trigger.transform.parent.gameObject)
        );

        foreach (var trampoline in Trampolines) { trampoline.SetActive(BnuyClient.Items.Contains("Bounce Pads")); }
    }

    public override void OnUpdate() => BnuyClient.Update();
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;