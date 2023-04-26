using ProcessMemory;
using static ProcessMemory.Extensions;
using SRTPluginProviderRE3.Structs;
using System;
using System.Diagnostics;
using SRTPluginProviderRE3.Structs.GameStructs;

namespace SRTPluginProviderRE3
{
    internal class GameMemoryRE3Scanner : IDisposable
    {
        private readonly int MAX_ENTITES = 32;
        private readonly int MAX_ITEMS = 20;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE3 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Addresses
        private int pAddressGameClock;
        private int pAddressGameRankSystem;
        private int pAddressPlayerManager;
        private int pAddressInventoryManager;
        private int pAddressEnemyManager;
        private int pAddressMainFlowManager;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameClock;
        private MultilevelPointer PointerGameClockGameSaveData;
        private MultilevelPointer PointerGameRankSystem;
        private MultilevelPointer PointerPlayerManager;
        private MultilevelPointer PointerCharacter;
        private MultilevelPointer PointerInventoryCount;
        private MultilevelPointer[] PointerInventoryManager;
        private MultilevelPointer[] PointerInventorySlots;
        private MultilevelPointer[] PointerEnemyManager;
        private MultilevelPointer PointerMainFlowManager;
        private MultilevelPointer PointerMainFlowManagerGameHeaderSaveData;

        private InventoryEntry EmptySlot = new InventoryEntry();

        internal GameMemoryRE3Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE3();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            GameVersion? gv = SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));
            if (gv == null)
                return; // Unknown version.

            int mainFlowManagerOffset = (gv == GameVersion.RE3_WW_20230425_1) ? 0x1A0 : 0x198;

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, ProcessMemory.PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                PointerGameClock = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressGameClock));
                PointerGameClockGameSaveData = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressGameClock), 0x60);
                PointerGameRankSystem = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressGameRankSystem));
                PointerCharacter = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressPlayerManager), 0x78);
                PointerPlayerManager = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressPlayerManager), 0x50, 0x10, 0x20, 0x2C0);
                PointerMainFlowManager = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressMainFlowManager));
                PointerMainFlowManagerGameHeaderSaveData = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressMainFlowManager), mainFlowManagerOffset); //0x198 on old version...

                PointerInventoryCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressInventoryManager), 0x50, 0x98);
                PointerInventoryManager = new MultilevelPointer[MAX_ITEMS];
                PointerInventorySlots = new MultilevelPointer[MAX_ITEMS];
                gameMemoryValues._playerInventory = new InventoryEntry[MAX_ITEMS];
                for (int i = 0; i < MAX_ITEMS; ++i)
                {
                    PointerInventoryManager[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressInventoryManager), 0x50, 0x98, 0x10, 0x20 + (i * 0x08), 0x18, 0x10);
                    PointerInventorySlots[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressInventoryManager), 0x50, 0x98, 0x10, 0x20 + (i * 0x08), 0x18);
                    gameMemoryValues.PlayerInventory[i] = EmptySlot;
                }

                gameMemoryValues._enemyHealth = new EnemyHP[MAX_ENTITES];
                for (int i = 0; i < MAX_ENTITES; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();

                GenerateEnemyEntries();
            }
        }

        private GameVersion? SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE3_WW_20230425_1:
                    {
                        pAddressMainFlowManager = 0x09A70808;
                        pAddressGameClock = 0x09A650A8;
                        pAddressGameRankSystem = 0x09A6E608;
                        pAddressPlayerManager = 0x09A772E8;
                        pAddressInventoryManager = 0x09A68190;
                        pAddressEnemyManager = 0x09A750C8;
                        return GameVersion.RE3_WW_20230425_1;
                    }

                case GameVersion.RE3_WW_20211217_1:
                    {
                        pAddressMainFlowManager = 0x08DAFBC0;
                        pAddressGameClock = 0x08DB3BA0;
                        pAddressGameRankSystem = 0x08D816A8;
                        pAddressPlayerManager = 0x08D89DF0;
                        pAddressInventoryManager = 0x08D85BA0;
                        pAddressEnemyManager = 0x08D888A8;
                        return GameVersion.RE3_WW_20211217_1;
                    }

                case GameVersion.RE3_WW_20200930_1:
                    {
                        pAddressMainFlowManager = 0x08DACBC0;
                        pAddressGameClock = 0x08DB0BA0;
                        pAddressGameRankSystem = 0x08D7E6A8;
                        pAddressPlayerManager = 0x08D82BA0;
                        pAddressInventoryManager = 0x08D82BA0;
                        pAddressEnemyManager = 0x08D838B0;
                        return GameVersion.RE3_WW_20200930_1;
                    }

                case GameVersion.BIO3_CEROZ_20200930_1:
                    {
                        pAddressMainFlowManager = 0x08CEAC70;
                        pAddressGameClock = 0x08CEEC58;
                        pAddressGameRankSystem = 0x08CBC6C0;
                        pAddressPlayerManager = 0x08CC0C00;
                        pAddressInventoryManager = 0x08CC0C00; // incorrect
                        pAddressEnemyManager = 0x08CC1898;
                        return GameVersion.BIO3_CEROZ_20200930_1;
                    }

                case GameVersion.RE3_WW_20200806_1:
                    {
                        pAddressMainFlowManager = 0x08DB3BB0;
                        pAddressGameClock = 0x08DB7B90;
                        pAddressGameRankSystem = 0x08D85680;
                        pAddressPlayerManager = 0x08D89B90;
                        pAddressInventoryManager = 0x08D89B90;
                        pAddressEnemyManager = 0x08D8A8A0;
                        return GameVersion.RE3_WW_20200806_1;
                    }

                case GameVersion.BIO3_CEROZ_20200806_1:
                    {
                        pAddressMainFlowManager = 0x08DB3BB0;
                        pAddressGameClock = 0x08DB7B90;
                        pAddressGameRankSystem = 0x08D85680;
                        pAddressPlayerManager = 0x08D89B90;
                        pAddressInventoryManager = 0x08D89B90;
                        pAddressEnemyManager = 0x08D8A8A0;
                        return GameVersion.BIO3_CEROZ_20200806_1;
                    }

                case GameVersion.RE3_WW_20200603_1:
                    {
                        pAddressMainFlowManager = 0x08CE4720;
                        pAddressGameClock = 0x08CE8430;
                        pAddressGameRankSystem = 0x08CB62A8;
                        pAddressPlayerManager = 0x08CBA618;
                        pAddressInventoryManager = 0x08CBA618;
                        pAddressEnemyManager = 0x08CB8618;
                        return GameVersion.RE3_WW_20200603_1;
                    }

                case GameVersion.BIO3_CEROZ_20200603_1:
                    {
                        pAddressMainFlowManager = 0x08DA66F0;
                        pAddressGameClock = 0x08DAA3F0;
                        pAddressGameRankSystem = 0x08D78258;
                        pAddressPlayerManager = 0x08D7C5E8;
                        pAddressInventoryManager = 0x08D7C5E8;
                        pAddressEnemyManager = 0x08D7A5A8;
                        return GameVersion.BIO3_CEROZ_20200603_1;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return null;
        }

        private unsafe void GenerateEnemyEntries()
        {
            if (PointerEnemyManager == null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerEnemyManager = new MultilevelPointer[MAX_ENTITES]; // Create a new enemy pointer table array with the detected size.
                for (int i = 0; i < MAX_ENTITES; ++i) // Loop through and create all of the pointers for the table.
                    PointerEnemyManager[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAddressEnemyManager), 0x78, 0x10, 0x20 + (i * 0x08), 0x300);
            }
        }

        internal void UpdatePointers()
        {
            PointerGameClock.UpdatePointers();
            PointerGameClockGameSaveData.UpdatePointers();
            PointerGameRankSystem.UpdatePointers();
            PointerPlayerManager.UpdatePointers();
            PointerCharacter.UpdatePointers();
            PointerMainFlowManager.UpdatePointers();
            PointerMainFlowManagerGameHeaderSaveData.UpdatePointers();

            PointerInventoryCount.UpdatePointers();
            for (int i = 0; i < MAX_ITEMS; ++i)
            {
                PointerInventoryManager[i].UpdatePointers();
                PointerInventorySlots[i].UpdatePointers();
            }

            GenerateEnemyEntries(); // This has to be here for the next part.
            for (int i = 0; i < MAX_ENTITES; ++i)
                PointerEnemyManager[i].UpdatePointers();
        }

        internal unsafe IGameMemoryRE3 Refresh()
        {
            gameMemoryValues._timer = PointerGameClockGameSaveData.Deref<GameClockSaveData>(0x0);
            gameMemoryValues._gameClock = PointerGameClock.Deref<GameClock>(0x0);
            gameMemoryValues._rankManager = PointerGameRankSystem.Deref<GameRankSystem>(0x0);
            gameMemoryValues._player = PointerPlayerManager.Deref<HitPointController>(0x0);
            gameMemoryValues._playerCharacter = PointerCharacter.DerefInt(0x18);
            gameMemoryValues._playerDeathCount = PointerMainFlowManager.DerefInt(0xC0);
            gameMemoryValues._difficulty = PointerMainFlowManagerGameHeaderSaveData.DerefInt(0x20);
            gameMemoryValues._saves = PointerMainFlowManagerGameHeaderSaveData.DerefInt(0x24);

            // Inventory
            gameMemoryValues._playerInventoryCount = PointerInventoryCount.DerefInt(0x18);
            for (int i = 0; i < MAX_ITEMS; ++i)
            {
                var entry = PointerInventoryManager[i].Deref<InventorySlot>(0x0);
                gameMemoryValues.PlayerInventory[i].SlotPosition = PointerInventorySlots[i].DerefInt(0x28);
                gameMemoryValues.PlayerInventory[i].ItemID = entry.ItemID;
                gameMemoryValues.PlayerInventory[i].WeaponID = entry.WeaponID;
                gameMemoryValues.PlayerInventory[i].Attachments = entry.Attachments;
                gameMemoryValues.PlayerInventory[i].BulletID = entry.BulletID;
                gameMemoryValues.PlayerInventory[i].Quantity = entry.Quantity;
            }

            //Enemies
            GenerateEnemyEntries();
            for (int i = 0; i < MAX_ENTITES; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyManager[i].Address != IntPtr.Zero)
                    {
                        gameMemoryValues.EnemyHealth[i]._maximumHP = PointerEnemyManager[i].DerefInt(0x54);
                        gameMemoryValues.EnemyHealth[i]._currentHP = PointerEnemyManager[i].DerefInt(0x58);
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                    }
                }
                catch
                {
                    gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                    gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                }
            }

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
