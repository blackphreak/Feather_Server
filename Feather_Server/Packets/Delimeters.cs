using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets
{
    public class Delimeters
    {
        // can only be: 0x00000001 to 0xFFFFFFFF
        // naming format:
        //   <Target>_<ActionName>
        // terminology:
        //   NPC: Non-Player-Control (i.e.: Mobs, Boss, Pets)
        //   Hero: Player Entity
        //   Entity: NPC / Hero
        //   Self: Data Receiver (the game client)

        public static readonly byte[] ALERT_BOX = { 0x21, 0x02 };
        
        public static readonly byte[] LOGIN_SUCCESS = { 0x49, 0x0e };
        public static readonly byte[] LOGIN_HERO_VIEW = { 0x49, 0x0d };
        public static readonly byte[] HERO_CREATION = { 0x3d, 0x2f }; // !REVIEW! confirm signature will not crush with another pkt

        public static readonly byte[] PROGRESS_BAR_SHOW = { 0x5e, 0x02 };
        public static readonly byte[] PROGRESS_BAR_COMPLETE = { 0x5e, 0x01 };

        public static readonly byte[] DROP_ITEM_SPAWN = { 0x2e };
        public static readonly byte[] BAG_ITEM_ADD = { 0x2b };
        public static readonly byte[] BAG_ITEM_ACTIVE = { 0xa6, 0x0a };

        public static readonly byte[] NPC_SPAWN = { 0x69, 0x80 };
        public static readonly byte[] HERO_SPAWN_NORMAL = { 0x69, 0x0a };
        public static readonly byte[] HERO_SPAWN_ANIMATED = { 0x69, 0x0b };

        public static readonly byte[] ITEM_DESC = { 0x31 };

        public static readonly byte[] ENTITY_TITLE_CURRENT = { 0xa2, 0x14 };
        public static readonly byte[] TASK_LOG = { 0xa9, 0x1a };
        public static readonly byte[] HERO_EFFECTS = { 0x6f };

        public static readonly byte[] SELF_BUFFS = { 0x81 };
        public static readonly byte[] ENTITY_BUFFS = { 0x83 };

        public static readonly byte[] ENTITY_DESPAWN = { 0x78 };

        public static readonly byte[] HERO_HP_INFO_UPDATE = { 0x54, 0x03 };
        public static readonly byte[] HERO_MP_INFO_UPDATE = { 0x54, 0x04 };

        #region Self Attributes
        public static readonly byte[] SELF_HP = { 0x3d, 0x14 };
        public static readonly byte[] SELF_HP_MAX = { 0x3d, 0x15 };
        public static readonly byte[] SELF_MP = { 0x3d, 0x16 };
        public static readonly byte[] SELF_MP_MAX = { 0x3d, 0x17 };
        public static readonly byte[] SELF_PA = { 0x3d, 0x18 };
        public static readonly byte[] SELF_PD = { 0x3d, 0x19 };
        public static readonly byte[] SELF_MA = { 0x3d, 0x1A };
        public static readonly byte[] SELF_MD = { 0x3d, 0x1B };
        // TODO: missing 0x1C
        public static readonly byte[] SELF_CRITICAL_HIT_RATE = { 0x3d, 0x1D };

        public static readonly byte[] SELF_DODGE = { 0x3d, 0xf6 };
        public static readonly byte[] SELF_HIT = { 0x3d, 0xf7 };
        public static readonly byte[] SELF_EXP = { 0x3d, 0x28 };

        public static readonly byte[] SELF_LV_UP_ANIMATE = { 0x3d, 0x2c };
        #endregion

        public static readonly byte[] ENTITY_HP_BAR = { 0x2a };
        public static readonly byte[] ENTITY_FACING = { 0x5e }; // !REVIEW! confirmation
        public static readonly byte[] HERO_ACT = { 0x40 }; // !REVIEW! confirmation

        /// <summary>
        /// Sit / Stand / ...?
        /// </summary>
        public static readonly byte[] HERO_STATE_UPDATE = { 0xb9, 0xa2 }; // !REVIEW! confirmation

        // !REVIEW! make a better description for [0xa9, 0x17] and [0x67]:
        /// <summary>
        /// Path from other heros, act as relay, forward ascii
        /// </summary>
        public static readonly byte[] HERO_PATH_SYNC = { 0xa9, 0x17 };
        /// <summary>
        /// time, X, Y and facing
        /// </summary>
        public static readonly byte[] HERO_LOCATION_SYNC = { 0x67 };

        // TODO: player join delimeter list
    }
}
