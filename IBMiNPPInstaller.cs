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
            StringBuilder notepadInstallationDir = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETNPPDIRECTORY, Win32.MAX_PATH, notepadInstallationDir);
            string installationDirectory = $"{notepadInstallationDir.ToString()}/";
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
    }
}
