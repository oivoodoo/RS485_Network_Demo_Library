using System;
using Voodoo.Libraries.RS485Library.AbstractClasses;
using Voodoo.Libraries.RS485Library.Helpers;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485Client : RS485Base
    {
        public RS485Client(String portname) : base(portname)
        {
        }

        public virtual void Send(byte data)
        {
            byte[] buffer = new byte[] {data};
            rs.Port.Write(buffer, 0, 1);
        }
    }
}
