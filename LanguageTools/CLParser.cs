using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.LanguageTools
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CLCommand
    {
        public string command;

        public CLCommand(string name)
        {
            command = name;
        }

        public bool Equals(String c)
        {
            return this.command == c;
        }
    }

    public class CLParser
    {
        // In memory cache of cl commands
        public static List<CLCommand> CLCommands { get; set; }

        public static string GetCommandAtColumn(string curLine, int curPos)
        {
            if (curPos > curLine.Length) return "";
            StringBuilder sb = new StringBuilder();

            // Scan Backwards
            for (int i = curPos; i >= 0; i--)
            {
                if (i == curPos && curLine[i] == ' ') return "";
                else
                {
                    if (curLine[i] == ' ' || curLine[i] == '\'') break;
                    else
                    {
                        sb.Append(curLine[i]);
                    }
                }
            }

            // Scan Forward
            for (int i = curPos + 1; i < curLine.Length; i++)
            {
                if (i == curPos && curLine[i] == ' ') break;
                else
                {
                    if (curLine[i] == ' ' || curLine[i] == '\'') break;
                    else
                    {
                        sb.Insert(0, curLine[i]);
                    }
                }
            }

            char[] buffer = sb.ToString().ToCharArray();
            Array.Reverse(buffer);
            string command = new string(buffer);
            if (IBMiUtilities.IsValidQSYSObjectName(command))
            {
                return command;
            }
            else
            {
                return "";
            }
        }

        internal static void LoadFileCache()
        {
            CLCommands = new List<CLCommand>();
            foreach (string file in Directory.GetFiles(Main.FileCacheDirectory))
            {
                if (!file.EndsWith(".cdml"))
                {
                    continue;
                }

                //TODO 
            }
        }

        internal static void LoadCMD(string lookupString)
        {
            CLCommands.Add(new CLCommand(lookupString));

        }
    }
}
