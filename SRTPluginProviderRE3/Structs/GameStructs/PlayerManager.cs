using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    public class Player
    {
        private SurvivorType currentSurvivor;
        private Costume currentCostume;
        private HitPointController hitPointController;
        private bool isPoisoned;
        private bool hasParasite;
        private Vec3 position;

        public SurvivorType CurrentSurvivor { get => currentSurvivor; set => currentSurvivor = value; }
        public string CurrentSurvivorString => CurrentSurvivor.ToString();
        public Costume CurrentCostume { get => currentCostume; set => currentCostume = value; }
        public string CurrentCostumeString => CurrentCostume.ToString();
        public HitPointController Health { get => hitPointController; set => hitPointController = value; }
        public bool IsPoisoned { get => isPoisoned; set => isPoisoned = value; }
        public bool HasParasite { get => hasParasite; set => hasParasite = value; }
        public bool IsLoaded => (int)CurrentSurvivor <= (int)SurvivorType.PROTECTIVECLOTH;
        public Vec3 Position { get => position; set => position = value; }
        public PlayerState HealthState
        {
            get =>
                !Health.IsAlive ? PlayerState.Dead :
                IsPoisoned ? PlayerState.Poisoned :
                HasParasite ? PlayerState.Gassed :
                Health.Percentage >= 0.66f ? PlayerState.Fine :
                Health.Percentage >= 0.33f ? PlayerState.Caution :
                PlayerState.Danger;
        }
        public string CurrentHealthState => HealthState.ToString();

        public Player()
        {
            CurrentSurvivor = default(SurvivorType);
            CurrentCostume = default(Costume);
            Health = default(HitPointController);
            IsPoisoned = false;
            HasParasite = false;
            Position = new Vec3(0,0,0);
        }

        public void SetValues(PlayerCondition pc, CostumeChanger cc, HitPointController hpc)
        {
            CurrentSurvivor = pc.SurvivorType;
            CurrentCostume = cc.CurrentCostume;
            Health = hpc;
            IsPoisoned = pc.IsPoison;
            HasParasite = pc.IsParasite;
            Position.Update(pc.X, pc.Y, pc.Z);
        }
    }

    public class Vec3
    {
        private float x;
        private float y;
        private float z;
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Z { get => z; set => z = value; }

        public Vec3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Update(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x3E0)]
    public struct PlayerCondition
    {
        [FieldOffset(0x54)] private int survivorType;
        [FieldOffset(0x288)] private nint costumeChanger;
        [FieldOffset(0x2C0)] private nint hitPointController;
        [FieldOffset(0x2E8)] private byte isPoison;
        [FieldOffset(0x2E9)] private byte isParasite;
        [FieldOffset(0x3B0)] private Vector3 pastPosition;

        public SurvivorType SurvivorType => (SurvivorType)survivorType;
        public string SurvivorTypeString => SurvivorType.ToString();
        public IntPtr CostumeChanger => IntPtr.Add(costumeChanger, 0x0);
        public IntPtr HitPointController => IntPtr.Add(hitPointController, 0x0);
        public bool IsPoison => isPoison != 0;
        public bool IsParasite => isParasite != 0;
        public float X => pastPosition.X;
        public float Y => pastPosition.Y;
        public float Z => pastPosition.Z;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xD0)]
    public struct CostumeChanger
    {
        [FieldOffset(0x54)] private int currentCostume;

        public Costume CurrentCostume => (Costume)currentCostume;
        public string CurrentCostumeString => CurrentCostume.ToString();
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x64)]
    public struct HitPointController
    {
        [FieldOffset(0x54)] private int defaultHitPoint;
        [FieldOffset(0x58)] private int currentHitPoint;
        [FieldOffset(0x5C)] private byte invincible;
        [FieldOffset(0x62)] private byte noDamage;

        public int MaxHP => defaultHitPoint;
        public int CurrentHP => currentHitPoint;
        public bool Invincible => invincible != 0;
        public bool NoDamage => noDamage != 0;
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
    }

    public enum MapID
    {
        Invalid = 0,
        st00_0101_0 = 1,
        st00_0102_0 = 2,
        st00_0201_0 = 3,
        st00_0202_0 = 4,
        st00_0203_0 = 5,
        st00_0204_0 = 6,
        st00_0205_0 = 7,
        st00_0206_0 = 8,
        st00_0207_0 = 9,
        st00_0208_0 = 10,
        st00_0209_0 = 11,
        st01_0101_0 = 12,
        st01_0102_0 = 13,
        st01_0103_0 = 14,
        st01_0104_0 = 15,
        st01_0201_0 = 16,
        st01_0202_0 = 17,
        st01_0301_0 = 18,
        st01_0302_0 = 19,
        st01_0303_0 = 20,
        st01_0304_0 = 21,
        st01_0305_0 = 22,
        st01_0306_0 = 23,
        st01_0307_0 = 24,
        st01_0308_0 = 25,
        st01_0401_0 = 26,
        st01_0501_0 = 27,
        st01_0502_0 = 28,
        st01_0601_0 = 29,
        st02_0101_0 = 30,
        st02_0102_0 = 31,
        st02_0103_0 = 32,
        st02_0104_0 = 33,
        st02_0105_0 = 34,
        st02_0106_0 = 35,
        st02_0201_0 = 36,
        st02_0202_0 = 37,
        st02_0203_0 = 38,
        st02_0204_0 = 39,
        st02_0205_0 = 40,
        st02_0206_0 = 41,
        st02_0207_0 = 42,
        st02_0208_0 = 43,
        st02_0209_0 = 44,
        st02_0210_0 = 45,
        st02_0211_0 = 46,
        st02_0212_0 = 47,
        st02_0213_0 = 48,
        st02_0214_0 = 49,
        st02_0215_0 = 50,
        st02_0216_0 = 51,
        st02_0217_0 = 52,
        st02_0301_0 = 53,
        st02_0302_0 = 54,
        st02_0303_0 = 55,
        st02_0304_0 = 56,
        st02_0305_0 = 57,
        st02_0306_0 = 58,
        st02_0307_0 = 59,
        st02_0308_0 = 60,
        st02_0309_0 = 61,
        st02_0310_0 = 62,
        st02_0311_0 = 63,
        st02_0312_0 = 64,
        st02_0313_0 = 65,
        st02_0314_0 = 66,
        st02_0401_0 = 67,
        st02_0401_1 = 68,
        st02_0402_0 = 69,
        st02_0403_0 = 70,
        st02_0404_0 = 71,
        st02_0405_0 = 72,
        st02_0406_0 = 73,
        st02_0407_0 = 74,
        st02_0408_0 = 75,
        st02_0409_0 = 76,
        st02_0410_0 = 77,
        st02_0411_0 = 78,
        st02_0412_0 = 79,
        st02_0501_0 = 80,
        st02_0502_0 = 81,
        st02_0503_0 = 82,
        st02_0504_0 = 83,
        st02_0505_0 = 84,
        st02_0506_0 = 85,
        st02_0507_0 = 86,
        st02_0508_0 = 87,
        st02_0601_0 = 88,
        st02_0602_0 = 89,
        st02_0603_0 = 90,
        st02_0604_0 = 91,
        st02_0605_0 = 92,
        st02_0606_0 = 93,
        st02_0607_0 = 94,
        st02_0608_0 = 95,
        st02_0609_0 = 96,
        st02_0610_0 = 97,
        st02_0650_0 = 98,
        st02_0701_0 = 99,
        st02_0702_0 = 100,
        st02_0703_0 = 101,
        st02_0704_0 = 102,
        st02_0705_0 = 103,
        st02_0708_0 = 104,
        st02_0709_0 = 105,
        st02_0710_0 = 106,
        st02_0711_0 = 107,
        st02_0712_0 = 108,
        st02_0713_0 = 109,
        st02_0714_0 = 110,
        st02_0715_0 = 111,
        st02_0716_0 = 112,
        st02_0717_0 = 113,
        st02_0750_0 = 114,
        st02_0751_0 = 115,
        st02_0752_0 = 116,
        st02_0753_0 = 117,
        st02_0754_0 = 118,
        st02_0755_0 = 119,
        st02_0800_0 = 120,
        st02_0509_0 = 121,
        st03_0101_0 = 122,
        st03_0102_0 = 123,
        st03_0103_0 = 124,
        st03_0104_0 = 125,
        st03_0105_0 = 126,
        st03_0106_0 = 127,
        st03_0107_0 = 128,
        st03_0108_0 = 129,
        st03_0151_0 = 130,
        st03_0152_0 = 131,
        st03_0153_0 = 132,
        st03_0154_0 = 133,
        st03_0201_0 = 134,
        st03_0202_0 = 135,
        st03_0203_0 = 136,
        st03_0204_0 = 137,
        st03_0205_0 = 138,
        st03_0206_0 = 139,
        st03_0207_0 = 140,
        st03_0208_0 = 141,
        st03_0209_0 = 142,
        st03_0210_0 = 143,
        st03_0211_0 = 144,
        st03_0212_0 = 145,
        st03_0213_0 = 146,
        st03_0214_0 = 147,
        st03_0215_0 = 148,
        st03_0216_0 = 149,
        st03_0217_0 = 150,
        st03_0218_0 = 151,
        st03_0219_0 = 152,
        st03_0220_0 = 153,
        st03_0221_0 = 154,
        st03_0222_0 = 155,
        st03_0223_0 = 156,
        st03_0224_0 = 157,
        st03_0225_0 = 158,
        st03_0226_0 = 159,
        st03_0227_0 = 160,
        st03_0228_0 = 161,
        st03_0229_0 = 162,
        st03_0230_0 = 163,
        st03_0231_0 = 164,
        st03_0232_0 = 165,
        st03_0233_0 = 166,
        st03_0234_0 = 167,
        st03_0235_0 = 168,
        st03_0236_0 = 169,
        st03_0251_0 = 170,
        st03_0252_0 = 171,
        st03_0253_0 = 172,
        st03_0254_0 = 173,
        st03_0255_0 = 174,
        st03_0256_0 = 175,
        st03_0257_0 = 176,
        st03_0258_0 = 177,
        st03_0259_0 = 178,
        st03_0260_0 = 179,
        st03_0261_0 = 180,
        st03_0262_0 = 181,
        st03_0301_0 = 182,
        st03_0302_0 = 183,
        st03_0303_0 = 184,
        st03_0304_0 = 185,
        st03_0305_0 = 186,
        st03_0306_0 = 187,
        st03_0307_0 = 188,
        st03_0308_0 = 189,
        st03_0309_0 = 190,
        st03_0310_0 = 191,
        st03_0311_0 = 192,
        st03_0312_0 = 193,
        st03_0313_0 = 194,
        st03_0314_0 = 195,
        st03_0315_0 = 196,
        st03_0401_0 = 197,
        st03_0402_0 = 198,
        st03_0403_0 = 199,
        st03_0404_0 = 200,
        st03_0451_0 = 201,
        st03_0452_0 = 202,
        st03_0501_0 = 203,
        st03_0502_0 = 204,
        st03_0503_0 = 205,
        st03_0601_0 = 206,
        st03_0602_0 = 207,
        st03_0603_0 = 208,
        st03_0604_0 = 209,
        st03_0605_0 = 210,
        st03_0606_0 = 211,
        st03_0607_0 = 212,
        st03_0608_0 = 213,
        st03_0609_0 = 214,
        st03_0610_0 = 215,
        st03_0611_0 = 216,
        st03_0612_0 = 217,
        st03_0613_0 = 218,
        st03_0614_0 = 219,
        st03_0615_0 = 220,
        st03_0616_0 = 221,
        st03_0617_0 = 222,
        st03_0618_0 = 223,
        st03_0619_0 = 224,
        st03_0620_0 = 225,
        st03_0621_0 = 226,
        st03_0701_0 = 227,
        st03_0702_0 = 228,
        st03_0703_0 = 229,
        st03_0704_0 = 230,
        st03_0705_0 = 231,
        st03_0706_0 = 232,
        st03_0751_0 = 233,
        st03_0752_0 = 234,
        st03_0753_0 = 235,
        st03_0754_0 = 236,
        st03_0801_0 = 237,
        st03_0802_0 = 238,
        st03_0803_0 = 239,
        st03_0804_0 = 240,
        st03_0805_0 = 241,
        st03_0806_0 = 242,
        st04_0101_0 = 243,
        st04_0102_0 = 244,
        st04_0103_0 = 245,
        st04_0104_0 = 246,
        st04_0105_0 = 247,
        st04_0106_0 = 248,
        st04_0107_0 = 249,
        st04_0108_0 = 250,
        st04_0109_0 = 251,
        st04_0110_0 = 252,
        st04_0111_0 = 253,
        st04_0112_0 = 254,
        st04_0113_0 = 255,
        st04_0201_0 = 256,
        st04_0202_0 = 257,
        st04_0203_0 = 258,
        st04_0204_0 = 259,
        st04_0205_0 = 260,
        st04_0206_0 = 261,
        st04_0207_0 = 262,
        st04_0208_0 = 263,
        st04_0209_0 = 264,
        st04_0210_0 = 265,
        st04_0211_0 = 266,
        st04_0501_0 = 267,
        st04_0101_1 = 268,
        st04_0102_1 = 269,
        st04_0103_1 = 270,
        st04_0104_1 = 271,
        st04_0105_1 = 272,
        st04_0106_1 = 273,
        st04_0107_1 = 274,
        st04_0108_1 = 275,
        st04_0109_1 = 276,
        st04_0110_1 = 277,
        st04_0111_1 = 278,
        st04_0112_1 = 279,
        st04_0113_1 = 280,
        st04_0201_1 = 281,
        st04_0202_1 = 282,
        st04_0203_1 = 283,
        st04_0204_1 = 284,
        st04_0205_1 = 285,
        st04_0206_1 = 286,
        st04_0207_1 = 287,
        st04_0208_1 = 288,
        st04_0209_1 = 289,
        st04_0210_1 = 290,
        st04_0211_1 = 291,
        st04_0301_1 = 292,
        st04_0302_1 = 293,
        st04_0401_1 = 294,
        st04_0402_1 = 295,
        st04_0403_1 = 296,
        st04_0404_1 = 297,
        st04_0405_1 = 298,
        st04_0406_1 = 299,
        st04_0407_1 = 300,
        st04_0408_1 = 301,
        st04_0409_1 = 302,
        st04_0410_1 = 303,
        st05_0101_0 = 304,
        st05_0102_0 = 305,
        st05_0103_0 = 306,
        st05_0104_0 = 307,
        st05_0105_0 = 308,
        st05_0106_0 = 309,
        st05_0107_0 = 310,
        st05_0108_0 = 311,
        st05_0109_0 = 312,
        st05_0110_0 = 313,
        st05_0111_0 = 314,
        st05_0112_0 = 315,
        st05_0201_0 = 316,
        st05_0202_0 = 317,
        st05_0203_0 = 318,
        st05_0301_0 = 319,
        st05_0302_0 = 320,
        st05_0303_0 = 321,
        st05_0401_0 = 322,
        st05_0501_0 = 323,
        st05_0601_0 = 324,
        MAP_NUM = 325,
    };

    public enum LocationID
    {
        invalid = 0,
        Title = 1,
        UpTown = 2,
        RPD = 3,
        DownTown = 4,
        ClockTower = 5,
        Hospital = 6,
        Park = 7,
        DisusedPlant = 8,
        Escape_GrayBox = 9,
        Opening = 10,
        Opening1 = 11,
        RPD_B1 = 12,
        Laboratory = 13,
        Hospital2 = 14,
        GameOver = 15,
        Result = 16,
        Ending = 17,
        LOCATION_NUM = 18,
    };

    public enum PlayerState
    {
        Dead,
        Fine,
        Caution,
        Danger,
        Poisoned,
        Gassed
    }

    public enum SurvivorType : int
    {
        Invalid = -1,
        Carlos = 0,
        PL1000 = 1,
        Jill = 2,
        PL2700 = 3,
        PL2900 = 4,
        PL2910 = 5,
        PL4000 = 6,
        PL5000 = 7,
        PL7000 = 8,
        PL7010 = 9,
        PL7020 = 10,
        PL7030 = 11,
        PL8000 = 12,
        PL8010 = 13,
        PL8020 = 14,
        PL8030 = 15,
        PL8040 = 16,
        PL8050 = 17,
        PL8060 = 18,
        PL8070 = 19,
        PL8080 = 20,
        PL8090 = 21,
        PL8100 = 22,
        PL8140 = 23,
        PL8150 = 24,
        PL8170 = 25,
        PL8200 = 26,
        PL8500 = 27,
        PL8510 = 28,
        PL8520 = 29,
        PL8530 = 30,
        PL8540 = 31,
        PL8550 = 32,
        PL8560 = 33,
        PL9000 = 34,
        PL9100 = 35,
        PL9200 = 36,
        PL9300 = 37,
        PL9400 = 38,
        PL9500 = 39,
        PL9600 = 40,
        PL9700 = 41,
        PL9800 = 42,
        PL9900 = 43,
        INVALID = -1,
        CARLOS = 0,
        CLAIRE = 1,
        JILL = 2,
        JILL_MIRROR = 4,
        JILL_MIRROR_DEF = 5,
        NICHOLAI = 6,
        MIKHAIL = 7,
        BRAD = 12,
        ZOMBIE_A = 8,
        ZOMBIE_B = 9,
        ZOMBIE_C = 10,
        POLICE_H = 11,
        TYRELL = 13,
        MURPHY = 14,
        DARIO = 15,
        AMY = 16,
        RACCOON_CITIZEN = 17,
        NATHANIEL = 18,
        CARLOSZONBI = 19,
        MAVIN = 22,
        BRIAN = 23,
        BEN = 24,
        ROBERT = 25,
        POLICEMAN_A = 27,
        POLICEMAN_B = 28,
        RESEARCHER_MOB = 29,
        PROTECTIVECLOTH = 33,
    }

    public enum Costume : int
    {
        Invalid = -1,
        Default = 0,
        Costume_1 = 1,
        Costume_2 = 2,
        Costume_3 = 3,
        Costume_4 = 4,
        Costume_5 = 5,
        Costume_6 = 6,
        Costume_7 = 7,
        Costume_8 = 8,
        Costume_9 = 9,
        Costume_A = 10,
        Costume_B = 11,
        Costume_C = 12,
        Costume_D = 13,
        Costume_E = 14,
        Costume_F = 15,
        Scenario = 16,
        Classic = 17,
    };
}