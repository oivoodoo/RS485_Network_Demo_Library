using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class ProtocolToken
    {
        private ProtocolDictionary parameters;
        private String pattern;

        public ProtocolToken(String pattern, ProtocolDictionary parameters)
        {
            this.pattern = pattern;
            this.parameters = parameters;
        }

        public ProtocolToken(String pattern)
        {
            this.pattern = pattern;
            this.parameters = new ProtocolDictionary();
        }

        public String Pattern
        {
            get { return pattern; }
        }

        /// <summary>
        /// It's mapping for protocol params.
        /// </summary>
        public ProtocolDictionary Parameters
        {
            get { return parameters; }
        }

        internal void InitializeParams(GroupCollection groups)
        {
            int iterator = 0;
            foreach (Group group in groups)
            {
                if (iterator != 0)
                {
                    foreach (Capture capture in group.Captures)
                    {
                        parameters.Set(capture.Value, iterator - 1);
                    }
                }
                iterator++;
            }
        }
    }
}
