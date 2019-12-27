using Feather_Server.MobRelated;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Feather_Server.Entity.NPC_Related;
using Feather_Server.Entity;
using Feather_Server.PlayerRelated.Skills.Rides;
using Feather_Server.PlayerRelated;
using Feather_Server.PlayerRelated.Items;

namespace Feather_Server.ServerRelated
{
    /// <summary>
    /// Provide functions for encode / decode packets.
    /// </summary>
    public static class PacketEncoder
    {
        public static byte[] alertBox(uint msgID)
        {
            // idx:2102
            byte[] pkts = new byte[0];
            concatPacket(Lib.hexToBytes(
                "2102"
                //+ "76d86a00" // formatstring.dat, msg ID | 7002230 账号密码错误
                + Lib.toHex(msgID)
                + "00"), ref pkts);
            return pkts;
        }

        public static byte[] loginSuccess(HeroBasicInfo[] players, bool isInHeroCreation = false)
        {
            // idx:490e
            // idx:3d2f
            byte[] pkts = new byte[0];
            concatPacket(Lib.hexToBytes("07490e0000000000"), ref pkts, false);

            if (isInHeroCreation)
                concatPacket(Lib.hexToBytes("073d2fcee4cabf00"), ref pkts, false);

            for (byte i = 1; i <= 6; i++)
            {
                if (i - 1 >= players.Length || players[i - 1] == null)
                    concatPacket(Lib.hexToBytes($"490d{Lib.toHex(i)}000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005e450f00730100000020640000000000"), ref pkts);
                else
                    concatPacket(Lib.hexToBytes(
                        $"490d{Lib.toHex(i)}"
                        + players[i - 1].toHex()
                    ), ref pkts);
            }

            return pkts;
        }

        public static byte[] showProgressBar(ushort time, int msgID)
        {
            // idx:5e02
            byte[] pkts = new byte[0];

            // $2: progressbar time (in seconds)
            // $4: progressbar string format ID (@ formatstring.dat)
            //
            //         $2-- $4------ __
            // __ 5a02 0500 b31d0000 00
            concatPacket(Lib.hexToBytes(
                "5a02"
                + Lib.toHex(time)
                + Lib.toHex(msgID)
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] progressBarComplete(bool isSuccess)
        {
            // idx:5e01
            byte[] pkts = new byte[0];

            // $1: 0x00: fail (red color), 0x01: success
            //         $1 __
            // __ 5a01 01 00
            concatPacket(Lib.hexToBytes(
                "5a01"
                + (isSuccess ? "01" : "00")
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] spawnDropItem(int itemModelID, int itemNameID, byte x, byte y)
        {
            // idx:2e??
            //       drop---- X--- Y--- itemModel ???? ???? ???? itemName __
            // __ 2e 9b801400 EE00 6F00 ee780200  0000 0000 0000 ee780200 00
            byte[] pkts = new byte[0];
            concatPacket(Lib.hexToBytes(
                $"2e"
                + "12345678" // drop enetity ID (whats the point lol?..)
                + Lib.toHex(x)
                + "00"
                + Lib.toHex(y)
                + "00"
                + Lib.toHex(itemModelID)
                + "0000 0000 0000"
                + Lib.toHex(itemNameID)
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] addBagItem(Item newlyAdded)
        {
            // every successfully added item MUST have position > 0
            if (newlyAdded.position == 0)
                return null;

            byte[] pkts = new byte[0];

            // $1: item description ID
            // $2: item position in bag (start counting from 1)
            // $3: baseID
            // $4: stack (ushort)
            // $5: itemType [00: un-usable, 02: directly usable (gadgets), 09: healing medicine, 0a: mp medicine, 18: toward mobs (catch mob to pet), 1a: feed pets, 20: exp buff]
            // $6: quality
            // $7: level requirement (reach this level to use this item)
            // $8: description ID
            // $9: itemID

            //       $1------ $2 $3------ $4-- $5 $6 $7 $8 __ $9------ __
            // __ 2b ea152c00 02 BC2E0200 0100 00 00 00 00 00 BC2E0200 00
            concatPacket(Lib.hexToBytes(
                "2b"
                + Lib.toHex(newlyAdded.itemUID)
                + Lib.toHex(newlyAdded.position)
                + Lib.toHex(newlyAdded.baseID)
                + Lib.toHex(newlyAdded.stack)
                + Lib.toHex(newlyAdded.itemType)
                + Lib.toHex(newlyAdded.quality)
                + Lib.toHex(newlyAdded.lvRequirement)
                + (newlyAdded is EquippableItem ? Lib.toHex((byte)(newlyAdded as EquippableItem).slotIndex) : "00")
                + "00"
                + Lib.toHex(newlyAdded.itemID)
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] activeBagItem(uint itemUID, bool isActive)
        {
            byte[] pkts = new byte[0];

            concatPacket(Lib.hexToBytes(
                "a60a"
                + (isActive ? "01" : "00")
                + Lib.toHex(itemUID)
                + "12345678"
                + "00"
            ), ref pkts);
            return pkts;
        }

        // Spawn an NPC entity on map for ONE player only. Mob included.
        public static byte[] spawnNPC(NPC npc)
        {
            // mob:
            // $1: facing
            // $2: type [0x80: NPC (blue)[Ally], 0x81: golden, 0x82: white, 0x83: yellow, 0x84: Lv1 Elite Mob]
            // $3: movement speed in second (0x01=fast, 0xFF=slow)
            // $4: model ID
            // $5: health bar (0%: 0x00, 50%: 0x19, 100%: 0x32)
            // $6: element (元素) [0: none, 1: , 2: , 3: , 4: fire, 5: , 6: water, 7: ]
            // $7: color?
            // $8: color?
            // $9: if appears, use format string, otherwise, name only.
            // $10: 
            // $11: mobNameID

            //          eid----- X--- Y--- $1 $2 $3 $4------ $5 $6 $7-- $8-- $9 $10----- __ $11----- __ mobLv---
            // __ 69 80 a1530000 b300 3300 04 82 01 c3410600 32 06 0000 0000 00 91b00a00 64 c3410600 64 07000000 00
            // __ 69 80 4a2b1200 2c00 7e00 05 82 01 f6410600 32 06 0000 0000 00 91b00a00 64 f6410600 64 0a000000 00
            // __ 69 80 6fd22b00 9700 5800 00 83 01 b1570700 32 04 0000 0000 00 91b00a00 64 52650600 64 46000000 00 // 火鳳凰
            // __ 69 80 bb021700 ac00 d300 05 82 01 f6410600 32 06 0000 0000 00 6c420f00 64 f6410600 64 35150000 00 // 雨天寶寶 [white name]
            // __ 69 80 91690000 d400 8100 03 80 01 69690600 00 00 0000 0000 00 6acb1000 64 a5bf0400             00 // 小鴨
            // __ 69 80 e21b0a00 ad00 7f00 04 82 01 9b410600 16 06 0000 0000 00 91b00a00 64 9b410600 64 03000000 00 // 半血小鴨
            // __ 69 80 95690000 ae00 5800 02 80 01 c7240000 00 00 0000 0000 00 66420f00 64 12340500             00 // 天师导师
            // __ 69 80 43690000 a800 6500 04 80 01 ee130000 00 00 0000 0000 00 66420f00 64 30340500             00 // 术士导师
            // __ 69 80 42690000 9900 6000 04 80 01 e7260000 00 00 0000 0000 00 66420f00 64 1c340500             00 // 武士导师
            // __ 69 80 95690000 ae00 5800 02 80 01 c7240000 00 00 0000 0000 00 66420f00 64 12340500             00 // 天师导师
            // __ 69 80 94690000 b500 7200 04 80 01 1d150000 00 00 0000 0000 00 66420f00 64 200d0500             00 // 防具店老板
            // __ 69 80 99690000 a600 5f00 06 80 01 7d150000 00 00 0000 0000 00 66420f00 64 62340500             00 // 术士导师助手
            // __ 69 80 98690000 9200 6100 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 4e340500             00 // 武士导师助手
            // __ 69 80 44690000 a900 5500 01 80 01 7d150000 00 00 0000 0000 00 66420f00 64 44340500             00 // 天师导师助手
            // __ 69 80 06a91600 6600 8700 00 80 01 9d070700 00 00 0000 0000 c9bdd2b0d1fdb9d6                    00 // 山野
            // __ 69 80 38a91600 b500 5600 00 80 01 9d070700 00 00 0000 0000 b1a9c5adc9bdd2b0d1fdb9d6            00 // 暴怒山野
            //                                                                                       __ nameLen- GBK PlyName-
            // __ 69 80 3a433000 a100 7f00 00 81 01 cdbf0400 29 02 0000 0000 00 b5ae0a00 64 f8bf0400 73 06000000 caaed7d6befc 00 // 寵物 - using default name
            // __ 69 80 17513300 d500 5400 02 81 0f 4e650600 2b 02 0000 0000 00 b5ae0a00 64 5a650600 73 08000000 c2e4c0e1cedebadb 00 // 寵物 - using default name
            //                                                                                       __ nameLen- GBK Pet name--------    nameLen- GBK PlyName-
            // __ 69 80 c8843300 d300 5500 04 81 0f 9c690600 19 02 0000 0000 00 b6ae0a00             73 0a000000 d0a1d0a1c0b6beabc1e9 73 06000000 d1ccd3eaa4a3 00 // 寵物

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


            // __ 690a0c d3 3000e600 76 00030204eb03f92a00000000e903037af9030000f90300003b000000ea0300000000000004000000020000003d08000000000000320039040000bdb4d3cda4c520202020202020202020 00 // 醬油XO
            // __ 6f3a433000cdbf040002010200 // 光環

            byte[] pkts = new byte[0];
            string pre = "6980"
                + Lib.toHex(npc.entityID)
                + Lib.toHex(npc.locX)
                + Lib.toHex(npc.locY)
                + Lib.toHex(npc.facing) // $1
                + "82" // $2
                + Lib.toHex(npc.animationTime) // $3
                + Lib.toHex(npc.modelID) // $4
                + Lib.toHex((byte)(npc.hp / npc.maxHP * 0x32)) // $5
                + ((npc is MobNPC) ? Lib.toHex(npc.elementToProduce) : "00") // $6
                + Lib.toHex(npc.param7) // $7
                + Lib.toHex(npc.param8) // $8
                ;

            if (npc is INamedNPC)
                pre += Lib.gbkToBytes((npc as INamedNPC).name);
            else if (npc is IIDNamedNPC)
            {
                pre += (
                    "00"
                    + "91b00a00" // or 66420f00 // TODO: analysis
                    + "64"
                    + Lib.toHex((npc as IIDNamedNPC).nameID)
                );
                if (npc is MobNPC)
                    pre += "64" + Lib.toHex((npc as MobNPC).level);
            }

            concatPacket(Lib.hexToBytes(
                //$"698096690000a2007800028001ee1300000000000000000066420f0064{Lib.toHex(npcID)}00"
                pre
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] genItemDesc(Item item)
        {
            // idx: 31??
            byte[] pkts = new byte[0];
            List<byte[]> descList;

            if (item is WeaponItem)
                descList = ((WeaponItem)item).toDesc();
            else if (item is EquippableItem)
                descList = ((EquippableItem)item).toDesc();
            else
                descList = item.toDesc();

            foreach (var pkt in descList)
            {
                var arr = pkt.Prepend((byte)0x31).ToArray();
                concatPacket(
                    arr
                , ref pkts);
            }
            return pkts;
        }

        public static byte[] spawnHero(Hero p, bool newlyAppear)
        {
            // working:
            //                X--- Y--- facing
            // 69 0b dc7b0b00 f200 6000 02                                      body_color
            // readyState (0100: joining (green named), >0200: ready (blue named))
            //      icon               hair color                  hat       body      wing      mask      tail      wp-- wpCode--
            // 0100 0100 1127 00000000 0100 e780 00000000 00000000 0800 0000 1800 0000 0000 0000 0000 0000 0000 0000 3d01 00000000 0000 321a5c0c 0001 0200 09370000 d0fd312020202020202020202020202000
            // 69 0b 47ac0400 90 00 5A 00 01

            // 69 0b dc7b0b00 f2 00 60 00 02
            // 0100 0100 1127 00000000 0100 e780 00000000 00000000 0800 0000 1800 0000 0000 0000 0000 0000 0000 0000 3d01 00000000 0000 321a5c0c 0001 0200 09370000 d0fd312020202020202020202020202000
            // 69 0a b6a60600 94 00 5e 00 04
            // 0201 0100 1127 00000000 0100 e780 07002cd3 07002cd3 e903 9b42 0b00 0000 0000 0000 0800 0000 0100 0000 3d08 00000000 0000 0e00f70b 0000 c0cfcde6bcd23920202020202020202000
            // 0201 0400 1127 00000000 0800 0c63 26000000 26000000 0c00 0000 2800 1552 0000 0000 0400 0000 0700 0000 3e0f 00001ff8 0505 1900e43f 0000 a4b8bbd2ccabc0c7d8bc20202020202000
            // 69 0b e3060000 E3 00 6F 00 01
            // 0100 0100 1127 00000000 0100 e780 00000000 00000000 0800 0000 1800 0000 0000 0000 0000 0000 0000 0000 3d01 00000000 0000 311a5c0c 0001 0200 e3060000 bcd3d0fd30352020202020202020202000

            // ref: pkt_model_00.jpg
            // 69 0a dedf0d00 ee 00 6e 00 02
            // 0201 0100 1127 00000000 0200 a4aa 00000000 00000000 1400 0000 1600 0000 0000 0000 0000 0000 0200 0000 3e08 0000df7b 0000 30009d36 0000 c0adcbb9c0d7b6c8202020202020202000

            // ref: pkt_model_02.jpg [wp: 6p 10*]
            // 69 0a d2bf0d00 ef 00 84 00 03
            // 0201 0400 1127 00000000 0800 c982 24000000 24000000 e903 9b42 2800 c9a1 0000 0000 0c00 0000 0700 0000 3d0c 0000bffc 0303 2f00fb07 0000 a6af6fcdedb7e76fa6af20202020202000

            // ref: pkt_model_01.jpg [girl mask: 0x0002] [wp: 6p 10*] [body: color No.1 (0x1DE7)]
            // 69 0a 94d10d00 e7 00 86 00 02
            // 0201 ea03 f92a 00000000 0a00 c982 0304089a 0304c84b 1d04 6a46 1500 e7d1 0000 0000 0700 0000 0700 0000 3e0a 00001ff8 0303 3100b96f 0000 d1ccd3eac4dec9d1202020202020202000

            // ref: pkt_model_03.jpg [boy]
            // 69 0a 6ed70d00 a7 00 63 00 08
            // 0201 0100 1127 00000000 0800 0c63 00000000 00000000 4000 0000 2800 3232 0000 0000 0000 0000 0700 0000 4007 0000bffc 0303 2e00bc71 0000 bfaad0c4b6b9d8bc202020202020202000

            // ref: pkt_model_04.jpg [boy] [sit]
            // 69 0a 60010e00 b0 00 59 00 04
            // 0204 0100 1127 00000000 0800 0c63 24000000 24000000 e903 9b42 2800 e7d1 0000 0000 0600 0000 1400 0000 3e07 0000bffc 0505 3200da29 0000 c0b6ccec20202020202020202020202000

            // ref: pkt_model_06.jpg [girl]
            // 69 0a dac30d00 d2 00 57 00 01
            // 0204 e903 f92a 00000000 0b00 895a ea030000 ea030000 1d04 6a46 ef03 1552 0000 0000 1100 0000 0700 0000 3e0c 0000bffc 0404 2f009a3a 0000 a6f3a6f3c2b7a4c5c2b720202020202000

            // ref: pkt_model_05.jpg [boy] [body: color No.10 (0xe269)]
            // 69 0a e1ca0d00 d5 00 59 00 03
            // 0201 0100 1127 00000000 0100 e780 24000000 24000000 e903 9b42 0700 e269 0000 0000 0400 0000 0700 0000 3e0c 00001ff8 0303 32003466 0000 c1e8ece120202020202020202020202000

            // ref: pkt_model_07.jpg [boy] [body: color No.6 (0e548x) [sit]
            // 69 0a 07e30d00 ec 00 93 00 08
            // 0204 0300 1127 00000000 0200 e780 00000000 00000000 e903 9b42 1500 e548 0000 0000 0000 0000 0700 0000 3e0f 00001ff8 0404 3200bd31 0000 c4eac9d9b2bbcdf7202020202020202000

            byte[] pkts = new byte[0];
            concatPacket(Lib.hexToBytes(
                "69"
                + (newlyAppear ? /*withAni*/"0b" : /*showOnly*/"0a")
                + Lib.toHex(p.heroID)
                + Lib.toHex(p.locX)
                + Lib.toHex(p.locY)
                + Lib.toHex(p.facing)
                + Lib.toHex(p.state)
                + Lib.toHex(p.act)
                + Lib.toHex(p.hair.icon)
                + Lib.toHex((short)p.gender)
                + "00000000" // unk
                + Lib.toHex(p.hair.model)
                + Lib.toHex(p.hair.color)
                + "00000000" // ?
                + "00000000" // ?
                + p.model.toModelHex()
                //wp   wpColor-      HP ?? ?? ??
                //3d01 00000000 0000 32 1a 5c 0c 0001 0200 <uid > <name> [0b]
                //3e0c 0000bffc 0404 2f 00 9a 3a 0000 <name>        [0a]
                + "0000"
                + Lib.toHex((byte)(p.hp / p.maxHP * 0x32))
                + "1a5c0c"
                + ((newlyAppear) ? "00010200" + Lib.toHex(p.heroID) : "0000")
                + Lib.padWithString(Lib.toHex(Lib.gbkToBytes(p.heroName)), "20", 16 * 2)
                + "00"
            ), ref pkts);

            //if (p.selectedTitleIndex != 0)
            //    concatPacket(PacketEncoder.rideOn(p), ref pkts, false);

            if (p.ride != null)
                concatPacket(PacketEncoder.rideOn(p), ref pkts, false);

            concatPacket(PacketEncoder.playerEffectList(p), ref pkts, false);

            concatPacket(PacketEncoder.entityBuffList(p), ref pkts, false);
            return pkts;
        }

        public static byte[] playerEffectList(Hero p)
        {
            // idx:6f??
            byte[] pkts = new byte[0];
            p.effects.ForEach(effect =>
            {
                concatPacket(Lib.hexToBytes(
                    "6f"
                    + Lib.toHex(p.heroID)
                    + effect.toHex()
                    + "00"), ref pkts);
            });
            return pkts;
        }

        public static byte[] selfBuffList(Hero p)
        {
            // idx:81??
            byte[] pkts = new byte[0];
            p.Buffs.Values.ToList().ForEach(buff =>
            {
                // add buff to entity (target heroID)
                //       heroID-- skillID-
                // __ 83 e5661700 d83d0800 0100 00

                // add buff to the received client (self)
                //       skillID- 
                // __ 81 743d0800 0807 01 00 // 0x708 (30mins buff)
                concatPacket(Lib.hexToBytes(
                    "81"
                    + Lib.toHex(buff.buffID)
                    + Lib.toHex(buff.duration)
                    + "00"
                    + "00"), ref pkts);
            });
            return pkts;
        }

        public static byte[] entityBuffList(ILivingEntity e)
        {
            // idx:83??
            byte[] pkts = new byte[0];
            e.Buffs.Values.ToList().ForEach(buff =>
            {
                // add buff to entity
                //       entityID skillID-
                // __ 83 e5661700 d83d0800 0100 00
                concatPacket(Lib.hexToBytes(
                    "83"
                    + Lib.toHex(((IEntity)e).entityID)
                    + Lib.toHex(buff.buffID)
                    + Lib.toHex(buff.duration)
                    + "00"
                    + "00"), ref pkts);
            });
            return pkts;
        }

        public static byte[] despawnEntity(Hero p)
        {
            byte[] pkts = new byte[0];
            // 06 78 34271600 00
            concatPacket(Lib.hexToBytes(
                "06" // sz
                + "78"
                + Lib.toHex(p.heroID)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroCurrentHealth(Hero p)
        {
            // idx:3d14
            byte[] pkts = new byte[0];
            // __ 3d14 5a010000 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3d14"
                + Lib.toHex(p.hp)
                + "00"), ref pkts, false);
            return pkts;
        }
        
        public static byte[] setHeroMaxHealth(Hero p)
        {
            // idx:3d15
            byte[] pkts = new byte[0];
            // __ 3d15 5a010000 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3d15"
                + Lib.toHex(p.maxHP)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroCurrentMP(Hero p)
        {
            // idx:3d16
            byte[] pkts = new byte[0];
            // __ 3d16 5a010000 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3d16"
                + Lib.toHex(p.mp)
                + "00"), ref pkts, false);
            return pkts;
        }
        
        public static byte[] setHeroMaxMP(Hero p)
        {
            // idx:3d17
            byte[] pkts = new byte[0];
            // __ 3d17 5a010000 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3d17"
                + Lib.toHex(p.maxMP)
                + "00"), ref pkts, false);
            return pkts;
        }
        
        public static byte[] setHeroMeleeAttack(Hero p)
        {
            // idx:3d18
            byte[] pkts = new byte[0];
            // __ 3d 18 11000000 34000000 00 // 17 | 52 | set Melee Attack  (min 17, max 52)
            concatPacket(Lib.hexToBytes(
                "0b" // sz
                + "3d18"
                + Lib.toHex(p.meleeDamage - 35 <= 10 ? 10 : p.meleeDamage - 35)
                + Lib.toHex(p.meleeDamage)
                + "00"), ref pkts, false);
            return pkts;
        }
        
        public static byte[] setHeroMeleeDefense(Hero p)
        {
            // idx:3d19
            byte[] pkts = new byte[0];
            // __ 3d 19 11000000 34000000 00 // 17 | 52 | set Melee Defense (min 17, max 52)
            concatPacket(Lib.hexToBytes(
                "0b" // sz
                + "3d19"
                + Lib.toHex(p.meleeDefense - 35 <= 10 ? 10 : p.meleeDefense - 35)
                + Lib.toHex(p.meleeDefense)
                + "00"), ref pkts, false);
            return pkts;
        }
        
        public static byte[] setHeroMagicAttack(Hero p)
        {
            // idx:3d1a | TODO: confirm pkt
            byte[] pkts = new byte[0];
            // __ 3d 1a 11000000 34000000 00 // 17 | 52 | set Magic Attack  (min 17, max 52)
            concatPacket(Lib.hexToBytes(
                "0b" // sz
                + "3d1a"
                + Lib.toHex(p.magicDamage - 35 <= 10 ? 10 : p.magicDamage - 35)
                + Lib.toHex(p.magicDamage)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroMagicDefense(Hero p)
        {
            // idx:3d1b | TODO: confirm pkt
            byte[] pkts = new byte[0];
            // __ 3d 1b 11000000 34000000 00 // 17 | 52 | set Magic Defense (min 17, max 52)
            concatPacket(Lib.hexToBytes(
                "0b" // sz
                + "3d1b"
                + Lib.toHex(p.magicDefense - 35 <= 10 ? 10 : p.magicDefense - 35)
                + Lib.toHex(p.magicDefense)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroCriticalHitRate(Hero p)
        {
            // idx:3d1d
            byte[] pkts = new byte[0];
            // __ 3d 1d 64000000 00 // set critical hit rate to 1.00%
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3d1d"
                + Lib.toHex(p.criticalHitRate)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroDodge(Hero p)
        {
            // idx:3df6
            byte[] pkts = new byte[0];
            // __ 3d f6 04000000 00 // set dodge to 4
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3df6"
                + Lib.toHex(p.dodge)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroHit(Hero p)
        {
            // idx:3df7
            byte[] pkts = new byte[0];
            // __ 3d f7 04000000 00 // set hit to 4
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "3df7"
                + Lib.toHex(p.hit)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setEntityHealthBar(ILivingEntity e)
        {
            // idx:2a??
            byte hpBar = (byte)(e.hp / e.maxHP * 0x32);
            
            byte[] pkts = new byte[0];
            // __ 2a 25842100 30 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "2a"
                + Lib.toHex(((IEntity)e).entityID)
                + Lib.toHex(hpBar)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] setHeroExp(Hero p)
        {
            // idx:3d28
            // the client will self-determine the EXP is gain or lose based old the updated value.
            byte[] pkts = new byte[0];
            //         ____ ???????? newExp--
            // __ 3d28 0000 25000000 5f010000 00 // set player EXP, 5f010000: 351
            // __ 3d28 0000 6dd80000 f2070000 00
            concatPacket(Lib.hexToBytes(
                "0d" // sz
                + "3d28"
                + "0000" // unk
                + "12345678" // unk (25000000)
                + Lib.toHex(p.exp)
                + "00"), ref pkts, false);
            return pkts;
        }

        public static byte[] heroLevelUpAnimate(Hero p)
        {
            // idx:3d28
            byte[] pkts = new byte[0];
            //         newLv newLvExp
            // __ 3d2c 0300  c3000000 00 // lv up head-msg-animate
            concatPacket(Lib.hexToBytes(
                "09"
                + "3d2c"
                + Lib.toHex(p.lv)
                + Lib.toHex(Lib.lvUpExp[p.lv])
                + "00"), ref pkts, false);
            return pkts;
        }

        /// <summary>
        /// Update player model for both Acting and Facing
        /// </summary>
        /// <param name="p">Hero</param>
        /// <returns>Packets based on Hero(p) </returns>
        public static byte[] updatePlayerState(Hero p)
        {
            // idx:5e??
            byte[] pkts = new byte[0];
            // 07 5e 34271600 02 00 00
            concatPacket(Lib.hexToBytes(
                "07" // sz
                + "5e"
                + Lib.toHex(p.heroID)
                + Lib.toHex(p.act)
                + Lib.toHex(p.facing)
                + "00"
            ), ref pkts, false);
            return pkts;
        }

        public static byte[] playerAct(Hero p)
        {
            // TODO: confirm pkt | wtf is this?
            // idx:40??
            byte[] pkts = new byte[0];
            // act: 01~06: walk animate
            //      08
            //       heroID-- rnd----- act
            // __ 40 <uid>
            byte[] rnd = new byte[4];
            new Random().NextBytes(rnd);
            concatPacket(Lib.hexToBytes(
                "40"
                + Lib.toHex(p.heroID)
                + Lib.toHex(rnd)
                + Lib.toHex(p.act)
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] playerUpdateState(Hero p)
        {
            // TODO: confirm pkt
            byte[] pkts = new byte[0];
            //             heroID-- act __
            // __ b9 a2 01 f9d41800 04  00
            // 09_b9_a2_01 11270000 0b  00
            concatPacket(Lib.hexToBytes(
                "b9a201"
                + Lib.toHex(p.heroID)
                + Lib.toHex(p.act)
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] playerPathSync(Hero p, string rnd, string path)
        {
            // idx:a917
            byte[] pkts = new byte[0];
            // src: path 26903 95,157 210 1 0
            //          time---- heroID--
            // 1d a9 17 f68f0000 34271600 29000000 39352c313537203231302031203000
            // 1d a9 17 42900000 34271600 2a000000 39362c313538203231302031203000
            concatPacket(Lib.hexToBytes(
                "a917"
                + Lib.toHex(int.Parse(rnd))
                + Lib.toHex(p.heroID)
                + "29000000"
                + Lib.toHex(Lib.gbkToBytes(path))
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] playerLocSync(Hero p, string time)
        {
            // idx:67??
            byte[] pkts = new byte[0];
            // facing: use ascii to describe that facing. ie: 8 -> 34
            // sz    heroID-- ???? ???? X--- Y--- f- --
            // 0f 67 34271600 6c39 9b12 6000 9e00 34 00
            // 0f 67 d8221600 be35 9d12 6200 9a00 32 00
            concatPacket(Lib.hexToBytes(
                "67"
                + Lib.toHex(p.heroID)
                + Lib.toHex(int.Parse(time))
                + Lib.toHex(p.locX)
                + Lib.toHex(p.locY)
                + Lib.toHex(Lib.gbkToBytes(p.facing.ToString()))
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] playerHPUpdate(int newHP)
        {
            return Lib.hexToBytes($"073d14{Lib.toHex(newHP)}00");
        }

        public static byte[] playerMPUpdate(int newMP)
        {
            return Lib.hexToBytes($"073d16{Lib.toHex(newMP)}00");
        }

        #region player join server
        public static byte[] playerJoin(Hero p)
        {
            byte[] pkts = new byte[0];
            concatPacket(Lib.hexToBytes(
                "490e0000000000"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "3d2fcee4cabf00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "490200"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "5a010000"
            ), ref pkts);

            lastLoginRecord(p.heroID, ref pkts);
            getGameNotice(ref pkts);

            // @ Server.cs : JoinGame-Mark1
            concatPacket(Lib.hexToBytes(
                "3d2fcee4cabf00"
            ), ref pkts);
            concatPacket(Lib.hexToBytes(
                "3d910000"
            ), ref pkts);

            // @ Server.cs : JoinGame-Mark2 - BackPack
            if (p.bag != null)
                concatPacket(Lib.hexToBytes(
                    p.bag.fullBagToHex()
                ), ref pkts, false);

            // @ Server.cs : JoinGame-Mark3
            concatPacket(Lib.hexToBytes(
                "490a00000000"
            ), ref pkts);

            // @ Server.cs - Player Location
            // idx:3c1127
            concatPacket(Lib.hexToBytes(
                "3c1127"
                + Lib.toHex(p.map)
                + Lib.toHex(p.map)
                + Lib.toHex(p.locX)
                + Lib.toHex(p.locY)
                + "00"
                + Lib.toHex(p.map)
                + "0000"
                + "00" // null-term
            ), ref pkts);

            // spawn this player
            concatPacket(spawnHero(p, true), ref pkts, false);

            concatPacket(Lib.hexToBytes(
                "7f016a"
                + Lib.toHex(p.heroID)
                + "a500a500"
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "7f016b"
                + Lib.toHex(p.heroID)
                + "54005400"
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "7f016c"
                + Lib.toHex(p.heroID)
                + "54005400"
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "7f0170"
                + Lib.toHex(p.heroID)
                + "3b003c00"
                + "00"
            ), ref pkts);

            // start from window title to itembar
            getHeroFullInfo(p, ref pkts);

            //// @ Server.cs - JoinGame-MarkZ
            //concatPacket(Lib.hexToBytes(
            //    "81245c"
            //    + "0c000000"
            //    + "01"
            //    + "00" // null-term
            //), ref pkts);

            // player ready
            // idx:3d01
            concatPacket(Lib.hexToBytes(
                "3d01"
                + Lib.toHex(p.heroID)
                + "02"
                + "00" // null-term
            ), ref pkts);

            return pkts;
        }

        private static void lastLoginRecord(int playerUID, ref byte[] pkts)
        {
            // TODO: query database

            // login record
            // 3e 01 250f00007313000000323031392d31322d31322031303a30353a3237730e000000 3132342e3234342e31342e313137 00
            concatPacket(Lib.hexToBytes(
                "3e01"
                + "25" // 25: last 26: current
                + "0f00007313000000"
                + "323031392d31322d31322031303a30353a3237" // date time [2019-12-12 10:05:27]
                + "730e000000"
                + "3131312e3131312e3131312e313131" // IP
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "3e01"
                + "26" // 25: last 26: current
                + "0f00007313000000"
                + "323031392d31322d31322031303a30353a3238" // date time [2019-12-12 10:05:28]
                + "730e000000"
                + "3131312e3131312e3131312e313131" // IP
                + "00"
            ), ref pkts);
        }

        private static void getGameNotice(ref byte[] pkts)
        {
            //concatPacket(Lib.hexToBytes(
            //    "ffff0d02a20f010d0ad3c9c0cfcde6b6f9d3cecfb7b6c0bcd2b4fac0eda1a2b9e3d6ddcdf8d3cecafdc2ebd1d0b7a2b5c44d4d4f525047beadb5e451c3c8cdf8d3cea1b6ccecd3f0b4abc6e6a1b7cfd6d2d1d5fdcabdbfaac6f4b2bbc9beb5b5cad7b7fea1b0c4b0c9cfd3f0bfaaa1b1a3a10d0ab6e0d1f9b5c4d5bdb6b7cfb5cdb3a1a2beabd6c251c3c8b5c4bbadc3e6a1a2c5e4d2d4d4add6add4adceb6b5c4beadb5e4cde6b7a8a3acd4e7d2d1c8c3b8f7cebbd0c2c0cfcde6bcd2cde6bcd2c7cccad7d2d4c5cea3a10d0ac0cfcde6b6f9d3cecfb7bdabbcccd0f8b1fcb3d0a1b0b4b4d4ecbfecc0d6a3accfedcadcc9fabbeea1b1b5c4d7dad6bca3acceaad3cecfb7cce1b9a9cec8b6a8b5c4d4cbd3aad6a7b3d6a3acceaad3c3bba7cce1b9a9d3c5d6cab5c4d3cecfb7b7fecef1a3acb9b9bda8cec8b6a8a1a2b0b2c8aba1a2b9abc6bdb5c4d3cecfb7bbb7beb3a1a30d0abbb6d3adb4f3bcd2d4dacce5d1e9b9fdb3ccd6d0cce1b3f6b8f7d6d6b1a6b9f3b5c4d2e2bcfbbacdbda8d2e9a3acd2b2cfa3cdfbb8f7cebbcde6bcd2c4dcd2d4bfcdb9dbb0fcc8ddb5c4ccacb6c8b6d4b4fdb2e2cad4b9fdb3ccd6d0b3f6cfd6b5c4cecacce2a3acd7a3c4fad3cecfb7d3e4bfeca1a30d0aced2c3c7d2d1beadbfaacda8c1cba1b6ccecd3f0b4abc6e6a1b7bfcdb7fecda8b5c0a3accbe6cab1ceaab9e3b4f3cde6bcd2cce1b9a9b0efd6fabacdb7fecef1a1a300"
            //), ref pkts, false);
        }

        private static void getHeroFullInfo(Hero p, ref byte[] pkts)
        {
            // window title - player id
            concatPacket(Lib.hexToBytes(
                "3da0"
                + Lib.toHex(p.heroID)
                + Lib.toHex(Lib.gbkToBytes(p.heroName))
                + "00"
            ), ref pkts);

            // title list
            //__ a2 0100 74270000 73040000 00423030327304000000c0b4d0c5 00
            //__ a2 0100 7b270000 73040000 004430303364 2b270000 00
            //__ a2 0100 7b270000 73040000 004730303564 42270000 00
            //__ a2 0100 7b270000 73040000 004430303164 29270000 00
            //__ a2 0100 75270000 73040000 0042303031730e000000602b433078323766353263602d43 64 11270000 00 // GBK colorCode & titleID
            //__ a2 0100 77270000 73040000 00433030367306000000 天青色 64 28270000 00
            //__ a2 0100 75270000 73040000 0056303032730e000000602b433078666666663030602d43 64 69270000 00
            //__ a2 0100 76270000 73040000 00433030337306000000 天青色 64 fe1100006425270000 00
            //concatPacket(Lib.hexToBytes(
            //    "a214" // currently using's user title
            //    + Lib.toHex(p.uid)
            //    + "1b43b7e7bba8d1a9d4c2" // title (GBK)
            //    + "00"
            //), ref pkts);
            //concatPacket(Lib.hexToBytes(
            //    "b90201" // avalible title
            //    + Lib.toHex(p.uid)
            //    + "1b43b7e7bba8d1a9d4c2" // title
            //    + "00"
            //), ref pkts);

            // gifts
            // idx:3da2
            concatPacket(Lib.hexToBytes(
                "3da2"
                + Lib.toHex(p.gifts[0])
                + Lib.toHex(p.gifts[1])
                + Lib.toHex(p.gifts[2])
                + Lib.toHex(p.gifts[3])
                + Lib.toHex(p.gifts[4])
                + "00"
            ), ref pkts);

            // current status
            // idx:3da3
            concatPacket(Lib.hexToBytes(
                "3da3"
                + Lib.toHex(p.hp           )
                + Lib.toHex(p.maxHP        )
                + Lib.toHex(p.mp           )
                + Lib.toHex(p.maxMP        )
                + Lib.toHex(p.meleeDamage  - 35 <= 10 ? 10 : p.meleeDamage  - 35)
                + Lib.toHex(p.meleeDamage)
                + Lib.toHex(p.meleeDefense - 35 <= 10 ? 10 : p.meleeDefense - 35)
                + Lib.toHex(p.meleeDefense)
                + Lib.toHex(p.magicDamage  - 35 <= 10 ? 10 : p.magicDamage  - 35)
                + Lib.toHex(p.magicDamage )
                + Lib.toHex(p.magicDefense - 35 <= 10 ? 10 : p.magicDefense - 35)
                + Lib.toHex(p.magicDefense) 
                + "000000000000" // unk
                + "00"
            ), ref pkts);

            // some amounts
            // idx:3da4
            concatPacket(Lib.hexToBytes(
                "3da4"
                + "0000" // unk
                + "fd93" // unk
                + "0c00" // unk
                + "0000" // unk
                + "0000" // unk
                + Lib.toHex(p.gd       )
                + Lib.toHex(p.giftPoint)
                + Lib.toHex(p.lv       ) + "00" // part of level
                + Lib.toHex(p.exp)
                + Lib.toHex(Lib.lvUpExp[p.lv])
                + Lib.toHex(p.silver)
                + "00000000" // unk
                + "00"
            ), ref pkts);

            // some more amounts
            // 3da500000000000000000000F1F2F3F4F5F6F7F80000000000000000000000000000/*體力-2B*/181900000000000000143da6000000000000000101010101010101000000
            // idx:3da5
            concatPacket(Lib.hexToBytes(
                "3da5"
                + "0000000000000000000000000000000000000000000000000000000000000000" // unk
                + Lib.toHex(p.tl)
                + "00000000000000143da60000000000000001010101010101010000" // unk
                + "00"
            ), ref pkts);

            // 爆擊
            // idx:3d1d
            concatPacket(Lib.hexToBytes(
                "3d1d"
                + Lib.toHex(p.criticalHitRate)
                + "00"
            ), ref pkts);

            // backpack
            // idx:3d48
            // The followings are in decimal:
            // 01 -> expired 6
            // 02 -> expired 12
            // 03 -> expired 24
            // 06 -> valid-- 6
            // 12 -> valid-- 12
            // 24 -> valid-- 24
            // 25 -> forever 24
            // [__][__][__][__]
            //  $4  $3  $2  $1
            concatPacket(Lib.hexToBytes(
                "3d48"
                + Lib.toHex(uint.Parse(
                    // decimal:
                    p.bag.extraBag[2].ToString()   // bag 4 - GUI right most
                    + p.bag.extraBag[1].ToString() // bag 3
                    + p.bag.extraBag[0].ToString() // bag 2
                    + "24" // bag 1 - default (GUI left most)
                ))
                + "00"
            ), ref pkts);

            /* missing :
                0x07, 0x3d, 0x48, 0xFF, 0xFE, 0x00, 0x00, 0x00, // unk (maybe luck?)
                0x05, 0x3d, 0x7a, 0xFA, 0xFB, 0x00,
                0x04, 0x3d, 0x80, 0x00, 0x00,
                0x04, 0x3d, 0x81, 0x00, 0x00,
                0x04, 0x3d, 0x85, 0xFC, 0x00, // ?
                0x05, 0x3d, 0x8f, 0xFC, 0xFD, 0x00,
                0x05, 0x3d, 0x90, 0xFE, 0xFF, 0x00,
             */

            // 回避
            // idx:3df6
            concatPacket(Lib.hexToBytes(
                "3df6"
                + Lib.toHex(p.dodge)
                + "00"
            ), ref pkts);

            // 命中
            // idx:3df7
            concatPacket(Lib.hexToBytes(
                "3df7"
                + Lib.toHex(p.hit)
                + "00"
            ), ref pkts);

            // 戰績
            // idx:a508
            concatPacket(Lib.hexToBytes(
                "a508"
                + Lib.toHex(p.zj)
                + "00"
            ), ref pkts);

            // 修為
            // idx:a20a
            concatPacket(Lib.hexToBytes(
                "a20a"
                + Lib.toHex(p.xwLv)
                + "00"
            ), ref pkts);

            /* missing:
                0x06, 0x74, 0x5a, 0x31, 0x69, 0x12, 0x00,
                0x07, 0xa2, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00,
             */
            // test, is PK?
            //concatPacket(Lib.hexToBytes(
            //    "a211FFFFFF7F"
            //    + "00"
            //), ref pkts);

            // 元寶 (gold)
            // idx:4507
            concatPacket(Lib.hexToBytes(
                "4507"
                + Lib.toHex(p.gold)
                + "00"
            ), ref pkts);

            // missing: 0x07, 0xad, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00,

            // skill tree
            // TODO: build skill tree
            // __ 55 858b080005000000000000858b0800 00 // 攻擊提升 (部落技)
            // __ 55 be93080003000000000000be930800 00 // 偵查術

            // item bar
            // TODO: build item bar
        }
        #endregion

        #region Ride
        public static byte[] rideOn(Hero p)
        {
            byte[] pkts = new byte[0];

            if (p.ride == null)
                return null;

            // 2. update player state (act)
            concatPacket(playerUpdateState(p), ref pkts, false);

            // 3. spawn ride model
            concatPacket(Lib.hexToBytes(
                "62"
                + Lib.toHex(p.heroID)
                + p.ride.toHex()
                + "00"
            ), ref pkts);
            return pkts;
        }

        public static byte[] rideItem(Hero p, byte rideIndex)
        {
            // idx:b3??
            var pkts = new byte[0];

            if (rideIndex > p.rideList.Count)
                return pkts;

            var ride = p.rideList[rideIndex - 1];

            concatPacket(Lib.hexToBytes(
                "b300"
                + Lib.toHex(rideIndex) + "000000"
                + Lib.toHex(ride.baseItemID)
                + "00"
            ), ref pkts);

            // currently activated
            if (p.ride?.descItemID == ride.descItemID)
                concatPacket(Lib.hexToBytes(
                    "b302"
                    + Lib.toHex(rideIndex) + "000000"
                    + "01000000"
                    + "00"
                ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "b304"
                + Lib.toHex(rideIndex) + "000000"
                + "00"
                + Lib.toHex(ride.descItemID)
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "b301"
                + Lib.toHex(rideIndex) + "000000"
                + Lib.toHex(ride.generation) + "000000"
                + Lib.toHex(ride.lv) + "000000"
                + Lib.toHex(ride.stat[1]) + "0000"
                + Lib.toHex(ride.stat[2]) + "0000"
                + Lib.toHex(ride.stat[3]) + "0000"
                + Lib.toHex(ride.stat[4]) + "0000"
                + Lib.toHex(ride.stat[5]) + "0000"
                + Lib.toHex(ride.stat[0]) + "0000" // usable stat point
                + "00000000" // unk
                + "00000000" // unk
                + Lib.toHex(ride.exp)
                + Lib.toHex(Lib.rideLvUpExp[ride.lv])
                //+ Lib.toHex(ride.modelID) + "0000"
                //+ Lib.toHex(ride.modelColor) + "0000" // test
                //+ "00000000" // unk
                //+ "00000000" // unk
                + ride.toHex()
                + Lib.toHex(ride.baseItemID)
                + Lib.toHex(ride.shareStat) + "0000"
                + "00"
            ), ref pkts);

            concatPacket(Lib.hexToBytes(
                "b304"
                + Lib.toHex(rideIndex) + "000000"
                + "00"
                + Lib.toHex(ride.descItemID)
                + "00"
            ), ref pkts);

            if (!ride.speak.Equals(string.Empty))
                concatPacket(Lib.hexToBytes(
                    "b305"
                    + Lib.toHex(rideIndex) + "000000"
                    + Lib.toHex(Lib.gbkToBytes(ride.speak))
                    + "00"
                ), ref pkts);

            foreach (RideSkill skill in ride.cubSkills)
                concatPacket(Lib.hexToBytes(
                    "b314"
                    + Lib.toHex(rideIndex) + "000000"
                    + Lib.toHex(skill.skillID)
                    + "0000"
                    + "64"
                    + Lib.toHex(skill.skillID)
                    + "00"
                ), ref pkts);

            foreach (RideSkill skill in ride.cubSkills)
                concatPacket(Lib.hexToBytes(
                    "b314"
                    + Lib.toHex(rideIndex) + "000000"
                    + Lib.toHex(skill.skillID)
                    + "0100"
                    + "64"
                    + Lib.toHex(skill.skillID)
                    + "00"
                ), ref pkts);

            foreach (RideSkill skill in ride.cubSkills)
                concatPacket(Lib.hexToBytes(
                    "b314"
                    + Lib.toHex(rideIndex) + "000000"
                    + Lib.toHex(skill.skillID)
                    + "0200"
                    + "64"
                    + Lib.toHex(skill.skillID)
                    + "00"
                ), ref pkts);
            return pkts;
        }
        #endregion

        #region Broadcast / Notice / System / GM
        public static byte[] scrollingNoticeWithFormat(int msgID, params int[] argv)
        {
            // idx:a301
            // sep: 64
            // TypeA: 64 as sep
            //           msgID        __ param $1     __ param $2    __ param $3
            // __ a3_01_ 56_75_00_00_ 64_68_00_00_00_ 64_f0_00_00_00_64_9d_00_00_00_ 00_
            // TypeB: 7306 as sep
            // __ a3_01_ 64_75_00_00_ 73_06_00_00_00_ bb_b9_cb_c0_b9_ed_ 73_06_ 00_00_00_d1_e0_b3_e0_cf_bc_00_
            var pkts = new byte[0];
            var sb = new StringBuilder();

            foreach (int val in argv)
                sb.Append("64" + Lib.toHex(val));

            concatPacket(Lib.hexToBytes(
                "a301"
                + Lib.toHex(msgID)
                + sb.ToString()
                + "00"
            ), ref pkts);
            return pkts;
        }
        #endregion

        public static List<string> parse(byte[] packet)
        {
            // TODO: parser (i dont think client will send packets to me lo = =.., it uses commands only..)
            

            return new List<string>();
        }

        /// <summary>
        /// Add a size byte at the beinging of packet.
        /// </summary>
        /// <returns>Packet total size (newly added size byte included)</returns>
        public static void concatPacket(in byte[] src, ref byte[] concatTo, bool prependSize = true)
        {
            var sz = (byte)(src.Length & 0x000000FF); // read the lowest bits only.
            int szPkts = concatTo.Length;

            if (!prependSize)
                szPkts -= 1;

            Array.Resize(ref concatTo, szPkts + sz + 1);
            Buffer.BlockCopy(src, 0, concatTo, szPkts + 1, sz);

            if (prependSize)
                concatTo[szPkts] = sz;
        }
    }
}
