using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    public class VooConfiguration
    {
        public const String HelpFile = "voo.txt";

        private static String help = String.Empty;

        /// <summary>
        /// Open files only if exists.
        /// </summary>
        internal static String GetFileData(String filename)
        { 
            String data = String.Empty;
            using(StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        public static String Help
        {
            get 
            {
                if (String.IsNullOrEmpty(help))
                {
                    help = GetFileData(HelpFile);
                }
                return help;
            }
        }
    }
}
