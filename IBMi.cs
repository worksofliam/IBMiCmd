using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using NppPluginNET;

namespace IBMiCmd
{
    class IBMi
    {
        private static Boolean _notConnected = false;
        private static Dictionary<string, string> _config = new Dictionary<string, string>();
        private static List<string> _output = new List<string>();
        private static string _ConfigFile;

        public static void loadConfig(string FileLoc)
        {
            _ConfigFile = FileLoc + ".cfg";
            string[] data;
            if (!File.Exists(_ConfigFile))
            {
                _config.Add("system", "mysystem");
                _config.Add("username", "myuser");
                _config.Add("password", "mypass");
                _config.Add("relicdir", "rpgapp");
                _config.Add("reliclib", "#dev");
				_config.Add("datalibl", "mylibl");
                _config.Add("curlib", "mypwd");

                _config.Add("binds", "COMPILE|RELIC|BUILD");
                _config.Add("COMPILE", "CD '/home/MYUSER'|CRTSQLRPGI OBJ(#MYUSER/%file%) SRCSTMF('%file%.%ext%') OPTION(*EVENTF) REPLACE(*YES) COMMIT(*NONE)|ERRORS #MYUSER %file%");
                _config.Add("RELIC", "CRTBNDRPG OBJ(#MYUSER/RELIC) SRCSTMF('RelicPackageManager/QSOURCE/RELIC.SQLRPGLE') OPTION(*EVENTF) REPLACE(*YES) COMMIT(*NONE)");
                _config.Add("BUILD", "CD '/home/MYUSER'|CRTBNDRPG PGM(#MYUSER/BUILD) SRCSTMF('RelicPackageManager/QSOURCE/BUILD.SQLRPGLE') OPTION(*EVENTF) REPLACE(*YES)|ERRORS #MYUSER BUILD");

				printConfig();

                MessageBox.Show("Thanks for using IBMiCmds. You will now be prompted to enter in a Remote System.");
                Main.remoteSetup();
            }

            foreach (string Line in File.ReadAllLines(_ConfigFile))
            {
                data = Line.Split('=');
                for (int i = 0; i < data.Length; i++) data[i] = data[i].Trim();

                if (_config.ContainsKey(data[0]))
                {
                    _config[data[0]] = data[1];
                }
                else
                {
                    _config.Add(data[0], data[1]);
                }
            }
        }

        private static void printConfig()
        {
            List<string> fileout = new List<string>();
            foreach (var key in _config.Keys)
            {
                fileout.Add(key + '=' + _config[key]);
            }
            File.WriteAllLines(_ConfigFile, fileout.ToArray());
        }

        public static string getConfig(string key)
        {
            if (_config.ContainsKey(key))
            {
                return _config[key];
            }
            else
            {
                return "";
            }
        }

        public static void setConfig(string key, string value)
        {
            if (_config.ContainsKey(key))
            {
                _config[key] = value;
            }
            else
            {
                _config.Add(key, value);
            }

            printConfig();
        }

        public static void remConfig(string key)
        {
            if (_config.ContainsKey(key))
            {
                _config.Remove(key);
            }
        }

        public static void addOutput(string text)
        {
            _output.Add(text);
        }

        public static string[] getOutput()
        {
            string[] result = _output.ToArray();
            _output.Clear();
            return result;
        }

        public static void flushOutput()
        {
            _output.Clear();
        }

        public static void runCommands(string[] list)
        {
            try
            {
                flushOutput();
                string tempfile = Path.GetTempFileName();
                File.Move(tempfile, tempfile + ".ftp");
                tempfile += ".ftp";
                List<string> lines = new List<string>();

                lines.Add("user " + _config["username"]);
                lines.Add(_config["password"]);
                lines.Add("bin");
                foreach (string cmd in list)
                {
                    if (cmd == null) continue;
                    if (cmd.Trim() != "")
                    {
                        IBMiUtilities.DebugLog("Collecting command for ftp file: " + cmd);
                        lines.Add(cmd);
                    }
                }
#if DEBUG
                lines.Add("QUOTE RCMD DSPJOBLOG");
#endif
                lines.Add("quit");

                File.WriteAllLines(tempfile, lines.ToArray());
                runFTP(tempfile);
                File.Delete(tempfile);

            }
            catch(Exception e) {
                IBMiUtilities.Log(e.ToString());
            }
        }

        private static void runFTP(string FileLoc)
        {
            _notConnected = false;
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c FTP -n -s:\"" + FileLoc + "\" " + _config["system"];
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            IBMiUtilities.DebugLog("Starting FTP of command file " + FileLoc);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            IBMiUtilities.DebugLog("FTP of command file " + FileLoc + " completed");
        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            //Console.WriteLine(outLine.Data);
            if (outLine.Data != null)
            {
                if (outLine.Data.Length >= 5)
                {
                    if (outLine.Data.Trim() == "Not connected.")
                    {
                        if (!_notConnected) _output.Add("Not connected to " + _config["system"]);
                        _notConnected = true;
                    }
                    else
                    {
                        switch (outLine.Data.Substring(0, 3))
                        {
                            case "250":
                            case "550":
                                _output.Add("> " + outLine.Data.Substring(4));
                                break;
                        }
                    }
                }
            }
        }
    }
}
