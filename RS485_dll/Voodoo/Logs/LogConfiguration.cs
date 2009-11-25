using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Voodoo.Libraries.Logs
{
    public static class LogConfiguration
    {
        private static String userApplicationFolder;
        public const String LogFolder = "logs";

        public static String UserApplicationFolder
        {
            get
            {
                if (String.IsNullOrEmpty(userApplicationFolder))
                {
                    userApplicationFolder = String.Format("{0}\\{1}", System.Environment.CurrentDirectory, LogFolder);

                    if (!Directory.Exists(LogFolder))
                    {
                        if (Directory.CreateDirectory(userApplicationFolder) != null)
                        {
                            throw new ApplicationException("Logs folder can't be create. Please ensure your rights in current folder or call for administrator.");
                        }
                    }
                }
                return userApplicationFolder;
            }
        }
    }
}
