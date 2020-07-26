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
        //   Self: Data Receiver (the game client) (only if no EntityID)

        public static readonly byte[] ALERT_BOX = { 0x21, 0x02 };
        
        public static readonly byte[] LOGIN_SUCCESS = { 0x49, 0x0e };
        public static readonly byte[] LOGIN_HERO_VIEW = { 0x49, 0x0d };


        public static readonly byte[] PROGRESS_BAR_SHOW = { 0x5e, 0x02 };
        public static readonly byte[] PROGRESS_BAR_COMPLETE = { 0x5e, 0x01 };

        public static readonly byte[] DROP_ITEM_SPAWN = { 0x2e };
        public static readonly byte[] BAG_ITEM_ADD = { 0x2b };
        public static readonly byte[] BAG_ITEM_ACTIVATE = { 0xa6, 0x0a };

        public static readonly byte[] NPC_SPAWN = { 0x69, 0x80 };
        public static readonly byte[] HERO_SPAWN_NORMAL = { 0x69, 0x0a };
        public static readonly byte[] HERO_SPAWN_ANIMATED = { 0x69, 0x0b };

        public static readonly byte[] ITEM_DESC = { 0x31 };

        // @ 409817
        // public static readonly byte[] SELF_ = { 0xa2, 0x0f }; // online msg (p2p?)
        // @ 409955
        public static readonly byte[] ENTITY_TITLE_CURRENT = { 0xa2, 0x14 }; // [3]: heroID, [7==0]: readFromDB, [7>0]: direct name

        public static readonly byte[] TASK_LOG = { 0xa9, 0x1a };
        public static readonly byte[] HERO_EFFECTS = { 0x6f };

        public static readonly byte[] SELF_BUFFS = { 0x81 };
        public static readonly byte[] SELF_BUFF_DESC = { 0x82 };
        public static readonly byte[] ENTITY_BUFFS = { 0x83 };
        public static readonly byte[] ENTITY_BUFF_DESC = { 0x84 }; // -- 84 <EID,4> ...

        public static readonly byte[] ENTITY_DESPAWN = { 0x78 };

        public static readonly byte[] HERO_HP_INFO_UPDATE = { 0x54, 0x03 };
        public static readonly byte[] HERO_MP_INFO_UPDATE = { 0x54, 0x04 };

        #region Self Attributes (0x3D Family)
        // @ 4F7A14
        public static readonly byte[] SELF_PLAYER_NAME = { 0x3d, 0x04 };

        public static readonly byte[] SELF_GIFT_VIT = { 0x3d, 0x0A };
        public static readonly byte[] SELF_GIFT_SPIR = { 0x3d, 0x0B };
        public static readonly byte[] SELF_GIFT_INTELL = { 0x3d, 0x0C };
        public static readonly byte[] SELF_GIFT_STR = { 0x3d, 0x0D };
        public static readonly byte[] SELF_GIFT_STAM = { 0x3d, 0x0E };

        public static readonly byte[] SELF_HP = { 0x3d, 0x14 };
        public static readonly byte[] SELF_HP_MAX = { 0x3d, 0x15 };
        public static readonly byte[] SELF_MP = { 0x3d, 0x16 };
        public static readonly byte[] SELF_MP_MAX = { 0x3d, 0x17 };
        public static readonly byte[] SELF_PA = { 0x3d, 0x18 };
        public static readonly byte[] SELF_PD = { 0x3d, 0x19 };
        public static readonly byte[] SELF_MA = { 0x3d, 0x1A };
        public static readonly byte[] SELF_MD = { 0x3d, 0x1B };
        public static readonly byte[] SELF_HERO_CRIT_HIT_RATE = { 0x3d, 0x1D };
        // @ 4F7B7A
        public static readonly byte[] SELF_HERO_MIN_UNK1 = { 0x3d, 0x1E };
        public static readonly byte[] SELF_HERO_MAX_UNK1 = { 0x3d, 0x1F };

        // @ 4F7BE8
        // public static readonly byte[] SELF_ = { 0x3d, 0x20 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x21 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x23 }; // strcpy to playerAttr->charCC, charSz: 200

        public static readonly byte[] SELF_EXP = { 0x3d, 0x28 }; // level related... is that lv up things?
        public static readonly byte[] SELF_GOLD_AMOUNT = { 0x3d, 0x29 };
        public static readonly byte[] SELF_GIFT_POINT_AMOUNT = { 0x3d, 0x2A };
        public static readonly byte[] SELF_LV = { 0x3d, 0x2B };
        public static readonly byte[] SELF_LV_UP_ANIMATE = { 0x3d, 0x2C };
        // public static readonly byte[] SELF_ = { 0x3d, 0x2E };
        public static readonly byte[] SELF_HERO_CREATION = { 0x3d, 0x2F }; // strcpy, charSz: 39 (cee4cabf == gbk: 武士)

        // public static readonly byte[] SELF_ = { 0x3d, 0x30 };
        // omitted: { 0x3d, 0x31 } ~ { 0x3d, 0x43 }


        public static readonly byte[] SELF_VIGOR = { 0x3d, 0x44 };
        public static readonly byte[] SELF_SILVER = { 0x3d, 0x46 };
        // public static readonly byte[] SELF_UNK1_AFTER_SILVER = { 0x3d, 0x47 };
        public static readonly byte[] SELF_HERO_BAG_SLOT_AVALIABLE = { 0x3d, 0x48 };

        // public static readonly byte[] SELF_ = { 0x3d, 0x50 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x51 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x53 }; // build str with formatID
        // @ 4F8B1C
        // public static readonly byte[] SELF_ = { 0x3d, 0x54 }; // build str with formatID @ [7], heroID @ [3]
        // public static readonly byte[] SELF_ = { 0x3d, 0x55 };

        // public static readonly byte[] SELF_ = { 0x3d, 0x7A }; // 0x05, 0x3d, 0x7a, 0xFA, 0xFB, 0x00
        // public static readonly byte[] SELF_ = { 0x3d, 0x7C };
        // public static readonly byte[] SELF_ = { 0x3d, 0x7D };
        // public static readonly byte[] SELF_ = { 0x3d, 0x7F };
        // public static readonly byte[] SELF_ = { 0x3d, 0x80 }; // 0x04, 0x3d, 0x80, 0x00, 0x00
        // public static readonly byte[] SELF_ = { 0x3d, 0x81 }; // 0x04, 0x3d, 0x81, 0x00, 0x00
        // public static readonly byte[] SELF_ = { 0x3d, 0x82 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x83 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x83 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x85 }; // 0x04, 0x3d, 0x85, 0xFC, 0x00
        // public static readonly byte[] SELF_ = { 0x3d, 0x86 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x87 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x88 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x89 };
        // public static readonly byte[] SELF_ = { 0x3d, 0x8F }; // set flag to addr: 8C8FC0, pkt: 053d8f000100 TODO: test test wt is this? || 0x05, 0x3d, 0x8f, 0xFC, 0xFD, 0x00
        // @ 4F8E11
        // public static readonly byte[] SELF_ = { 0x3d, 0x90 }; // 0x05, 0x3d, 0x90, 0xFE, 0xFF, 0x00
        // public static readonly byte[] SELF_ = { 0x3d, 0x91 }; // set flag to: playerAttributes->flag_byte2998 || TODO: test pkt 043d910100

        public static readonly byte[] SELF_SET_WINDOW_TITLE = { 0x3d, 0xa0 };
        // @ 4F8EC6
        // public static readonly byte[] SELF_ = { 0x3d, 0xa1 }; // set role name and more...
        public static readonly byte[] SELF_HERO_GIFTS = { 0x3d, 0xa2 };
        public static readonly byte[] SELF_HERO_ATTRIBUTES = { 0x3d, 0xa3 };
        // @ 4F92DB
        //  0  1 2  3 4 5 - 8 9
        // __ 3da4 0000 QWord 
        public static readonly byte[] SELF_HERO_AMOUNT = { 0x3d, 0xa4 };
        // @ 4F9549
        // __ 3d a5 1111 2222 3333 4444 5555 6666 7777 8888 vigor(WORD) 99 AA --
        public static readonly byte[] SELF_HERO_AMOUNT2 = { 0x3d, 0xa5 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xa6 }; // GUI related
        // public static readonly byte[] SELF_ = { 0x3d, 0xb1 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xc8 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xc9 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xca };
        // public static readonly byte[] SELF_ = { 0x3d, 0xcb };
        // public static readonly byte[] SELF_ = { 0x3d, 0xcc };
        // public static readonly byte[] SELF_ = { 0x3d, 0xcd };

        // public static readonly byte[] SELF_ = { 0x3d, 0xd1 };

        // public static readonly byte[] SELF_ = { 0x3d, 0xf0 }; // set flag to: flag_dword_8C8EE4 || TODO: test pkt 053df0000100
        // public static readonly byte[] SELF_ = { 0x3d, 0xf1 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xf2 }; // set flag to: playerAttributes->flag_byte29C9 || TODO: test pkt 053df2000100
        // public static readonly byte[] SELF_ = { 0x3d, 0xf5 }; // heroID @ [3], *&plyLocPtr_8C8EDC->gap1761[0x11F] = cmd[7];
        public static readonly byte[] SELF_HERO_DODGE = { 0x3d, 0xf6 };
        public static readonly byte[] SELF_HERO_HIT = { 0x3d, 0xf7 };
        // public static readonly byte[] SELF_ = { 0x3d, 0xf8 }; // heroID @ [3], *&plyLocPtr_8C8EDC[1].heroID[0xAC] = cmd[7];
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
        // @782

        public static readonly byte[] SELF_LAST_LOGIN_RECORD = { 0x3e, 0x01 };
        public static readonly byte[] SELF_HERO_HONOR_POINT = { 0xa5, 0x08 };
        public static readonly byte[] SELF_HERO_CULTIVATION_LEVEL = { 0xa2, 0x0a };
        // TODO: wt is this? @ PacketEncoder.cs - 1098
        public static readonly byte[] SELF_HERO_UNK1 = { 0xa2, 0x11 };
        public static readonly byte[] SELF_HERO_GOLD = { 0x45, 0x07 };

        public static readonly byte[] SELF_HERO_SKILL_TREE = { 0x53 };
        /// <summary>
        /// Skill Item on ItemBar?
        /// </summary>
        public static readonly byte[] SELF_HERO_SKILL_ITEM = { 0x64 };

        public static readonly byte[] SELF_HERO_MAP_INFO = { 0x3c, 0x11 };

        #region Unnamed
        public static readonly byte[] LOGIN_UNK = { 0x49, 0x02 }; // TODO: what is this?
        public static readonly byte[] LOGIN_UNK2 = { 0x5a, 0x01 }; // TODO: what is this?
        #endregion
    }
}

// 51: quest? @ 405C11
// 55: skill gui? @ 406554
