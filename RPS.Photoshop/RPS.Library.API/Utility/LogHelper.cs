using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace RPS.Library.API.Utility
{
    public class LogHelper
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public static void TraceLog(string message)
        {
            //this will display message to log
            logger.Trace(message);
        }
        
        public static void ErrorLog(string message)
        {
            //this will only appear in log
            logger.Error(message);
        }
    }
}
