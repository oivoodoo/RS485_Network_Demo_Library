using System;
using System.Collections.Generic;
using System.Text;

namespace Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class Session
    {
        private String source;
        private String destination;

        public Session(String source, String destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public String Source
        {
            get { return source; }
        }

        public String Destination
        {
            get { return destination; }
        }

        public override string ToString()
        {
            return String.Format("{0}_{1}", source, destination);
        }
    }
}
