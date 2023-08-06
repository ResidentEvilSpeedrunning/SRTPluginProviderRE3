using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderRE3.Structs.GameStructs;

namespace SRTPluginProviderRE3
{
    internal class GameMemoryRE3Scanner : IDisposable
    {
        private readonly int MAX_ENTITES = 32;
        private readonly int MAX_ITEMS = 20;
        private readonly int MAX_SHORTCUTS = 4;
        private readonly int mSize = 0x18;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE3 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public uint ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Addresses
        private int paGameClock;
        private int paGameRankSystem;
        private int paPlayerManager;
        private int paInventoryManager;
        private int paEnemyManager;
        private int paLocationId;
        private int paMapId;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameClock { get; set; }
        private MultilevelPointer PointerGameRankSystem { get; set; }
        private MultilevelPointer PointerPlayerCondition { get; set; }
        private MultilevelPointer PointerInventoryManager { get; set; }
        private MultilevelPointer PointerShortcutManager { get; set; }
        private MultilevelPointer PointerSubShortcutManager { get; set; }
        private MultilevelPointer PointerMainSlot { get; set; }
        private MultilevelPointer PointerSubSlot { get; set; }
        private MultilevelPointer PointerEnemyManager { get; set; }
        private MultilevelPointer PointerLocationId { get; set; }
        private MultilevelPointer PointerMapId { get; set; }

        private DifficultyParamClass[] dpc;

        internal GameMemoryRE3Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE3();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void InitGMV()
        {
            dpc = new DifficultyParamClass[5];
            gameMemoryValues._timer = new GameTimer();
            gameMemoryValues._rankManager = new RankManager();
            gameMemoryValues._playerManager = new Player();
            gameMemoryValues._items = new InventoryEntry[MAX_ITEMS];
            gameMemoryValues._shortcuts = new InventoryEntry[MAX_SHORTCUTS];
            gameMemoryValues._subShortcuts = new InventoryEntry[MAX_SHORTCUTS];
            gameMemoryValues._enemies = new Enemy[MAX_ENTITES];
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            GameVersion gv = SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));
            if (gv == GameVersion.Unknown)
                return; // Unknown version.

            // int mainFlowManagerOffset = (gv == GameVersion.RE3_WW_20230425_1) ? 0x1A0 : 0x198;

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler((uint)pid);
            if (ProcessRunning)
            {
                BaseAddress = process?.MainModule?.BaseAddress ?? IntPtr.Zero; // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
                InitGMV();
                PointerGameClock = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paGameClock));
                PointerGameRankSystem = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paGameRankSystem));
                PointerPlayerCondition = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paPlayerManager), 0x50, 0x10, 0x20);
                PointerInventoryManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x58);
                PointerShortcutManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x50, 0xB8);
                PointerSubShortcutManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x50, 0xC0);
                PointerMainSlot = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x50, 0xA0, 0x18, 0x10);
                PointerSubSlot = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paInventoryManager), 0x50, 0xA8, 0x18, 0x10);
                PointerEnemyManager = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paEnemyManager));
                PointerLocationId = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paLocationId));
                PointerMapId = new MultilevelPointer(memoryAccess, (nint*)(BaseAddress + paMapId));
            }
        }

        private GameVersion SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE3_WW_11026988:
                    {
                        // pAddressMainFlowManager = 0x09A70808;
                        paEnemyManager = 0x09A750C8;
                        paGameClock = 0x09A650A8;
                        paGameRankSystem = 0x09A6E608;
                        paInventoryManager = 0x09A68190;
                        paPlayerManager = 0x09A772E8;
                        paLocationId = 0x09A76450;
                        paMapId = 0x09A76458;
                        return GameVersion.RE3_WW_11026988;
                    }
                case GameVersion.RE3_WW_11047294:
                    {
                        paEnemyManager = 0x08C72D60;
                        paGameClock = 0x08C63308;
                        paGameRankSystem = 0x08C6B358;
                        paInventoryManager = 0x08C6F648;
                        paPlayerManager = 0x08C73F98;
                        paLocationId = 0x08C74300;
                        paMapId = 0x08C74308;
                        return GameVersion.RE3_WW_11047294;
                    }
                case GameVersion.RE3_CEROD_11026646:
                    {
                        paEnemyManager = 0x0;
                        paGameClock = 0x0;
                        paGameRankSystem = 0x0;
                        paInventoryManager = 0x0;
                        paPlayerManager = 0x0;
                        paLocationId = 0x0;
                        paMapId = 0x0;
                        return GameVersion.RE3_CEROD_11026646;
                    }
                case GameVersion.RE3_CEROD_11047603:
                    {
                        paEnemyManager = 0x0;
                        paGameClock = 0x0;
                        paGameRankSystem = 0x0;
                        paInventoryManager = 0x0;
                        paPlayerManager = 0x0;
                        paLocationId = 0x0;
                        paMapId = 0x0;
                        return GameVersion.RE3_CEROD_11047603;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return GameVersion.Unknown;
        }

        internal void UpdatePointers()
        {
            PointerGameClock.UpdatePointers();
            PointerGameRankSystem.UpdatePointers();
            PointerPlayerCondition.UpdatePointers();
            PointerInventoryManager.UpdatePointers();
            PointerShortcutManager.UpdatePointers();
            PointerSubShortcutManager.UpdatePointers();
            PointerMainSlot.UpdatePointers();
            PointerSubSlot.UpdatePointers();
            PointerEnemyManager.UpdatePointers();
            PointerLocationId.UpdatePointers();
            PointerMapId.UpdatePointers();
        }

        private unsafe void UpdateGameClock()
        {
            // GameClock
            var gc = PointerGameClock.Deref<GameClock>(0x0);
            var gsd = memoryAccess.GetAt<GameClockGameSaveData>((nuint*)gc.GameSaveData);
            gameMemoryValues._timer.SetValues(gc, gsd);
        }

        private unsafe void UpdateGameRankSystem()
        {
            // GameRankSystem
            var grs = PointerGameRankSystem.Deref<GameRankSystem>(0x0);
            var grpd = memoryAccess.GetAt<GameRankParameterData>((nuint*)grs.GameRankParameter);
            dpc[0] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamEasy);
            dpc[1] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamNormal);
            dpc[2] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamHard);
            dpc[3] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamVeryHard);
            dpc[4] = memoryAccess.GetAt<DifficultyParamClass>((nuint*)grpd.DifficultyParamImpossible);
            gameMemoryValues._rankManager.SetValues(grs, dpc);
        }

        private unsafe void UpdatePlayerManager()
        {
            // PlayerManager
            var pc = PointerPlayerCondition.Deref<PlayerCondition>(0x0);
            var cc = memoryAccess.GetAt<CostumeChanger>((nuint*)pc.CostumeChanger);
            var hpc = memoryAccess.GetAt<HitPointController>((nuint*)pc.HitPointController);
            gameMemoryValues._playerManager.SetValues(pc, cc, hpc);
        }

        private unsafe void UpdateInventoryManager()
        {
            // InventoryManager
            var im = PointerInventoryManager.Deref<InventoryManager>(0x0);
            var invArray = memoryAccess.GetAt<ListInventory>((nuint*)im.Inventory);
            var inv = memoryAccess.GetAt<Inventory>((nuint*)invArray._ListInventory);
            var slots = memoryAccess.GetAt<Slots>((nuint*)inv.ListSlots);
            gameMemoryValues._inventoryCount = inv.CurrentSlotSize;
            gameMemoryValues._inventoryMaxCount = slots.Count;

            for (int i = 0; i < MAX_ITEMS; i++)
            {
                var position = (i * 0x8) + 0x20;
                var slotAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slots._Slots, position));
                var slot = memoryAccess.GetAt<Slot>(slotAddress);
                var itemAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slot._Slot, 0x10));
                var slotId = memoryAccess.GetIntAt((nuint*)IntPtr.Add(slot._Slot, 0x28));
                var item = memoryAccess.GetAt<PrimitiveItem>(itemAddress);
                gameMemoryValues._items[i].SetValues(slotId, item, slot);
            }
        }

        private unsafe void UpdateShortcutManager()
        {
            // InventoryManager
            var sm = PointerShortcutManager.Deref<ShortcutManager>(0x0);
            var ssm = PointerSubShortcutManager.Deref<ShortcutManager>(0x0);

            for (int i = 0; i < MAX_SHORTCUTS; i++)
            {
                // Main
                var position = (i * 0x18) + 0x30;
                var slotAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)sm.Entries, position));
                var slot = memoryAccess.GetAt<Slot>(slotAddress);
                var itemAddress = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slot._Slot, 0x10));
                var slotId = memoryAccess.GetIntAt((nuint*)IntPtr.Add(slot._Slot, 0x28));
                var item = memoryAccess.GetAt<PrimitiveItem>(itemAddress);
                gameMemoryValues._shortcuts[i].SetValues(slotId, item, slot);

                // Sub
                var slotAddress2 = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ssm.Entries, position));
                var slot2 = memoryAccess.GetAt<Slot>(slotAddress2);
                var itemAddress2 = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add(slot2._Slot, 0x10));
                var slotId2 = memoryAccess.GetIntAt((nuint*)IntPtr.Add(slot2._Slot, 0x28));
                var item2 = memoryAccess.GetAt<PrimitiveItem>(itemAddress2);
                gameMemoryValues._subShortcuts[i].SetValues(slotId2, item2, slot2);
            }
        }

        private unsafe void UpdateEnemyManager()
        {
            // EnemyManager
            var em = PointerEnemyManager.Deref<EnemyManager>(0x0);
            gameMemoryValues._enemyKillCount = em.TotalEnemyKillCount;
            var ael = memoryAccess.GetLongAt((nuint*)IntPtr.Add(em.ActiveEnemyList, 0x10));
            gameMemoryValues._enemyCount = memoryAccess.GetIntAt((nuint*)IntPtr.Add(em.ActiveEnemyList, mSize));

            for (int i = 0; i < MAX_ENTITES; i++)
            {
                if (i >= gameMemoryValues.EnemyCount)
                {
                    gameMemoryValues._enemies[i].SetValues(-1, null);
                    continue;
                }
                var position = (i * 0x8) + 0x20;
                var ec = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ael, position));
                var ecc = memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ec, 0x170));
                var id = memoryAccess.GetIntAt((nuint*)IntPtr.Add((IntPtr)ecc, 0x54));
                var hc = (long*)memoryAccess.GetLongAt((nuint*)IntPtr.Add((IntPtr)ec, 0x300));
                var enemyHP = memoryAccess.GetAt<HitPointController>(hc);
                gameMemoryValues._enemies[i].SetValues(id, enemyHP);
            }
        }

        private unsafe void UpdateLocation()
        {
            gameMemoryValues._locationID = memoryAccess.GetIntAt(PointerLocationId.BaseAddress);
            gameMemoryValues._locationName = ((LocationID)gameMemoryValues._locationID).ToString();

            gameMemoryValues._mapID = memoryAccess.GetIntAt(PointerMapId.BaseAddress);
            gameMemoryValues._mapName = ((MapID)gameMemoryValues._mapID).ToString();
        }

        private unsafe void UpdateEquippedItems()
        {
            gameMemoryValues._mainSlot = PointerMainSlot.Deref<PrimitiveItem>(0x0);
            gameMemoryValues._subSlot = PointerSubSlot.Deref<PrimitiveItem>(0x0);
        }

        internal unsafe IGameMemoryRE3 Refresh()
        {
            UpdateGameClock();
            UpdateGameRankSystem();
            UpdatePlayerManager();
            UpdateInventoryManager();
            UpdateShortcutManager();
            UpdateEquippedItems();
            UpdateEnemyManager();
            UpdateLocation();
            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
