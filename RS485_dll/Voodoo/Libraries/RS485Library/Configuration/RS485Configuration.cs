using System;
using System.Collections.Generic;
using System.Text;

namespace RS485_dll.Voodoo.Libraries.RS485Library.Configuration
{
    public class RS485Configuration
    {
        public static readonly String[] DefaultAddresses = new String[] { "1", "2", "3" };
        private List<String> addresses;
        private int currentAddress;

        public RS485Configuration(String[] addresses, int currentAddress)
        {
            this.addresses = new List<string>(addresses);
            this.currentAddress = currentAddress;
        }

        public String NextAddress()
        {
            return Addresses[EnsurePosition(currentAddress, 1)];
        }

        public String PreviousAddress()
        {
            return Addresses[EnsurePosition(currentAddress, -1)];
        }

        private int EnsurePosition(int index, int value)
        { 
            index += value;
            if (index < 0)
            {
                index = addresses.Count - 1;
            }
            else if (index >= addresses.Count)
            {
                index = 0;
            }
            return index;
        }

        public List<string> Addresses
        {
            get
            {
                return addresses;
            }
        }
    }
}
