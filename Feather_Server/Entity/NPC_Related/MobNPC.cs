using Feather_Server.Entity.NPC_Related;
using Feather_Server.Packets.PacketLibs;
using System;

namespace Feather_Server.MobRelated
{
    public class MobNPC : NPC
    {
        public int nameID;
        public int level;

        protected MobNPC(int entityID, int modelID)
        {
            base.entityID = entityID;
            base.modelID = modelID;
        }
    }
}
