using Feather_Server.Entity;
using Feather_Server.Entity.NPC_Related;
using Feather_Server.Packets.PacketLibs;
using System;

namespace Feather_Server.MobRelated
{
    public class MobNPC : NPC
    {
        public uint nameID;
        public uint level = 0;

        protected MobNPC(uint entityID, uint modelID, FormatString fs = null) : base(entityID, modelID, fs)
        {
            if (fs == null)
                base.FormatString = new FormatString(EFormatString.TEMPLATE_ENTITY_NAME_ID_LV, nameID, level);
        }
    }
}
