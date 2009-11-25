using System;
using System.Collections.Generic;
using System.Text;
using RS485_dll.Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Protocol.Models;
using Voodoo.Libraries.RS485Library.Helpers;
using System.Diagnostics;
using System.IO;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models;
using Voodoo.Libraries.RS485Library;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network
{
    /// <summary>
    /// Classes VooTerminal and VooServer split abstraction layer of voodoo protocol.
    /// There is only for flexible model testing.
    /// </summary>
    public class VooTerminal
    {
        #region [  Protocol Connection Fields  ]

        protected List<User> users;
        protected User currentUser;
        protected List<VooFile> pushes = new List<VooFile>();
        protected List<VooFile> fetches = new List<VooFile>();

        // Store sessions by machine name.
        // All machines has to presence names.
        protected List<String> sessions;
        protected String application;
        protected String filename;
        protected String machine;
        protected String currentApplication;

        #endregion

        protected VooTerminal()
        {
            users = new List<User>();
            sessions = new List<String>();

            InitializeEvents();
        }

        protected void InitializeEvents()
        {
            RS485VooEvents.Instance.OutputDataEvent += new RS485VooEvents.OutputDataDelegate(OutputData);
            RS485VooEvents.Instance.TerminateMachineEvent += new RS485VooEvents.TerminateMachineDelegate(TerminateMachine);
            RS485VooEvents.Instance.BeginSessionEvent += new RS485VooEvents.BeginSessionDelegate(BeginSession);
            RS485VooEvents.Instance.EndSessionEvent += new RS485VooEvents.EndSessionDelegate(EndSession);
            RS485VooEvents.Instance.ExecuteCommandEvent += new RS485VooEvents.ExecuteCommandDelegate(ExecuteCommand);
            RS485VooEvents.Instance.FetchEvent += new RS485VooEvents.FetchDelegate(Fetch);
            RS485VooEvents.Instance.LoginUserEvent += new RS485VooEvents.LoginUserDelegate(LoginUser);
            // Пришла команда push, значит нам требуется создать туннель для передачи данных.
            // Мы знаем информацию о компьютере отправителе. Тем самым можем определить сессию данных.
            // Работая в данной сессии заливать данные в нужным нам файлик до полного завершения.
            RS485VooEvents.Instance.PushEvent += new RS485VooEvents.PushDelegate(Push);
            RS485VooEvents.Instance.RegisterUserEvent += new RS485VooEvents.RegisterUserDelegate(RegisterUser);
            RS485VooEvents.Instance.UnRegisterUserEvent += new RS485VooEvents.UnRegisterUserDelegate(UnRegisterUser);
            RS485VooEvents.Instance.CreateApplicationEvent += new RS485VooEvents.CreateApplicationDelegate(CreateApplication);
            RS485VooEvents.Instance.RemoveApplicationEvent += new RS485VooEvents.RemoveApplicationDelegate(RemoveApplication);
            RS485VooEvents.Instance.ErrorEvent += new RS485VooEvents.ErrorDelegate(Error);
            RS485VooEvents.Instance.ReceiveDataEvent += new RS485VooEvents.ReceiveDataDelegate(ReceiveData);
        }

        public void TerminateMachine(RS485Constants.Machine machine)
        {
            switch (machine)
            {
                case RS485Constants.Machine.Client:
                case RS485Constants.Machine.Server:
                case RS485Constants.Machine.ServerAndClient:
                    // Stop();
                    break;
            }
        }

        public void OutputData(String text)
        {
            // Console.WriteLine("Application: OutputData(String text)");
            Console.WriteLine(text);
        }

        protected void BeginSession(VooProtocol protocol)
        {
            Console.WriteLine("Application: BeginSession(VooProtocol protocol)");
            machine = protocol.Get("machine");
            Console.WriteLine("Application: BeginSession -> machine is {0}.", machine);

            // Store last machine.
            if (currentUser != null && !String.IsNullOrEmpty(machine))
            {
                sessions.Add(machine);
                Console.WriteLine("Application: BeginSession -> Add machine('{0}') to session.", machine);
                Console.WriteLine("Application: BeginSession -> Done.", machine);
            }
        }

        protected void EndSession(VooProtocol protocol)
        {
            Console.WriteLine("Application: EndSession(VooProtocol protocol)");
            machine = protocol.Get("machine");
            Console.WriteLine("Application: EndSession -> Remove machine('{0}') from sessions.", machine);
            if (!String.IsNullOrEmpty(machine) && sessions.Count > 0)
            {
                sessions.Remove(machine);
                machine = null;
                Console.WriteLine("Application: EndSession -> Done.", machine);
            }
        }

        protected void ExecuteCommand(VooProtocol protocol)
        {
            Console.WriteLine("Application: ExecuteCommand(VooProtocol protocol)");

            String result = String.Empty;
            switch (protocol.CurrentPacket.Key)
            {
                case VooProtocol.Commands.CLIENT_CD:
                    String temp = protocol.Get("folder");
                    if (temp == "..")
                    {
                        if (application.Split('\\').Length > 1)
                        {
                            application = application.Substring(0, application.LastIndexOf('\\') + 1);
                        }
                    }
                    else
                    {
                        application = Path.Combine(application, temp);
                    }
                    RS485VooEvents.Instance.SendPackets(new RS485PacketManager(
                        new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(RS485Packet.PriorityEnum.Highest,
                            DirAction(application), protocol.Destination, protocol.Source, protocol.Destination), 64));
                    break;
                case VooProtocol.Commands.CLIENT_DIR:
                    RS485VooEvents.Instance.SendPackets(new RS485PacketManager(
                        new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(RS485Packet.PriorityEnum.Highest,
                            DirAction(Path.Combine(application, protocol.Get("application"))), protocol.Destination, protocol.Source, protocol.Destination), 64));
                    break;
            }
            RS485VooEvents.Instance.OutputData(result);
        }

        private String DirAction(String application)
        {
            String collector = String.Empty;
            Console.WriteLine("DirAction -> application = '{0}'", application);
            foreach(String directory in Directory.GetDirectories(application))
            {
                Console.WriteLine(directory.ToUpper());
                collector += directory.ToUpper() + Environment.NewLine;
            }
            foreach (String file in Directory.GetFiles(application))
            {
                Console.WriteLine(Path.GetFileName(file.ToLower()));
                collector += file.ToLower() + Environment.NewLine;
            }
            return collector;
        }

        protected String ExecuteShellCommand(String command)
        {
            Console.WriteLine("Application: ExecuteShellCommand(String command)");

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            StreamWriter sw = process.StandardInput;
            StreamReader sr = process.StandardOutput;

            sw.AutoFlush = true;
            sw.Write(command + Environment.NewLine);

            return sr.ReadToEnd();
        }

        protected void Fetch(VooProtocol protocol)
        {
            Console.WriteLine("Application: Fetch(VooProtocol protocol)");
            Console.WriteLine("Application: Fetch -> from = '{0}', to = '{1}'", 
                protocol.Get("from"), protocol.Get("to"));
            VooFile file = new VooFile(protocol.Source, protocol.Destination,
                protocol.Get("from"), protocol.Get("to"));
            fetches.Add(file);
            Console.WriteLine("Application: Fetch -> read data");
            file.ReadToEnd();
            RS485PacketManager manager = new RS485PacketManager(new RS485_dll.Voodoo.Libraries.RS485Library.RS485Terminal.InputData(RS485Packet.PriorityEnum.Highest, 
                file.Data, protocol.Destination, protocol.Source, protocol.Destination), 32);
            RS485Packet packet = new RS485Packet(protocol.Destination, protocol.Destination, protocol.Source, 
                RS485Packet.PriorityEnum.Highest, RS485Packet.CommandEnum.Data, protocol.CreatePushCommand());
            manager.Packets.Insert(0, packet);
            Console.WriteLine("Application: Call send packets.");
            RS485VooEvents.Instance.SendPackets(manager);
        }

        protected void LoginUser(VooProtocol protocol)
        {
            Console.WriteLine("Application: LoginUser(VooProtocol protocol)");
            currentUser = FindUser(protocol.Get("username"), protocol.Get("password"), true);
            if (currentUser != null)
            {
                Console.WriteLine("Application: LoginUser -> username = '{0}', password = '{0}'", 
                    currentUser.UserName, currentUser.Password);
            }
            else
            {
                Console.WriteLine("Application: LoginUser -> User can't be found.");
            }
        }

        protected void Push(VooProtocol protocol)
        {
            Console.WriteLine("Application: Push(VooProtocol protocol)");
            VooFile file = new VooFile(protocol.Source, protocol.Destination, protocol.Get("from"), protocol.Get("to"), Convert.ToInt64(protocol.Get("size")));
            Console.WriteLine("Application: Push from = '{0}', to = '{1}', size = '{2}'", file.From, file.To, file.Size);
            pushes.Add(file);
        }

        public void ReceiveData(VooProtocol protocol)
        {
            // Console.WriteLine("Application: ReceiveData(VooProtocol protocol)");
            VooFile file = FindPushes(protocol);
            if (file != null)
            {
                if (file.Collect(protocol.RawData))
                {
                    Console.WriteLine("Application ReceiveData -> file is downloaded.");
                    pushes.Remove(file);
                }
            }
            else
            {
                Console.WriteLine(protocol.RawData);
            }
        }

        private VooFile FindPushes(VooProtocol protocol)
        {
            return FindFile(protocol, pushes);
        }

        private VooFile FindFile(VooProtocol protocol, List<VooFile> files)
        {
            VooFile result = null;
            foreach (VooFile file in files)
            {
                if (file.IsParent(protocol.Source, protocol.Destination))
                {
                    result = file;
                    break;
                }
            }
            return result;
        }

        protected void RegisterUser(VooProtocol protocol)
        {
            Console.WriteLine("Application: RegisterUser(VooProtocol protocol)");
            User user = new User(protocol.Get("username"), protocol.Get("password"));
            Console.WriteLine("Application: RegisterUser -> username = '{0}', password = '{1}'", user.UserName, user.Password);
            users.Add(user);
        }

        protected void UnRegisterUser(VooProtocol protocol)
        {
            Console.WriteLine("Application: UnRegisterUser(VooProtocol protocol)");

            User user = FindUser(protocol.Get("username"), protocol.Get("password"), true);
            if (user != null)
            {
                Console.WriteLine("Application: UnRegisterUser -> username = '{0}', password = '{1}'", user.UserName, user.Password);
                users.Remove(user);
                Console.WriteLine("Application: UnRegisterUser -> Done.");
            }
        }

        private User FindUser(String username, String password, bool isCheckPassword)
        {
            Console.WriteLine("Application: FindUser(String username, String password, bool isCheckPassword)");

            foreach (User user in users)
            {
                if (user.UserName == username)
                {
                    if (user.Password == password && isCheckPassword)
                    {
                        return user;
                    }
                }
            }
            return null;
        }

        public void CreateApplication(VooProtocol protocol)
        {
            Console.WriteLine("Application: CreateApplication(VooProtocol protocol)");
            application = protocol.Get("application");
            Console.WriteLine("Application: CreateApplication -> application = '{0}'", application);
        }

        protected void RemoveApplication(VooProtocol protocol)
        {
            Console.WriteLine("Application: RemoveApplication(VooProtocol protocol)");
            application = String.Empty;
            Console.WriteLine("Application: RemoveApplication -> application = '{0}'", protocol.Get("application"));
        }

        protected void Error(String error)
        {
            Console.WriteLine("Application: Error(String error)");

            Console.WriteLine(error);
        }

        #region [  Singleton  ]

        private static object synchronized = new object();
        private static VooTerminal _instance;

        public static VooTerminal Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (synchronized)
                    {
                        if (_instance == null)
                        {
                            _instance = new VooTerminal();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion
    }
}
