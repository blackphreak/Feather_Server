using Feather_Server.Entity.NPC_Related;

namespace Feather_Server.MobRelated
{
    public class MobNPC : NPC, IIDNamedNPC
    {
        public int nameID;
        public int level;

        int IIDNamedNPC.nameID => nameID;

        protected MobNPC(int entityID, int modelID)
        {
            base.entityID = entityID;
            base.modelID = modelID;
        }
    }
}
