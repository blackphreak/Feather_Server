using Feather_Server.Entity.PlayerRelated.Items;
using Feather_Server.ServerRelated;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.PlayerRelated.Items
{
    // Such item can be equipped, upgraded in "StarMake" & "Strength" & MAYBE "Embeddable".
    public class EquippableItem : Item
    {
        // max stack: 1
        public int starLevel;

        public ushort modelID = 0x0000; // only use for generate model packet bytes. it can be looked-up by baseID (?
        public ushort color   = 0x0000; // only use for generate model packet bytes. it can be looked-up by baseID (?
        public EquipmentSlot slotIndex;

        public EquippableItem()
        {
        }

        public EquippableItem(ushort modelID, ushort color)
        {
            this.modelID = modelID;
            this.color = color;
        }

        public override string toHex()
        {
            return
                "162b" // size: 16, pkt head: 2b
                + Lib.toHex((int)itemUID)
                + Lib.toHex(position) // slot, 101 to 112 are equipment slots.
                + Lib.toHex((int)baseID)
                + Lib.toHex(stack)
                + "00"
                + Lib.toHex(quality)
                + Lib.toHex(lvRequirement)
                + Lib.toHex((byte)slotIndex - 100) // desc index (armor only)
                + "00"
                + Lib.toHex((int)itemID)
                + "00"
                ;
        }

        public new static EquippableItem fromJson(string json)
        {
            return JsonConvert.DeserializeObject<EquippableItem>(json, Lib.jsonSetting);
        }

        public override string toJson()
        {
            return JsonConvert.SerializeObject(this, Lib.jsonSetting);
        }

        public virtual byte[] toEquipDesc(bool isSelf = true)
        {
            return new byte[0];
        }

    }
}
