using Feather_Server.Entity.NPC_Related;
using Feather_Server.MobRelated;
using Feather_Server.Packets.PacketLibs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class NPCPacket
    {


        public static PacketStreamData spawnNPC(NPC npc)
        {
            // mob:
            // $1: facing
            // $2: cate [0x80: NPC (blue)[Ally], 0x81: golden, 0x82: white, 0x83: yellow, 0x84: Lv1 Elite Mob]
            // $3: movement speed in second (0x01=fast, 0xFF=slow)
            // $4: model ID
            // $5: health bar (0%: 0x00, 50%: 0x19, 100%: 0x32)
            // $6: element (元素) [0: none, 1: , 2: , 3: , 4: fire, 5: , 6: water, 7: ]
            // $7: color?
            // $8: color?
            // $9: if appears, use format string, otherwise, name only.
            // $10: string format ID
            // $11: mobNameID

            //          eid----- X--- Y--- $1 $2 $3 $4------ $5 $6 $7-- $8-- $9 $10----- __ $11----- __ mobLv---
            // __ 69 80 a1530000 b300 3300 04 82 01 c3410600 32 06 0000 0000 00 91b00a00 64 c3410600 64 07000000 00
            // __ 69 80 4a2b1200 2c00 7e00 05 82 01 f6410600 32 06 0000 0000 00 91b00a00 64 f6410600 64 0a000000 00
            // __ 69 80 e21b0a00 ad00 7f00 04 82 01 9b410600 16 06 0000 0000 00 91b00a00 64 9b410600 64 03000000 00 // 半血小鴨
            // __ 69 80 91690000 d400 8100 03 80 01 69690600 00 00 0000 0000 00 6acb1000 64 a5bf0400             00 // 小鴨 (0x0x10CB6A)1100650: ^1, (0x04BFA5)311205: (摇滚小鸡使者?!)
            // __ 69 80 6fd22b00 9700 5800 00 83 01 b1570700 32 04 0000 0000 00 91b00a00 64 52650600 64 46000000 00 // 火鳳凰
            // __ 69 80 95690000 ae00 5800 02 80 01 c7240000 00 00 0000 0000 00 66420f00 64 12340500             00 // 天师导师
            // __ 69 80 43690000 a800 6500 04 80 01 ee130000 00 00 0000 0000 00 66420f00 64 30340500             00 // 术士导师
            // __ 69 80 42690000 9900 6000 04 80 01 e7260000 00 00 0000 0000 00 66420f00 64 1c340500             00 // 武士导师
            // __ 69 80 95690000 ae00 5800 02 80 01 c7240000 00 00 0000 0000 00 66420f00 64 12340500             00 // 天师导师
            // __ 69 80 94690000 b500 7200 04 80 01 1d150000 00 00 0000 0000 00 66420f00 64 200d0500             00 // 防具店老板
            // __ 69 80 99690000 a600 5f00 06 80 01 7d150000 00 00 0000 0000 00 66420f00 64 62340500             00 // 术士导师助手
            // __ 69 80 98690000 9200 6100 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 4e340500             00 // 武士导师助手
            // __ 69 80 44690000 a900 5500 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 44340500             00 // 天师导师助手

            //          eid----- X--- Y--- $1 $2 $3 $4------ $5 $6 $7-- $8-- GBK Name----------------
            // __ 69 80 06a91600 6600 8700 00 80 01 9d070700 00 00 0000 0000 c9bdd2b0d1fdb9d6                    00 // 山野
            // __ 69 80 38a91600 b500 5600 00 80 01 9d070700 00 00 0000 0000 b1a9c5adc9bdd2b0d1fdb9d6            00 // 暴怒山野

            //          eid----- X--- Y--- $1 $2 $3 $4------ $5 $6 $7-- $8-- $9 $10----- __ $11----- __ FSID----
            // __ 69 80 bb021700 ac00 d300 05 82 01 f6410600 32 06 0000 0000 00 6c420f00 64 f6410600 64 35150000 00 // 雨天寶寶 [white name] (0x0641F6): 雨天幽灵, (0x1535): 宝宝
            //                                                                                       __ nameLen- GBK PlyName-
            // __ 69 80 3a433000 a100 7f00 00 81 01 cdbf0400 29 02 0000 0000 00 b5ae0a00 64 f8bf0400 73 06000000 caaed7d6befc 00 // 寵物 - using default name
            // __ 69 80 17513300 d500 5400 02 81 0f 4e650600 2b 02 0000 0000 00 b5ae0a00 64 5a650600 73 08000000 c2e4c0e1cedebadb 00 // 寵物 - using default name
            //                                                                                       __ nameLen- GBK Pet name--------    nameLen- GBK PlyName-
            // __ 69 80 c8843300 d300 5500 04 81 0f 9c690600 19 02 0000 0000 00 b6ae0a00             73 0a000000 d0a1d0a1c0b6beabc1e9 73 06000000 d1ccd3eaa4a3 00 // 寵物 [0x0AAEB6]700086: $1($2)

            // __ 69 80 97690000 c100 4500 05 80 01 901a0000 00 00 0000 0000 00 66420f00 64 3a340500 00 // 劍客導師
            // __ 69 80 97700000 B900 4700 03 80 01 901a0000 00 00 0000 0000 00 66420f00 64 6c340500 00 // 劍客導師助手
            // __ 69 80 95690000 ae00 5800 02 80 01 c7240000 00 00 0000 0000 00 66420f00 64 12340500 00 // 天师导师
            // __ 69 80 44690000 a900 5500 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 44340500 00 // 天师导师助手
            // __ 69 80 43690000 a800 6500 04 80 01 ee130000 00 00 0000 0000 00 66420f00 64 30340500 00 // 术士导师
            // __ 69 80 99690000 a600 5f00 06 80 01 7d150000 00 00 0000 0000 00 66420f00 64 62340500 00 // 术士导师助手
            // __ 69 80 42690000 9900 6000 04 80 01 e7260000 00 00 0000 0000 00 66420f00 64 1c340500 00 // 武士导师
            // __ 69 80 98690000 9200 6100 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 4e340500 00 // 武士导师助手
            // __ 69 80 94690000 b500 7200 04 80 01 1d150000 00 00 0000 0000 00 66420f00 64 200d0500 00 // 防具店老板
            // __ 69 80 96690000 a200 7800 02 80 01 ee130000 00 00 0000 0000 00 66420f00 64 26340500 00 // 祭師導師
            // __ 69 80 45690000 9f00 8200 02 80 01 7d150000 00 00 0000 0000 00 66420f00 64 58340500 00 // 祭師導師助手
            // __ 69 80 66690000 8000 7e00 01 80 01 9b230000 00 00 0000 0000 00 66420f00 64 1ae60400 00 // 李四 居民丁
            // __ 69 80 15690000 8100 9000 06 80 01 c7240000 00 00 0000 0000 00 66420f00 64 10e60400 00 // 張三 居民丙
            // __ 69 80 3a690000 5200 a200 01 80 01 c7240000 00 00 0000 0000 00 66420f00 64 fce50400 00 // 道界尊者
            // __ 69 80 6a690000 9e00 9600 02 80 01 81150000 00 00 0000 0000 00 66420f00 64 74e60400 00 // 遊方道人
            // __ 69 80 8c690000 a900 9700 02 80 01 51140000 00 00 0000 0000 00 66420f00 64 29c00400 00 // 落日城戰召喚兵
            // __ 69 80 21690000 b000 9a00 01 80 01 ee130000 00 00 0000 0000 00 66420f00 64 32bf0400 00 // 部落管理員
            // __ 69 80 1a690000 c300 8200 08 80 01 45160000 00 00 0000 0000 00 66420f00 64 7ee60400 00 // 孔兄 乞丐
            // __ 69 80 6b690000 dc00 6b00 06 80 01 c7240000 00 00 0000 0000 00 66420f00 64 88e60400 00 // 劉先 居民甲
            // __ 69 80 17690000 e800 6b00 01 80 01 e7260000 00 00 0000 0000 00 66420f00 64 42e60400 00 // 護成衛兵

            // __ 69 83 ... TODO: wtf is this?


            // __ 690a0c d3 3000e600 76 00030204eb03f92a00000000e903037af9030000f90300003b000000ea0300000000000004000000020000003d08000000000000320039040000bdb4d3cda4c520202020202020202020 00 // 醬油XO
            // __ 6f3a433000cdbf040002010200 // 光環

            var stream = new PacketStream();

            /* JS_D: Desc[Spawn NPC] */
            stream.setDelimeter(Delimeters.NPC_SPAWN)
            /* JS: Desc[EntityID] */
            .writeDWord(npc.entityID)
            /* JS: Desc[LocX] */
            .writeWord(npc.locX)
            /* JS: Desc[LocY] */
            .writeWord(npc.locY)
            /* JS: Desc[Facing] Fn[eFacing] */
            .writeByte(npc.facing)
            /* JS: Desc[NPC Cate] Fn[eNPCCate] */
            .writeByte(npc.cate)
            /* JS: Desc[Animate Time (sec)] */
            .writeByte(npc.animationTime)
            /* JS: Desc[ModelID] */
            .writeDWord(npc.modelID)
            /* JS: Desc[Health Bar] */
            .writeByte((byte)(npc.HP / npc.maxHP * 0x32))
            /* JS: Desc[To Be Determined] */
            .writeByte(0x0)
            /* JS: Desc[Param7] */
            .writeWord(npc.param7)
            /* JS: Desc[Param8] */
            .writeWord(npc.param8)
            ;

            /* JS_SC: PT Jump[JS_J_A,JS_CONT_A] Desc[Direct GBK Named] */
            /* JS_SC: PT Jump[JS_J_B,JS_CONT_FORMAT] Desc[Named with ID - 1 Name] */
            /* JS_SC: PT Jump[JS_J_C,JS_CONT_FORMAT] Desc[Named with ID - 2 Names] */
            /* JS_SC: PT Jump[JS_J_D,JS_CONT_FORMAT] Desc[Named with ID, GBK] */
            /* JS_SC: PT Jump[JS_J_E,JS_CONT_FORMAT] Desc[Named with ID, LV] */
            /* JS_SC: PT Jump[JS_J_F,JS_CONT_FORMAT] Desc[Named with ID, FSID] */
            /* JS_SC: PT Jump[JS_J_G,JS_CONT_FORMAT] Desc[Named with GBK, GBK] */
            switch (npc.FormatString.Format)
            {
                case EFormatString.TEMPLATE_DIRECT_GBK:
                    /* JS_J_A */
                    /* JS: Desc[Name] NoCodeSign[$gbk] */
                    stream += npc.FormatString.Fragment;
                    /* JS_J_A_END */
                    /* JS_CONT_A */
                    return stream.pack();
                case EFormatString.TEMPLATE_ENTITY_NAME_ID_NAME1:
                    /* JS_J_B */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[Name] R[NPC] NoCodeSign[$4] */
                    /* JS_J_B_END */
                    break;
                case EFormatString.TEMPLATE_ENTITY_NAME_ID_NAME2:
                    /* JS_J_C */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[FormatStringID] Fn[parseFS] NoCodeSign[$4] */
                    /* JS: Desc[NameID] R[NPC,name,name2] NoCodeSign[64$4] */
                    /* JS_J_C_END */
                    break;
                case EFormatString.TEMPLATE_ENTITY_NAME_ID_GBK:
                    /* JS_J_D */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[FormatStringID] Fn[parseFS] NoCodeSign[$4] */
                    /* JS: Desc[NameID] R[NPC] NoCodeSign[64$4] */
                    /* JS: Desc[Owner Name] NoCodeSign[73$4$gbk] */
                    /* JS_J_D_END */
                    break;
                case EFormatString.TEMPLATE_ENTITY_NAME_ID_LV:
                    /* JS_J_E */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[FormatStringID] Fn[parseFS] NoCodeSign[91b00a00] */
                    /* JS: Desc[Name] R[NPC] NoCodeSign[64$4] */
                    /* JS: Desc[Level] NoCodeSign[64$4] */
                    /* JS_J_E_END */
                    break;
                case EFormatString.TEMPLATE_ENTITY_NAME_ID_FSID:
                    /* JS_J_F */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[FormatStringID] Fn[parseFS] NoCodeSign[6c420f00] */
                    /* JS: Desc[Name] R[NPC] NoCodeSign[64$4] */
                    /* JS: Desc[Sub-Name] R[FS] NoCodeSign[64$4] */
                    /* JS_J_F_END */
                    break;
                case EFormatString.TEMPLATE_ENTITY_NAME_GBK_GBK:
                    /* JS_J_G */
                    /* JS: Desc[Padding] */
                    stream.writePadding(1);
                    /* JS: Desc[FormatStringID] Fn[parseFS] NoCodeSign[$4] */
                    /* JS: Desc[Pet Name] NoCodeSign[73$4$gbk] */
                    /* JS: Desc[Owner Name] NoCodeSign[73$4$gbk] */
                    /* JS_J_G_END */
                    break;
                default:
                    throw new NotImplementedException();
            }

            /* JS_CONT_FORMAT */
            stream += npc.FormatString.Fragment;
            return stream.pack();
        }
    }
}
