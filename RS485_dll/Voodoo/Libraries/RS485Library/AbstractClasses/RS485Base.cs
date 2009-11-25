using System;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Models;

namespace Voodoo.Libraries.RS485Library.AbstractClasses
{
    public abstract class RS485Base : IDisposable
    {
        protected RS485 rs;
        protected RS485Events events;

        public RS485 RS
        {
            get { return rs; }
            set { rs = value; }
        }

        public RS485Base()
        {
            events = new RS485Events();
        }

        public RS485Base(RS485 rs) : this()
        {
            this.rs = rs;

            rs.Port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(Port_ErrorReceived);
        }

        public RS485Base(String portname) : this()
        {
            rs = new RS485(portname);

            rs.Port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(Port_ErrorReceived);
        }

        protected virtual void Port_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            RS485Events.Output(this, new RS485Events.OutputEventArg("Data received error", RS485Events.OutputType.Error));
        }

        public RS485Events Events
        {
            get { return events; }
        }

        public virtual void Open()
        {
            if (rs != null && !rs.Port.IsOpen)
            {
                rs.Open();
            }
        }

        public void Dispose()
        {
            if (rs != null && rs.Port.IsOpen)
            {
                rs.Close();
            }
        }
    }
}
