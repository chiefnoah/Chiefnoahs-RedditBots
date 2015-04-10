using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    class RunAllBots {
        static void Main(string[] args) {
            AutoTagBot autoTagBot = new AutoTagBot();
            WriteLog(autoTagBot.Run());
        }

        private static void WriteLog(string log)
        {
            using (StreamWriter sw = File.AppendText("cron.log"))
            {
                sw.WriteLine(log);
            }
        }
    }
}
