using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    public struct Enemy
    {
        private int enemyId;
        private int currentHP;
        private int maxHP;

        public KindID EnemyID { get => (KindID)enemyId; set => enemyId = (int)value; }
        public int CurrentHP { get => currentHP; set => currentHP = value; }
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP > 0;

        public void SetValues(int id, HitPointController? hpc)
        {
            EnemyID = (KindID)id;
            CurrentHP = hpc?.CurrentHP ?? 0;
            MaxHP = hpc?.MaxHP ?? 0;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x1F8)]
    public struct EnemyManager
    {
        [FieldOffset(0x78)] private nint activeEnemyList;
        [FieldOffset(0x190)] private int totalEnemyKillCount;
        public IntPtr ActiveEnemyList => IntPtr.Add(activeEnemyList, 0x0);
        public int TotalEnemyKillCount => totalEnemyKillCount;
    }

    public enum KindID : int
    {
        em0000 = 0,
        em0020 = 1,
        em0100 = 2,
        em0200 = 3,
        em0300 = 4,
        em0400 = 5,
        em0500 = 6,
        em0600 = 7,
        em0700 = 8,
        em0800 = 9,
        em1000 = 10,
        em2500 = 11,
        em2600 = 12,
        em2700 = 13,
        em3000 = 14,
        em3300 = 15,
        em3400 = 16,
        em3500 = 17,
        em4000 = 18,
        em7000 = 19,
        em7100 = 20,
        em7200 = 21,
        em8400 = 22,
        em9000 = 23,
        em9010 = 24,
        em9020 = 25,
        em9030 = 26,
        em9040 = 27,
        em9050 = 28,
        em9091 = 29,
        em9100 = 30,
        em9200 = 31,
        em9201 = 32,
        em9210 = 33,
        em9300 = 34,
        em9400 = 35,
        em9401 = 36,
        em9410 = 37,
        em9999 = 38,
        MAX = 39,
        Invalid = -1,
    };

}