using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Voodoo.Libraries.RS485Library.Helpers;
using Voodoo.Libraries.RS485Library.Models;
using RS485_dll.Voodoo.Libraries.RS485Library;
using System.Threading;

namespace Voodoo.Libraries.RS485Library
{
    public class RS485PacketServer : RS485Server
    {
        private bool startPacket;
        private int length = -1;
        private byte command = RS485Packet.CommandEnum.nil;
        private byte priority = RS485Packet.PriorityEnum.nil;
        private String destination = String.Empty;
        private String source = String.Empty;
        private String data = String.Empty;
        private String current = String.Empty;
        public String name;

        public RS485PacketServer()
            : base(RS485Constants.DEFAULT_SERVER_PORT)
        {
        }

        public RS485PacketServer(RS485 rs)
            : base(rs)
        { }

        public RS485PacketServer(String name) : base(RS485Constants.DEFAULT_SERVER_PORT)
        {
            this.name = name;
            Reset();
        }

        public RS485PacketServer(String name, String portname) : base(portname)
        {
            this.name = name;
            Reset();
        }

        /// <summary>
        /// Buffer: FEND,CMD,N,DATA
        /// </summary>
        protected override void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            base.Port_DataReceived(sender, e);
            foreach (byte b in buffer)
            {
                if (b == Convert.ToByte((char) RS485Packet.TokenBusEnum.FSTART) && !startPacket)
                {
                    startPacket = true;
                }
                else if (startPacket && command == RS485Packet.CommandEnum.nil)
                {
                    command = b;
                }
                else if (command != RS485Packet.CommandEnum.nil && priority == RS485Packet.PriorityEnum.nil)
                {
                    priority = b;
                }   
                else if (priority != RS485Packet.PriorityEnum.nil && String.IsNullOrEmpty(destination))
                {
                    destination = Convert.ToString((char)b);
                }
                else if (!String.IsNullOrEmpty(destination) && String.IsNullOrEmpty(source))
                {
                    source = Convert.ToString((char)b);
                }
                else if (!String.IsNullOrEmpty(source) && String.IsNullOrEmpty(current))
                {
                    current = Convert.ToString((char)b);
                }
                else if (!String.IsNullOrEmpty(current) && length == -1)
                {
                    try
                    {
                        length = b;
                    }
                    catch
                    {
                    }
                }
                else if (length != -1)
                {
                    --length;
                    data += Convert.ToString((char) b);
                    if (length == 0)
                    {
                        RS485Packet packet = new RS485Packet(source, current, destination, priority, command, data);
                        RS485Terminal.StopTimer();
                        Thread.Sleep(500);
                        if (!IsEcho(packet))
                        {
                            AnalysePacket(packet);
                        }
                        Reset();
                        rs.Port.BaseStream.Flush();
                    }    
                }
            }// end for
        }// end method

        private bool IsEcho(RS485Packet packet)
        {
            RS485Packet p = RS485Terminal.LastSend;
            if (p != null)
            {
                return p.Equals(packet);
            }
            return false;
        }

        protected virtual bool AnalyzeCommand(RS485Packet packet)
        {
            return true;
        }

        private void AnalysePacket(RS485Packet packet)
        {
            // Так как у нас сеть передаёт во всех направлениях одновременно, то для
            // реализации кольца, нам нужно как фильтровать пакеты.
            // Ввели в кажлый пакет дополнительное поле current, которое говорит 
            // админу компа может ли он принять данный пакет, если может, то тогда
            // можно начать анализировать пакет, если же нет, то хз.
            if (RS485Terminal.NextDestination(packet.current) == name)
            {
                // Получили наш пакет
                packet = new RS485Packet(packet.source, RS485Terminal.NextDestination(packet.current), packet.destination, packet.priority, packet.command, packet.RawData);
                if (name == packet.destination)
                {
                    if (packet.command != RS485Packet.CommandEnum.Successed && packet.command != RS485Packet.CommandEnum.AllowTransmit)
                    {
                        // сразу же отправляем пакет дальше с Successed
                        RS485Events.ReceivedPackets(this, new RS485Events.ReceivedPacketsArg(packet));
                        // Console.WriteLine("send successed packet.");
                        if (packet.command == RS485Packet.CommandEnum.Data)
                        {
                            // В этой области нам не нужно ничего сразу отправлять
                            // сначала мы анализируем пакет, далее если всё - таки нам нужно
                            // отправить результат выполненной команды, мы должны дождаться
                            // когда к нам придёт маркер и дальше перенаправить нужную информацию.
                            AnalyzeCommand(packet);
                        }
                    }
                    else if (packet.command == RS485Packet.CommandEnum.Successed)
                    {
                        // Если нам пришёл ответ на пакет, что был удачно доставлен.
                        // то нам можно просто пропустить и продолжить дальше работать.
                        RS485Events.AllowNext(this, new RS485Events.ReceivedPacketsArg(packet)); // GET AllowTransmit
                        RS485PacketManager manager = RS485Terminal.GetFirstManager();
                        if (manager != null)
                        {
                            RS485Events.RemovePacketAfterSuccessed(manager.FirstPacket);
                        }
                        // Console.WriteLine("packet successed received.");
                    }
                }
                else
                {
                    // Successed пакет ходит с наивысшим приоритетом.
                    if (packet.command == RS485Packet.CommandEnum.Successed)
                    {
                        // передать управление следующему в сети хосту.
                        // Console.WriteLine("We are received packets and now we have to send forward order of pointers.");
                        RS485Events.SendNext(packet);
                    }
                    else
                    {
                        // Пакет не нам. Смотрим приоритеты.
                        RS485PacketManager manager = RS485Terminal.GetFirstManager();
                        if (manager != null && manager.HasPackets())
                        {
                            RS485Packet clientPacket = manager.FirstPacket;
                            if (clientPacket.priority > packet.priority)
                            {
                                packet = clientPacket;
                            }
                        }
                        if (packet.command == RS485Packet.CommandEnum.Data)
                        {
                            RS485Events.SendNext(packet);
                        }
                    }
                }
                if (packet.command == RS485Packet.CommandEnum.AllowTransmit)
                {
                    // получил разрешение на отправление пакета.
                    RS485Events.EnableMarker();
                }
            }
            else if (packet.current == name)
            {
                RS485Events.EnableMarker();
            }
        }

        protected virtual void Reset()
        {
            startPacket = false;
            data = String.Empty;
            length = -1;
            destination = String.Empty;
            source = String.Empty;
            current = String.Empty;
            command = RS485Packet.CommandEnum.nil;
            priority = RS485Packet.PriorityEnum.nil;
        }
    }
}
