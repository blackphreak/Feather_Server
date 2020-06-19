using Feather_Server.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.PlayerRelated.Model
{
    public class Effect : IPacketStreamFragment
    {
        public byte effectID = 0x70;
        public byte animationDuration = 0x04;
        public byte layer = 0x02;
        public byte onceOrLoop = 0x02;

        public Effect(byte effectID, byte duration, byte layer, byte onceOrLoop)
        {
            this.effectID = effectID;
            this.animationDuration = duration;
            this.layer = layer;
            this.onceOrLoop = onceOrLoop;
        }

        public void toFragment(ref PacketStream stream)
        {
            /* JS_F: Here[HeroEffect] */
            stream
                /* JS: Desc[EffectID] Fn[eHeroEffect,@AniDur,@Layer,@OL] Mark[REPEAT_START] */
                .writeDWord(0x000CD100 + effectID)
                /* JS: Desc[Duration] Mark[Param,@AniDur] */
                .writeByte(animationDuration)
                /* JS: Desc[Layer] Mark[Param,@Layer] */
                .writeByte(layer)
                /* JS: Desc[Once Or Loop] Mark[Param,@OL] Mark[REPEAT_END] */
                .writeByte(onceOrLoop);
        }
    }
}
