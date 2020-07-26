using Feather_Server.Packets;
using Feather_Server.Packets.Actual;
using Feather_Server.Packets.PacketLibs;
using Feather_Server.Packets.Utils;
using Feather_Server.ServerRelated;
using System;

namespace Feather_Server.Packets
{
    /// <summary>
    /// Merge few packets to perform a series of actions.
    /// </summary>
    public static class PacketFactory
    {
        public static PacketStreamData loginSuccess(HeroBasicInfo[] heros, bool isInHeroCreation = false) =>
            LoginPacket.loginSuccess() + LoginPacket.HeroPreviews(heros, isInHeroCreation);

        /// <summary>
        /// 1. 07 490e0000000000
        /// 2. 07 3d2f cee4cabf 00 // unk
        /// 3. 03 490200    // unk
        /// 4. 04 5a0100 00 // unk
        /// 5. lastLoginRecord (disconnect + now)
        /// 6. game notice board
        /// 7. 07 3d2f cee4cabf 00 // same as @2.
        /// 8. 04 3d910000 // actual join game
        /// 9. player bag
        /// 10. 07 490a0000000000
        /// 11. __ 3c11 27 ... // player location & SFX (by map ID)
        /// 12. spawnHero // animated
        /// 13. __ 7f01 6a &lt;heroID&gt; a500 a500 00 // unk, 0xA5 = 165
        /// 14. __ 7f01 6b &lt;heroID&gt; 5400 5400 00 // unk, 0x54 = 84
        /// 15. __ 7f01 6c &lt;heroID&gt; 5400 5400 00 // unk, 0x54 = 84
        /// ... should be more? (6d, 6e, 6f) ... // TODO: test
        /// 16. __ 7f01 70 &lt;heroID&gt; 3b00 3c00 00 // unk, 0x3B = 59, 0x3C = 60
        /// 17. getHeroFullInfo // ?
        /// 18. __ 81 245c 0c000000 01 00
        /// </summary>
        /// <param name="hero">who is joining</param>
        /// <returns></returns>
        public static PacketStreamData playerJoin(Hero hero)
        {
            var stream = new PacketStreamData();
            stream += LoginPacket.loginSuccess();
            stream.AddRange(new byte[] { 0x07, 0x3d, 0x2f, 0xce, 0xe4, 0xca, 0xbf, 0x00 }); // TODO: give them a name?
            stream.AddRange(new byte[] { 0x03, 0x49, 0x02, 0x00 });
            stream.AddRange(new byte[] { 0x04, 0x5a, 0x01, 0x00, 0x00 });

            // 5. lastLoginRecord
            var cli = Lib.clientList.GetValueOrDefault(hero.heroID);
            if (cli == null)
                throw new System.Exception($"Failed to get client object by heroID.\n    heroID[{hero.heroID}]");

            // TODO: replace placeholder, insert login record to database
            stream += LoginPacket.lastLoginRecord(true, "{Placeholder}", cli.addrInfo);
            stream += LoginPacket.lastLoginRecord(false, DateTime.Now.ToString("u")[..^1], cli.addrInfo);

            // 6. game notice board
            //stream += 


            stream.AddRange(new byte[] { 0x07, 0x3d, 0x2f, 0xce, 0xe4, 0xca, 0xbf, 0x00 });
            stream.AddRange(new byte[] { 0x04, 0x3d, 0x91, 0x00, 0x00 });

            // 9. player bag
            foreach (var item in hero.bag.getAllItems())
                stream += BagPacket.addBagItem(item);

            stream.AddRange(new byte[] { 0x07, 0x49, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00 });

            stream += HeroPacket.mapInfo(hero); // 3c11 27 ...
            stream += HeroPacket.spawnPlayerAnimated(hero);

            // omitted 13. ~ 16.
            // TODO: add back 13 ~ 16

            // 17. getHeroFullInfo
            #region getHeroFullInfo
            // set window title
            stream += HeroPacket.setWindowTitle(hero);
            // hero title list ...
            // TODO: title list

            // hero gifts
            stream += HeroPacket.setGifts(hero);
            // hero attributes
            stream += HeroPacket.setAttributes(hero);
            // some numbers ...
            stream += HeroPacket.setNumbersA(hero);
            // some more numbers ...
            stream += HeroPacket.setNumbersB(hero);

            // set criticate hit rate
            stream += HeroPacket.setCH(hero);

            // set bag slot
            stream += BagPacket.setBagSlot(hero.bag);
            #endregion

            stream.AddRange(new byte[] { 0x09, 0x81, 0x24, 0x5c, 0x0c, 0x00, 0x00, 0x00, 0x01, 0x00 });

            return stream;
        }

        public static PacketStreamData alertBox(EFormatString e) => UtilsPacket.alertBox(e);
    }
}
