using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class HeroPacket
    {
        public static PacketStreamData playerEffects(Hero p)
        {
            var stream = new PacketStream();
            /* JS_D: Desc[Login Success] */
            stream.setDelimeter(Delimeters.HERO_EFFECTS)
            /* JS: Desc[HeroID] */
            .writeDWord((uint)p.heroID);

            p.effects.ForEach(effect =>
            {
                /* JS_F: To[HeroEffect@Feather_Server/Entity/PlayerRelated/Model/Effect.cs] */
                effect.toFragment(ref stream);
            });

            return stream.pack();
        }

        public static PacketStreamData spawnPlayerNormal(Hero p)
        {
            var stream = new PacketStream();
            /* JS_D: Desc[Spawn Hero Without Animation] */
            stream.setDelimeter(Delimeters.HERO_SPAWN_NORMAL);

            /* JS_F: To[Hero_Infos@Feather_Server/Entity/PlayerRelated/Hero.cs] */
            p.toFragment_HeroInfos(ref stream);

            /* JS: Desc[Player Name] */
            stream.writeString(p.heroName.PadRight(16, ' '));

            return stream.pack();
        }

        public static PacketStreamData spawnPlayerAnimated(Hero p)
        {
            var stream = new PacketStream();
            /* JS_D: Desc[Spawn Hero With Animation] */
            stream.setDelimeter(Delimeters.HERO_SPAWN_ANIMATED);

            /* JS_F: To[Hero_Infos@Feather_Server/Entity/PlayerRelated/Hero.cs] */
            p.toFragment_HeroInfos(ref stream);

            // TODO: Confirm the animation ID
            // TODO: ID list & Fn @ Parser
            /* JS: Desc[Animation ID?] */
            stream.writeWord(0x0002);

            /* JS: Desc[HeroID (Shown on 'W')] */
            stream.writeDWord(p.heroID);

            /* JS: Desc[Player Name] */
            stream.writeString(p.heroName.PadRight(16, ' '));

            return stream.pack();
        }

        public static PacketStreamData setWindowTitle(Hero p)
        {
            return new PacketStream()
                /* JS_D: Desc[Set Window Title] */
                .setDelimeter(Delimeters.SELF_SET_WINDOW_TITLE)
                /* JS: Desc[HeroID] */
                .writeDWord(p.heroID)
                /* JS: Desc[Hero Name] */
                .writeString(p.heroName)
                .pack();
        }

        public static PacketStreamData mapInfo(Hero p)
        {
            return new PacketStream()
                /* JS_D: Desc[Map Info] */
                .setDelimeter(Delimeters.SELF_HERO_MAP_INFO)
                /* JS: Desc[SubCate Byte 27] */
                .writeByte(0x27)
                /* JS: Desc[Map ID] */
                .writeWord(p.map)
                /* JS: Desc[Map ID] */
                .writeWord(p.map)
                /* JS: Desc[Loc X] */
                .writeWord(p.locX)
                /* JS: Desc[Loc Y] */
                .writeWord(p.locY)
                /* JS: Desc[Map ID] */
                .writeWord(p.map)
                /* JS: Desc[Padding?] */
                .writePadding(2)
                .pack();
        }

        public static PacketStreamData locationSync(Hero p)
        {
            // sz    heroID-- ???? ???? X--- Y--- f- --
            // 0f 67 34271600 6c39 9b12 6000 9e00 34 00
            return new PacketStream()
                /* JS_D: Desc[Hero Location Sync] */
                .setDelimeter(Delimeters.HERO_LOCATION_SYNC)
                /* JS: Desc[entityID] */
                .writeDWord(p.heroID)
                /* JS: Desc[Time] */
                .writeDWord(Lib.timeGetTime())
                /* JS: Desc[LocX] */
                .writeWord(p.locX)
                /* JS: Desc[LocY] */
                .writeWord(p.locY)
                /* JS: Desc[Facing] Fn[eFacing] */
                .writeString(p.facing.ToString())
                .pack();
        }

        public static PacketStreamData setCH(Hero p)
        {
            return new PacketStream()
                .setDelimeter(Delimeters.SELF_HERO_CRIT_HIT_RATE)
                .writeDWord(0x64)
                .pack();
        }

        public static PacketStreamData setNumbersB(Hero p)
        {
            return new PacketStream()
                .setDelimeter(Delimeters.SELF_HERO_AMOUNT2)
                .writePadding(32)
                .writeWord(0x64)
                .writePadding(17)
                .pack();
        }
        
        public static PacketStreamData setNumbersA(Hero p)
        {
            return new PacketStream()
                .setDelimeter(Delimeters.SELF_HERO_AMOUNT)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeDWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeDWord(0x64)
                .writeDWord(0x640)
                .writeDWord(0x640)
                .writeWord(0x64)
                .writeWord(0x64)
                .pack();
        }
        
        public static PacketStreamData setAttributes(Hero p)
        {
            return new PacketStream()
                .setDelimeter(Delimeters.SELF_HERO_ATTRIBUTES)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeDWord(1000)
                .writeWord(1000)
                .writeWord(1000)
                .writeWord(1000)
                .pack();
        }
        
        public static PacketStreamData setGifts(Hero p)
        {
            return new PacketStream()
                .setDelimeter(Delimeters.SELF_HERO_GIFTS)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .writeWord(0x64)
                .pack();
        }
    }
}
