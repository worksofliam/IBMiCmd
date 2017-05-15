using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DataStructure
    {
        public string name;
        public List<string> fields;

        public DataStructure(string name) {
            this.name = name;
            this.fields = new List<string>();
        }

        public bool Contains(String n) {
            return this.name == n;
        }
    }

    public class RPGParser
    {
        public const int RCDFMT_NAME = 47;
        public const int RCDFMT_NAME_LEN = 10;
        public const int RCDFMT_FIELD_NAME = 130;
        public const int RCDFMT_FIELD_NAME_LEN = 10;
        public const int RCDFMT_ALT_FIELD_NAME = 262;
        public const int RCDFMT_ALT_FIELD_LEN = 30;

        static List<DataStructure> dataStructures = null;

        public static void LoadFFD(string f, SourceLine srcLine) {
            if (dataStructures == null) dataStructures = new List<DataStructure>();

            bool firstLine = true;
            bool exists = false;
            DataStructure d = new DataStructure();  
            int i = 0;
            foreach (string line in File.ReadAllLines(f))
            {
                if (firstLine) {
                    foreach (DataStructure ds in dataStructures) {
                        if (ds.Contains(line.Substring(RCDFMT_NAME, RCDFMT_NAME_LEN))) {
                            exists = true;
                            d.name = ds.name;
                            d.fields = ds.fields;
                            i = dataStructures.IndexOf(ds);
                            break;
                        }
                    }
                    if (!exists) {
                        d.name = line.Substring(RCDFMT_NAME, RCDFMT_NAME_LEN);
                        d.fields = new List<string>();
                    }
                }

#if DEBUG
                IBMiUtilities.Log("File Substring" + line.Substring(RCDFMT_NAME, RCDFMT_NAME_LEN));
                IBMiUtilities.Log("Field Substring" + line.Substring(RCDFMT_FIELD_NAME, RCDFMT_FIELD_NAME_LEN));
                IBMiUtilities.Log("Alias Substring" + line.Substring(RCDFMT_ALT_FIELD_NAME, RCDFMT_ALT_FIELD_LEN));               
#endif

                if (srcLine.alias) {
                    if (line.Substring(RCDFMT_ALT_FIELD_NAME, RCDFMT_ALT_FIELD_LEN) != ""){

                        d.fields.Add(line.Substring(RCDFMT_ALT_FIELD_NAME, RCDFMT_ALT_FIELD_LEN));
                    } else {
                        d.fields.Add(line.Substring(RCDFMT_FIELD_NAME, RCDFMT_FIELD_NAME_LEN));
                    }
                } else {
                    d.fields.Add(line.Substring(RCDFMT_FIELD_NAME, RCDFMT_FIELD_NAME_LEN));
                }
            }

            if (!exists) dataStructures.Add(d);
            else dataStructures.Insert(i, d);
        }

        public static void LoadLikeDS(string f)
        {
            if (dataStructures == null) dataStructures = new List<DataStructure>();

            foreach (string line in File.ReadAllLines(f))
            {
                IBMi.addOutput("> " + line);
            }
        }


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

#if DEBUG
            IBMiUtilities.Log("Starting to parse current file for extName definitions...");
#endif

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

#if DEBUG
            IBMiUtilities.Log("Parsing completed... found " + lines.Count + " extName definitions.");
#endif

            return lines;
        }
    }
}
