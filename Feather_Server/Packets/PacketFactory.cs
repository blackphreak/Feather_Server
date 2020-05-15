using Feather_Server.Packets;
using Feather_Server.Packets.Actual;
using Feather_Server.Packets.PacketLibs;
using Feather_Server.Packets.Utils;
using Feather_Server.ServerRelated;

namespace Feather_Server.Packets
{
    /// <summary>
    /// Merge few packets to perform a series of actions
    /// </summary>
    public static class PacketFactory
    {
        public static PacketStreamData loginSuccess(HeroBasicInfo[] heros, bool isInHeroCreation = false)
        {
            return
                LoginPacket.loginSuccess()
                + LoginPacket.HeroPreviews(heros, isInHeroCreation);
        }
    }
}
