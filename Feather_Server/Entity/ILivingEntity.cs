using Feather_Server.Entity;
using Feather_Server.PlayerRelated.Skills;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.MobRelated
{
    /// <summary>
    /// An interface that defines an entity should contains basic stats,
    /// hp, mp, damage, defense, etc.
    /// </summary>
    public interface ILivingEntity : IEntity
    {
        public int hp { get; set; }
        public int maxHP { get; set; }

        public int mp { get; set; }
        public int maxMP { get; set; }

        public int meleeDamage { get; set; }
        public int meleeDefense { get; set; }

        public int magicDamage { get; set; }
        public int magicDefense { get; set; }

        public int hit { get; set; }
        public int dodge { get; set; }
        public int criticalHitRate { get; set; }

        public byte state { get; set; }
        public byte act { get; set; }

        public Dictionary</*buffID*/int, Buff> Buffs { get; }

        public abstract void addBuff(int buffID, ushort duration);

        public abstract void removeBuff(int buffID);

        public Dictionary</*skillID*/int, PlayerSkill> skillList { get; }
    }
}
