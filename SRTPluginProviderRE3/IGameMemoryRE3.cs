using SRTPluginProviderRE3.Structs;
using SRTPluginProviderRE3.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE3
{
    public interface IGameMemoryRE3
    {
        string GameName { get; }

        string VersionInfo { get; }

        GameTimer Timer { get; }

        RankManager RankManager { get; }

        Player PlayerManager { get; }

        int InventoryCount { get; }

        InventoryEntry[] Items { get; }

        int ShortcutCount { get; }

        InventoryEntry[] Shortcuts { get; }

        int SubShortcutCount { get; }

        InventoryEntry[] SubShortcuts { get; }

        PrimitiveItem MainSlot { get; }

        PrimitiveItem SubSlot { get; }

        int EnemyCount { get; }

        Enemy[] Enemies { get; }

        int EnemyKillCount { get; }

        int LocationID { get; }

        string LocationName { get; }

        int MapID { get; }

        string MapName { get; }
    }
}
