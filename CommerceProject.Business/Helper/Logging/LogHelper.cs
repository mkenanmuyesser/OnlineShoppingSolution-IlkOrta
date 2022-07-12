using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Business.Helper.Logging
{
    public sealed class LogHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LogKaydet(LogLevel logLevel, string logIcerik)
        {
            //logger.Trace("Sample trace message");
            //logger.Debug("Sample debug message");
            //logger.Info("Sample informational message");
            //logger.Warn("Sample warning message");
            //logger.Error("Sample error message");
            //logger.Fatal("Sample fatal error message");

            logger.Log(logLevel, logIcerik);
        }
    }
}