using Feather_Server.Packets.PacketLibs;
using Feather_Server.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feather_Server.Entity
{
    public class FormatString
    {
        public PacketStreamData Fragment {
            get;
            private set;
        }

        public EFormatString Format {
            get;
            private set;
        }

        public FormatString(EFormatString fs, params object[] param)
        {
            if (!fs.Equals(EFormatString.TEMPLATE_DIRECT_GBK))
                this.writeFormat(fs);

            foreach (var tmp in param)
                if (tmp is string)
                    writeString((string)tmp);
                else
                    writeParam(tmp);

            Fragment.AsReadOnly();
            Format = fs;
        }

        ~FormatString()
        {
            Fragment = null;
        }

        private void writeFormat(EFormatString fs)
        {
            Fragment.AddRange(BitConverter.GetBytes((uint)fs));
        }

        private void writeParam(object data)
        {
            Fragment.Add(0x64);

            // little-endian
            Fragment.AddRange(
                BitConverter.GetBytes( Convert.ToUInt32(data) )
            );
        }

        private void writeString(string rawText)
        {
            Fragment.Add(0x73);

            var converted = Lib.gbkToBytes(rawText);
            // length of bytes (max: UInt32.MAX_VALUE)
            Fragment.AddRange(BitConverter.GetBytes((uint)converted.Length));
            Fragment.AddRange(converted);
        }
    }
}
