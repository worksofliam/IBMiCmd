using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NppPluginNET;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;

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
    public struct DataColumn
    {
        public string name;
        public string type;
        public int length;
        //public int size; 
    }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DataStructure
    {
        public string name;
        public List<DataColumn> fields;
        public List<DataStructure> dataStructures;

        public DataStructure(string name) {
            this.name = name;
            this.fields = new List<DataColumn>();
            this.dataStructures = null;
        }

        public bool Contains(String n) {
            return this.name == n;
        }
    }

    public class RPGParser
    {
        // In memory cache of data structures referenced by source code
        public static List<DataStructure> dataStructures { get; set; }

        ///  DSPFFD Format 
        private const int DSPFFD_FILE_NAME                      = 46;
        private const int DSPFFD_FILE_NAME_LEN                  = 10;
        private const int DSPFFD_FIELD_NAME                     = 129;
        private const int DSPFFD_FIELD_NAME_LEN                 = 10;
        private const int DSPFFD_FIELD_ALIAS                    = 781;
        private const int DSPFFD_FIELD_ALIAS_LEN                = 258;
        private const int DSPFFD_FIELD_BYTE_SIZE                = 159;
        private const int DSPFFD_FIELD_BYTE_SIZE_LEN            = 5;
        private const int DSPFFD_FIELD_NO_OF_DIGITS             = 164;
        private const int DSPFFD_FIELD_NO_OF_DIGITS_LEN         = 2;
        private const int DSPFFD_FIELD_NO_OF_DECMIALS           = 166;
        private const int DSPFFD_FIELD_NO_OF_DECMIALS_LEN       = 2;
        private const int DSPFFD_FIELD_GRAHPIC_CHAR_COUNT       = 586;
        private const int DSPFFD_FIELD_GRAHPIC_CHAR_COUNT_LEN   = 5;
        private const int DSPFFD_FIELD_TYPE                     = 321;
        private const int DSPFFD_FIELD_TYPE_LEN                 = 1;
        private const int DSPFFD_FIELD_CCSID                    = 491;
        private const int DSPFFD_FIELD_CCSID_LEN                = 3;
        //        private const int DSPFFD_FILE_NAME_LEN = 10;

        public static string GetVariableAtColumn(string curLine, int curPos )
        {
            if (curPos > curLine.Length) return "";
            StringBuilder sb = new StringBuilder();
            if (curLine[--curPos] == '.')
            {                
                for (int i = curPos - 1; i >= 0; i--)
                {
                    if (curLine[i] == ' ' || curLine[i] == '.' || curLine[i] == ')' || curLine[i] == '(') break;
                    else
                    {
                        if (curLine[i] == ' ') break;
                        else
                        {
                            sb.Append(curLine[i]);
                        }                        
                    }                    
                }
            }
            else
            {
                for (int i = curPos; i >= 0; i--)
                {
                    if (i == curPos && curLine[i] == ' ') return "";
                    else
                    {
                        if (curLine[i] == ' ' || curLine[i] == '.' || curLine[i] == ')' || curLine[i] == '(') break;
                        else
                        {
                            sb.Append(curLine[i]);
                        }
                    }
                }
            }

            if (Regex.Match(sb.ToString(), "^[a-zA-Z]\\w*$").Success)
            {
                char[] buff = sb.ToString().ToCharArray();
                Array.Reverse(buff);
                return new string(buff);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Executes remote commands on the configured server to collect data on externally
        /// referenced tables that is then put into both memory and file cache
        /// </summary>
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
        /// Gets a path to a file containing the output of a DSPFFD command
        /// Parses the output into a DataStructure item and updates the file cache
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
                if (firstLine)
                {
                    firstLine = false;
                    foreach (DataStructure ds in dataStructures)
                    {
                        if (ds.Contains(l.Substring(DSPFFD_FILE_NAME, DSPFFD_FILE_NAME_LEN)))
                        {
                            exists = true;
                            d.name = ds.name;
                            d.fields = new List<DataColumn>(); // Create new list because format might have changed
                            i = dataStructures.IndexOf(ds);
                            break;
                        }
                    }
                    if (!exists)
                    {
                        d.name = l.Substring(DSPFFD_FILE_NAME, DSPFFD_FILE_NAME_LEN);
                        d.fields = new List<DataColumn>();
                    }                    
                }
                d.fields.Add(FFDParseColumn(l, srcLine));
            }

            if (!exists) dataStructures.Add(d);
            else
            {
                dataStructures.RemoveAt(i);
                dataStructures.Insert(i, d);
            }

            UpdateFileCache(d);
        }

        private static DataColumn FFDParseColumn(string l, SourceLine srcLine)
        {
            DataColumn column = new DataColumn()
            {
                name = FFDParseColumnName(l, srcLine),
                type = FFDParseColumnType(l, srcLine),
                length = FFDParseColumnLength(l, srcLine)
            };
            return column;
        }

        private static int FFDParseColumnLength(string l, SourceLine srcLine)
        {
           return int.Parse(l.Substring(DSPFFD_FIELD_BYTE_SIZE, DSPFFD_FIELD_BYTE_SIZE_LEN));
        }

        private static string FFDParseColumnType(string l, SourceLine srcLine)
        {
            int length = 0, ccsid = 37;

            string type = l.Substring(DSPFFD_FIELD_TYPE, DSPFFD_FIELD_TYPE_LEN);
            try { 
                length = int.Parse(l.Substring(DSPFFD_FIELD_BYTE_SIZE, DSPFFD_FIELD_BYTE_SIZE_LEN).Replace('0', ' '));
                //ccsid = int.Parse(l.Substring(DSPFFD_FIELD_CCSID, DSPFFD_FIELD_CCSID_LEN)); TODO, how to extract packed numerics
            }
            catch (Exception e) {
                IBMiUtilities.Log($"Tried to parse {l.Substring(DSPFFD_FIELD_BYTE_SIZE, DSPFFD_FIELD_BYTE_SIZE_LEN)} and {l.Substring(DSPFFD_FIELD_CCSID, DSPFFD_FIELD_CCSID_LEN)} to integer. {e.ToString()}");
            }

            switch (type)
            {
                case "B":
                    switch (length)
                    {
                        case 1:
                            return "int(3)";
                        case 2:
                            return "int(5)";
                        case 4:
                            return "int(10)";
                        case 8:
                            return "int(20)";
                        default:
                            return "binary";
                    }
                case "A":
                    return "char";
                case "S":
                    return "zoned";
                case "P":
                    return "packed";
                case "F":
                    return "float";
                case "O":
                    return "unknown";
                case "J":
                    return "unknown";
                case "E":
                    return "unknown";
                case "H":
                    return "rowid";
                case "L":
                    return "date";
                case "T":
                    return "time";
                case "Z":
                    return "timestamp";
                case "G":
                    return "graphic";                    
                case "1":
                    switch (ccsid)
                    {
                        case 65535:
                            return "blob";
                        default:
                            return "clob";
                    }
                case "2":
                    return "unknown";
                case "3":
                    return "dbclob";
                case "4":
                    return "datalink";
                case "5":
                    return "binary";
                case "6":
                    return "dbclob";
                case "7":
                    return "xml";
                default:
                    return "unknown";
            }
        }

        private static string FFDParseColumnName(string l, SourceLine srcLine)
        {
            if (srcLine.alias)
            {
                if (l.Substring(DSPFFD_FIELD_ALIAS, DSPFFD_FIELD_ALIAS_LEN) != "")
                {
                    return l.Substring(DSPFFD_FIELD_ALIAS, DSPFFD_FIELD_ALIAS_LEN);
                }
                else
                {
                    return l.Substring(DSPFFD_FIELD_NAME, DSPFFD_FIELD_NAME_LEN);
                }
            }
            else
            {
                return l.Substring(DSPFFD_FIELD_NAME, DSPFFD_FIELD_NAME_LEN);
            }
        }

        /// <summary>
        /// Loads the files in the plugin folder /plugins/config/cache/
        /// and parses and adds the content into the data structure list
        /// </summary>
        internal static void LoadFileCache()
        {
            IBMiUtilities.DebugLog("Loading cached files...");
            dataStructures = new List<DataStructure>();
            foreach(string file in Directory.GetFiles(Main.fileCacheDirectory))
            {
                IBMiUtilities.DebugLog($"Loading cached file {file}");
                DataStructure ds = new DataStructure(" ");
                if (!file.EndsWith(".ffd"))
                {
                    IBMiUtilities.Log($"{file} does not match the plugin cache format");
                    continue;
                }
                bool firstLine = true;
                foreach (string line in File.ReadAllLines(file))
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        if (!line.StartsWith("<ffd name="))
                        {
                            IBMiUtilities.Log($"{file} does not match the plugin cache format");
                            break;
                        }
                        ds.name = IBMiUtilities.ExtractString(line, "=\"", "\"");
                    }
                    else
                    {
                        if (line.TrimStart().StartsWith("<column"))
                        {
                            DataColumn dc = new DataColumn();
                            dc.name = IBMiUtilities.ExtractString(line, ">", "<");
                            dc.type = IBMiUtilities.ExtractString(line, "type=\"", "\"");

                            try
                            {
                                dc.length = int.Parse(IBMiUtilities.ExtractString(line, "length=\"", "\""));
                            }
                            catch (Exception e)
                            {
                                IBMiUtilities.Log($"Loading cached column length data failed: {e.ToString()}");
                                dc.length = -1;
                            }
                            ds.fields.Add(dc);
                        }
                        else
                        {
                            if (!line.Equals("</ffd>")) 
                            {
                                IBMiUtilities.Log($"{file} does not match the plugin cache format");
                                break;
                            }
                            else
                            {
                                dataStructures.Add(ds);   
                            }
                        }
                    }
                }
            }
            IBMiUtilities.DebugLog("Loading cached files completed.");
        }

        private static void UpdateFileCache(DataStructure d)
        {
            IBMiUtilities.DebugLog("Updating file cache...");
            string cacheFile = $"{Main.fileCacheDirectory}/{d.name.TrimEnd()}.ffd";
            string[] fileContent = new string[2 + d.fields.Count];

            int i = 0;
            fileContent[i++] = $"<ffd name=\"{d.name.TrimEnd()}\"";
            foreach (DataColumn field in d.fields)
            {
                fileContent[i++] = $"\t\t<column type=\"{field.type}\" length=\"{field.length}\">{field.name.TrimEnd()}</column>";
            }
            fileContent[i++] = "</ffd>";

            File.WriteAllLines(cacheFile, fileContent);
            IBMiUtilities.DebugLog("Updating file cache completed.");
        }

        /// <summary>
        /// TODO: Parse all data structures in code and retrieve definitions for them
        ///       follow include files recursivly...
        /// </summary>
        /// <param name="f"></param>
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
                        searchResult = IBMiUtilities.ExtractString(sourceStatement, "EXTNAME('", "')"),
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
    }
}
