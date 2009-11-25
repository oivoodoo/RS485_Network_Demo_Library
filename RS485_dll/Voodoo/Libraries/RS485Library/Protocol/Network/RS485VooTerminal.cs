using System;
using System.Collections.Generic;
using System.Text;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network;
using Voodoo.Libraries.RS485Library.Helpers;
using Voodoo.Libraries.RS485Library.Protocol.Models;
using System.Diagnostics;
using System.IO;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models;
using System.Threading;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    public class RS485VooTerminal : RS485Terminal
    {
        private User user;
        private Session session;
        private String application = String.Empty;
        private Thread menu;

        private readonly String[] Menus = 
            {"1. Chat.", 
             "2. Push.", 
             "3. Fetch.", 
             "4. Terminal.", 
             "5. Help.", 
             "6. Add user.",
             "7. Remove user.",
             "8. Set session.",
             "--------------"};


        public RS485VooTerminal(String port, String source, bool hasMarker)
            : base(port, source, hasMarker)
        {
            RS485VooEvents.Instance.SendPacketsEvent += new RS485VooEvents.SendPacketsDelegate(SendPackets);
            // Неявно создаём объекты, чтобы подписаться на события.
            VooServer vooServer = VooServer.Instance;
            VooTerminal terminal = VooTerminal.Instance;
        }

        protected override void StartServer()
        {
            //  Данный метод пришлось переопределить,
            // так как наша модель немного изменилась.
            using (server = new RS485VooServer(client.RS))
            {
                server.name = source;
                RS485Events.OutputEvent += new RS485Events.OutputDelegate(Events_OutputEvent);
                server.Loop();
            }
        }

        protected void SendPackets(RS485PacketManager manager)
        {
            lock (RS485Events.synchronized)
            {
                Managers.Add(manager);
            }
        }

        /// <summary>
        /// Override client work process.
        /// </summary>
        protected override RS485Terminal.InputData GetInputText()
        {
            if (menu == null || !menu.IsAlive)
            {
                menu = new Thread(new ThreadStart(DrawMenu));
                menu.Start();
            }
            return null;
        }

        private void DrawMenu()
        {
            foreach (String menu in Menus)
            {
                Console.WriteLine(menu);
            }
            switch(Console.ReadLine().Trim())
            {
                case "1":
                    ChatAction();
                    break;
                case "2":
                    SendFileAction();
                    break;
                case "3":
                    FetchFileAction();
                    break;
                case "4":
                    TerminalAction();
                    break;
                case "5":
                    Console.WriteLine(VooConfiguration.Help);
                    Console.ReadLine();
                    break;
                case "6":
                    AddUserAction();
                    break;
                case "7":
                    AuthUserAction();
                    break;
                case "8":
                    SetSessionAction();
                    break;
            }
        }

        private InputData ChatAction()
        {
            if (session != null)
            {
                Console.WriteLine("Data:");
                String data = Console.ReadLine();
                RS485PacketManager manager = new RS485PacketManager(
                            new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(
                                RS485Packet.PriorityEnum.High, data,
                                session.Source, session.Destination, session.Source),
                            32);
                // lock (RS485Events.synchronized)
                {
                    Managers.Add(manager);
                }
            }
            else
            {
                Console.WriteLine("Please input session information.");
            }
            return null;
        }

        private void SendFileAction()
        {
            if (session != null)
            {
                // TODO: Create custom packets for sending files.
                Console.Write("Push: ");
                String from = Console.ReadLine();
                Console.Write("To: ");
                String to = Console.ReadLine();
                if (File.Exists(from))
                {
                    VooFile file = new VooFile(session.Source, session.Destination, from, to);
                    Console.WriteLine("Application: SendFileAction() -> read file('{0}')", from);
                    file.ReadToEnd();
                    // lock (RS485Events.synchronized)
                    {
                        RS485PacketManager manager = new RS485PacketManager(
                            new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(
                                RS485Packet.PriorityEnum.High, file.Data, 
                                session.Source, session.Destination, session.Source), 
                            32);
                        RS485Packet packet = new RS485Packet(session.Source, session.Source, session.Destination, RS485Packet.PriorityEnum.High, 
                            RS485Packet.CommandEnum.Data, VooProtocol.CreatePushCommand(session.ToString(), from, to, Convert.ToString(file.Data.Length)));
                        manager.Packets.Insert(0, packet);
                        Managers.Add(manager);
                    }
                }
                else
                {
                    Console.WriteLine("File doesn't exist.");
                }
            }
            else
            { 
                Console.WriteLine("Please input session information."); 
            }
        }

        private void FetchFileAction()
        {
            if (session != null)
            {
                Console.Write("Download: ");
                String from = Console.ReadLine();
                Console.Write("To: ");
                String to = Console.ReadLine();
                RS485PacketManager manager = new RS485PacketManager(
                    new InputData(RS485Packet.PriorityEnum.High,
                        VooProtocol.CreateFetchCommand(session.ToString(), from, to),
                        session.Source, session.Destination, session.Source),
                    32);
               // lock (RS485Events.synchronized)
                {
                    Managers.Add(manager);
                }
            }
            else
            { 
                Console.WriteLine("Please input session information."); 
            }
        }

        // TODO: Using this method you can perform low protocol level
        //  commands such as '/auth --username="user1" --password="password"'
        //  and so on.
        private void TerminalAction()
        {
            if (session != null)
            {
                Console.Write(">");
                String command = Console.ReadLine();
                RS485PacketManager manager = new RS485PacketManager(
                    new InputData(RS485Packet.PriorityEnum.High, command, session.Source, session.Destination, session.Source),
                    32);
              //  lock (RS485Events.synchronized)
                {
                    Managers.Add(manager);
                }
            }
            else
            {
                Console.WriteLine("Please input session information.");
            }
        }

        private void AddUserAction()
        {
            if (session != null)
            {
                Console.WriteLine("username: ");
                String username = Console.ReadLine();
                Console.WriteLine("password: ");
                String password = Console.ReadLine();
                user = new User(username.Trim(), password.Trim());
                if (user.IsValid())
                {
                    RS485PacketManager manager = new RS485PacketManager(
                        new InputData(RS485Packet.PriorityEnum.High, VooProtocol.CreateAddUser(user.UserName, user.Password), session.Source, session.Destination, session.Source),
                        32);
                    RS485Packet packet = new RS485Packet(session.Source, session.Source, session.Destination, RS485Packet.PriorityEnum.High,
                                        RS485Packet.CommandEnum.Data, VooProtocol.CreateAuthUser(user));
                    manager.Packets.Add(packet);
            //        lock (RS485Events.synchronized)
                    {
                        Managers.Add(manager);
                    }
                }
            }
            else
            {
                Console.WriteLine("Please input session information.");
            }
        }

        private void SetSessionAction()
        {
            RS485PacketManager manager;
            if (session != null)
            {
                // Send request to remove new session.
                manager = new RS485PacketManager(
                    new InputData(RS485Packet.PriorityEnum.High, VooProtocol.CreateEndSessionCommand(session), session.Source, session.Destination, session.Source), 
                    32);
                lock (RS485Events.synchronized)
                {
                    
                    Managers.Add(manager);
                }
            }
            Console.Write("Destination = ");
            String destination = Console.ReadLine().Trim();
            // Console.Write("Entry application = ");
            // application = Console.ReadLine().Trim();
            application = @"C:\";
            session = new Session(source, destination);
            // Send request to begin new session.
            manager = new RS485PacketManager(
                   new InputData(RS485Packet.PriorityEnum.High, VooProtocol.CreateBeginSessionCommand(session), session.Source, session.Destination, session.Source),
                   32);
            RS485Packet packet = new RS485Packet(session.Source, session.Source, session.Destination, RS485Packet.PriorityEnum.High,
                                        RS485Packet.CommandEnum.Data, VooProtocol.CreateCApplicationCommand(application));
            manager.Packets.Add(packet);
       //     lock (RS485Events.synchronized)
            {
                Managers.Add(manager);
            }
        }

        private void AuthUserAction()
        {
            if (user.IsValid())
            {
                RS485PacketManager manager = new RS485PacketManager(
                   new InputData(RS485Packet.PriorityEnum.High, VooProtocol.CreateAuthUser(user.UserName, user.Password), session.Source, session.Destination, session.Source),
                   32);
                lock (RS485Events.synchronized)
                {
                    Managers.Add(manager);
                }
            }
        }
    }
}
