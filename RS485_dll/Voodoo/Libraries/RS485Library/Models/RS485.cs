using System;
using System.IO.Ports;
using System.Text;
using Voodoo.Libraries.RS485Library.Helpers;

namespace Voodoo.Libraries.RS485Library.Models
{
    public class RS485 : IDisposable
    {
        private SerialPort port;

        public RS485(String portname)
        {
            port = new SerialPort();
            port.WriteTimeout = RS485Constants.DEFAULT_WRITE_TIMEOUT;
            port.ReadTimeout = RS485Constants.DEFAULT_READ_TIMEOUT;
            port.Encoding = Encoding.ASCII;
            port.DataBits = 8;
            port.PortName = portname;
        }

        public void Open()
        {
            port.Open();
        }

        public void Dispose()
        {
            port.Close();
        }

        public void Close()
        {
            Dispose();
        }

        public SerialPort Port
        {
            get { return port; }
        }
    }
}
