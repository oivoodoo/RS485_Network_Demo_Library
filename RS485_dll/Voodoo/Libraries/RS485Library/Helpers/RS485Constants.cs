using System;

namespace Voodoo.Libraries.RS485Library.Helpers
{
    public class RS485Constants
    {
        public const String DEFAULT_SERVER_PORT = "COM7";
        public const String DEFAULT_CLIENT_PORT = "COM8";

        public const int DEFAULT_WRITE_TIMEOUT = 500;
        public const int DEFAULT_READ_TIMEOUT = 500;

        public const byte START_DELIMITER = (byte)'#';
        public const byte END_DELIMITER = (byte)'$';

        public enum Machine
        { 
            Server = 0x01,
            Client = 0x02,
            ServerAndClient = 0x04
        }
    }
}
