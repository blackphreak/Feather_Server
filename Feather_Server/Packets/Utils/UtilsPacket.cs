using Feather_Server.Packets.PacketLibs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Packets.Utils
{
    public class UtilsPacket
    {
        public static PacketStreamData alertBox(EFormatString e)
        {
            return new PacketStream()
                /* JS_D: Desc[Alert Box] */
                .setDelimeter(Delimeters.ALERT_BOX)
                /* JS: Desc[Alert Msg] R[FS] */
                .writeFormat(e)
            .pack();
        }

        public static PacketStreamData showProgressBar(ushort time, int msgID)
        {
            return new PacketStream()
                /* JS_D: Desc[Show Progress Bar] */
                .setDelimeter(Delimeters.PROGRESS_BAR_SHOW)
                /* JS: Desc[Time (sec)] */
                .writeWord(time)
                /* JS: Desc[Progress Bar Msg] R[FS] */
                .writeDWord(msgID)
            .pack();
        }

        public static PacketStreamData progressBarComplete(bool isSuccess)
        {
            return new PacketStream()
                /* JS_D: Desc[Progress Bar Complete] */
                .setDelimeter(Delimeters.PROGRESS_BAR_COMPLETE)
                /* JS: Desc[isSuccess] */
                .writeByte((byte)(isSuccess ? 1 : 0))
            .pack();
        }

    }
}
