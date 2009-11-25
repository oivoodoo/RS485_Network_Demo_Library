using Voodoo.Libraries.Logs;

[assembly: ResourceFileConfigurator("Voodoo.RS485.Libraries.Resources.log4net.xml.config")]
namespace Voodoo.Libraries.Logs
{
    public class Log
    {
        private static readonly object root = new object();
        /// <summary>
        /// Logger for <see cref="Sword.DragNet2.Core.Logging.Log"/> type.
        /// </summary>
        public static Logger CoreLog
        {
            get
            {
                if (coreLog == null)
                {
                    lock(root)
                    {
                        if (coreLog == null)
                        {
                            coreLog = Logger.GetLogger(typeof (Log));
                        }
                    }
                }
                return coreLog;
            }
        }

        /// <summary>
        /// Logger for <see cref="Sword.DragNet2.Core.Logging.Log"/> type.
        /// </summary>
        private static Logger coreLog;
    }
}
