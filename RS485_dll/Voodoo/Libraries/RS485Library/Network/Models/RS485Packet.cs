using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Voodoo.Libraries.RS485Library.Helpers;

namespace Voodoo.Libraries.RS485Library
{
    /// <summary>
    /// Implement Wake protocol.
    /// </summary>
    public class RS485Packet : IEnumerator<byte>, IEnumerable<byte>
    {
        public class TokenBusEnum
        { 
            public const byte FSTART = 0x7E;
            public const byte FEND = 0x7F;
        }

        public class CommandEnum
        { 
            public const byte Poll = 0x01;
            public const byte Successed = 0x02;
            public const byte Data = 0x03;
            public const byte AllowTransmit = 0x04;
            public const byte nil = 0x05;
            public const byte Command = 0x06;
        }

        public class PriorityEnum
        { 
            public const byte Lowest = 0x01;
            public const byte Low = 0x02;
            public const byte High = 0x03;
            public const byte Highest = 0x04;
            public const byte nil = 0x05;
        }

        private String data;
        private int position;
        public String RawData;
        public String source;
        public String destination;
        public byte priority;
        public byte command;
        public String current;

        public const byte FEND = 0xC0;
        public const byte FESC = 0xDB;
        public const byte TFEND = 0xDC;
        public const byte TFESC = 0xDD;
        public const UInt32 MaxSize = 128;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="data"></param>
        public RS485Packet(String source, String current, String destination, byte priority, byte command, String data)
        {
            this.RawData = data;
            this.position = -1;
            // this.data = ReplaceESCSequence(data);
            this.data = data;
            this.source = source;
            this.destination = destination;
            this.priority = priority;
            this.command = command;
            this.current = current;
            // FSTART, COMMAND, PRIORITY, DESTINATION, SOURCE, DATA_LENGTH, DATA
            this.data = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", (char)TokenBusEnum.FSTART, (char)command, (char) priority, destination, source, current, ((char)((byte)data.Length)), data);
        }

        public RS485Packet(String packet)
        {
            this.RawData = packet;
            // TODO: Add default values of packet.
        }

        public String ReplaceESCSequence(String text)
        {
            return text.Replace(GetStringByte(TokenBusEnum.FSTART), String.Format("{0}{1}", GetStringByte(FESC), GetStringByte(TFEND)))
                       .Replace(GetStringByte(TokenBusEnum.FEND), String.Format("{0}{1}", GetStringByte(FESC), GetStringByte(TFESC)));
        }

        public String GetStringByte(byte code)
        {
            return Convert.ToString((char) code);
        }

        public static String GetValidData(String text, byte startDelimiter, byte endDelimiter)
        {
            return text.Replace(Convert.ToString((char) startDelimiter) + Convert.ToString((char) startDelimiter),
                                Convert.ToString((char) startDelimiter))
                       .Replace(Convert.ToString((char) endDelimiter) + Convert.ToString((char) endDelimiter),
                                Convert.ToString((char) endDelimiter));
        }

        public static String GetValidData(String text)
        {
            return GetValidData(text, (byte)TokenBusEnum.FSTART, (byte) TokenBusEnum.FEND);
        }

        public RS485Packet ToReceivedPacket()
        {
            // this.data = GetValidData(data, startDelimiter, endDelimiter);
            return this;
        }

        byte IEnumerator<byte>.Current
        {
            get { return (byte) data[position]; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            position++;
            if (position >= data.Length)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current
        {
            get
            {
                return (byte) data[position];
            }
        }

        public int Position
        {
            get { return position; }
        }

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return this;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<byte>) this).GetEnumerator();
        }

        public String Data
        {
            get { return data;}
        }

        public override bool Equals(object obj)
        {
            RS485Packet packet = (RS485Packet) obj;
            return packet.RawData == this.RawData &&
                   packet.source == this.source &&
                   packet.destination == this.destination;
        }
    }
}
