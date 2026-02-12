using Conbunnipelago.Patches;
using UnityEngine;
using static Conbunnipelago.Archipelago.BnuyClient;

namespace Conbunnipelago.Archipelago;

// stolen from: https://github.com/FyreDay/TCG-CardShop-Sim-APClient/blob/master/APGui.cs
public class APGui : MonoBehaviour
{
    public static bool ShowGUI = true;
    public static string State = "";
    public static Vector2 Offset = new(100, 100);

    public static GUIStyle TextStyle = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.white,
        },
    };

    public static GUIStyle TextStyleGreen = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.green,
        },
    };

    public static GUIStyle TextStyleRed = new()
    {
        fontSize = 12,
        normal =
        {
            textColor = Color.red,
        },
    };

    public static MainMenuManager Manager = null;

    private void Awake()
    {
        Manager ??= FindObjectOfType<MainMenuManager>();
    }

    void OnGUI()
    {
        if (!ShowGUI) return;

        if (!Client.IsConnected)
        {
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y, 200, 300), "AP Client");
        
            GUI.Label(new Rect(20 + Offset.x, 40 + Offset.y, 300, 30), "Address:port", TextStyle);
            Data.AddressPort = GUI.TextField(new Rect(20 + Offset.x, 60 + Offset.y, 180, 25), Data.AddressPort, 25);
        
            GUI.Label(new Rect(20 + Offset.x, 90 + Offset.y, 300, 30), "Password", TextStyle);
            Data.Password = GUI.TextField(new Rect(20 + Offset.x, 110 + Offset.y, 180, 25), Data.Password, 25);
        
            GUI.Label(new Rect(20 + Offset.x, 140 + Offset.y, 300, 30), "Slot", TextStyle);
            Data.SlotName = GUI.TextField(new Rect(20 + Offset.x, 160 + Offset.y, 180, 25), Data.SlotName, 25);
        }
        else
        {
            GUI.Box(new Rect(10 + Offset.x, 10 + Offset.y + 100, 200, 150), "AP Client");
        }
        
        if (!Client.IsConnected && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Connect"))
        {
            var error = TryConnect(Data.AddressPort, Data.Password, Data.SlotName);
            
            if (error is not null)
            {
                State = string.Join("\n", error);
                return;
            }
        
            State = "";
            SaveFile();
        }
        
        if (Client.IsConnected && GUI.Button(new Rect(20 + Offset.x, 210 + Offset.y, 180, 30), "Disconnect"))
        {
            Client.TryDisconnect();
        }
        
        GUI.Label(new Rect(20 + Offset.x, 240 + Offset.y, 300, 30),
            State != "" ? State : Client.IsConnected ? "Connected" : "Not Connected",
            Client.IsConnected ? TextStyleGreen : TextStyleRed);
    }
}