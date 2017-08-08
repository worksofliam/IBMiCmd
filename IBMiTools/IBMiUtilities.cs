
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;

namespace IBMiCmd.IBMiTools
{
    public class IBMiUtilities
    {
        private static string _LogFile = null;

        /// <summary>
        /// Returns true if input string is a valid IBM i QSYS Object Name
        /// </summary>
        /// <param name="s">Object name to be verified</param>
        /// <returns>True if valid s is a valid object name</returns>
        public static bool IsValidQSYSObjectName(string s)
        {
            if (s == null || s == "") return false;
            if (s.Length > 10) return false;

            return true;
        }

        public static string[] GetMemberList(string Lib, string Obj)
        {
            List<string> commands = new List<string>();

            Lib = Lib.ToUpper();
            Obj = Obj.ToUpper();

            if (Lib == "*CURLIB") Lib = IBMi.GetConfig("curlib");

            commands.Add("cd /QSYS.lib/" + Lib + ".lib/" + Obj + ".file");
            commands.Add("ls");

            if (IBMi.RunCommands(commands.ToArray()) == false)
            {
                return IBMi.GetListing();
            }
            else
            {
                return null;
            }
        }

        public static string DownloadMember(string Lib, string Obj, string Mbr)
        {
            string filetemp = Path.GetTempPath() + Mbr + "." + Obj;
            List<string> commands = new List<string>();

            if (!File.Exists(filetemp)) File.Create(filetemp).Close();

            Lib = Lib.ToUpper();
            Obj = Obj.ToUpper();
            Mbr = Mbr.ToUpper();

            if (Lib == "*CURLIB") Lib = IBMi.GetConfig("curlib");

            commands.Add("ASCII");
            commands.Add("cd /QSYS.lib");
            commands.Add("recv \"" + Lib + ".lib/" + Obj + ".file/" + Mbr + ".mbr\" \"" + filetemp + "\"");

            if (IBMi.RunCommands(commands.ToArray()) == false)
            {
                return filetemp;
            }
            else
            {
                return "";
            }
        }

        public static bool UploadMember(string Local, string Lib, string Obj, string Mbr)
        {
            List<string> commands = new List<string>();

            Lib = Lib.ToUpper();
            Obj = Obj.ToUpper();
            Mbr = Mbr.ToUpper();

            if (Lib == "*CURLIB") Lib = IBMi.GetConfig("curlib");

            commands.Add("ASCII");
            commands.Add("cd /QSYS.lib");
            commands.Add("put \"" + Local + "\" \"" + Lib + ".lib/" + Obj + ".file/" + Mbr + ".mbr\"");

            //Returns true if successful
            return !IBMi.RunCommands(commands.ToArray());
        }

        /// <summary>
        /// Extracts the first substring between the search and limit parameters. 
        /// This method excludes the search and limit data from the result.
        /// </summary>
        /// <param name="input">String to search</param>
        /// <param name="search">String that marks the begining</param>
        /// <param name="limit">(Optional) String that marks the end of the result string</param>
        /// <returns>Substring of input</returns>
        public static string ExtractString(string input, string search, string limit = "")
        {
            if (input == null || search == null || search.Length > input.Length) return "";

            int start = input.IndexOf(search) + search.Length;
            if (start < 0) return "";
            string part = input.Substring(start);

            if (limit.Length == 0)
            {
                return part.Substring(0);
            }
            else {
                int end = part.IndexOf(limit);
                if (end < 0) return "";
                return part.Substring(0, end);
            }
        }

        internal static void CreateLog(string logFilePath)
        {
            _LogFile = logFilePath + ".log";
            File.WriteAllText(_LogFile, DateTime.Now.ToString() + " : Log Created..." + Environment.NewLine);
        }

        internal static string GetLogPath()
        {
            return _LogFile;
        }

        internal static void Log(string m)
        {
            File.AppendAllText(_LogFile, DateTime.Now.ToString() + " : " + m + Environment.NewLine);
        }
        
        internal static void DebugLog(string m)
        {
            File.AppendAllText(_LogFile, DateTime.Now.ToString() + " : " + m + Environment.NewLine);
        }
    }
}
