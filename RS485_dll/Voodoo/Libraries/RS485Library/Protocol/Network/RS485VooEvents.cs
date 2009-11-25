using System;
using System.Collections.Generic;
using System.Text;
using Voodoo.Libraries.RS485Library.Helpers;
using RS485_dll.Voodoo.Libraries.RS485Library;
using Voodoo.Libraries.RS485Library;

namespace Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Network
{
    public class RS485VooEvents
    {
        #region [  Singleton  ]

        public static object synchronizedObject = new object();
        private static RS485VooEvents instance;

        public static RS485VooEvents Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (synchronizedObject)
                    {
                        if (instance == null)
                        {
                            instance = new RS485VooEvents();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region [  Events  ]
        /// <summary>
        /// It's output devent generally using for outputting data without support determining IO.
        /// </summary>
        public delegate void OutputDataDelegate(String text);
        public event OutputDataDelegate OutputDataEvent;

        public delegate void TerminateMachineDelegate(RS485Constants.Machine machine);
        public event TerminateMachineDelegate TerminateMachineEvent;

        public delegate void RegisterUserDelegate(VooProtocol protocol);
        public event RegisterUserDelegate RegisterUserEvent;

        public delegate void UnRegisterUserDelegate(VooProtocol protocol);
        public event UnRegisterUserDelegate UnRegisterUserEvent;

        public delegate void LoginUserDelegate(VooProtocol protocol);
        public event LoginUserDelegate LoginUserEvent;

        public delegate void ExecuteCommandDelegate(VooProtocol protocol);
        public event ExecuteCommandDelegate ExecuteCommandEvent;

        public delegate void BeginSessionDelegate(VooProtocol protocol);
        public event BeginSessionDelegate BeginSessionEvent;

        public delegate void EndSessionDelegate(VooProtocol protocol);
        public event EndSessionDelegate EndSessionEvent;

        public delegate void FetchDelegate(VooProtocol protocol);
        public event FetchDelegate FetchEvent;

        public delegate void PushDelegate(VooProtocol protocol);
        public event PushDelegate PushEvent;

        public delegate void CreateApplicationDelegate(VooProtocol protocol);
        public event CreateApplicationDelegate CreateApplicationEvent;

        public delegate void RemoveApplicationDelegate(VooProtocol protocol);
        public event RemoveApplicationDelegate RemoveApplicationEvent;

        public delegate void ErrorDelegate(String protocol);
        public event ErrorDelegate ErrorEvent;

        public delegate void ReceiveDataDelegate(VooProtocol protocol);
        public event ReceiveDataDelegate ReceiveDataEvent;

        public delegate void SendPacketsDelegate(RS485PacketManager manager);
        public event SendPacketsDelegate SendPacketsEvent;

        public void SendPackets(RS485PacketManager manager)
        {
            SendPacketsDelegate local = SendPacketsEvent;
            if (local != null)
            {
                local(manager);
            }
        }

        public void Error(String error)
        {
            ErrorDelegate local = ErrorEvent;
            if (local != null)
            {
                local(error);
            }
        }

        public void RemoveApplication(VooProtocol protocol)
        {
            RemoveApplicationDelegate local = RemoveApplicationEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void CreateApplication(VooProtocol protocol)
        {
            CreateApplicationDelegate local = CreateApplicationEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void Push(VooProtocol protocol)
        {
            PushDelegate local = PushEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void Fetch(VooProtocol protocol)
        {
            FetchDelegate local = FetchEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void BeginSession(VooProtocol protocol)
        {
            BeginSessionDelegate local = BeginSessionEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void EndSession(VooProtocol protocol)
        {
            EndSessionDelegate local = EndSessionEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void ExecuteCommand(VooProtocol protocol)
        {
            ExecuteCommandDelegate local = ExecuteCommandEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void LoginUser(VooProtocol protocol)
        {
            LoginUserDelegate local = LoginUserEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void UnRegisterUser(VooProtocol protocol)
        {
            UnRegisterUserDelegate local = UnRegisterUserEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void RegisterUser(VooProtocol protocol)
        {
            RegisterUserDelegate local = RegisterUserEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        public void OutputData(String text)
        {
            OutputDataDelegate local = OutputDataEvent;
            if (local != null)
            {
                local(text);
            }
        }

        public void TerminateMachine(RS485Constants.Machine machine)
        {
            TerminateMachineDelegate local = TerminateMachineEvent;
            if (local != null)
            {
                local(machine);
            }
        }

        public void ReceiveData(VooProtocol protocol)
        {
            ReceiveDataDelegate local = ReceiveDataEvent;
            if (local != null)
            {
                local(protocol);
            }
        }

        #endregion
    }
}
