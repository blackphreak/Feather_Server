using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class RidePacket
    {

        public static PacketStreamData rideOn(Hero p)
        {
            var stream = new PacketStream();
            /* JS_D: Desc[Spawn Hero With Animation] */
            stream.setDelimeter(Delimeters.HERO_SPAWN_ANIMATED);

            
        }
    }
}
