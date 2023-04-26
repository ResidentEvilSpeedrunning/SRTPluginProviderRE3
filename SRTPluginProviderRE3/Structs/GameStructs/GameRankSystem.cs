using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x88)]

    public struct GameRankSystem
    {
        [FieldOffset(0x58)] private int _gameRank;
        [FieldOffset(0x5C)] private float _rankPoint;

        public int Rank => _gameRank;
        public float RankScore => _rankPoint;
    }
}