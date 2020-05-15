using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets
{
    public interface IPacketStreamFragment
    {
        void toFragment(ref PacketStream stream);
    }
}
