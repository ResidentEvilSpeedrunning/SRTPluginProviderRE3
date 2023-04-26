using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x80)]

    public struct GameClock
    {
        [FieldOffset(0x50)] public byte MeasureGameElapsedTime;
        [FieldOffset(0x51)] public byte MeasureDemoSpendingTime;
        [FieldOffset(0x52)] public byte MeasureInventorySpendingTime;
        [FieldOffset(0x53)] public byte MeasurePauseSpendingTime;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x38)]

    public struct GameClockSaveData
    {
        [FieldOffset(0x18)] private long _GameElapsedTime;
        [FieldOffset(0x20)] private long _DemoSpendingTime;
        [FieldOffset(0x28)] private long _InventorySpendingTime;
        [FieldOffset(0x30)] private long _PauseSpendingTime;

        public long GameElapsedTime => _GameElapsedTime;
        public long DemoSpendingTime => _DemoSpendingTime;
        public long InventorySpendingTime => _InventorySpendingTime;
        public long PauseSpendingTime => _PauseSpendingTime;
    }
}