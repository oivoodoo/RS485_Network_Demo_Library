using System;
using System.Collections.Generic;
using System.Text;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class ExtendedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        protected List<String> mapping = new List<String>();

        public new ExtendedDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            mapping.Add(key.ToString());
            base.Add(key, value);
            return this;
        }
    }
}
