using System;
using System.Collections.Generic;
using NppPluginNET;
using System.Text;

namespace IBMiCmd
{
    public class RPGAutoCompleter
    {
        /// <summary>
        /// Uses informtion about data structures to notify NPP
        /// </summary>
        /// <param name="dataStructures"></param>
        internal static void ProvideSuggestions(List<DataStructure> dataStructures)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();

            int curLine = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETCURRENTLINE, 0, 0);
            int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, curLine, 0);

            if (lineLength <= 0) return;

            StringBuilder sb = new StringBuilder(lineLength);
            Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, curLine, sb);

            int cursorPosition = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_GETCURRENTPOS, 0, 0);         

            string variable = RPGParser.GetVariableAtColumn(sb.ToString(), cursorPosition);

            NotifyAutoCompletion(MatchLine(variable));
        }

        private static void NotifyAutoCompletion(List<string> matches)
        {


            return;
        }

        public static List<string> MatchLine(string variable)
        {
            List<string> matches = new List<string>();
            List<DataStructure> dataStructures = RPGParser.dataStructures;

            DataStructure start = new DataStructure()
            {
                name = "",
                fields = new List<string>(),
                dataStructures = dataStructures
            };
            MatchLineInDS(variable, matches, start);

            return matches;
        }

        private static void MatchLineInDS(string variable, List<string> matches, DataStructure dataStructure)
        {
            if (dataStructure.name.StartsWith(variable) && (dataStructure.name.Length >= variable.Length)) matches.Add(dataStructure.name);

            foreach (string field in dataStructure.fields)
            {
                if (field.StartsWith(variable) && (field.Length >= variable.Length)) matches.Add(field);
            }

            if (dataStructure.dataStructures != null)
            {
                foreach (DataStructure inner in dataStructure.dataStructures)
                {
                    MatchLineInDS(variable, matches, inner);
                }
            }            
        }
    }
}