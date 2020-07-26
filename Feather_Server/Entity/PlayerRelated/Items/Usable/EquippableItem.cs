using Feather_Server.Entity.PlayerRelated.Items;
using Feather_Server.Packets;
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

        public override void toBagItem(ref PacketStream stream)
        {
            // $1: item position in bag (start counting from 1)
            // $2: baseID
            // $3: stack (ushort)
            // $4: itemType [00: un-usable, 02: directly usable (gadgets), 09: healing medicine, 0a: mp medicine, 18: toward mobs (catch mob to pet), 1a: feed pets, 20: exp buff]
            // $5: quality
            // $6: lvReq
            // $7: descIdx
            // $8: itemID

            //             $1 $2------ $3-- $4 $5 $6 $7 __ $8------
            // 2b 4fac0400 11 3bb00100 4f00 0a 00 00 00 00 3bb00100 00 // no bind      - consumable 顶级灵月液
            // 2b 49ac0400 67 b2d80100 0000 01 00 0A 03 00 a8d80100 00 // newbie knife, no bind
            // 2b c3bc1200 0e dad80100 0000 01 05 37 03 00 dfd80100 00 // 5p 55lv knife - use then bind
            // 2b 6edf1200 67 c8e00100 0000 01 06 50 03 00 c8e00100 00 // 6p 80lv knife - forever bind
            // 2b bd992100 03 a7570200 0100 02 02 00 00 00 a7570200 00 // binded - 50% exp up item
            // 2b d8862100 01 91530200 0100 02 02 00 00 00 91530200 00 // 2000 exp book (binded?)
            // 2b b5bc1200 0a 8d8a0100 1400 00 01 00 00 00 8d8a0100 00 // forever bind - consumable
            // 2b b6bc1200 09 8e8a0100 1400 00 01 00 00 00 8e8a0100 00 // forever bind - consumable
            // 2b b7bc1200 06 56540200 0000 02 02 00 00 00 5e540200 00 // forever bind - call ride
            // 2b b9bc1200 13 ee550200 0000 02 02 00 00 00 ee550200 00 // return home's feather - 4 more to use.
            // 2b 84df1200 06 56540200 0000 02 02 00 00 00 5e540200 00 // 黄滕羽翼配方

            // TODO: eItemType, eItemQuality
            /* JS_F: Here[ItemInBag] */
            stream
                /* JS: Desc[ItemUID] */
                .writeDWord(itemUID)
                /* JS: Desc[Position] */
                .writeByte(position)
                /* JS: Desc[BaseID (Display)] R[ITEM,name] */
                .writeDWord(baseID)
                /* JS: Desc[Stack] */
                .writeWord(stack)
                /* JS: Desc[ItemType] Fn[eItemType] */
                .writeByte(itemType)
                /* JS: Desc[Quality] Fn[eItemQuality] */
                .writeByte(quality)
                /* JS: Desc[Lv. Req.] */
                .writeByte(lvRequirement)
                /* JS: Desc[Desc Index (Armor Only)] */
                .writeByte((byte)(slotIndex - 100))
                /* JS: Desc[Desc Index - 2?] */
                .writePadding(1) // TODO: is that part of desc index?
                /* JS: Desc[ItemID (Desc)] R[ITEM,dec] */
                .writeDWord(itemID)
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
