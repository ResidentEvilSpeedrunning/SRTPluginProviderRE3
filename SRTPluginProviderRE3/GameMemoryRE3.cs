using SRTPluginProviderRE3.Structs.GameStructs;
using SRTPluginProviderRE3.Structs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3
{
    public class GameMemoryRE3 : IGameMemoryRE3
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss";

        public string GameName => "RE3R";

        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public CharacterEnumeration PlayerCharacter { get => (CharacterEnumeration)_playerCharacter; set => _playerCharacter = (int)value; }
        internal int _playerCharacter;

        public GamePlayer Player { get => _player; set => _player = value; }
        internal GamePlayer _player;

        public string PlayerName => string.Format("{0}: ", PlayerCharacter.ToString());

        public int PlayerDeathCount { get => _playerDeathCount; set => _playerDeathCount = value; }
        internal int _playerDeathCount;

        public int PlayerInventoryCount { get => _playerInventoryCount; set => _playerInventoryCount = value; }
        internal int _playerInventoryCount;

        public GameInventoryEntry[] PlayerInventory { get => _playerInventory; set => _playerInventory = value; }
        internal GameInventoryEntry[] _playerInventory;

        public EnemyHP[] EnemyHealth { get => _enemyHealth; set => _enemyHealth = value; }
        internal EnemyHP[] _enemyHealth;

        public GameTimer Timer { get => _timer; set => _timer = value; }
        internal GameTimer _timer;

        public int Difficulty { get => _difficulty; set => _difficulty = value; }
        internal int _difficulty;

        public GameRankManager RankManager { get => _rankManager; set => _rankManager = value; }
        internal GameRankManager _rankManager;

        public int Saves { get => _saves; set => _saves = value; }
        internal int _saves;

        public int MapID { get => _mapID; set => _mapID = value; }
        internal int _mapID;

        public float FrameDelta { get => _frameDelta; set => _frameDelta = value; }
        internal float _frameDelta;

        public GameBools GameBools { get => _gameBools; set => _gameBools = value; }
        internal GameBools _gameBools;
        public bool IsRunning => GameBools.isRunning != 0x00;

        public bool IsCutscene => GameBools.isCutscene != 0x00;

        public bool IsMenu => GameBools.isMenu != 0x00;

        public bool IsPaused => GameBools.isPaused != 0x00;

        // Public Properties - Calculated
        public long IGTCalculated => unchecked(Timer.IGTRunningTimer - Timer.IGTCutsceneTimer - Timer.IGTPausedTimer);

        public long IGTCalculatedTicks => unchecked(IGTCalculated * 10L);

        public TimeSpan IGTTimeSpan
        {
            get
            {
                TimeSpan timespanIGT;

                if (IGTCalculatedTicks <= TimeSpan.MaxValue.Ticks)
                    timespanIGT = new TimeSpan(IGTCalculatedTicks);
                else
                    timespanIGT = new TimeSpan();

                return timespanIGT;
            }
        }

        public string IGTFormattedString => IGTTimeSpan.ToString(IGT_TIMESPAN_STRING_FORMAT, CultureInfo.InvariantCulture);

        public string DifficultyName
        {
            get
            {
                switch (Difficulty)
                {
                    case 0:
                        return "Assisted";
                    case 1:
                        return "Standard";
                    case 2:
                        return "Hardcore";
                    case 3:
                        return "Nightmare";
                    case 4:
                        return "Inferno";
                    default:
                        return "Unknown";
                }
            }
        }

        public string ScoreName
        {
            get
            {
                TimeSpan SRank;
                TimeSpan BRank;
                if (Difficulty == 0)
                {
                    SRank = new TimeSpan(0, 2, 30, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }
                else if (Difficulty == 1 || Difficulty == 3 || Difficulty == 4)
                {
                    SRank = new TimeSpan(0, 2, 0, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }
                else if (Difficulty == 2)
                {
                    SRank = new TimeSpan(0, 1, 45, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }
                else
                {
                    SRank = new TimeSpan();
                    BRank = new TimeSpan();
                }

                if (IGTTimeSpan <= SRank && Saves <= 5)
                    return "S";
                else if (IGTTimeSpan <= SRank && Saves > 5)
                    return "A";
                else if (IGTTimeSpan > SRank && IGTTimeSpan <= BRank)
                    return "B";
                else if (IGTTimeSpan > BRank)
                    return "C";
                else
                    return string.Empty;
            }
        }
    }
}
