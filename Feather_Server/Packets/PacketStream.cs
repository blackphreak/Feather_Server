using Feather_Server.Packets.PacketLibs;
using System;
using System.Collections.Generic;

namespace Feather_Server.Packets
{
    public class PacketStream
    {
        private PacketStreamData previous_packet = null;
        private PacketStreamData packet = null;
        private bool isSized = false;

        public PacketStream()
        {
        }

        public PacketStream(ref PacketStreamData previous_packet)
        {
            this.previous_packet = previous_packet;
        }

        public PacketStream setDelimeter(byte[] delimeter)
        {
            packet = new PacketStreamData(delimeter);
            this.isSized = false;
            return this;
        }

        public PacketStream writeSubCate(byte subcate)
        {
            packet.Add(subcate);
            return this;
        }

        public PacketStream writePadding(byte length)
        {
            while (length > 0)
            {
                packet.Add(0x00);
                length--;
            };
            return this;
        }

        public PacketStream writeByte(bool data)
        {
            writeByte((byte)(data ? 1 : 0));
            return this;
        }
        public PacketStream writeByte(byte data)
        {
            packet.Add(data);
            return this;
        }

        public PacketStream writeWord(short data)
        {
            return writeWord((ushort)data);
        }
        public PacketStream writeWord(ushort data)
        {
            packet.AddRange(BitConverter.GetBytes(data));
            return this;
        }

        public PacketStream writeDWord(int data)
        {
            return writeDWord((uint)data);
        }
        public PacketStream writeDWord(uint data)
        {
            packet.AddRange(BitConverter.GetBytes(data));
            return this;
        }

        public PacketStream writeString(string data)
        {
            packet.AddRange(Lib.gbkToBytes(data));
            return this;
        }

        public ParamPacketStream writeFormat(EFormatString data)
        {
            return writeFormat((uint)data);
        }

        public ParamPacketStream writeFormat(uint formatStringID)
        {
            this.writeDWord(formatStringID);
            return new ParamPacketStream(ref packet, ref previous_packet);
        }

        public PacketStream writeFragment(IPacketStreamFragment target_fragment)
        {
            var self = this;
            target_fragment.toFragment(ref self);
            this.isSized = false;
            return self;
        }

        public PacketStream nextPacket(bool packSelf = true)
        {
            if (packSelf)
                doPacketSizing();

            // store in previous_packet variable
            if (this.previous_packet == null)
                this.previous_packet = packet;
            else
            {
                this.previous_packet.AddRange(packet);
                this.packet = null; // re-null
            }

            // next packet must start with setDelimeter()
            return this;
        }

        private void doPacketSizing()
        {
            if (packet == null)
            {
                packet = previous_packet;
                return;
            }

            if (isSized)
                return;

            // null-terminator
            packet.Add(0x00);

            // prepend packet size before packet header
            var len = (uint)packet.Count;
            if (len > 0xFF)
            {
                // !REVIEW! confirm size correctness
                len -= 0xFF;
                packet.Insert(0, 0xFF);
                packet.Insert(1, (byte)(len & 0xFF));
            }
            else
                packet.Insert(0, (byte)len);

            isSized = true;
        }

        public PacketStreamData pack(bool doSizing = true)
        {
            if (doSizing)
                doPacketSizing();

            if (this.previous_packet == null)
                return packet;
            else
            {
                this.previous_packet.AddRange(packet);
                return this.previous_packet;
            }
        }

        /// <summary>
        /// Combine two PacketStream into same Stream.
        /// <br />
        /// Both PacketStream will forced to be sized before combine.
        /// </summary>
        /// <param name="left">LHS</param>
        /// <param name="right">RHS</param>
        /// <returns>LHS</returns>
        public static PacketStream combine(PacketStream left, PacketStream right)
        {
            // make sure both are sized.
            left.doPacketSizing();
            right.doPacketSizing();

            // store self current packet in previous_packet variable
            if (left.previous_packet == null)
                left.previous_packet = left.packet;

            // if RHS still have previous_packet,
            //   then add to LHS first
            if (right.previous_packet != null)
                left.previous_packet.AddRange(right.previous_packet);

            // add to LHS previous_packet
            left.previous_packet.AddRange(right.packet);

            // reset LHS
            left.packet = null;
            left.isSized = false;

            // clear RHS
            right.packet = null;
            right.previous_packet = null;
            right.isSized = false;

            return left;
        }

        public static PacketStream operator +(PacketStream left, PacketStreamData right)
        {
            left.packet.AddRange(right);
            return left;
        }

        ~PacketStream()
        {
            packet?.Clear();
            packet = null;
            previous_packet?.Clear();
            previous_packet = null;
        }
    }

    public class ParamPacketStream
    {
        private PacketStreamData previous_packet = null;
        private PacketStreamData packet = null;
        private bool isSized = false;

        public ParamPacketStream(ref PacketStreamData packet, ref PacketStreamData previous_packet)
        {
            this.packet = packet;
            this.previous_packet = previous_packet;
            this.isSized = false;
        }

        public PacketStream nextPacket(bool packSelf = true)
        {
            if (packSelf)
                doPacketSizing();

            // store in previous_packet variable
            if (this.previous_packet == null)
                this.previous_packet = packet;
            else
            {
                this.previous_packet.AddRange(packet);
                this.packet = null; // re-null
            }

            return new PacketStream(ref this.previous_packet);
        }

        public ParamPacketStream writeParam(int data)
        {
            return writeParam((uint)data);
        }
        public ParamPacketStream writeParam(uint data)
        {
            packet.Add(0x64);

            // little-endian
            packet.AddRange(BitConverter.GetBytes(data));
            return this;
        }

        public ParamPacketStream writeParam(short data)
        {
            return writeParam((ushort)data);
        }
        public ParamPacketStream writeParam(ushort data)
        {
            packet.Add(0x64);

            // little-endian
            packet.AddRange(BitConverter.GetBytes(0 + data));
            return this;
        }

        public ParamPacketStream writeParam(byte data)
        {
            packet.Add(0x64);

            // little-endian
            packet.AddRange(BitConverter.GetBytes(data));
            return this;
        }

        public ParamPacketStream writeString(string rawText)
        {
            packet.Add(0x73);

            var converted = Lib.gbkToBytes(rawText);
            // length of bytes (max: UInt32.MAX_VALUE)
            packet.AddRange(BitConverter.GetBytes((uint)converted.Length));
            packet.AddRange(converted);
            return this;
        }

        public ParamPacketStream writePadding(byte length)
        {
            while (length > 0)
            {
                packet.Add(0x00);
                length--;
            };
            return this;
        }

        private void doPacketSizing()
        {
            if (packet == null)
            {
                packet = previous_packet;
                return;
            }

            if (isSized)
                return;

            // null-terminator
            packet.Add(0x00);

            // prepend packet size before packet header
            var len = (uint)packet.Count;
            if (len > 0xFF)
            {
                // !REVIEW! confirm size correctness
                len -= 0xFF;
                packet.Insert(0, 0xFF);
                packet.Insert(1, (byte)(len & 0xFF));
            }
            else
                packet.Insert(0, (byte)len);

            isSized = true;
        }


        public PacketStreamData pack(bool withSizing = true)
        {
            if (withSizing)
                doPacketSizing();

            if (this.previous_packet == null)
                return packet;
            else
            {
                this.previous_packet.AddRange(packet);
                return this.previous_packet;
            }
        }

        ~ParamPacketStream()
        {
            packet?.Clear();
            packet = null;
            previous_packet?.Clear();
            previous_packet = null;
        }
    }

    public class PacketStreamData : List<byte>
    {
        public PacketStreamData() { }

        public PacketStreamData(byte[] delimeter) : base(delimeter) { }

        public static PacketStreamData operator +(PacketStreamData left, PacketStreamData right)
        {
            left.AddRange(right);
            // free RHS for GC
            right.Clear();
            return left;
        }
    }

    public static class ByteExtension
    {
        public static byte[] Append(this byte[] self, byte[] next)
        {
            var pkts = new byte[self.Length + next.Length];
            Array.Copy(pkts, self, self.Length);
            Array.Copy(pkts, self.Length, next, 0, next.Length);
            return pkts;
        }
    }
}
