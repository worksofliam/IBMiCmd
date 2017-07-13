using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IBMiCmd.IBMiTools
{
    class Config
    {
        public static void SwitchToConfig(string System)
        {
            IBMi.LoadConfig(Main.SystemsDirectory + System, System);
        }

        public static string[] GetConfigs()
        {
            string[] Files = Directory.GetFiles(Main.SystemsDirectory);

            for (var i = 0; i < Files.Length; i++)
            {
                Files[i] = Path.GetFileNameWithoutExtension(Files[i]);
            }

            return Files;
        }
    }
}
