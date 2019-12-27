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
        /// The one without buffdec
        /// </summary>
        public int baseItemID = 0x00000000; // (the one without buffdec) 101173 岚之契约 可永久召唤出凤尾狐。
        /// <summary>
        /// The one with buffdec
        /// </summary>
        public int buffItemID = 0x00000000; // (the one with buffdec) 101206 岚之契约 【功能】使用后可得到骑宠：凤尾狐二代 对应nr027

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