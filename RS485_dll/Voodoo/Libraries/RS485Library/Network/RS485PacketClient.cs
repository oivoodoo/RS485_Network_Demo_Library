using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Voodoo.Libraries.RS485Library.Helpers;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485PacketClient : RS485Client
    {
        public RS485PacketClient() : base(RS485Constants.DEFAULT_CLIENT_PORT)
        {
        }

        public RS485PacketClient(String portname) : base(portname)
        {
        }

        public virtual void Send(RS485Packet packet)
        {
            RS485Events.SetIgnorePacket(true);
            lock (RS485Events.synchronized)
            {
                rs.Port.RtsEnable = true;
                foreach (byte b in packet)
                {
                    Send(b);
                }
                Thread.Sleep(400);
                rs.Port.RtsEnable = false;
                // events.Output(this, new RS485Events.OutputEventArg(packet.Data, RS485Events.OutputType.Common, RS485Events.DestinationType.ClientPacket));
            }
        }
    }
}
