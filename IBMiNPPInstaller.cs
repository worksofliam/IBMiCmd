using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NppPluginNET;

namespace IBMiCmd
{
    class IBMiNPPInstaller
    {
        /// <summary>
        /// TODO: ?
        /// </summary>
        internal static void RebuildRelic()
        {
            Thread gothread = new Thread((ThreadStart)delegate {
                IBMiUtilities.DebugLog("RebuildRelic!");
                string tmpFile = Path.GetTempFileName();
                IBMi.AddOutput("Starting build of '" + IBMi.GetConfig("relicdir") + "' into " + IBMi.GetConfig("reliclib"));
                if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
                IBMi.RunCommands(IBMiCommandRender.RenderRelicRebuildScript(tmpFile));
                IBMi.AddOutput("");
                foreach (string line in File.ReadAllLines(tmpFile))
                {
                    IBMi.AddOutput($"> { line }");
                }
                IBMi.AddOutput("");
                IBMi.AddOutput("End of build.");
                File.Delete(tmpFile);
                if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
                IBMiUtilities.DebugLog("RebuildRelic - DONE!");
            });
            gothread.Start();
        }

        /// <summary>
        /// Installs the remote objects that the plugin requires on the server
        /// </summary>
        internal static void InstallRemoteLib(string library = "QGPL")
        {
            Thread thread = new Thread((ThreadStart) delegate {
                IBMiUtilities.DebugLog($"InstallRemoteLib -> {library}!");
                try
                {
                    List<string> sourceFiles = GenerateRemoteSource();

                    IBMi.RunCommands(IBMiCommandRender.RenderRemoteInstallScript(sourceFiles, library));

                    // Cleanup temp files
                    foreach (string file in sourceFiles)
                    {
                        File.Delete(file);
                    }

                    IBMi.SetConfig("installlib", library);
                } catch (Exception e) {
                    IBMiUtilities.Log(e.ToString()); // TODO: Show error?
                }
                IBMiUtilities.DebugLog("InstallRemoteLib - DONE!");
            });
            thread.Start();
        }

        internal static void InstallLocalDefinitions()
        {
            IBMiUtilities.DebugLog("InstallLocalDefinitions starting...");
            string functionList = $"%APPDATA%/Roaming/Notepad++/functionList.xml";
            List<string> outputBuffer = new List<string>();
            StringBuilder sb = new StringBuilder();

            bool associationFound = false;
            bool parserFound = false;
            IBMiUtilities.DebugLog("InstallLocalDefinitions parsing functionList...");
            foreach (string line in File.ReadAllLines(functionList))
            {
                if (!associationFound)
                {
                    if (line.Contains("<association ext=\".sqlrpgle\""))
                    {
                        associationFound = true;
                    }
                }
                if (line.Contains("</associationMap>") && !associationFound)
                {
                    IBMiUtilities.DebugLog("InstallLocalDefinitions writing association to functionList...");
                    outputBuffer.Add("<association ext=\".sqlrpgle\" userDefinedLangName=\"sqlrpgle\" id=\"sqlrpgle\"/>");
                }

                if (!parserFound)
                {
                    if (line.Contains("<parser id=\"sqlrpgle\""))
                    {
                        parserFound = true;
                    }
                }
                if (line.Contains("</parser>") && !parserFound)
                {
                    IBMiUtilities.DebugLog("InstallLocalDefinitions writing parser to functionList...");
                    outputBuffer.Add("\t\t\t<parser id=\"sqlrpgle\" displayName=\"SQLRPGLE\">");
                    outputBuffer.Add("\t\t\t\t<function");
                    outputBuffer.Add("\t\t\t\t\tmainExpr=\"(\bdcl - proc\\s)(\\w +)\"");
                    outputBuffer.Add("\t\t\t\t\tdisplayMode=\"$functionName\">");
                    outputBuffer.Add("\t\t\t\t\t<functionName>");
                    outputBuffer.Add("\t\t\t\t\t\t<nameExpr expr=\"(?<= dcl - proc).*\"/>");
                    outputBuffer.Add("\t\t\t\t\t</functionName>");
                    outputBuffer.Add("\t\t\t\t</function>");
                    outputBuffer.Add("\t\t\t</parser>");
                }
                outputBuffer.Add(line);
            }
            IBMiUtilities.DebugLog("InstallLocalDefinitions parsing functionList comeplted!");
            File.WriteAllLines(functionList, outputBuffer);
            IBMi.SetConfig("localDefintionsInstalled", "true");
            IBMiUtilities.DebugLog("InstallLocalDefinitions completed!");
        }

        /// <summary>
        /// Generates source that provides extra functionality to plugin
        /// </summary>
        /// <returns></returns>
        private static List<string> GenerateRemoteSource()
        {
            List<string> tmpFiles = new List<string>();
            string tmp = "";

            tmp = Path.GetTempFileName();
            File.Delete(tmp); 
            tmpFiles.Add(GenerateNPPDspFfdPgm(tmp));

            tmp = Path.GetTempFileName();
            File.Delete(tmp);
            tmpFiles.Add(GenerateNPPDspFfdCmd(tmp));

            return tmpFiles;
        }

        private static string GenerateNPPDspFfdPgm(string path)
        {
            List<string> src = new List<string>();
            src.Add("PGM          PARM(&FILE) ");
            src.Add("DCL          VAR(&FILE) TYPE(*CHAR) LEN(10)");
            src.Add("DCL          VAR(&USER) TYPE(*CHAR) LEN(10)");
            src.Add("             RTVJOBA    CURUSER(&USER)");
            src.Add("             DLTF       FILE(QTEMP/&FILE)");
            src.Add("             MONMSG     MSGID(CPF2105)");
            src.Add("             CRTPF      FILE(QTEMP/&FILE) RCDLEN(1730) +");
            src.Add("                          FILETYPE(*SRC) CCSID(*JOB) ");
            src.Add("             DSPFFD     FILE(*LIBL/&FILE) OUTPUT(*OUTFILE) +");
            src.Add("                          OUTFILE(QTEMP/TMP)");
            src.Add("             CPYF       FROMFILE(QTEMP/TMP) TOFILE(QTEMP/&FILE) +");
            src.Add("                          MBROPT(*REPLACE) FMTOPT(*CVTSRC)");
            src.Add("             CPYTOSTMF    FROMMBR('/QSYS.LIB/QTEMP.LIB/' *CAT &FILE +");
            src.Add("                         *TCAT '.FILE/' *CAT &FILE *TCAT '.MBR') +");
            src.Add("                         TOSTMF('/HOME/' +");
            src.Add("                         *CAT &USER *TCAT '/' *CAT &FILE *TCAT '.TMP') +");
            src.Add("                         STMFOPT(*REPLACE) ");
            src.Add("             DLTF       FILE(QTEMP/&FILE)");
            src.Add("             DLTF       FILE(QTEMP/TMP)");
            src.Add("ENDPGM");
            
            File.WriteAllLines(path + "-NPPDSPFFD.clp", src.ToArray());

            return path + "-NPPDSPFFD.clp";
        }

        private static string GenerateNPPDspFfdCmd(string path)
        {
            List<string> src = new List<string>();
            src.Add("            CMD        ALLOW(*ALL)");
            src.Add("FILE:       PARM       KWD(FILE) TYPE(*CHAR) LEN(10)");
            File.WriteAllLines(path + "-NPPDSPFFD.cmd", src.ToArray());
            return path + "-NPPDSPFFD.cmd";
        }


        private static string GenerateNPPRTVCMDPgm(string path)
        {
            List<string> src = new List<string>();
            
            File.WriteAllLines(path + "-NPPRTVCMD.cmd", src.ToArray());
            return path + "-NPPRTVCMD.rpgle";
        }

        private static string GenerateNPPRTVCMDCmd(string path)
        {
            List<string> src = new List<string>();
            src.Add("            CMD        ALLOW(*ALL)");
            src.Add("CMD:        PARM       KWD(CMD) TYPE(*CHAR) LEN(20)");
            File.WriteAllLines(path + "-NPPRTVCMD.cmd", src.ToArray());
            return path + "-NPPRTVCMD.cmd";
        }
    }
}
