using Feather_Server.Entity;
using Feather_Server.Entity.NPC_Related;
using Feather_Server.PlayerRelated.Skills;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.MobRelated
{
    public class Mob_Duck : MobNPC
    {
        public Mob_Duck(int entityID) : base(entityID, 410011) // 0x6419B
        {

        }
    }
}
