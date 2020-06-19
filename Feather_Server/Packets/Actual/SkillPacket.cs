using Feather_Server.Entity;
using Feather_Server.PlayerRelated.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets
{
    public static class SkillPacket
    {

        public static PacketStreamData addEntityBuff(ILivingEntity e, Buff buff)
        {
            return new PacketStream()
                /* JS_D: Desc[Entity Buffs] */
                .setDelimeter(Delimeters.ENTITY_BUFFS)
                /* JS: Desc[EntityID] */
                .writeDWord(e.entityID)
                /* JS: Desc[Buff ID] R[SKILL] */
                .writeDWord(buff.buffID)
                /* JS: Desc[Duration (sec)] */
                .writeWord(buff.duration)
                /* JS: Desc[Padding] */
                .writePadding(2)
                .pack();
        }
    }
}
