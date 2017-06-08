using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Serialization;
using IBMiCmd.Properties;

namespace IBMiCmd.IBMiTools
{
    class IBMiCommandRender
    {

        internal static string[] RenderFFDCollectionScript(List<SourceLine> src, string[] tmp)
        {
            IBMiUtilities.DebugLog("RenderFFDCollectionScript");
            string[] cmd = new string[(src.Count * 3) + 2];
            int i = 0, t = 0;
            // Run commands on remote
            cmd[i++] = "ASCII";
            cmd[i++] = $"QUOTE RCMD CHGLIBL LIBL({ IBMi.GetConfig("datalibl").Replace(',', ' ')})  CURLIB({ IBMi.GetConfig("curlib") })";
            foreach (SourceLine sl in src)
            {
                cmd[i++] = $"QUOTE RCMD { IBMi.GetConfig("installlib") }/IICDSPFFD { sl.searchResult }";
                cmd[i++] = $"RECV /home/{ IBMi.GetConfig("username") }/{ sl.searchResult }.tmp \"{ tmp[t++] }\"";
                cmd[i++] = $"QUOTE RCMD RMVLNK OBJLNK('/home/{ IBMi.GetConfig("username") }/{ sl.searchResult }.tmp')";
            }
            
            IBMiUtilities.DebugLog("RenderFFDCollectionScript - DONE!");
            return cmd;
        }

        internal static string[] RenderCommandDescriptionCollection(string command)
        {
            IBMiUtilities.DebugLog("RenderCommandDescriptionCollection");
            string[] cmd = new string[5];
            // Run commands on remote
            int i = 0;
            cmd[i] = "ASCII";
            cmd[++i] = $"QUOTE RCMD CHGLIBL LIBL({ IBMi.GetConfig("datalibl").Replace(',', ' ')})  CURLIB({ IBMi.GetConfig("curlib") })";
            cmd[++i] = $"QUOTE RCMD { IBMi.GetConfig("installlib") }/IICRTVCMD {command}";
            cmd[++i] = $"RECV /home/{ IBMi.GetConfig("username") }/{ command }.cdml \"{ Main.FileCacheDirectory }{ command }.cdml\"";
            cmd[++i] = $"QUOTE RCMD RMVLNK OBJLNK('/home/{ IBMi.GetConfig("username") }/{ command }.cdml')";

            IBMiUtilities.DebugLog("RenderCommandDescriptionCollection - DONE!");
            return cmd;
        }

        internal static string[] RenderRemoteInstallScript(List<string> sourceFiles, string library)
        {
            List<string> cmd = new List<string>
            {
                "ASCII",
                "QUOTE RCMD CRTPF FILE(QTEMP/IICCLSRC) RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy IBMiCmd plugin')",
                "QUOTE RCMD CRTPF FILE(QTEMP/IICCMDSRC) RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy IBMiCmd plugin')",
                "QUOTE RCMD CRTPF FILE(QTEMP/IICRPGSRC) RCDLEN(240) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy IBMiCmd plugin')"
            };
            foreach (string file in sourceFiles)
            {
                IBMiUtilities.DebugLog($"RenderRemoteInstallScript processsing {file}");
                string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                string member = fileName.Substring(fileName.LastIndexOf("-") + 1, fileName.LastIndexOf(".") - (fileName.LastIndexOf("-") + 1));
                string sourceFile = null, crtCmd = null;


                switch (fileName.Substring(fileName.Length - 4))
                {
                    case ".CLP":
                        sourceFile = Resources.RTVCMDCMD;
                        crtCmd = getCompileCommandCL(library, member);
                        break;
                    case ".CMD":
                        sourceFile = "IICCMDSRC";
                        crtCmd = getCompileCommandCmd(library, member);
                        break;
                    case ".RPG":
                        sourceFile = "IICRPGSRC";
                        crtCmd = getCompileCommandRpg(library, member);
                        break;
                    default:
                        sourceFile = "IICRPGSRC";
                        break;
                }

                cmd.Add($"SEND \"{ file }\" /home/{ IBMi.GetConfig("username") }/{ fileName }");
                cmd.Add($"QUOTE RCMD CPYFRMSTMF FROMSTMF('/home/{ IBMi.GetConfig("username") }/{ fileName }') TOMBR('/QSYS.LIB/QTEMP.LIB/{ sourceFile }.FILE/{ member }.MBR')");
                cmd.Add($"QUOTE RCMD RMVLNK OBJLNK('/home/{ IBMi.GetConfig("username") }/{ fileName }')");
                if (crtCmd != null)
                {
                    cmd.Add($"QUOTE RCMD { crtCmd }");
                }                
            }
            return cmd.ToArray();
        }

        private static string getCompileCommandCL(string library, string member)
        {
            return $"CRTCLPGM PGM({library}/{member}) SRCFILE(QTEMP/IICCLSRC) SRCMBR({member}) REPLACE(*YES) TEXT('{Main.PluginDescription}')"; 
        }

        private static string getCompileCommandCmd(string library, string member)
        {
            return $"CRTCMD CMD({library}/{member}) PGM({library}/{member}) SRCFILE(QTEMP/IICCMDSRC) SRCMBR({member}) REPLACE(*YES) TEXT('{Main.PluginDescription}')";
        }

        private static string getCompileCommandRpg(string library, string member)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"CRTSQLRPGI OBJ({library}/{member}) SRCFILE(QTEMP/IICRPGSRC) ");
            sb.Append($"COMMIT(*CHG) OBJTYPE(*PGM) OPTION(*XREF) RPGPPOPT(*LVL2) DLYPRP(*YES) ");
            sb.Append($"DATFMT(*ISO) TIMFMT(*ISO) REPLACE(*YES) ");
            sb.Append($"DBGVIEW(*SOURCE) USRPRF(*USER) LANGID(*JOBRUN) ");
            sb.Append($"COMPILEOPT('DFTACTGRP(*NO) ACTGRP(*CALLER)')");
            return sb.ToString();
        }
    }
}
