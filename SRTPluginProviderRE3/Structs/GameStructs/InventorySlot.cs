using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE3.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x24)]

    public struct InventorySlot
    {
        [FieldOffset(0x10)] private int itemID;
        [FieldOffset(0x14)] private int weaponID;
        [FieldOffset(0x18)] private int attachments;
        [FieldOffset(0x1C)] private int bulletId;
        [FieldOffset(0x20)] private int quantity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsItem)
                    return string.Format("[#{0}] Item {1} Quantity {2}", ItemID, Quantity);
                else if (IsWeapon)
                    return string.Format("[#{0}] Weapon {1} Quantity {2} Attachments {3}", WeaponID, Quantity, Attachments, BulletID);
                else
                    return string.Format("[#{0}] Empty Slot");
            }
        }

        public ItemEnumeration ItemID => (ItemEnumeration)itemID;
        public WeaponEnumeration WeaponID => (WeaponEnumeration)weaponID;
        public AttachmentsFlag Attachments => (AttachmentsFlag)attachments;
        public int Quantity => quantity;
        public int BulletID => bulletId;
        public bool IsItem => ItemID != ItemEnumeration.None && WeaponID == WeaponEnumeration.None;
        public bool IsWeapon => ItemID == ItemEnumeration.None && WeaponID != WeaponEnumeration.None;
        public bool IsEmptySlot => !IsItem && !IsWeapon;
    }
}