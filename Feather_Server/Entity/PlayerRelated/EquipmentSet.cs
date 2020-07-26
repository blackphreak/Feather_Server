using Feather_Server.Packets;
using Feather_Server.PlayerRelated.Items;
using System.Collections.Generic;

namespace Feather_Server.PlayerRelated
{
    public enum EquipmentSlot : byte
    {
        HELMET = 101,     // 0x65
        NECKLACE = 102,   // 0x66
        WEAPON = 103,     // 0x67
        CHESTPLATE = 104, // 0x68
        BODY = 105,       // 0x69 - style
        BOOTS = 106,      // 0x6A

        LRING = 107,      // 0x6B
        RRING = 108,      // 0x6C
        WINGS = 109,      // 0x6D
        HAT = 110,        // 0x6E - style
        MASK = 111,       // 0x6F - style
        TAIL = 112,       // 0x70 - style
    }

    public class EquipmentSet
    {
        private Dictionary<EquipmentSlot, EquippableItem> equips = new Dictionary<EquipmentSlot, EquippableItem>()
        {
            { EquipmentSlot.MASK, null },
            { EquipmentSlot.HAT, null },
            { EquipmentSlot.WINGS, null },
            { EquipmentSlot.BODY, null },
            { EquipmentSlot.TAIL, null },

            { EquipmentSlot.HELMET, null },
            { EquipmentSlot.NECKLACE, null },
            { EquipmentSlot.CHESTPLATE, null },
            { EquipmentSlot.WEAPON, null },
            { EquipmentSlot.LRING, null },
            { EquipmentSlot.BOOTS, null },
            { EquipmentSlot.RRING, null },
        };

        //// style
        //public EquippableItem mask;
        //public EquippableItem hat;
        //public EquippableItem wings;
        //public EquippableItem body;
        //public EquippableItem tail;

        //// real armor
        //public EquippableItem helmet;
        //public EquippableItem necklace;
        //public EquippableItem chestplate;
        //public WeaponItem weapon;
        //public EquippableItem ringLeft;
        //public EquippableItem boots;
        //public EquippableItem ringRight;

        public bool equip(EquippableItem item, EquipmentSlot slot)
        {
            return equips.TryAdd(slot, item);
        }

        public bool unequip(EquipmentSlot slot, out EquippableItem item)
        {
            if (equips.TryGetValue(slot, out item))
            {
                equips.Remove(slot);
                return true;
            }
            else
            {
                return false;
            }
        }

        public EquippableItem getItem(EquipmentSlot slot)
        {
            return equips.GetValueOrDefault(slot);
        }

        public void toFragment(ref PacketStream stream)
        {
            new HeroModel(
                equips.GetValueOrDefault(EquipmentSlot.MASK),
                equips.GetValueOrDefault(EquipmentSlot.HAT),
                equips.GetValueOrDefault(EquipmentSlot.WINGS),
                equips.GetValueOrDefault(EquipmentSlot.BODY),
                equips.GetValueOrDefault(EquipmentSlot.TAIL),
                (WeaponItem)equips.GetValueOrDefault(EquipmentSlot.WEAPON)
            ).toFragment(ref stream);
        }
    }
}
