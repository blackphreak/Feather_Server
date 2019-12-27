using Feather_Server.PlayerRelated;
using Feather_Server.ServerRelated;
using Newtonsoft.Json;
using System;

namespace Feather_Server.Entity.PlayerRelated.Items.Activable
{
    public class RideWing : Item, IActivable
    {
        [JsonIgnore]
        public bool isActive { get; protected set; } = false;

        public ushort wingsLv = 0x0000; // ride wings lv. [0x00: none, 0x0a: little, 0x0b: big]
        public ushort wingsID = 0x0000; // ride wings model ID

        public override byte[] use(Hero p)
        {
            var pktsAfterUse = new byte[0];

            isActive = !isActive;

            PacketEncoder.concatPacket(
                PacketEncoder.activeBagItem(this.itemUID, isActive)
            , ref pktsAfterUse, false);
            // TODO: should split to two pkts? one for self, one for broadcast?

            return pktsAfterUse;
        }
    }
}
