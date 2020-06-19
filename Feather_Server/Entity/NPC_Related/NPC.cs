using Feather_Server.MobRelated;
using Feather_Server.Packets.PacketLibs;
using Feather_Server.PlayerRelated.Skills;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;

namespace Feather_Server.Entity.NPC_Related
{
    public class NPC : ILivingEntity, IEntity
    {
        #region Variables
        public uint modelID { get; protected set; }

        public FormatString FormatString { get; protected set; }

        public Packets.PacketStreamData NameFragment { get => this.FormatString.Fragment; }

        // unknown place holder
        /// <summary>
        /// Animation Frames Per Second (0x01=fast, 0xFF=slow)
        /// </summary>
        public byte animationTime = 0x01;
        public byte elementToProduce = 0x00;
        public byte cate = 0x80; // TODO: check for cate & make cate enum (0x80: Ally-blue, 0x81: golden, 0x82: white, 0x83: yellow, 0x84: Lv1 Elite Mob)
        public readonly ushort param7 = 0x0000;
        public readonly ushort param8 = 0x0000;

        public uint entityID { get; protected set; }

        public int HP { get; set; }
        public int maxHP { get; set; }

        public int MP { get; set; }
        public int maxMP { get; set; }
        public int PA { get; set; }
        public int PD { get; set; }

        public int MA { get; set; }
        public int MD { get; set; }

        public int hit { get; set; }
        public int dodge { get; set; }
        public int criticalHitRate { get; set; }

        public byte state { get; set; } = 0x02;
        public byte act { get; set; } = 0x01;

        public Dictionary<int, Buff> Buffs { get; private set; }
        public Dictionary<int, PlayerSkill> skillList { get; private set; }

        void ILivingEntity.addBuff(int buffID, ushort duration)
        {
            throw new NotImplementedException();
        }

        void ILivingEntity.removeBuff(int buffID)
        {
            throw new NotImplementedException();
        }
        #endregion

        public NPC(uint entityID, uint modelID, FormatString fs)
        {
            this.entityID = entityID;
            this.modelID = modelID;

            this.FormatString = fs;
        }

        #region Map / Location / Facing
        public ushort locX { get; protected set; }
        public ushort locY { get; protected set; }
        public ushort map { get; protected set; }
        public byte facing { get; protected set; }

        void IEntity.updateLoc(ushort x, ushort y)
        {
            locX = x;
            locY = y;
        }

        void IEntity.updateMap(ushort newMap)
        {
            // remove the record from old map
            List<IEntity> lst = Lib.entityListByMap.GetValueOrDefault(this.map, null);
            if (lst != null)
                lst.Remove(this);

            lst = Lib.entityListByMap.GetValueOrDefault(newMap, new List<IEntity>());

            // if is empty List (not exists in current dictionary)
            if (lst.Count == 0)
                Lib.entityListByMap[newMap] = lst;

            lst.Add(this);

            this.map = newMap;
        }

        void IEntity.updateFacing(byte facing)
        {
            this.facing = facing;
        }
        #endregion
    }
}
