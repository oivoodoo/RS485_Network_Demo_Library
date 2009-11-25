using System;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Voodoo.Libraries.Logs
{
    /// <summary>
    /// Class performs logging routines.
    /// TODO: Add another repository for Workflows and Jobs
    /// </summary>
    public class Logger
    {
        private readonly ILog log;

        static Logger()
        {
        }

        private Logger(ILog ilog)
        {
            log = ilog;
        }

        /// <summary>
        /// Obtains logger by type.
        /// </summary>
        /// <param name="type">Type which to log.</param>
        /// <returns></returns>
        public static Logger GetLogger(Type type)
        {
            Logger logger = new Logger(LogManager.GetLogger(type));
            return logger;
        }

        /// <summary>
        /// Logs informational message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        public void Info(string msg)
        {
            log.Info(msg);
        }

        /// <summary>
        /// Logs debug message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        public void Debug(string msg)
        {
            log.Debug(msg);
        }

        /// <summary>
        /// Logs debug message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public void Debug(string msg, Exception ex)
        {
            log.Debug(msg, ex);
        }

        /// <summary>
        /// Logs error message.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        public void Error(Exception ex)
        {
            log.Error(ex);
        }

        /// <summary>
        /// Logs error message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public void Error(string msg, Exception ex)
        {
            log.Error(msg, ex);
        }

        /// <summary>
        /// Logs warning message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        public void Warning(string msg)
        {
            log.Warn(msg);
        }

        /// <summary>
        /// Logs warning message.
        /// </summary>
        /// <param name="msg">Message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public void Warning(string msg, Exception ex)
        {
            log.Warn(msg, ex);
        }
    }
}