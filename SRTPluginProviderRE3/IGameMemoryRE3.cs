using SRTPluginProviderRE3.Structs;
using SRTPluginProviderRE3.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE3
{
    public interface IGameMemoryRE3
    {
        string GameName { get; }
        string VersionInfo { get; }
        HitPointController Player { get; set; }
        string PlayerName { get; }
        int PlayerDeathCount { get; set; }
        int PlayerInventoryCount { get; set; }
        InventoryEntry[] PlayerInventory { get; set; }
        EnemyHP[] EnemyHealth { get; set; }
        GameClockSaveData Timer { get; set; }
        GameRankSystem RankManager { get; set; }
        int Saves { get; set; }
        bool IsRunning { get; }
        bool IsCutscene { get; }
        bool IsMenu { get; }
        bool IsPaused { get; }
        long IGTCalculated { get; }
        long IGTCalculatedTicks { get; }
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
        string DifficultyName { get; }
        string ScoreName { get; }
    }
}
