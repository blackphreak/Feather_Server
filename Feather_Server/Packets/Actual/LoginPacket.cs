using Feather_Server.Packets.PacketLibs;
using Feather_Server.ServerRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Actual
{
    public static class LoginPacket
    {
        public static PacketStreamData loginSuccess()
        {
            return new PacketStream()
                /* JS_D: Desc[Login Success] */
                .setDelimeter(Delimeters.LOGIN_SUCCESS)
                /* JS: Desc[Padding] */
                .writePadding(4)
                .pack();
        }

        public static PacketStreamData HeroPreviews(HeroBasicInfo[] players, bool isInHeroCreation = false)
        {
            var stream = new PacketStream();

            if (isInHeroCreation)
            {
                stream
                    /* JS_D: Desc[Hero Creation] */
                    .setDelimeter(Delimeters.HERO_CREATION)
                    /* JS: Desc[unk1] */
                    .writeDWord(0xBFCAE4CE)
                    .nextPacket();
            }

            for (byte i = 1; i <= 6; i++)
            {
                if (i - 1 >= players.Length || players[i - 1] == null)
                {
                    // same signature will be found below, so no need to fill this format.
                    stream
                        .setDelimeter(Delimeters.LOGIN_HERO_VIEW)
                        .writeByte(i)
                        .writePadding(49)
                        .writeFormat(EFormatString.TEMPLATE_LOGIN_NAME)
                        .writeString(" ")
                        .writeParam(0x0)
                        .nextPacket();
                }
                else
                {
                    stream
                        /* JS_D: Desc[Login Hero View] */
                        .setDelimeter(Delimeters.LOGIN_HERO_VIEW)
                        /* JS: Desc[Slot Index] */
                        .writeByte(i)

                        /* JS_F: To[Hero_Basic_Info@Feather_Server/Entity/PlayerRelated/HeroBasicInfo.cs] */
                        .writeFragment(players[i - 1])
                        .nextPacket();
                }
            }

            return stream.pack();
        }

        public static PacketStreamData lastLoginRecord(string date, string ip)
        {
            return new PacketStream()
                /* JS_D: Desc[Last Login Record] */
                .setDelimeter(Delimeters.SELF_LAST_LOGIN_RECORD)
                /* JS: Desc[FormatStringID] Fn[ParseFS] */
                .writeFormat(EFormatString.LAST_LOGIN_MESSAGE)
                /* JS: Desc[Login Date] */
                .writeString(date)
                /* JS: Desc[Login IP] */
                .writeString(ip)
                .pack();
        }

        public static PacketStreamData loginUnk()
        {
            return new PacketStream()
                /* JS_D: Desc[Login Unk] Mark[EOP] */
                .setDelimeter(Delimeters.LOGIN_UNK)
                .pack();
        }
    }
}
