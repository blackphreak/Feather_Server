using Feather_Server.PlayerRelated.Model;
using Feather_Server.PlayerRelated.Skills.Rides;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Feather_Server.ServerRelated
{
    public class Ride
    {
        // For ride list, please refer to the [@info.dat][Item] | search for [col:buffdec] nr0
        // for diviation (nr___ 變色):
        // nr026: 0x
        // nr027: 0x62b9
        // nr033: 0x1ae8
        public ushort modelID = 0x0000;
        public ushort modelColor = 0x0000; // colored
        [JsonIgnore]
        public ushort wingsLv = 0x0000; // ride wings lv. [0x00: none, 0x0a: little, 0x0b: big]
        [JsonIgnore]
        public ushort wingsID = 0x0000; // ride wings model ID

        /// <summary>
        /// (the one without buffdec) 101173 岚之契约 可永久召唤出凤尾狐。
        /// </summary>
        public uint baseItemID = 0x00000000;
        /// <summary>
        /// (the one with buffdec) 101206 岚之契约 【功能】使用后可得到骑宠：凤尾狐二代 对应nr027
        /// </summary>
        public uint descItemID = 0x00000000;

        public List<Effect> rideBottomEffect;

        // excluded from model packet
        public byte lv = 0; // 0 ~ 150
        public uint exp = 0;
        public byte generation = 1; // 1 ~ 3
        public ushort shareStat = 25; // 25%, 50%, 100%

        public string name = string.Empty;
        public string speak = string.Empty;

        public ushort[] stat = new ushort[] {
            // usable stat point
            0,
            // power
            10,
            // intelligence
            10,
            // spirit
            10,
            // stamina
            10,
            // strength
            10
        };

        public List<RideSkill> cubSkills;
        public List<RideSkill> advSkills;
        public List<RideSkill> sepSkills;

        public Ride(uint baseItemID, uint descItemID)
        {
            this.baseItemID = baseItemID;
            this.descItemID = descItemID;

            var effects = this.rideBottomEffect ?? new List<Effect>();
            switch (descItemID)
            {
                #region Gen1
                case 101224: // 雪灵鹿初代
                    this.modelID = 0x0001;
                    break;
                case 101288: // 蓝灵初代
                    this.modelID = 0x0001;
                    this.modelColor = 0x62b9; // TODO: find color
                    break;

                case 101226:
                    this.modelID = 0x0002;
                    break;
                case 101289: // 紫电初代
                    this.modelID = 0x0002;
                    this.modelColor = 0x62b9; // TODO: find color
                    break;

                case 101230:
                    this.modelID = 0x0003;
                    break;

                case 101227:
                    this.modelID = 0x0004;
                    break;
                case 101287: // 翠鳞初代
                    this.modelID = 0x0004;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101225:
                    this.modelID = 0x0005;
                    break;
                case 101285: // 赤焰初代
                    this.modelID = 0x0005;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101229:
                    this.modelID = 0x0006;
                    break;

                case 101228:
                    this.modelID = 0x0007;
                    break;
                case 101286: // 金刚初代
                    this.modelID = 0x0007;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101231: // 骆马契约
                    this.modelID = 0x000D;
                    break;

                case 101234: // 爬爬虫召唤石
                    this.modelID = 0x000E;
                    break;

                case 101235: // 轻风契约
                    this.modelID = 0x000F;
                    break;
                case 101201: // 欢乐圣诞座驾
                case 101300: // 欢乐圣诞座驾（3天）
                    this.modelID = 0x0011;
                    break;

                case 101202: // 年兽契约（1天）
                    this.modelID = 0x0012;
                    break;

                case 101203: // 爱之犬契约（1天）
                    this.modelID = 0x0013;
                    break;

                case 101204: // 元宵车契约（1天）
                    this.modelID = 0x0014;
                    break;

                case 101200: // 旋风神驱契约
                    this.modelID = 0x0015;
                    break;

                case 101223: // 潜水鲸鱼初代
                    this.modelID = 0x002C;
                    break;
                #endregion
                #region Gen2
                case 101205: // 凤尾狐一代
                    this.modelID = 0x001A;
                    this.generation = 2;
                    break;
                case 101294: // 紫电一代
                    this.modelID = 0x001A;
                    this.generation = 2;
                    this.modelColor = 0x62b9;
                    break;

                case 101207: // 焰之契约
                    this.modelID = 0x001C;
                    this.generation = 2;
                    break;
                case 101290: // 赤焰之魂
                    this.modelID = 0x001C;
                    this.generation = 2;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101209: // 力之契约
                    this.modelID = 0x001E;
                    this.generation = 2;
                    break;
                case 101291: // 金刚之魂
                    this.modelID = 0x001E;
                    this.generation = 2;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101211: // 地之契约
                    this.modelID = 0x0020;
                    this.generation = 2;
                    break;
                case 101292: // 翠鳞之魂
                    this.modelID = 0x0020;
                    this.generation = 2;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101213:
                    this.modelID = 0x0022;
                    this.generation = 2;
                    break;

                case 101215:
                    this.modelID = 0x0024;
                    this.generation = 2;
                    break;

                case 101217: // 雪灵鹿一代
                    this.modelID = 0x0026;
                    this.generation = 2;
                    break;
                case 101293: // 蓝灵一代
                    this.modelID = 0x0026;
                    this.generation = 2;
                    this.modelColor = 0x62b9; // TODO: find color
                    break;

                case 101219: // 轻风飞毯一代
                    this.modelID = 0x0028;
                    this.generation = 2;
                    break;

                case 101221: // 爬爬虫一代
                    this.modelID = 0x002A;
                    this.generation = 2;
                    break;

                case 101236: // 潜水鲸鱼一代
                    this.modelID = 0x0010;
                    this.generation = 2;
                    break;
                #endregion
                #region Gen3
                case 101206: // 岚之契约
                    this.modelID = 0x001B;
                    this.generation = 3;
                    break;
                case 101299: // 紫电之魂
                    this.modelID = 0x001B;
                    this.generation = 3;
                    this.modelColor = 0x62b9;
                    break;

                case 101208: // 焰之契约
                    this.modelID = 0x001D;
                    this.generation = 3;
                    break;
                case 101295: // 赤焰之魂
                    this.modelID = 0x001D;
                    this.generation = 3;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101210: // 力之契约
                    this.modelID = 0x001F;
                    this.generation = 3;
                    break;
                case 101296: // 金刚之魂
                    this.modelID = 0x001F;
                    this.generation = 3;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101212: // 地之契约
                    this.modelID = 0x0021;
                    this.generation = 3;
                    break;
                case 101297: // 翠鳞之魂
                    this.modelID = 0x0021;
                    this.generation = 3;
                    this.modelColor = 0x1ae8; // TODO: find color
                    break;

                case 101214:
                    this.modelID = 0x0023;
                    this.generation = 3;
                    break;

                case 101216:
                    this.modelID = 0x0025;
                    this.generation = 3;
                    break;

                case 101218: // 雪灵鹿二代
                    this.modelID = 0x0027;
                    this.generation = 3;
                    break;
                case 101298: // 蓝灵二代
                    this.modelID = 0x0027;
                    this.generation = 3;
                    this.modelColor = 0x62b9; // TODO: find color
                    break;

                case 101220: // 轻风飞毯二代
                    this.modelID = 0x0029;
                    this.generation = 3;
                    break;

                case 101222: // 爬爬虫二代
                    this.modelID = 0x002B;
                    this.generation = 3;
                    break;

                case 101237: // 潜水鲸鱼二代
                    this.modelID = 0x0037; // nr055
                    this.generation = 3;
                    break;
                    #endregion
            }

            if (this.generation == 2)
                this.shareStat = 50;
            else if (this.generation == 3)
                this.shareStat = 100;

            this.cubSkills = new List<RideSkill>();
            this.advSkills = new List<RideSkill>();
            this.sepSkills = new List<RideSkill>();
        }

        [JsonConstructor]
        public Ride()
        { }

        public string toHex()
        {
            return Lib.toHex(modelID)
                + "0000"
                + Lib.toHex(modelColor)
                + "0000"
                + Lib.toHex(wingsLv)
                + "0000"
                + Lib.toHex(wingsID)
                + "0000";
        }
    }
}