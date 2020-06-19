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
            stream.writeString(Lib.padWithString(Lib.toHex(Lib.gbkToBytes(p.heroName)), "20", 16 * 2));

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
            stream.writeString(Lib.padWithString(Lib.toHex(Lib.gbkToBytes(p.heroName)), "20", 16 * 2));

            return stream.pack();
        }
    }
}
