using Feather_Server.Entity.PlayerRelated.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class ItemPacket
    {

        public static PacketStreamData spawnDropItem(DropItem item)
        {
            var stream = new PacketStream()
            /* JS_D: Desc[Spawn Drop Item] */
            .setDelimeter(Delimeters.DROP_ITEM_SPAWN);

            /* JS_F: To[DropItem@Feather_Server/Entity/PlayerRelated/Items/DropItem.cs] */
            item.toFragment(ref stream);

            return stream.pack();
        }
    }
}
