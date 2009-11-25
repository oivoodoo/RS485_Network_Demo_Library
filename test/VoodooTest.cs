using System;
using System.Collections.Generic;
using System.Text;
using RS485_dll.Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network;

namespace test
{

    // ("{0}{1}{2}{3}{4}{5}{6}{7}", 
    //   FSTART command priority destination source current data.Length data;
    public class VoodooTest
    {
        protected String DefaultPacket = String.Format("{0}{1}{2}{3}{4}{5}", 
            RS485Packet.TokenBusEnum.FSTART, RS485Packet.CommandEnum.Command,
            RS485Packet.PriorityEnum.Highest, "1", "1", "1");

        protected VooServer server = VooServer.Instance;
        protected VooTerminal terminal = VooTerminal.Instance;

        public void Run()
        {
            RS485PacketManager manager = new RS485PacketManager(
                new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(RS485Packet.PriorityEnum.Highest,
                   "aaaaaaaaaaaaaaaaaaaaaaaab", "1", "2", "1"), 8, true);
            List<RS485Packet> packets = manager.Packets;

            // Push command
            Test3();
            // CR Session
            /*
            Test1();
            // CR Application
            Test2();
            // CR Authentication
            
            // Fetch command
            Test4();
            // Dir
            Test5();
            // Cd
            Test6();
            //
            Test7();
            // Dir
            Test8();
            // Cd
            Test9();
            */
        }

        public void Test1()
        {
            StartBorder(1);
            AddAuth();
            Auth();
            BeginSession();
            EndSession();
            RemoveAuth();
            EndBorder(1);
        }

        public void Test2()
        {
            StartBorder(2);
            CreateApplication();
            RemoveApplication();
            EndBorder(2);
        }

        public void Test3()
        {
            StartBorder(3);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();

            // C:
            Push();
            Data(); // 4 bytes |
            Data(); // 4 bytes |__8 bytes
            // S:

            EndSession();
            RemoveAuth();
            RemoveApplication();

            EndBorder(3);
        }

        public void Test4()
        {
            StartBorder(4);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();

            // C:
            Fetch();
            // Принимающий после получения данной команды
            // должен сделать push но с парамертрами из fetch.
            Push();
            Data(); // 4 bytes |
            Data(); // 4 bytes |__8 bytes
            // S:

            EndSession();
            RemoveAuth();
            RemoveApplication();

            EndBorder(4);
        }

        // DIR
        public void Test5()
        {
            StartBorder(5);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();
            Dir();
            EndSession();
            RemoveAuth();
            RemoveApplication();
            EndBorder(5);
        }

        // CD
        public void Test6()
        {
            StartBorder(6);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();
            Cd();
            EndSession();
            RemoveAuth();
            RemoveApplication();
            EndBorder(6);
        }

        // CD
        public void Test7()
        {
            StartBorder(7);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();
            Cd2();
            EndSession();
            RemoveAuth();
            RemoveApplication();
            EndBorder(7);
        }

        // CD
        public void Test8()
        {
            StartBorder(8);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();
            Cd3();
            EndSession();
            RemoveAuth();
            RemoveApplication();
            EndBorder(8);
        }

        // DIR
        public void Test9()
        {
            StartBorder(9);
            CreateApplication();
            AddAuth();
            Auth();
            BeginSession();
            Dir2();
            EndSession();
            RemoveAuth();
            RemoveApplication();
            EndBorder(9);
        }

        public void CreateApplication()
        {
            Analyze("_create -a=C:\\");
        }

        public void RemoveApplication()
        {
            Analyze("_remove -a=C:\\");
        }

        public void Exit()
        {
            Analyze("_exit");
        }

        public void Help()
        {
            string command = "_help";
            Analyze(command);
        }

        public void AddAuth()
        {
            Analyze("_add a -u=admin -p=admin");
        }

        public void RemoveAuth()
        {
            Analyze("_remove a -u=admin -p=admin");
        }

        public void Push()
        {
            Analyze("_push -s=1 -f=C:\\1.txt -t=C:\\2.txt -s=8");
        }

        public void Data()
        {
            Analyze("%!90_()aaaa");
        }

        public void Fetch()
        {
            Analyze("_fetch -s=1 -f=C:\\1.txt -t=C:\\2.txt");
        }

/*
        public void Console()
        {
            // TODO: Add support to work directly with command line.
        }
*/

        public void BeginSession()
        {
            Analyze("_begin_session -m=1");
        }

        public void EndSession()
        {
            Analyze("_end_session -m=1");
        }

        public void Dir()
        {
            // Use current session to output directory information.
            Analyze("_dir -a=C:\\");
        }

        public void Dir2()
        {
            // Use current session to output directory information.
            Analyze("_dir -a=C:\\windows");
        }

        public void Cd()
        {
            Analyze("_cd -f=temp");
        }

        public void Cd2()
        {
            Analyze("_cd -f=..");
        }

        public void Cd3()
        {
            Analyze("_cd --folder=windows");
        }

/*
 *      Now we are using push command to upload data.
 *      Please see more information in the newest voo specification.
        public void Upload()
        { }
*/

        public void Auth()
        {
            String auth = "_auth -u=admin -p=admin";
            Analyze(auth);
        }

        public void Analyze(string command)
        {
            VooServer.Instance.AnalyzeCommand(new RS485Packet(command));
        }

        private static void AssertEqual(String text, bool result, bool value)
        {
            Console.WriteLine("{0}: {1}", text, result == value);
        }

        #region [  Graphic Helpers  ]

        private void StartBorder(int border)
        {
            Console.WriteLine("-----===================== Begin Test {0} =====================-----", border);
        }

        private void EndBorder(int border)
        {
            Console.WriteLine("-----===================== End Test {0} =====================-----\n", border);
        }

        #endregion
    }
}
