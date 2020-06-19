using Feather_Server.Entity.PlayerRelated.Items;
using Feather_Server.PlayerRelated.Items;
using Feather_Server.ServerRelated;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Feather_Server.Entity.PlayerRelated.Items.ItemAttributes;
using Feather_Server.Packets;

namespace Feather_Server.PlayerRelated
{
    public class Item
    {
        /// <summary>
        /// the unique ID of this item. Used to store in database.
        /// </summary>
        public uint itemUID;

        /// <summary>
        /// [Front](bigger  number) the ID of the display item in bag/ground/equipment slot
        /// </summary>
        public uint baseID;

        /// <summary>
        /// [Tail ](smaller number) the ID of the item description (also responsable for display name)
        /// </summary>
        public uint itemID;

        public ushort stack;     // 2 bytes | max: 0x3E8 (dec: 1000)
        public int sellingPrice; // -1: unsellable

        /// <summary>
        /// Item quality, also used to define the item description color
        /// </summary>
        /// TODO: enum for colors.
        /// spec1 = 0x0009782A // light yellow
        ///    1p = 0x0009782B // white
        ///    2p = 0x0009782C // light blue
        ///    3p = 0x0009782D // 
        ///    4p = 0x0009782E // 
        ///    5p = 0x0009782F // dark red
        ///    6p = 0x00097830 // light red
        ///    7p = 0x00097831 // 
        ///    8p = 0x00097832 // 
        ///    9p = 0x00097833 // 
        public byte quality = 0x00;

        public byte lvRequirement = 0x00;

        /// <summary>
        /// Count start from 1
        /// </summary>
        public byte position;

        public byte itemType = 0x01;

        public List<EHeadAttribute> headAttributes = new List<EHeadAttribute>();
        public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();
        public EItemParts itemPart = EItemParts.NONE;

        /// <summary>
        /// List of description packets [size and pkt-type EXCLUDED]
        /// </summary>
        /// <returns>List of description packets [size and pkt-type EXCLUDED]</returns>
        public virtual List<byte[]> toDesc()
        {
            List<byte[]> pkts = new List<byte[]>();

            // XX: packet type (excluded, wait for PacketEncoder to build with header [pkt-sz and pkt-type])
            // $1: itemUID
            // upper one 6p 4* +6 (钰风战戒) | itemID: 141100 (0x2272C | 2C2702)
            //    XX $1------ __ $2------ __ $param1- __ $param2- __ $param3- __ $param4- __
            // __ 31 2c270200 01 00000000 00                                                 // tag start (item desc)
            // __ 31 2c270200 01 0b780900 64 2c270200 64 06000000 64 30780900 64 2c780900 00 // (|3^1 `=C|4+$2`=C) $1: itemID $2: 強化數 $3 colorCode $4: 強化ColorCode
            // __ 31 2c270200 01 09780900 00                                                 // 永綁 (09780900) (`N`+C0xff0000`-C [永久绑定]`=C)
            // __ 31 2c270200 01 0a780900 64 04000000 00                                     // 星星數
            // __ 31 2c270200 01 18790900 00                                                 // tag start (620824 `N 道具描述(后面无空格))
            // __ 31 2c270200 01 16780900 00                                                 // itemType [16780900: 戒指]
            // __ 31 2c270200 02 2c270200 00                                                 // item desc (by itemID)
            // __ 31 2c270200 01 18790900 00                                                 // tag end (620824 `N 道具描述(后面无空格))
            // __ 31 2c270200 01 48790900 64 27780900 64 64000000 00                         // lv Req 100 [red] (装备要求: |1$2 级`=C`N)
            // __ 31 2c270200 01 41780900 64 78000000 64 ff000000 00                         // 耐久 120/255 (耐久: $1 / $2`N)
            // __ 31 2c270200 01 44780900 64 30780900 73 03000000 2b3235 00                  // 精神 +25 [red] | 44780900
            // __ 31 2c270200 01 46780900 64 30780900 73 03000000 2b3236 00                  // 體質 +26 [red] | 46780900
            // __ 31 2c270200 01 47780900 64 30780900 73 03000000 2b3531 00                  // 命中 +51 [red] | 47780900
            // __ 31 2c270200 01 5e780900 64 30780900 73 05000000 2b33333933 00              // 生命 +3393 [red] | 5e780900
            // __ 31 2c270200 01 62780900 64 30780900 73 04000000 2b343238 00                // 法力 +428 [red] | 62780900 (|1法力: $2`=C`N)
            // __ 31 2c270200 01 77780900 64 31780900 73 05000000 2b31372e30 00              // 附加生命 | value: +17.0 [purple] | 77780900
            // __ 31 2c270200 01 78780900 64 31780900 73 05000000 2b31372e30 00              // 附加法力 | value: +17.0 [purple] | 78780900
            // __ 31 2c270200 01 7d780900 64 91e10800 64 01000000 00                         // 黑暗恐懼 Lv.1 - 91e10800 | 7d780900
            // __ 31 2c270200 01 7e780900 73 02000000 3037 73 02000000 3030 00               // 洞洞數 07 (可用) 00 (已入) | 7e780900 ([4:H$1:U$2]`N)
            // __ 31 2c270200 01 8c780900 64 59010000 00                                     // 賣出價 (345) | 8c780900
            // __ 31 2c270200 01 97780900 00                                                 // itemQuality [97780900: 六品裝備] (`+C0xffff00`-C[六品装备]`=C`N)
            // __ 31 2c270200 01 9b780900 73 04000000 cea8d2bb 00                            // maker name | 9b780900 (`N`+C0xffff00`-C[製作者：^1]`=C`N)
            // __ 31 2c270200 01 ffffffff 00                                                 // tag end (item desc)

            string header = Lib.toHex(this.itemUID);

            // start tag (item desc)
            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + "00000000"
                + "00"
            ));

            // item name (TODO: if is upgradable, use format_1 instead)
            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + Lib.toHex((uint)EItemAttribute.format_2)
                + "64"
                + Lib.toHex(0x0009782C)
                + "64"
                + Lib.toHex(this.itemID)
                + "00"
            ));

            foreach (var attr in headAttributes)
            {
                pkts.Add(Lib.hexToBytes(
                    header
                    + "01"
                    + Lib.toHex((uint)attr)
                    + "00"
                ));
            }
            // star level (TODO: only if it is upgradable)
            // ...

            // start tag
            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + "18790900"
                + "00"
            ));

            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + Lib.toHex((uint)this.itemPart)
                + "00"
            ));

            // end tag
            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + "18790900"
                + "00"
            ));

            foreach (var attr in itemAttributes)
            {
                pkts.Add(Lib.hexToBytes(
                    header
                    + "01"
                    + Lib.toHex((uint)attr.attr)
                    + "00"
                    + Lib.toHex(attr.descPkt)
                    + "00"
                ));
            }

            // end tag (item desc)
            pkts.Add(Lib.hexToBytes(
                header
                + "01"
                + "ffffffff"
                + "00"
            ));

            return pkts;
        }

        public static Item fromJson(string json)
        {
            return JsonConvert.DeserializeObject<Item>(json, Lib.jsonSetting);
        }

        public virtual string toJson()
        {
            return JsonConvert.SerializeObject(this, Lib.jsonSetting);
        }

        public virtual byte[] use(Hero p)
        {
            throw new NotImplementedException();
        }

        public virtual void toBagItem(ref PacketStream stream)
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
                .writeByte(0x0)
                /* JS: Desc[Desc Index - 2?] */
                .writePadding(1) // TODO: is that part of desc index?
                /* JS: Desc[ItemID (Desc)] R[ITEM,dec] */
                .writeDWord(itemID)
                ;
        }
    }
}
