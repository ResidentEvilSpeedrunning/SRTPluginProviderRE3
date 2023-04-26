using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SRTPluginProviderRE3.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct InventoryEntry
    {
        /// <summary>
        /// Debugger display message.
        /// </summary>
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

        //internal static readonly byte[] EMPTY_INVENTORY_ITEM = new byte[20] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        //public static readonly int[] EMPTY_INVENTORY_ITEM = new int[5] { 0x00000000, unchecked((int)0xFFFFFFFF), 0x00000000, 0x00000000, 0x01000000 };

        // Storage variable.
        public int SlotPosition { get; set; }
        public ItemEnumeration ItemID { get; set; }
        public WeaponEnumeration WeaponID { get; set; }
        public AttachmentsFlag Attachments { get; set; }
        public int BulletID { get; set; }
        public int Quantity { get; set; }

        public bool IsItem => ItemID != ItemEnumeration.None && (WeaponID == WeaponEnumeration.None || WeaponID == 0);
        public bool IsWeapon => ItemID == ItemEnumeration.None && WeaponID != WeaponEnumeration.None && WeaponID != 0;
        public bool IsEmptySlot => !IsItem && !IsWeapon;
    }
}
