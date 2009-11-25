using System;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485Events
    {
        public static Object synchronized = new object();

        public delegate void OutputDelegate(Object sender, OutputEventArg arg);
        public delegate void ReceivedPacketsDelegate(Object sender, ReceivedPacketsArg arg);
        public delegate void AllowNextDelegate(Object sender, ReceivedPacketsArg arg);
        public delegate void EnableMarkerDelegate();
        public delegate void SendNextDelegate(RS485Packet packet);
        public delegate void AnalysePacketDelegate(RS485Packet packet);
        public delegate void CheckLastPacketDelegate(RS485Packet packet);
        public delegate void SetIgnorePacketDelegate(bool isIgnore);
        public delegate void RemovePacketAfterSuccessedDelegate(RS485Packet packet);

        public static event OutputDelegate OutputEvent;
        public static event ReceivedPacketsDelegate ReceivedPacketsEvent;
        public static event AllowNextDelegate AllowNextEvent;
        public static EnableMarkerDelegate EnableMarkerEvent;
        public static SendNextDelegate SendNextEvent;
        public static AnalysePacketDelegate AnalysePacketEvent;
        public static CheckLastPacketDelegate CheckLastPacketEvent;
        public static SetIgnorePacketDelegate SetIgnorePacketEvent;
        public static RemovePacketAfterSuccessedDelegate RemovePacketAfterSuccessedEvent;

        public static void RemovePacketAfterSuccessed(RS485Packet packet)
        {
            RemovePacketAfterSuccessedDelegate local = RemovePacketAfterSuccessedEvent;
            if (local != null)
            {
                local(packet);
            }
        }

        public static void SetIgnorePacket(bool isIgnore)
        {
            SetIgnorePacketDelegate local = SetIgnorePacketEvent;
            if (local != null)
            {
                local(isIgnore);
            }
        }

        public static void CheckLastPacket(RS485Packet packet)
        {
            CheckLastPacketDelegate local = CheckLastPacketEvent;
            if (local != null)
            {
                local(packet);
            }
        }

        public static void AnalysePacket(RS485Packet packet)
        {
            AnalysePacketDelegate local = AnalysePacketEvent;
            if (local != null)
            {
                local(packet);
            }
        }

        public static void SendNext(RS485Packet packet)
        {
            SendNextDelegate local = SendNextEvent;
            if (local != null)
            {
                local(packet);        
            }
        }

        public static void AllowNext(Object sender, ReceivedPacketsArg arg)
        {
            AllowNextDelegate local = AllowNextEvent;
            if (local != null)
            {
                local(sender, arg);
            }
        }

        public static void EnableMarker()
        { 
            EnableMarkerDelegate local = EnableMarkerEvent;
            if (local != null)
            {
                local();
            }
        }

        public static void Output(Object sender, OutputEventArg arg)
        {
            if (OutputEvent != null)
            {
                OutputEvent(sender, arg);
            }
        }

        public static void ReceivedPackets(Object sender, ReceivedPacketsArg arg)
        {
            if (ReceivedPacketsEvent != null)
            {
                ReceivedPacketsEvent(sender, arg);
            }
        }

        public class ReceivedPacketsArg : EventArgs
        {
            private RS485Packet packet;

            public ReceivedPacketsArg(RS485Packet packet)
            {
                this.packet = packet;
            }

            public RS485Packet Packet
            {
                get { return this.packet; }
            }
        }

        public class OutputEventArg : EventArgs
        {
            private String message;
            private OutputType type;
            private DestinationType destinationType;

            public OutputEventArg(String message, OutputType type)
            {
                this.message = message;
                this.type = type;
                this.destinationType = RS485Events.DestinationType.None;
            }

            public OutputEventArg(String message, OutputType type, DestinationType destinationType) : this(message, type)
            {
                this.destinationType = destinationType;
            }

            public String Message
            {
                get { return message; }
                set { this.message = value; }
            }

            public OutputType Type
            {
                get { return type; }
                set { type = value; }
            }

            public DestinationType DestinationType
            {
                get { return destinationType; }
                set { destinationType = value; }
            }
        }

        public enum OutputType
        {
            Common,
            Warning,
            Error
        }

        public enum DestinationType
        {
            Server,
            Client,
            ServerPacket,
            ClientPacket,
            None
        }
    }
}
