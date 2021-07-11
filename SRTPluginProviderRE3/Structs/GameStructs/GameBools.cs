using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]

    public struct GameBools
    {
        [FieldOffset(0x0)] public byte isRunning;
        [FieldOffset(0x1)] public byte isCutscene;
        [FieldOffset(0x2)] public byte isMenu;
        [FieldOffset(0x3)] public byte isPaused;
    }
}