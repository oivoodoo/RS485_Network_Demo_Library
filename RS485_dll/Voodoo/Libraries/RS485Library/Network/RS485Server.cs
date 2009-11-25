using System;
using System.IO.Ports;
using System.Threading;
using Voodoo.Libraries.RS485Library.AbstractClasses;
using Voodoo.Libraries.RS485Library.Helpers;
using Voodoo.Libraries.RS485Library.Models;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485Server : RS485Base
    {
        protected Thread serverThread;
        protected byte[] buffer;

        public RS485Server(String portname) : base(portname)
        {
            rs.Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Port_DataReceived);
        }

        public RS485Server(RS485 rs) : base(rs)
        {
            rs.Port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Port_DataReceived);
        }

        protected virtual void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort) sender;
            buffer = new byte[port.BytesToRead];
            port.Read(buffer, 0, port.BytesToRead);
        }

        public void Loop()
        {
            while(true)
            {
                Thread.Sleep(300);
            }
        }
    }
}
