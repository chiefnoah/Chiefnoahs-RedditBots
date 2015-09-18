using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    class RunAllBots {
        static void Main(string[] args) {
            //AutoTagBot autoTagBot = new AutoTagBot();
            //WriteLog(autoTagBot.Run());

            BotKanMusus kanmusu = new BotKanMusus();
            OneTrueKongouBot oneTrueKongouBot = new OneTrueKongouBot();
            WhichMoeBot whichMoeBot = new WhichMoeBot();
            try {
                string output = kanmusu.Run() + oneTrueKongouBot.Run() + whichMoeBot(); //lol this is the longest running part of the code
                WriteLog(output);
            } catch (Exception e) {
                //Generic exception because we want to know exactly what caused the program to crash
                WriteLog("\r\n\r\nERROR: " + e.Message + "\r\nStacktrace: " + e.StackTrace);
            }

        }

        private static void WriteLog(string log)
        {
            using (StreamWriter sw = File.AppendText("cron.log"))
            {
                sw.WriteLine(log);
                Console.Write(log);
            }
        }
    }
}
