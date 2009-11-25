using System;
using System.Threading;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Helpers;
using RS485_dll.Voodoo.Libraries.RS485Library;
using RS485_dll.Voodoo.Libraries.RS485Library.Configuration;
using test;

namespace TestRS485
{
    class Program
    {
        // command arguments: <port> <computer_id> <has marker>
        static void Main(string[] args)
        {
            new VoodooTest().Run();
        }
    }
}