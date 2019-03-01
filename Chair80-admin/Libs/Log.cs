using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Chari80Admin.Libs
{
    public static class Logger
    {
        static string LogFile = (ConfigurationManager.AppSettings["Logs_path"] + "\\" + DateTime.Now.ToString("yyyy-MM-dd")+ ".txt").Replace("/","\\");
        public  static void log(string Message)
        {
            System.IO.File.AppendAllText(LogFile, string.Format("\r\n{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), Message));
           
        }
    }
}