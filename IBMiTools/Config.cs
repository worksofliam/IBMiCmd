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
        public static void SwitchToConfig(string Config)
        {
            IBMi.LoadConfig(Main.SystemsDirectory + Config + ".cfg", Config);
            File.WriteAllText(Main.ConfigDirectory + "dftcfg", Config);
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
