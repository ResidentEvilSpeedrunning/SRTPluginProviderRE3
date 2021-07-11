using SRTPluginProviderRE3.Structs;
using SRTPluginProviderRE3.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE3
{
    public interface IGameMemoryRE3
    {
        string GameName { get; }
        string VersionInfo { get; }
        CharacterEnumeration PlayerCharacter { get; set; }
        GamePlayer Player { get; set; }
        string PlayerName { get; }
        int PlayerDeathCount { get; set; }
        int PlayerInventoryCount { get; set; }
        GameInventoryEntry[] PlayerInventory { get; set; }
        EnemyHP[] EnemyHealth { get; set; }
        GameTimer Timer { get; set; }
        int Difficulty { get; set; }
        GameRankManager RankManager { get; set; }
        int Saves { get; set; }
        int MapID { get; set; }
        float FrameDelta { get; set;  }
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
