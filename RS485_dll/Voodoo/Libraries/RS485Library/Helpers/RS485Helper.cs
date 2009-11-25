using System;

namespace Voodoo.Libraries.RS485Library.Helpers
{
    public static class RS485Helper
    {
        public static String ConvertToString(byte[] bytes)
        {
            String result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString();
            }
            return result;
        }
    }
}
