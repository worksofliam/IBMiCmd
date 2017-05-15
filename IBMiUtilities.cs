﻿
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace IBMiCmd
{
    public class IBMiUtilities
    {
        private static string _LogFile = null;

        /// <summary>
        /// Returns true if input string is a valid IBM i QSYS Object Name
        /// </summary>
        /// <param name="s">Object name to be verified</param>
        /// <returns>True if valid s is a valid object name</returns>
        public static bool isValidQSYSObjectName(string s)
        {
            if (s == null) return false;
            return Regex.Match(s, "^[a-zA-Z]\\w{0,9}$").Success;
        }

        /// <summary>
        /// Extracts the first substring between the search and limit parameters, this method is excludes search and limit from the result.
        /// </summary>
        /// <param name="input">String to search</param>
        /// <param name="search">String that marks the begining</param>
        /// <param name="limit">(Optional) String that marks the end of the result string</param>
        /// <returns>Substring of input</returns>
        public static string extractString(string input, string search, string limit = "")
        {
            if (input == null || search == null) return "";
            if (search.Length > input.Length) return "";        
            int start = input.IndexOf(search) + search.Length;
            if (start > input.Length) return "";
            string part = input.Substring(start);

            if (limit.Length == 0)
            {
                return part.Substring(0);
            }
            else {
                int end = part.IndexOf(limit);
                if (end <= 0) return "";
                return part.Substring(0, end);
            }
        }

        public static void CreateLog(string logFilePath)
        {
            _LogFile = logFilePath + ".log";
            File.WriteAllText(_LogFile, DateTime.Now.ToString() + " : Log Created..." + Environment.NewLine);
        }

        public static void Log(string m)
        {
            File.AppendAllText(_LogFile, DateTime.Now.ToString() + " : " + m + Environment.NewLine);
        }
    }
}