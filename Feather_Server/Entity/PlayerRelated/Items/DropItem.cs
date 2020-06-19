using System;
using System.Collections.Generic;
using System.Text;
using Feather_Server.Packets;

namespace Feather_Server.Entity.PlayerRelated.Items
{
    public class DropItem : IPacketStreamFragment
    {
        // TODO: drop item class
        private uint uid = 0x00;
        private ushort locX = 0x00;
        private ushort locY = 0x00;

        private ushort unk1 = 0x00;
        private ushort unk2 = 0x00;
        private ushort unk3 = 0x00;

        /// <summary>
        /// (bigger  number) the ID of the display item in bag/ground/equipment slot
        /// </summary>
        private uint baseID = 0x00;

        /// <summary>
        /// (smaller number) the ID of the item description (also responsable for display name)
        /// </summary>
        private uint itemID = 0x00;

        public DropItem()
        {

        }

        // TODO: find out those unknowns. [possible: colors?] @ Lv[easy]
        public void toFragment(ref PacketStream stream)
        {
            /* JS_F: Here[DropItem] */
            stream
                /* JS: Desc[DropEntityID] */
                .writeDWord(uid)
                /* JS: Desc[LocX] */
                .writeWord(locX)
                /* JS: Desc[LocY] */
                .writeWord(locY)
                /* JS: Desc[BaseID (Model)] R[ITEM,name] */
                .writeDWord(baseID)
                /* JS: Desc[unk1] */
                .writeWord(unk1)
                /* JS: Desc[unk2] */
                .writeWord(unk2)
                /* JS: Desc[unk3] */
                .writeWord(unk3)
                /* JS: Desc[ItemID (Desc)] R[ITEM,dec] */
                .writeDWord(itemID)
            ;
        }
    }
}
