using System;
using System.Collections.Generic;
using NppPluginNET;
using System.Text;

namespace IBMiCmd
{
    public class RPGAutoCompleter
    {
        /// <summary>
        /// Uses information about data structures to notify SCI Autocompletion
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
            int lineOffsetPosition = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, curLine, 0);
            int linePosition = cursorPosition - lineOffsetPosition;

            string lookupString = RPGParser.GetVariableAtColumn(sb.ToString(), linePosition);

            if (lookupString == "") return;

            NotifyAutoCompletion(curScintilla, lookupString, SearchDataStructureDefinitions(lookupString));
        }

        private static void NotifyAutoCompletion(IntPtr curScintilla, string lookupString, List<string> matches)
        {
            if (matches.Count == 0)
            {                
                return; // TODO; add information message instead of selectable text?
                //matches.Add("No match was found");
            }

            StringBuilder sb = new StringBuilder();
            foreach (string item in matches)
            {
                sb.Append($"{item.Trim()} ");
            }

            Win32.SendMessage(curScintilla, SciMsg.SCI_AUTOCSHOW, 0, sb.ToString());
           
            return;
        }

        /// <summary>
        /// Searches the cached data structures and their fields for name matches
        /// </summary>
        /// <param name="lookupString"></param>
        /// <returns></returns>
        public static List<string> SearchDataStructureDefinitions(string lookupString)
        {
            List<string> matches = new List<string>();
            List<DataStructure> dataStructures = RPGParser.dataStructures;
            DataStructure start = new DataStructure()
            {
                name = "",
                fields = new List<DataColumn>(),
                dataStructures = dataStructures
            };
            SearchDataStructureDefinition(lookupString, matches, start);
            return matches;
        }

        private static void SearchDataStructureDefinition(string lookupString, List<string> matches, DataStructure dataStructure)
        {
            if (dataStructure.name.ToUpper().StartsWith(lookupString.ToUpper()) && (dataStructure.name.Length > lookupString.Length))
            {
                matches.Add(dataStructure.name);
            }
            else if (dataStructure.name.ToUpper().StartsWith(lookupString.ToUpper()) && (dataStructure.name.Length == lookupString.Length))
            {
                foreach (DataColumn field in dataStructure.fields)
                {
                    matches.Add(field.name);
                }
            }
            else
            {
                foreach (DataColumn field in dataStructure.fields)
                {
                    if (field.name.ToUpper().StartsWith(lookupString.ToUpper()) && (field.name.Length > lookupString.Length))
                    {
                        matches.Add(field.name);
                    }
                }
            }

            if (dataStructure.dataStructures != null)
            {
                foreach (DataStructure inner in dataStructure.dataStructures)
                {
                    SearchDataStructureDefinition(lookupString, matches, inner);
                }
            }            
        }
    }
}