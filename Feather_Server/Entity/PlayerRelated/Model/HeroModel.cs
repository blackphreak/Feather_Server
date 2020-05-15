using Feather_Server.Packets;
using Feather_Server.PlayerRelated.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Feather_Server.PlayerRelated
{
    public class HeroModel : IPacketStreamFragment
    {
        // style
        private ushort mask = 0x0000;
        private ushort mask_color = 0x0000;
        private ushort hat = 0x0000;
        private ushort hat_color = 0x0000;
        private ushort wings = 0x0000;
        private ushort wings_color = 0x0000;
        private ushort body = 0x0000;
        private ushort body_color = 0x0000;
        private ushort tail = 0x0000;
        private ushort tail_color = 0x0000;
        private ushort weapon = 0x0000;
        private Color weapon_color = Color.Empty;

        [JsonConstructor]
        public HeroModel(
            ushort mask = 0x0000,
            ushort mask_color = 0x0000,
            ushort hat = 0x0000,
            ushort hat_color = 0x0000,
            ushort wings = 0x0000,
            ushort wings_color = 0x0000,
            ushort body = 0x0000,
            ushort body_color = 0x0000,
            ushort tail = 0x0000,
            ushort tail_color = 0x0000,
            ushort weapon = 0x0000,
            uint weapon_color = 0x00000000
        )
        {
            this.mask = mask;
            this.mask_color = mask_color;
            this.hat = hat;
            this.hat_color = hat_color;
            this.wings = wings;
            this.wings_color = wings_color;
            this.body = body;
            this.body_color = body_color;
            this.tail = tail;
            this.tail_color = tail_color;
            this.weapon = weapon;
            this.weapon_color = Color.FromArgb((int)weapon_color);
        }

        public HeroModel(EquippableItem mask, EquippableItem hat, EquippableItem wings, EquippableItem body, EquippableItem tail, WeaponItem weapon)
        {
            this.mask = mask.modelID;
            this.mask_color = mask.color;
            this.hat = hat.modelID;
            this.hat_color = hat.color;
            this.wings = wings.modelID;
            this.wings_color = wings.color;
            this.body = body.modelID;
            this.body_color = body.color;
            this.tail = tail.modelID;
            this.tail_color = tail.color;
            this.weapon = weapon.modelID;
            this.weapon_color = weapon.color;
        }

        public HeroModel(EquipmentSet equips)
        {
            var eq = equips.getItem(EquipmentSlot.MASK);
            if (eq != null)
            {
                this.mask = eq.modelID;
                this.mask_color = eq.color;
            }
            
            eq = equips.getItem(EquipmentSlot.HAT);
            if (eq != null)
            {
                this.hat = eq.modelID;
                this.hat_color = eq.color;
            }

            eq = equips.getItem(EquipmentSlot.WINGS);
            if (eq != null)
            {
                this.wings = eq.modelID;
                this.wings_color = eq.color;
            }

            eq = equips.getItem(EquipmentSlot.BODY);
            if (eq != null)
            {
                this.body = eq.modelID;
                this.body_color = eq.color;
            }

            eq = equips.getItem(EquipmentSlot.TAIL);
            if (eq != null)
            {
                this.tail = eq.modelID;
                this.tail_color = eq.color;
            }

            eq = equips.getItem(EquipmentSlot.WEAPON);
            if (eq != null)
            {
                this.weapon = eq.modelID;
                this.weapon_color = ((WeaponItem)eq).color;
            }
        }

        //public string toModelHex()
        //{
        //    return
        //        // hat
        //        Lib.toHex(hat)
        //        + Lib.toHex(hat_color)
        //        // body
        //        + Lib.toHex(body)
        //        + Lib.toHex(body_color)
        //        // wings
        //        + Lib.toHex(wings)
        //        + Lib.toHex(wings_color)
        //        // mask
        //        + Lib.toHex(mask)
        //        + Lib.toHex(mask_color)
        //        // tail
        //        + Lib.toHex(tail)
        //        + Lib.toHex(tail_color)
        //        // wp
        //        + Lib.toHex(weapon)
        //        + Lib.toHex(weapon_color.R)
        //        + Lib.toHex(weapon_color.G)
        //        + Lib.toHex(weapon_color.B)
        //        + Lib.toHex(weapon_color.A)
        //        ;
        //}

        public void toFragment(ref PacketStream stream)
        {
            /* JS_F: Here[HeroModel.cs,Hero_Model] */
            stream
                /* JS: Desc[Hat ModelID] */
                .writeWord(hat)
                /* JS: Desc[Hat Color] */
                .writeWord(hat_color)

                /* JS: Desc[Body ModelID] */
                .writeWord(body)
                /* JS: Desc[Body Color] */
                .writeWord(body_color)

                /* JS: Desc[Wings ModelID] */
                .writeWord(wings)
                /* JS: Desc[Wings Color] */
                .writeWord(wings_color)

                /* JS: Desc[Mask ModelID] */
                .writeWord(mask)
                /* JS: Desc[Mask Color] */
                .writeWord(mask_color)

                /* JS: Desc[Tail ModelID] */
                .writeWord(tail)
                /* JS: Desc[Tail Color] */
                .writeWord(tail_color)

                /* JS: Desc[Weapon ModelID] */
                .writeWord(weapon)
                /* JS: Desc[Weapon Color R] */
                .writeByte(weapon_color.R)
                /* JS: Desc[Weapon Color G] */
                .writeByte(weapon_color.G)
                /* JS: Desc[Weapon Color B] */
                .writeByte(weapon_color.B)
                /* JS: Desc[Weapon Color Alpha] */
                .writeByte(weapon_color.A);
        }
    }
}
