using Feather_Server.PlayerRelated;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity.PlayerRelated.Items
{
    public class RideContract : Item
    {
        public override byte[] use(Hero p)
        {
            p.rideList ??= new List<Ride>();
            p.rideList.Add(new Ride(this.baseID, this.itemID));

            return PacketEncoder.rideItem(p, (byte)(p.rideList.Count));
        }
    }
}
