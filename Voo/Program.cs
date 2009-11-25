using System;
using System.Collections.Generic;
using System.Text;
using RS485_dll.Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.Logs;
using System.Text.RegularExpressions;
using System.Threading;

namespace VooTerminalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                bool hasMarker = false;
                if (args.Length > 2)
                {
                    hasMarker = Convert.ToBoolean(args[2]);
                }
                RS485VooTerminal terminal = new RS485VooTerminal(args[0], args[1], hasMarker);
                try
                {
                    terminal.Start();
                }
                catch (ApplicationException ex)
                { 
                    Console.Clear();
                    Console.WriteLine("Bye - Bye. See you later.\n Wrote by Voodoo(C)");
                    Thread.Sleep(1000);
                }
            }
            else
            {
                Log.CoreLog.Warning("Invalid data in command line. See example: <program> <host_address>.");
                throw new System.Exception("Invalid data in command line. See example: <program> <host_address>.");
            }
        }
    }
}
