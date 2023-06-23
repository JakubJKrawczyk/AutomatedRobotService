using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateRobotService.Tools
{
    class Logger
    {

        private string _logPath = "./loggs.txt";

        public async void LogEvent(int iloscPlikow, List<string> filesNames)
        {
            string _logMessageHeader = $"{DateTime.Now.ToString("g")} --------------------------------------------------------------------------" +
            $"Usunięto {iloscPlikow}:\n\n";

            string _logMessageFooter = "\n-------------------------------------------------------------------------------------------------------\n\n";

            if (File.Exists(_logPath))
            {

                FileStream stream =  File.Open(_logPath, FileMode.Append);

                StreamWriter writer = new StreamWriter(stream);
                string fullMessage = _logMessageHeader;

                foreach(string name in filesNames)
                {
                    fullMessage += $"- {name}\n";
                }
                fullMessage += _logMessageFooter;

                writer.Write(fullMessage);

                await writer.FlushAsync();

                stream.Close();
            }
            else
            {
                File.Create(_logPath).Close();
                LogEvent(iloscPlikow, filesNames);
            }
    }
    }
}
