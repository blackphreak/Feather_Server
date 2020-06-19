using Feather_Server.Entity.PlayerRelated.Items;
using Feather_Server.PlayerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class BagPacket
    {

        public static PacketStreamData addBagItem(Item item)
        {
            var stream = new PacketStream()
            /* JS_D: Desc[Add Item to Bag] */
            .setDelimeter(Delimeters.BAG_ITEM_ADD);

            /* JS_F: To[ItemInBag@Feather_Server/Entity/PlayerRelated/Items/Item.cs] */
            item.toBagItem(ref stream);

            return stream.pack();
        }

        public static PacketStreamData activateBagItem(uint itemUID, bool isActive)
        {
            return new PacketStream()
                /* JS_D: Desc[Activate Bag Item] */
                .setDelimeter(Delimeters.BAG_ITEM_ACTIVATE)
                /* JS: Desc[isActive] */
                .writeByte(isActive)
                /* JS: Desc[ItemUID] */
                .writeDWord(itemUID)
                /* JS: Desc[Unk1] */
                .writeDWord(0xDEADBEEF) // TODO: find out this unk1
            .pack();
        }
    }
}
