using System;
using System.Collections.Generic;
using System.Text;
using Voodoo.Libraries.Voodoo.Libraries.RS485Library.Protocol.Models;
using System.Text.RegularExpressions;
using Voodoo.Libraries.RS485Library.Protocol.Models;

namespace RS485_dll.Voodoo.Libraries.RS485Library
{
    /*
     * Так как у нас на более низком уровне в пакете приходит
     * информация о машине принимающей и отправляющей, а также
     * о типе передаваемых данных, то благодаря этой информации
     * мы может манипулировать сессиями передаваемых данных.
     * Т.е. как для каждого туннеля данных будет создаваться сессия.
     * comp1 -> comp2 (session1), comp2 -> comp1 (session2)
     * Благодаря этому мы можем организовать передачу на один
     * компьютер много различных сессионных данных.
     */
    public class VooProtocol : Protocol
    {
        public bool isFound;
        public enum Commands
        {
            // Generic Commands
            SERVER_CREATE_APPLICATION,
            SERVER_REMOVE_APPLICATION,
            SERVER_EXIT,
            SERVER_HELP,
            SERVER_ADD_AUTH,
            SERVER_REMOVE_AUTH,

            // Protocol Commands
            CLIENT_PUSH,
            CLIENT_FETCH,
            CLIENT_CONSOLE,
            CLIENT_BEGIN_SESSION,
            CLIENT_END_SESSION,
            CLIENT_DIR,
            CLIENT_CD,
            CLIENT_UPLOAD,
            CLIENT_AUTH,
            CLIENT_PUSH_DATA
        };

        private KeyValuePair<Commands, ProtocolToken> currentPacket;
        private static Dictionary<Commands, ProtocolToken> patterns;
        private String current;
        private String source;
        private String destination;
        private String data;

        static VooProtocol()
        {
            patterns = new Dictionary<Commands, ProtocolToken>();
            patterns.Add(Commands.SERVER_EXIT, 
                new ProtocolToken("_exit"));
            patterns.Add(Commands.SERVER_HELP, 
                new ProtocolToken("_help"));
            patterns.Add(Commands.SERVER_ADD_AUTH, 
                new ProtocolToken("_add a -u=(\\w+) -p=(\\w+)",
                    new ProtocolDictionary().Add("username").Add("password")));
            patterns.Add(Commands.SERVER_REMOVE_AUTH, 
                new ProtocolToken("_remove a -u=(\\w+) -p=(\\w+)",
                    new ProtocolDictionary().Add("username").Add("password")));
            patterns.Add(Commands.SERVER_CREATE_APPLICATION, 
                new ProtocolToken("_create -a=([:\\\\\\w]+)",
                    new ProtocolDictionary().Add("application")));
            patterns.Add(Commands.SERVER_REMOVE_APPLICATION,
                new ProtocolToken("_remove -a=([:\\\\\\w]+)",
                    new ProtocolDictionary().Add("application")));
            patterns.Add(Commands.CLIENT_CONSOLE, 
                new ProtocolToken("_console"));
            patterns.Add(Commands.CLIENT_BEGIN_SESSION, 
                new ProtocolToken("_begin_session -m=(\\w+)",
                    new ProtocolDictionary().Add("machine")));
            patterns.Add(Commands.CLIENT_END_SESSION,
                new ProtocolToken("_end_session -m=(\\w+)",
                    new ProtocolDictionary().Add("machine")));
            patterns.Add(Commands.CLIENT_DIR,
                new ProtocolToken("_dir -a=([\\.:\\\\\\w]+)",
                    new ProtocolDictionary().Add("application")));
            patterns.Add(Commands.CLIENT_CD,
                new ProtocolToken("_cd -f=([\\.:\\\\\\w]+)", 
                    new ProtocolDictionary().Add("folder")));
            patterns.Add(Commands.CLIENT_FETCH,
                new ProtocolToken("_fetch -s=(\\w+) -f=([\\.:\\\\\\w]+) -t=([\\.:\\\\\\w]+)",
                    new ProtocolDictionary().Add("session").Add("from").Add("to")));
            patterns.Add(Commands.CLIENT_PUSH,
                new ProtocolToken("_push -s=(\\w+) -f=([\\.:\\\\\\w]+) -t=([\\.:\\\\\\w]+) -s=(\\w+)",
                    new ProtocolDictionary().Add("session").Add("from").Add("to").Add("size")));
            patterns.Add(Commands.CLIENT_AUTH, 
                new ProtocolToken("_auth -u=(\\w+) -p=(\\w+)",
                    new ProtocolDictionary().Add("username").Add("password")));
        }

        private String rawData;

        public VooProtocol(String current, String source, 
            String destination, String rawData)
        {
            this.rawData = rawData;
            this.current = current;
            this.source = source;
            this.destination = destination;

            AnalyseRawData(rawData);
        }

        /// <summary>
        /// Using regex analyse data from packet and initialize command params.
        /// </summary>
        public override bool AnalyseRawData(String rawData)
        {
            foreach (KeyValuePair<Commands, ProtocolToken> pairs in patterns)
            {
                Match match = Regex.Match(rawData, pairs.Value.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                if (match.Captures.Count != 0)
                {
                    isFound = true;
                    currentPacket = pairs;
                    pairs.Value.InitializeParams(match.Groups);
                    return true;
                }
             }
             isFound = false;
             return false;
        }

        /// <summary>
        /// Get value from CurrentPacket by key.
        /// </summary>
        public String Get(String key)
        {
            String result = "";
            if (CurrentPacket.Value != null)
            {
                if (CurrentPacket.Value.Parameters.ContainsKey(key))
                {
                    result = CurrentPacket.Value.Parameters[key];
                }
            }
            return result;
        }

        public String CreatePushCommand()
        {
            return CreatePushCommand(Get("session"), Get("from"), Get("to"), Get("size"));
        }

        public static String CreatePushCommand(String session, String from, String to, String size)
        {
            return String.Format("_push -s={3} -f={0} -t={1} -s={2}", from, to, size, session);
        }

        public static String CreateFetchCommand(String session, String from, String to)
        {
            return String.Format("_fetch -s={2} -f={0} -t={1}", from, to, session);
        }

        public static String CreateBeginSessionCommand(String source, String destination)
        {
            return String.Format("_begin_session -m={0}_{1}", source, destination);
        }

        public static String CreateBeginSessionCommand(Session session)
        {
            return CreateBeginSessionCommand(session.Source, session.Destination);
        }

        public static String CreateEndSessionCommand(String source, String destination)
        {
            return String.Format("_end_session -m={0}_{1}", source, destination);
        }

        public static String CreateEndSessionCommand(Session session)
        {
            return CreateEndSessionCommand(session.Source, session.Destination);
        }

        /// <summary>
        /// Create "creation of application" command.
        /// </summary>
        public static String CreateCApplicationCommand(String application)
        {
            return String.Format("_create -a={0}", application);
        }

        /// <summary>
        /// Create "removement of application" command.
        /// </summary>
        public static String CreateRApplicationCommand(String application)
        {
            return String.Format("_remove -a={0}", application);
        }

        public static String CreateAddUser(String username, String password)
        {
            return String.Format("_add a -u={0} -p={1}", username, password);
        }

        public static String CreateAddUser(User user)
        {
            return CreateAddUser(user.UserName, user.Password);
        }

        public static String CreateRemoveUser(String username, String password)
        {
            return String.Format("_remove a -u={0} -p={1}", username, password);
        }

        public static String CreateAuthUser(String username, String password)
        {
            return String.Format("_auth -u={0} -p={1}", username, password);
        }

        public static String CreateAuthUser(User user)
        {
            return CreateAuthUser(user.UserName, user.Password);
        }

        public String CreateDataCommand(String data)
        {
            return String.Format("${0}", data);
        }

        public static Dictionary<Commands, ProtocolToken> Patterns
        {
            get 
            {
                return patterns;
            }
        }

        /// <summary>
        /// Packet that's finded in data of packet.
        /// </summary>
        public KeyValuePair<Commands, ProtocolToken> CurrentPacket
        {
            get
            {
                return currentPacket;
            }
        }

        public String Source
        {
            get
            {
                return source;
            }
        }

        public String Destination
        {
            get
            {
                return destination;
            }
        }

        public String RawData
        {
            get
            {
                return rawData;
            }
        }
    }
}
