using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NppPluginNET;
using System.Runtime.InteropServices;

namespace IBMiCmd
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SourceLine
    {
        public string statement;
        public string searchResult;
        public int lineNumber;
        public bool alias;
    }

    public class RPGParser
    {
        /// <summary>
        /// Returns a list of SourceLine structs that contains names of externally described files
        /// in the searchResult field of the data structure
        /// </summary>
        /// <returns>SourceLine(s) from current file containing EXTNAME</returns>
        public static List<SourceLine> parseCurrentFileForExtName()
        {
            // extname('F')
            const int MINIMUM_LINE_LENGTH_FOR_EXTNAME = 12;
            const short END_OF_FILE = 0;
            int line = 0;
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();

            List<SourceLine> lines = new List<SourceLine>();

            while (true)
            {
                line++;
                int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, line, 0);

                if (lineLength == END_OF_FILE) break;
                else if (lineLength < MINIMUM_LINE_LENGTH_FOR_EXTNAME) continue;

                StringBuilder sb = new StringBuilder(lineLength);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, line, sb);

                if (sb == null) break;

                string sourceStatement = sb.ToString().ToUpper();
                if (sourceStatement.Contains("EXTNAME("))
                {
                    SourceLine result = new SourceLine();

                    result.statement = sourceStatement;
                    result.searchResult = IBMiUtilities.extractString(sourceStatement, "EXTNAME('", "')");
                    result.lineNumber = line;

                    if (sourceStatement.Contains("ALIAS")) result.alias = true;
                    else result.alias = false;

                    lines.Add(result);
                }
            }

            return lines;
        }
    }
}
