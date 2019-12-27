using Feather_Server.PlayerRelated;
using Newtonsoft.Json;
using System;

namespace Feather_Server.ServerRelated
{
    public class HeroBasicInfo
    {
        [JsonIgnore]
        public int heroID;
        [JsonIgnore]
        public string heroName;
        public Gender gender;
        public Role role;
        public Hair hair;

        public byte lv;
        /// <summary>
        /// when equip changes, also update this
        /// TODO: updates when equip changes
        /// </summary>
        public HeroModel model;

        public HeroBasicInfo(int heroID, string heroName, Gender gender, Role role, Hair hair, byte lv, HeroModel model)
        {
            this.heroID = heroID;
            this.heroName = heroName;
            this.gender = gender;
            this.role = role;
            this.hair = hair;
            this.lv = lv;
            this.model = model;
        }

        [JsonConstructor]
        public HeroBasicInfo()
        {
        }

        public string toHex()
        {
            // fixed
            // ....pos uid ---- lv sex- ________ cut- color                 hat- ---- body ---- wing ____ face ____ tail ____ wp-- wpCode__ fixed--- __ nameLen- name------           __ role ???? __
            // 490d 01 16300000 a0 1127 00000000 e903 e780 0000000000000000 0100 0000 1500 0000 0100 0000 0100 0000 0100 0000 073e ffffff00 5e450f00 73 0a000000 b2d9c4e3c2e8b1c63032 64 fd11 0000 00
            // 490d 01 0c750000 00 f92a 00000000 e903 e780 0000000000000000 0000 0000 0101 0000 0100 0000 0100 0000 0700 0000 5001 00000000 5e450f00 73 05000000 4141414261           64 fe11 0000 00
            // 490d 01 9d360000 51 1127 00000000 0200 a4aa 0000000000000000 1400 0000 1600 0000 0000 0000 0000 0000 0200 0000 3e08 0000df7b 5e450f00 73 08000000 c0adcbb9c0d7b6c8     64 ff11 0000 00
            // 490d 01 16300000 A0 1127 00000000 E903 E780 0000000000000000 0900 0000 1500 0000 0100 0000 0900 0000 0100 0000 3E07 00000000 5e450f00 73 0A000000 B2D9C4E3C2E8B1C63031 64 FD11 0000 00

            var name = Lib.gbkToBytes(heroName);
            return
                Lib.toHex(heroID)  // 4 byte
                + Lib.toHex(lv) // 1 byte
                + Lib.toHex((short)gender) // 2 byte
                + "0000" // unk
                + "0000" // unk
                + Lib.toHex(hair.model)
                + Lib.toHex(hair.color)
                + "00000000" // unk
                + "00000000" // unk
                + model.toModelHex()
                + "5e450f00" // fixed | 0x000F455E = 1000798
                + "73" // name sep
                + Lib.toHex(name.Length) // int length
                + Lib.toHex(name)
                + "64" // sep
                + Lib.toHex((short)role) + "0000"
                + "00"; // null-term
        }

        public static HeroBasicInfo fromJson(string json)
        {
            return JsonConvert.DeserializeObject<HeroBasicInfo>(json, Lib.jsonSetting);
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this, Lib.jsonSetting);
        }

    }
}