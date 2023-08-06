using System;
using SRTPluginProviderRE3.Structs.GameStructs;

namespace SRTPluginProviderRE3.Structs
{
    public struct Weapon : IEquatable<Weapon>
    {
        public WeaponType WeaponID;
        public WeaponParts Attachments;

        public bool Equals(Weapon other) => (int)this.WeaponID == (int)other.WeaponID && (int)this.Attachments == (int)other.Attachments;
    }
}