using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RS485_dll.Voodoo.Libraries.RS485Library.Configuration;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Helpers;
using System.IO;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    public class RS485Terminal
    {
        private Thread clientThread;
        private Thread serverThread;
        private static RS485Packet lastSend;

        protected String port;
        protected List<String> clients = new List<String>();
        protected String source;
        private bool hasMarker;
        private static List<RS485PacketManager> managers = new List<RS485PacketManager>();
        private static List<RS485Packet> queue = new List<RS485Packet>();

        private static System.Timers.Timer packetsTimer = new System.Timers.Timer(100);
        private System.Timers.Timer inputTimer = new System.Timers.Timer(100);

//        protected object synchronized = new object();

        protected RS485PacketClient client;
        protected RS485PacketServer server;
        private static int max = 4;
        
        public RS485Terminal(String port, String source, bool hasMarker)
        {
            this.port = port;
            this.source = source;
            this.hasMarker = hasMarker;

            // clients.AddRange(new string[] { "1", "2", "3" });
            clients.AddRange(new string[] { "1", "2", "3" });
        }

        public virtual void Start()
        {
            clientThread = new Thread(new ThreadStart(StartClient));
            clientThread.Start();
        }

        public virtual void Stop()
        {
            inputTimer.Stop();
            packetsTimer.Stop();
            serverThread.Abort();
            clientThread.Abort();
        }

        protected virtual void StartServer()
        {
            using (server = new RS485PacketServer(client.RS))
            {
                server.name = source;
                RS485Events.OutputEvent += new RS485Events.OutputDelegate(Events_OutputEvent);
                server.Loop();
            }
        }

        protected virtual void StartClient()
        {
            client = new RS485PacketClient(port);
            InitializeEvents();
            client.Open();

            serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.Start();

            if (hasMarker)
            {
                StartSend(this, (System.Timers.ElapsedEventArgs)null);
                hasMarker = false;
            }
            inputTimer.Start();
        }

        private void TypeData(Object sender, System.Timers.ElapsedEventArgs arg)
        {
            inputTimer.Stop();
            InputData data = GetInputText();
            if (data != null)
            {
                AddPackets(data);
            }
            inputTimer.Start();
        }

        protected void AddPackets(InputData data)
        {
           // lock (RS485Events.synchronized)
            {
                managers.Add(new RS485PacketManager(data, 32));
            }
        }

        protected virtual void InitializeEvents()
        {
            packetsTimer.Elapsed += new System.Timers.ElapsedEventHandler(StartSend);
            inputTimer.Elapsed += new System.Timers.ElapsedEventHandler(TypeData);

            // INFO: Выводить ошибку при передаче на console.
            RS485Events.OutputEvent += new RS485Events.OutputDelegate(Events_OutputEvent);
            // INFO: Получили наш пакет и отправить successed.
            RS485Events.ReceivedPacketsEvent += new RS485Events.ReceivedPacketsDelegate(Events_ReceivedPackets);
            // INFO: Передача маркера дальше по кругу.
            RS485Events.AllowNextEvent += new RS485Events.AllowNextDelegate(Events_AllowNextEvent);
            // INFO: Разрешить отправку пакета(достать из очереди и передать дальше по сети).
            RS485Events.EnableMarkerEvent += new RS485Events.EnableMarkerDelegate(Events_EnableMarker);
            // INFO: Если пакет не наш, то отослать дальше по сети.
            RS485Events.SendNextEvent += new RS485Events.SendNextDelegate(Events_SendNext);
            // INFO: Для 2-й лабы, проверять коллизии(если на эхо приходит пакет не соответствующий текущему в очереди, то нужно покурить, т.е. дописать проверку и если что - то не так, то нужно обрубить соединение)
            RS485Events.CheckLastPacketEvent += new RS485Events.CheckLastPacketDelegate(Events_CheckLastPacket);
            // INFO: Для 1-й лабы. игнорирование эхо.
            RS485Events.SetIgnorePacketEvent += new RS485Events.SetIgnorePacketDelegate(Events_SetIgnorePacket);
            // INFO: Если пришёл successed, то удалить из очереди пакетов отправленный.
            RS485Events.RemovePacketAfterSuccessedEvent += new RS485Events.RemovePacketAfterSuccessedDelegate(Events_RemovePacketAfterSuccessed);
        }

        private void Events_RemovePacketAfterSuccessed(RS485Packet packet)
        {
            for (int i = 0; i < managers.Count; i++)
            {
                try
                {
                    for (int j = 0; j < managers[i].Packets.Count; j++)
                    {
                        RS485Packet localpacket = managers[i].Packets[j];
                        if (localpacket == packet)
                        {
                            managers[i].Packets.Remove(packet);
                            if (!managers[i].HasPackets())
                            {
                                managers.Remove(managers[i]);
                            }
                        }
                    }
                }
                catch
                { }
            }
        }

        private void Events_SetIgnorePacket(bool isIgnorePacket)
        {
            lock (RS485Events.synchronized)
            {
                SharedObject.isIgnorePacket = isIgnorePacket;
            }
        }

        private void Events_CheckLastPacket(RS485Packet packet)
        {
        }

        private void StartSend(Object sender, System.Timers.ElapsedEventArgs arg)
        {
           // lock (RS485Events.synchronized)
            {
                if (managers.Count > 0)
                {
                    for (int i = 0; i < managers.Count; i++)
                    {
                        RS485PacketManager manager = managers[i];
                        if (manager.HasPackets())
                        {
                            client.Send(manager.FirstPacket);
                            lastSend = manager.FirstPacket;
                            break;
                        }
                    }
                }
                else
                {
                    RS485Packet packet = new RS485Packet(source, source, NextDestination(source), RS485Packet.PriorityEnum.Lowest, RS485Packet.CommandEnum.AllowTransmit, "x");
                    client.Send(packet);
                    lastSend = packet;
                }
                // packetsTimer.Start();
            }
        }

        public static void StopTimer()
        {
            if (packetsTimer != null)
            {
                packetsTimer.Stop();
            }
        }

        public static RS485Packet LastSend
        {
            get
            { 
                return lastSend;
            }
            protected set
            {
                lastSend = value;
            }
        }

        protected void Events_AllowNextEvent(object sender, RS485Events.ReceivedPacketsArg arg)
        {
            // lock (RS485Events.synchronized)
            {
                RS485Packet packet = new RS485Packet(source, source, NextDestination(source), RS485Packet.PriorityEnum.Highest, RS485Packet.CommandEnum.AllowTransmit, "x");
                lastSend = packet;
                client.Send(packet);
            }
        }

        protected void Events_EnableMarker()
        {
            // lock (RS485Events.synchronized)
            {
                packetsTimer.Start();
            //    StartSend(this, (System.Timers.ElapsedEventArgs) null);
            }
        }

        protected void Events_ReceivedPackets(Object sender, RS485Events.ReceivedPacketsArg arg)
        {
            // lock (RS485Events.synchronized)
            {
                RS485Packet packet = new RS485Packet(source, source, arg.Packet.source, RS485Packet.PriorityEnum.Highest, RS485Packet.CommandEnum.Successed, "*");
                lastSend = packet;
                client.Send(packet);
            }
        }

        public static RS485PacketManager GetFirstManager()
        {
            try
            {
                return managers[managers.Count - 1];
            }
            catch
            {
                return null;
            }
        }

        public static String NextDestination(String source)
        {
            int n = Convert.ToInt32(source);
            n = n + 1;
            if (n >= max)
            {
                n = 1;
            }
            return Convert.ToString(n);
        }

        protected virtual InputData GetInputText()
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("ID = {0}", source);
            Console.WriteLine("Network List: ");
            foreach (String client in clients)
            {
                Console.Write("{0} ", client);
            }
            Console.WriteLine("");
            Console.WriteLine("Type priority(Low, Lowest, High, Highest)->");
            byte priority = GetPriority(Console.ReadLine());
            String data = String.Empty;
            if (priority == RS485Packet.PriorityEnum.Low ||
                priority == RS485Packet.PriorityEnum.Lowest)
            {
                Console.WriteLine("> Please, input file name|: ");
                String filename = Console.ReadLine();
                if (File.Exists(filename))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))
                    {
                        data = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                Console.WriteLine("Data:");
                data = Console.ReadLine();
            }
            Console.WriteLine("Destination:");
            String destination = Console.ReadLine();
            return new InputData(priority, data, source, destination, source);
        }

        protected static byte GetPriority(String priority)
        {
            if (priority == "Low")
            {
                return RS485Packet.PriorityEnum.Low;
            }
            else if (priority == "Lowest")
            {
                return RS485Packet.PriorityEnum.Low;
            }
            else if (priority == "High")
            {
                return RS485Packet.PriorityEnum.High;
            }
            else if (priority == "Highest")
            {
                return RS485Packet.PriorityEnum.Highest;
            }
            return RS485Packet.PriorityEnum.nil;
        }

        protected void Events_SendNext(RS485Packet packet)
        {
            // lock (RS485Events.synchronized)
            {
                RS485Packet newPacket = new RS485Packet(packet.source, packet.current, 
                    packet.destination, packet.priority, packet.command, packet.RawData);
                lastSend = newPacket;
                client.Send(newPacket);
            }
        }

        protected void Events_OutputEvent(object sender, RS485Events.OutputEventArg arg)
        {
        }

        public String Port
        {
            get
            {
                return port;
            }
        }

        protected List<RS485PacketManager> Managers
        {
            get { return managers; }
        }

        public class InputData
        {
            public InputData(byte Priority, String Data, String Source, 
                String Destination, String Current)
            {
                this.Priority = Priority;
                this.Data = Data;
                this.Source = Source;
                this.Destination = Destination;
                this.Current = Current;
            }

            public byte Priority;
            public String Data;
            public String Source;
            public String Destination;
            public String Current;
        }
    }

}
