using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x74)]

    public struct GameInventoryEntry
    {
        [FieldOffset(0x28)] private int slotPosition;
        [FieldOffset(0x60)] private int itemID;
        [FieldOffset(0x64)] private int weaponID;
        [FieldOffset(0x68)] private int attachments;
        [FieldOffset(0x70)] private int quantity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsItem)
                    return string.Format("[#{0}] Item {1} Quantity {2}", SlotPosition, ItemID, Quantity);
                else if (IsWeapon)
                    return string.Format("[#{0}] Weapon {1} Quantity {2} Attachments {3}", SlotPosition, WeaponID, Quantity, Attachments);
                else
                    return string.Format("[#{0}] Empty Slot", SlotPosition);
            }
        }

        public int SlotPosition => slotPosition;
        public ItemEnumeration ItemID => (ItemEnumeration)itemID;
        public WeaponEnumeration WeaponID => (WeaponEnumeration)weaponID;
        public AttachmentsFlag Attachments => (AttachmentsFlag)attachments;
        public int Quantity => quantity;
        public bool IsItem => ItemID != ItemEnumeration.None && (WeaponID == WeaponEnumeration.None || WeaponID == 0);
        public bool IsWeapon => ItemID == ItemEnumeration.None && WeaponID != WeaponEnumeration.None && WeaponID != 0;
        public bool IsEmptySlot => !IsItem && !IsWeapon;
    }
}