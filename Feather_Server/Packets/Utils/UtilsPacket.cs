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
    }
}
