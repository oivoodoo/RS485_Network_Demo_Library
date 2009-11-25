using System;
using System.Collections.Generic;
using System.Text;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class ProtocolDictionary : ExtendedDictionary<String, String>
    {
        public ProtocolDictionary Set(String value, int index)
        {
            if (mapping.Count > index)
            {
                this[mapping[index]] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("You are using illegal index in mapping of fields.");
            }
            return this;
        }

        public ProtocolDictionary Add(String key)
        {
            base.Add(key, String.Empty);
            return this;
        }
    }
}
