using System;
using System.Collections.Generic;
using System.Text;
using RS485_dll.Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library.Helpers;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network
{
    /// <summary>
    /// Classes VooTerminal and VooServer split abstraction layer of voodoo protocol.
    /// There is only for flexible model testing.
    /// </summary>
    public class VooServer
    {
        protected VooServer()
        { }

        public bool AnalyzeCommand(RS485Packet packet)
        {
            VooProtocol protocol = new VooProtocol(packet.current, 
                packet.source, packet.destination, packet.RawData);
            if (protocol.isFound)
            {
                switch (protocol.CurrentPacket.Key)
                {
                    case VooProtocol.Commands.SERVER_HELP:
                        RS485VooEvents.Instance.OutputData(VooConfiguration.Help);
                        break;
                    case VooProtocol.Commands.SERVER_EXIT:
                        RS485VooEvents.Instance.TerminateMachine(RS485Constants.Machine.ServerAndClient);
                        break;
                    case VooProtocol.Commands.SERVER_ADD_AUTH:
                        RS485VooEvents.Instance.RegisterUser(protocol);
                        break;
                    case VooProtocol.Commands.SERVER_REMOVE_AUTH:
                        RS485VooEvents.Instance.UnRegisterUser(protocol);
                        break;
                    case VooProtocol.Commands.SERVER_CREATE_APPLICATION:
                        RS485VooEvents.Instance.CreateApplication(protocol);
                        break;
                    case VooProtocol.Commands.SERVER_REMOVE_APPLICATION:
                        RS485VooEvents.Instance.RemoveApplication(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_BEGIN_SESSION:
                        RS485VooEvents.Instance.BeginSession(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_END_SESSION:
                        RS485VooEvents.Instance.EndSession(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_CD:
                        RS485VooEvents.Instance.ExecuteCommand(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_DIR:
                        RS485VooEvents.Instance.ExecuteCommand(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_FETCH:
                        RS485VooEvents.Instance.Fetch(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_PUSH:
                        RS485VooEvents.Instance.Push(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_AUTH:
                        RS485VooEvents.Instance.LoginUser(protocol);
                        break;
                    case VooProtocol.Commands.CLIENT_PUSH_DATA:
                        break;
                    default:
                        // TODO: Output information about error of protocol data.
                        // RS485VooEvents.Instance.Error("You have error in protocol parser.");
                        // It's for other messages not for implementation protocol.
                        RS485VooEvents.Instance.ReceiveData(protocol);
                        break;
                }
            }
            else { RS485VooEvents.Instance.ReceiveData(protocol); }
            return true;
        }

        #region [  Singleton  ]

        private static object synchronized = new object();
        private static VooServer _instance;

        public static VooServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(synchronized)
                    {
                        if (_instance == null)
                        {
                            _instance = new VooServer();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion
    }
}
