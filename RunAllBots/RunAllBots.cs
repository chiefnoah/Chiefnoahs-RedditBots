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

            string output = kanmusu.Run() + oneTrueKongouBot.Run(); //lol this is the longest running part of the code
            WriteLog(output);
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
