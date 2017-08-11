using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace IBMiCmd.IBMiTools
{
    class IBMi
    {
        readonly static Dictionary<string, string> FTPCodeMessages = new Dictionary<string, string>()
        {
            { "426", "Connection closed; transfer aborted." },
            { "530", "Configuration username and password incorrect." }
        };

        public static string _ConfigFile { get; set; }

        private static Boolean _NotConnected = false;
        private static string _Failed = "";
        private static Dictionary<string, string> _config = new Dictionary<string, string>();
        private static List<string> _output = new List<string>();

        private static Boolean _getList = false;
        private static List<string> _list = new List<string>();

        private static void CheckExist(string key, string value)
        {
            if (!_config.ContainsKey(key))
                _config.Add(key, value);
        }

        public static void LoadConfig(string FileLoc, string System = "mysystem")
        {
            _config.Clear();
            _ConfigFile = FileLoc;
            string[] data;
            if (!File.Exists(_ConfigFile))
            {
                CheckExist("system", System);
                CheckExist("username", "myuser");
                CheckExist("password", "mypass");
                CheckExist("datalibl", "SYSTOOLS");
                CheckExist("curlib", "SYSTOOLS");
                CheckExist("clrcdlen", "80");
                CheckExist("installlib", "QGPL");
                CheckExist("intellisense", "false");
                CheckExist("experimental", "false");

                CheckExist("localDefintionsInstalled", "false");

                CheckExist("binds", "MBR_CRTBNDRPG|MBR_CRTSQLRPGI|MBR_CRTBNDCL|MBR_CRTBNDC|IFS_CRTBNDRPG");
                CheckExist("MBR_CRTBNDRPG", "CRTBNDRPG PGM(%openlib%/%openmbr%) SRCFILE(%openlib%/%openspf%) OPTION(*EVENTF) DBGVIEW(*SOURCE)|ERRORS %openlib% %openmbr%");
                CheckExist("MBR_CRTSQLRPGI", "CRTSQLRPGI OBJ(%openlib%/%openmbr%) SRCFILE(%openlib%/%openspf%) COMMIT(*NONE) OPTION(*EVENTF *XREF)|ERRORS %openlib% %openmbr%");
                CheckExist("MBR_CRTBNDCL", "CRTBNDCL PGM(%openlib%/%openmbr%) SRCFILE(%openlib%/%openspf%) OPTION(*EVENTF)|ERRORS %openlib% %openmbr%");
                CheckExist("MBR_CRTBNDC", "CRTBNDC PGM(%openlib%/%openmbr%) SRCFILE(%openlib%/%openspf%) DBGVIEW(*SOURCE)|ERRORS %openlib% %openmbr%");
                CheckExist("IFS_CRTBNDRPG", "CRTBNDRPG PGM(%curlib%/%file%) SRCSTMF('%file%.%ext%') OPTION(*EVENTF) DBGVIEW(*SOURCE)|ERRORS %curlib% %file%");

                PrintConfig();

                MessageBox.Show("You will now be prompted to enter in a Remote System.");
                Main.RemoteSetup();
            }
            else
            {
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

                CheckExist("system", System);
                CheckExist("username", "myuser");
                CheckExist("password", "mypass");
                CheckExist("datalibl", "SYSTOOLS");
                CheckExist("curlib", "SYSTOOLS");
                CheckExist("clrcdlen", "80");
                CheckExist("installlib", "QGPL");
                CheckExist("intellisense", "false");
                CheckExist("experimental", "false");
                PrintConfig();
            }


        }

        private static void PrintConfig()
        {
            List<string> fileout = new List<string>();
            foreach (var key in _config.Keys)
            {
                fileout.Add(key + '=' + _config[key]);
            }
            File.WriteAllLines(_ConfigFile, fileout.ToArray());
        }

        public static string GetConfig(string key)
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

        public static void SetConfig(string key, string value)
        {
            if (_config.ContainsKey(key))
            {
                _config[key] = value;
            }
            else
            {
                _config.Add(key, value);
            }

            PrintConfig();
        }

        public static void RemConfig(string key)
        {
            if (_config.ContainsKey(key))
            {
                _config.Remove(key);
            }
        }

        public static string[] GetListing()
        {
            return _list.ToArray();
        }

        public static void AddOutput(string text)
        {
            _output.Add(text);
        }

        public static string[] GetOutput()
        {
            string[] result = _output.ToArray();
            _output.Clear();
            return result;
        }

        public static void FlushOutput()
        {
            _output.Clear();
        }

        public static Boolean RunCommands(string[] list)
        {
            Boolean result = true;
            try
            {
                FlushOutput();
                string tempfile = Path.GetTempFileName();
                File.Move(tempfile, tempfile + ".ftp");
                tempfile += ".ftp";
                List<string> lines = new List<string>();

                lines.Add("user " + _config["username"]);
                lines.Add(_config["password"]);

                lines.Add($"QUOTE RCMD CHGLIBL LIBL({ IBMi.GetConfig("datalibl").Replace(',', ' ')})  CURLIB({ IBMi.GetConfig("curlib") })");
                foreach (string cmd in list)
                {
                    if (cmd == null) continue;
                    if (cmd.Trim() != "")
                    {
                        AddOutput("> " + cmd);
                        IBMiUtilities.DebugLog("Collecting command for ftp file: " + cmd);
                        lines.Add(cmd);
                    }
                }
#if DEBUG
                lines.Add("QUOTE RCMD DSPJOBLOG");
#endif
                lines.Add("quit");

                File.WriteAllLines(tempfile, lines.ToArray());
                result = RunFTP(tempfile);
                File.Delete(tempfile);

            }
            catch (Exception e)
            {
                IBMiUtilities.Log(e.ToString());
                MessageBox.Show(e.ToString());
            }

            return result;
        }

        private static Boolean RunFTP(string FileLoc)
        {
            IBMiUtilities.DebugLog("Starting FTP of command file " + FileLoc);

            _list.Clear();
            _NotConnected = false;
            _Failed = "";

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c FTP -n -s:\"" + FileLoc + "\" " + _config["system"];
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            IBMiUtilities.DebugLog("FTP of command file " + FileLoc + " completed");

            if (_NotConnected)
            {
                MessageBox.Show("Not able to connect to " + GetConfig("system"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (_Failed != "")
            {
                if (FTPCodeMessages.ContainsKey(_Failed))
                    MessageBox.Show(FTPCodeMessages[_Failed], "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return _Failed != "" || _NotConnected;
        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                IBMiUtilities.Log(outLine.Data);
                if (outLine.Data.Length >= 5)
                {
                    if (outLine.Data.Trim() == "Not connected.")
                    {
                        if (!_NotConnected) _output.Add("Not connected to " + _config["system"]);
                        _NotConnected = true;
                    }
                    else
                    {
                        switch (outLine.Data.Substring(0, 3))
                        {
                            case "125":
                                _getList = true;
                                break;
                            case "250":
                                _getList = false;
                                _output.Add("> " + outLine.Data.Substring(4));
                                break;
                            case "426":
                            case "530":
                            case "550":
                                _Failed = outLine.Data.Substring(0, 3);
                                _output.Add("> " + outLine.Data.Substring(4));
                                break;
                            default:
                                if (_getList) _list.Add(outLine.Data);
                                break;
                        }
                        if (GetConfig("experimental") == "true") _output.Add("* " + outLine.Data);
                    }
                }
            }
        }
    }
}
