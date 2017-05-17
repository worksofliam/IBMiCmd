using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NppPluginNET;
using System.Runtime.InteropServices;
using System.Threading;

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
        public List<DataStructure> dataStructures;

        public DataStructure(string name) {
            this.name = name;
            this.fields = new List<string>();
            this.dataStructures = null;
        }

        public bool Contains(String n) {
            return this.name == n;
        }
    }

    public class RPGParser
    {
        private const int DSPFFD_FILE_NAME       = 46;
        private const int DSPFFD_FILE_NAME_LEN   = 10;
        private const int DSPFFD_FIELD_NAME      = 129;
        private const int DSPFFD_FIELD_NAME_LEN  = 10;

        internal static string GetVariableAtColumn(string curLine, int curPos )
        {
            return "";
        }

        private const int DSPFFD_ALT_FIELD_NAME  = 261;
        private const int DSPFFD_ALT_FIELD_LEN   = 30;

        public static List<DataStructure> dataStructures { get; set; }

        internal static void LaunchFFDCollection()
        {
            Thread thread = new Thread((ThreadStart)delegate {
                IBMiUtilities.DebugLog("launchFFDCollection start");
                List<SourceLine> src = ParseCurrentFileForExtName();
                if (src.Count == 0) return;
                // Generate temporary files to receive data
                string[] tmp = new string[src.Count];
                for (int i = 0; i < src.Count; i++)
                {
                    tmp[i] = Path.GetTempFileName();
                }

                // Receive record formats via remote command NPPDSPFFD
                IBMi.runCommands(IBMiCommandRender.RenderFFDCollectionScript(src, tmp)); // Get all record formats to local temp files

                // Load Context & Cleanup temp files
                for (int i = 0; i < src.Count; i++)
                {
                    try
                    {
                        RPGParser.LoadFFD(tmp[i], src[i]);
                    }
                    catch (Exception e)
                    {
                        IBMiUtilities.Log(e.ToString()); // TODO: Show error?
                    }
                    finally
                    {
                        File.Delete(tmp[i]);
                    }
                }
                IBMiUtilities.DebugLog("launchFFDCollection end");
            });
            thread.Start();
        }

        /// <summary>
        /// Gets a file containing the output of a DSPFFD command
        /// Parses the output into a DataStructure item
        /// </summary>
        /// <param name="f">File with DSPFFD output</param>
        /// <param name="srcLine">source line with extName</param>
        internal static void LoadFFD(string f, SourceLine srcLine) {
            IBMiUtilities.DebugLog("LoadFFD -> File name: " + f);
            if (f == null || f == "") return; 
            if (dataStructures == null) dataStructures = new List<DataStructure>();

            bool firstLine = true;
            bool exists = false;
            DataStructure d = new DataStructure();  
            int i = 0;
            foreach (string l in File.ReadAllLines(f))
            {
                if (firstLine) {
                    foreach (DataStructure ds in dataStructures) {
                        if (ds.Contains(l.Substring(DSPFFD_FILE_NAME, DSPFFD_FILE_NAME_LEN))) {
                            exists = true;
                            d.name = ds.name;
                            d.fields = new List<string>(); // Createw new list because format might have changed
                            i = dataStructures.IndexOf(ds);
                            break;
                        }
                    }
                    if (!exists) {
                        d.name = l.Substring(DSPFFD_FILE_NAME, DSPFFD_FILE_NAME_LEN);
                        d.fields = new List<string>();
                    }
                }             

                if (srcLine.alias) {
                    if (l.Substring(DSPFFD_ALT_FIELD_NAME, DSPFFD_ALT_FIELD_LEN) != ""){
                        d.fields.Add(l.Substring(DSPFFD_ALT_FIELD_NAME, DSPFFD_ALT_FIELD_LEN));
                    } else {
                        d.fields.Add(l.Substring(DSPFFD_FIELD_NAME, DSPFFD_FIELD_NAME_LEN));
                    }
                } else {
                    d.fields.Add(l.Substring(DSPFFD_FIELD_NAME, DSPFFD_FIELD_NAME_LEN));
                }
            }

            if (!exists) dataStructures.Add(d);
            else dataStructures.Insert(i, d);
        }

        internal static void LoadLikeDS(string f)
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
        private static List<SourceLine> ParseCurrentFileForExtName()
        {
            IBMiUtilities.DebugLog("Starting to parse current file for extName definitions...");
            const int MINIMUM_LINE_LENGTH_FOR_EXTNAME = 12;
            const short END_OF_FILE = 0;
            int line = 0;
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();

            List<SourceLine> lines = new List<SourceLine>();

            while (true)
            {
                int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, ++line, 0);
                if (lineLength == END_OF_FILE) break;
                else if (lineLength < MINIMUM_LINE_LENGTH_FOR_EXTNAME) continue;

                StringBuilder sb = new StringBuilder(lineLength);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, line, sb);

                if (sb == null) break;

                string sourceStatement = sb.ToString().ToUpper();

                if (sourceStatement.Contains("EXTNAME("))
                {
                    SourceLine result = new SourceLine()
                    {
                        statement = sourceStatement,
                        searchResult = IBMiUtilities.extractString(sourceStatement, "EXTNAME('", "')"),
                        lineNumber = line
                    };
                    if (sourceStatement.Contains("ALIAS")) result.alias = true;
                    else result.alias = false;

                    lines.Add(result);
                }
            }

            IBMiUtilities.DebugLog("Parsing completed... found " + lines.Count + " extName definitions.");

            return lines;
        }

        /// <summary>
        /// TODO Sends information to the auto complete function of notepad++
        /// </summary>
        internal static void NotifyNPP()
        {
            IBMiUtilities.DebugLog("TODO: Update NPP Definitions for auto complete!");
        }
    }
}
