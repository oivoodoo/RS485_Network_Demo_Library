using System;
using System.Collections.Generic;
using System.Text;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    public abstract class Protocol
    {
        private String rawData;

        public abstract bool AnalyseRawData(String rawData);

        public String RawData
        {
            get
            {
                return rawData;
            }
            set 
            {
                this.rawData = value;
            }
        }
    }
}
