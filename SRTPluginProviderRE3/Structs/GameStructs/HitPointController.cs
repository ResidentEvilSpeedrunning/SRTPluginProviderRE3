using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x64)]

    public struct HitPointController
    {
        [FieldOffset(0x54)] private int defaultHitPoints;
        [FieldOffset(0x58)] private int currentHitPoints;
        [FieldOffset(0x62)] private byte noDamage;

        public int MaxHP => defaultHitPoints;
        public int CurrentHP => currentHitPoints;
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP : 0f;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
        public PlayerState HealthState
        {
            get =>
                !IsAlive ? PlayerState.Dead :
                Percentage >= 0.66f ? PlayerState.Fine :
                Percentage >= 0.33f ? PlayerState.Caution :
                PlayerState.Danger;
        }
        public string CurrentHealthState => HealthState.ToString();
    }

    public enum PlayerState
    {
        Dead,
        Fine,
        Caution,
        Danger
    }
}