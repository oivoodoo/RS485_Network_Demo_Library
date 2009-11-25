using System;
using System.Collections.Generic;
using System.Text;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Helpers;
using Voodoo.Libraries.RS485Library.Models;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    public class RS485VooServer : RS485PacketServer
    {
        public RS485VooServer()
            : base()
        {
        }

        public RS485VooServer(RS485 rs)
            : base(rs)
        { }

        public RS485VooServer(String name) : base(name)
        {
        }

        public RS485VooServer(String name, String portname)
            : base(name, portname)
        {
        }

        protected override bool AnalyzeCommand(RS485Packet packet)
        {
            return VooServer.Instance.AnalyzeCommand(packet);
        }
    }
}
