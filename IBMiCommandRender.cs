using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IBMiCmd
{
    class IBMiCommandRender
    {

        internal static string[] RenderRelicRebuildScript(string tmp)
        {
            string buildDir = IBMi.getConfig("relicdir");
            if (!buildDir.EndsWith("/"))
            {
                buildDir += '/';
            }

            return new string[] {
                $"QUOTE RCMD CD '{ buildDir }'",
                $"QUOTE RCMD RBLD { IBMi.getConfig("reliclib") }",
                "ASCII",
                $"RECV { buildDir }RELICBLD.log \"{ tmp }\""
            };

        }

        internal static string[] RenderFFDCollectionScript(List<SourceLine> src, string[] tmp)
        {
            IBMiUtilities.DebugLog("RenderFFDCollectionScript");
            string[] cmd = new string[(src.Count * 3) + 2];
            int i = 0, t = 0;
            // Run commands on remote
            cmd[i++] = "ASCII";
            cmd[i++] = $"QUOTE RCMD CHGLIBL LIBL({ IBMi.getConfig("datalibl").Replace(',', ' ')})  CURLIB({ IBMi.getConfig("curlib") })";
            foreach (SourceLine sl in src)
            {
                cmd[i++] = $"QUOTE RCMD NPPDSPFFD {sl.searchResult}";
                cmd[i++] = $"RECV /home/{ IBMi.getConfig("username") }/{ sl.searchResult }.tmp \"{ tmp[t++] }\"";
                cmd[i++] = $"QUOTE RCMD RMVLNK OBJLNK('/home/{ IBMi.getConfig("username") }/{ sl.searchResult }.tmp')";
            }
            
            IBMiUtilities.DebugLog("RenderFFDCollectionScript - DONE!");
            return cmd;
        }

        internal static string[] RenderRemoteInstallScript(List<string> sourceFiles, string library)
        {
            IBMiUtilities.DebugLog("RenderRemoteInstallScript");
            // Make room for <upload, copy, delete, compile> for each file
            string[] cmd = new string[sourceFiles.Count * 4 + 3];
            int i = 0;
            cmd[i++] = "ASCII";
            cmd[i++] = "QUOTE RCMD CRTPF FILE(QTEMP/NPPCLSRC)  RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy NPP plugin commands')";
            cmd[i++] = "QUOTE RCMD CRTPF FILE(QTEMP/NPPCMDSRC) RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy NPP plugin commands')";
            foreach (string file in sourceFiles)
            {
                string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                string member = fileName.Substring(fileName.LastIndexOf("-") + 1, fileName.LastIndexOf(".") - (fileName.LastIndexOf("-") + 1));
                string sourceFile = null, crtCmd = null;

                switch (fileName.Substring(fileName.Length - 4))
                {
                    case ".clp":
                        sourceFile = "NPPCLSRC";
                        crtCmd = $"CRTCLPGM PGM({library}/{member}) SRCFILE(QTEMP/NPPCLSRC) SRCMBR({member}) REPLACE(*YES) TEXT('{Main.PluginDescription}')";
                        break;
                    case ".cmd":
                        sourceFile = "NPPCMDSRC";
                        crtCmd = $"CRTCMD CMD({library}/{member}) PGM({library}/{member}) SRCFILE(QTEMP/NPPCMDSRC) SRCMBR({member}) REPLACE(*YES) TEXT('{Main.PluginDescription}')";
                        break;
                    default:
                        continue;
                }

                cmd[i++] = $"SEND { file } /home/{ IBMi.getConfig("username") }/{ fileName }";
                cmd[i++] = $"QUOTE RCMD CPYFRMSTMF FROMSTMF('/home/{ IBMi.getConfig("username") }/{ fileName }') TOMBR('/QSYS.LIB/QTEMP.LIB/{ sourceFile }.FILE/{ member }.MBR')";
                cmd[i++] = $"QUOTE RCMD RMVLNK OBJLNK('/home/{ IBMi.getConfig("username") }/{ fileName }')";
                cmd[i++] = $"QUOTE RCMD { crtCmd }";
            }
            IBMiUtilities.DebugLog("RenderRemoteInstallScript - DONE!");
            return cmd;
        }
    }
}
