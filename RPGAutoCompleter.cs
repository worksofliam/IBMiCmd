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

            string lookupString = RPGParser.GetVariableAtColumn(sb.ToString(), cursorPosition - 1);

            NotifyAutoCompletion(curScintilla, lookupString, MatchLine(lookupString));
        }

        private static void NotifyAutoCompletion(IntPtr curScintilla, string variable, List<string> matches)
        {
            if (matches.Count == 0)
            {
                // TODO; add information message instead of selectable text?
                return;
                //matches.Add("No match was found");
            }

            StringBuilder sb = new StringBuilder();
            foreach (string item in matches)
            {
                sb.Append($"{item.Trim()} ");
            }

            Win32.SendMessage(curScintilla, SciMsg.SCI_AUTOCSHOW, variable.Length, sb.ToString());
            return;
        }

        public static List<string> MatchLine(string variable)
        {
            List<string> matches = new List<string>();
            List<DataStructure> dataStructures = RPGParser.dataStructures;

            DataStructure start = new DataStructure()
            {
                name = "",
                fields = new List<DataColumn>(),
                dataStructures = dataStructures
            };
            MatchLineInDS(variable, matches, start);

            return matches;
        }

        private static void MatchLineInDS(string variable, List<string> matches, DataStructure dataStructure)
        {
            if (dataStructure.name.StartsWith(variable) && (dataStructure.name.Length >= variable.Length)) matches.Add(dataStructure.name);

            foreach (DataColumn field in dataStructure.fields)
            {
                if (field.name.StartsWith(variable) && (field.name.Length >= variable.Length)) matches.Add(field.name);
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