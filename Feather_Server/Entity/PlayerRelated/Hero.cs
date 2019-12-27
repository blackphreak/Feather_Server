using Feather_Server.MobRelated;
using Feather_Server.PlayerRelated;
using Feather_Server.PlayerRelated.Model;
using Feather_Server.PlayerRelated.Skills;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Feather_Server.Entity;
using System;

namespace Feather_Server.ServerRelated
{
    public class Hero : HeroBasicInfo, ILivingEntity, IDamageable
    {
        public int bl;              // 部落 id (TODO: check size)
        public string[] titleList;  // 稱號列表 (with color code)
        public int selectedTitleIndex; // 名下稱號 (titleList[selectedTitleIndex])
        public byte tyLv;    // 天元等級 TODO: check size

        public List<Effect> effects; // 人物效果

        public ushort xwLv; // 修為等級 (2 bytes)

        // hidden stats
        public int critical_damage; // 爆擊傷害 (TODO: check existance & size)
        public int luck;            // 幸運值

        public List<Pet> petList;   // default max 4, can be expanded.
        public List<Ride> rideList;

        /// <summary>
        /// Currently ridding's ride
        /// </summary>
        [JsonIgnore]
        public Ride ride;

        public int gold;  // 元寶
        public int silver; // game $
        public int silverGold;  // 銀元

        /// <summary>
        /// 體力
        /// </summary>
        public ushort tl;
        /// <summary>
        /// 功德
        /// </summary>
        public int gd;
        /// <summary>
        /// 戰績
        /// </summary>
        public int zj;
        /// <summary>
        /// pk值
        /// </summary>
        public int pk;

        public EquipmentSet equips;

        public Bag bag;
        public Bag lifeMake; // 生活製作素材 -- fixed size: 4
        //public ItemBar[] itembar;

        public int[] friendList;
        public int[] blackList;
        public int teacher;

        //public byte facing { get; set; } = 0x03; // 0 to 8
        //[JsonIgnore]
        //public byte state = 0x01;  // player ready state. [1: not ready, 2: ready]
        //[JsonIgnore]
        //public byte act = 0x01;    // 0 to 11 (? | [1: stand, 2: running, 3: just die, 4: sit, 5: sitWithPet, 6~8: attacking, 9: use skill, A: use skill faster, B: act fight, C: flying in the air (riding 飛氈), D: flying & moving in the air (riding 飛氈), E: not exist, F: sit]
        [JsonIgnore]
        public string aiming; // selected target

        // 屬性
        public ushort[] gifts = { 23, 23, 23, 23, 23 };
        public ushort giftPoint = 0;

        public uint exp = 0;

        public Dictionary<int, PlayerSkill> skillList { get; private set; }

        //public byte team; // check size

        public Hero(int heroID, string heroName, Gender gender, Role role, Hair hair, byte lv, HeroModel model)
        {
            base.heroID = heroID;
            base.heroName = heroName;
            base.gender = gender;
            base.role = role;
            base.hair = hair;
            base.lv = lv;
            base.model = model;
        }

        public Hero(HeroBasicInfo info)
        {
            base.heroID = info.heroID;
            base.heroName = info.heroName;
            base.gender = info.gender;
            base.role = info.role;
            base.hair = info.hair;
            base.lv = info.lv;
            base.model = info.model;
        }

        [JsonConstructor]
        public Hero()
        {
            // init. add this new player to the list
            ((IEntity)this).updateMap(map);
        }

        public new static Hero fromJson(string json)
        {
            return JsonConvert.DeserializeObject<Hero>(json, Lib.jsonSetting);
        }

        public string playerToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, Lib.jsonSetting);
        }

        int IEntity.entityID { get => heroID; }

        #region Map / Location / Facing
        // REVIEW: update default location to Map.LYC[x, y] @ FinalRelease
        public ushort locX { get; protected set; } = 0x00EA; // 234
        public ushort locY { get; protected set; } = 0x006F; // 111
        public ushort map { get; protected set; } = (ushort)Map.TYC;
        public byte facing { get; protected set; } = 0x04;

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

        #region Basic Stats

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

        public byte state { get; set; } = 0x02;
        public byte act { get; set; } = 0x01;
        #endregion

        #region Buff Related
        [JsonIgnore]
        public Dictionary<int, Buff> Buffs { get; private set; } = new Dictionary<int, Buff>();

        void ILivingEntity.addBuff(int buffID, ushort duration)
        {
            // clear the duplicated buff
            Buffs.Remove(buffID);
            Buffs.Add(buffID, new Buff(buffID, duration));

            // sync buffs
            Lib.sendPktByHeroID(this.heroID, PacketEncoder.selfBuffList(
                    this
            ));
            Lib.sendToNearby(this, PacketEncoder.entityBuffList(
                this
            ));
        }

        void ILivingEntity.removeBuff(int buffID)
        {
            Buffs.Remove(buffID);

            // sync buffs
            Lib.sendPktByHeroID(this.heroID, PacketEncoder.selfBuffList(
                    this
            ));
            Lib.sendToNearby(this, PacketEncoder.entityBuffList(
                this
            ));
        }
        #endregion


        void IDamageable.makeDamage(ILivingEntity damagedBy, bool ignoreDefense)
        {
            // TODO: damage logic
            //int meleeDamage = damagedBy.meleeDamage;
            //if (damagedBy is Hero)
            
            this.hp -= damagedBy.meleeDamage;
        }
    }
}
