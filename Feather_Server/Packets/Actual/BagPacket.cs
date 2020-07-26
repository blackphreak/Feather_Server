using Feather_Server.PlayerRelated;

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

        public static PacketStreamData setBagSlot(Bag bag)
        {
            return new PacketStream()
                /* JS_D: Desc[Avaliable Bag Slot] */
                .setDelimeter(Delimeters.SELF_HERO_BAG_SLOT_AVALIABLE)
                /* JS: Desc[Bag Slots] Fn[eStuffbag] */
                .writeDWord(uint.Parse(
                    bag.extraBag[2].ToString()
                    + bag.extraBag[1].ToString()
                    + bag.extraBag[0].ToString()
                    + "24")
                )
            .pack();
        }
    }
}
