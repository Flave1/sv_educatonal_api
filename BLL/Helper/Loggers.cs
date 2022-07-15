using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qrious.Logger
{
    public class Loggers
    {
        private static string logPath = string.Format(@"{0}\Log", AppDomain.CurrentDomain.BaseDirectory);

        public Loggers()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        public static string Log(string error, string type = "Error")
        {
            string logfile = string.Format(@"{0}\{1}Log-" + DateTime.Now.ToString("dd-MM-yyyy") + ".log", logPath, type);
            //List<string[]> values = new List<string[]>();

            //write new log
            using (StreamWriter writer = new StreamWriter(logfile, true))
            {
                writer.WriteLine("\r" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                writer.WriteLine(error);
                writer.WriteLine("\n\r");
                writer.Close();
                using (StreamReader reader = new StreamReader(logfile, true))
                {
                    //while (reader.Peek() >= 0)
                    //{
                    string value = reader.ReadToEnd();
                    return value;
                    //}
                }
            }
        }
    }
}
