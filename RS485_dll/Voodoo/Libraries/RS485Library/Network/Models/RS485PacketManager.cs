using System;
using System.Collections.Generic;
using System.Text;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485PacketManager
    {
        private String text;
        private List<RS485Packet> packets = new List<RS485Packet>();
        private int size;
        private RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData input;
        private bool isRawProcess = false;

        public RS485PacketManager(String text, int size)
        {
            this.text = text;
            this.size = size;
        }

        public RS485PacketManager(RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData input, int size)
        {
            this.input = input;
            this.size = size;
        }

        public RS485PacketManager(RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData input, int size, bool isRawProcess)
        {
            this.input = input;
            this.size = size;
            this.isRawProcess = isRawProcess;
        }

        public RS485Packet FirstPacket
        {
            get
            {
                if (Packets.Count > 0)
                {
                    return Packets[0];
                }
                return null;
            }
        }

        public bool HasPackets()
        {
            return Packets.Count > 0;
        }

        public RS485Packet PopPacket()
        {
            RS485Packet packet = null;
            if (HasPackets())
            {
                packet = Packets[0];
                Packets.Remove(packet);
            }
            return packet;
        }

        private List<RS485Packet> ProcessText()
        {
            return ProcessText(this.text, this.size);
        }

        private List<RS485Packet> ProcessInputData()
        {
            return ProcessInputData(this.input, this.size);
        }

        private List<RS485Packet> ProcessInputRawData()
        {
            return ProcessInputRawData(this.input, this.size);
        }

        public static List<RS485Packet> ProcessText(String text, int size)
        {
            List<RS485Packet> result = new List<RS485Packet>();
            /*while(text.Length != 0)
            {
                if(size > text.Length)
                {
                    size = text.Length;
                }
                String packetData = text.Substring(0, size);
                text = text.Substring(size);
                // RS485Packet packet = new RS485Packet(packetData);
                result.Add(packet);
            }*/
            return result;
        }

        public static List<RS485Packet> ProcessInputData(RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData input, int size)
        {
            List<RS485Packet> result = new List<RS485Packet>();
            while (input.Data.Length != 0)
            {
                if (size > input.Data.Length)
                {
                    size = input.Data.Length;
                }
                String packetData = input.Data.Substring(0, size);
                input.Data = input.Data.Substring(size);
                RS485Packet packet = new RS485Packet(input.Source, input.Current, input.Destination, input.Priority, RS485Packet.CommandEnum.Data, packetData);
                result.Add(packet);
            }
            return result;
        }

        public static List<RS485Packet> ProcessInputRawData(RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData input, int size)
        {
            List<RS485Packet> result = new List<RS485Packet>();
            while (input.Data.Length != 0)
            {
                if (size > input.Data.Length)
                {
                    size = input.Data.Length;
                }
                String packetData = input.Data.Substring(0, size);
                if (input.Data.Length > packetData.Length)
                {
                    input.Data = input.Data.Substring(size + 1, input.Data.Length - packetData.Length - 1);
                }
                else
                { input.Data = String.Empty; }
                RS485Packet packet = new RS485Packet(input.Source, input.Current, input.Destination, input.Priority, RS485Packet.CommandEnum.Data, packetData);
                result.Add(packet);
            }
            return result;
        }

        public List<RS485Packet> Packets
        {
            get
            {
                if (packets.Count == 0)
                {
                    if (!String.IsNullOrEmpty(text))
                    {
                        packets = ProcessText();
                    }
                    else
                    {
                        if (!isRawProcess)
                        {
                            packets = ProcessInputData();
                        }
                        else
                        {
                            packets = ProcessInputRawData();
                        }
                    }
                }
                return packets;
            }
            protected set
            {
                packets = value;
            }
        }
    }
}
