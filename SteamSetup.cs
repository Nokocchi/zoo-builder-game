using Godot;
using System;
using GodotSteam;

public partial class SteamSetup : Node
{
    private const uint AppId = 480;

    public override void _EnterTree()
    {
        OS.SetEnvironment("SteamAppId", AppId.ToString());
        OS.SetEnvironment("SteamGameId", AppId.ToString());
    }

    public override void _Ready()
    {
        Steam.SteamInit();

        bool isSteamRunning = Steam.IsSteamRunning();
        if (!isSteamRunning)
        {
            GD.Print("Steam is not running.");
            return;
        }
        
        ulong steamId = Steam.GetSteamID();
        string name = Steam.GetFriendPersonaName(steamId);
    }
}