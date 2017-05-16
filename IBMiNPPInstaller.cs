using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace IBMiCmd
{
    class IBMiNPPInstaller
    {
        /// <summary>
        /// Installs the remote objects that the plugin requires on the server
        /// </summary>
        internal static void installRemoteLib(string library = "QGPL")
        {
            //IBMiUtilities.Log("launchFFDCollection...");
            Thread thread = new Thread((ThreadStart) delegate {
#if DEBUG
                IBMiUtilities.Log("Thread installRemoteLib Starting...");
#endif
                try
                {
                    List<string> sourceFiles = generateRemoteSource();
                    // Make room for <upload, copy, delete, compile> for each file
                    string[] cmd = new string[sourceFiles.Count * 4 + 4];

                    int i = 0;
                    cmd[i++] = "ASCII";
                    cmd[i++] = "QUOTE RCMD CRTPF FILE(QTEMP/NPPCLSRC)  RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy NPP plugin commands')";
                    cmd[i++] = "QUOTE RCMD CRTPF FILE(QTEMP/NPPCMDSRC) RCDLEN(112) FILETYPE(*SRC) MAXMBRS(*NOMAX) TEXT('Deploy NPP plugin commands')";
                    foreach (string f in sourceFiles)
                    {
                        string file = f.Substring(f.LastIndexOf("\\") + 1);
                        string member = file.Substring(file.LastIndexOf("-") + 1, file.LastIndexOf(".") - (file.LastIndexOf("-") + 1));
                        string sourceFile = null, crtCmd = null;

                        switch (file.Substring(file.Length - 4))
                        {
                            case ".clp":
                                sourceFile = "NPPCLSRC";
                                crtCmd = "CRTCLPGM PGM(QGPL/" + member + ") SRCFILE(QTEMP/NPPCLSRC) SRCMBR(" + member + ") REPLACE(*YES)";
                                break;
                            case ".cmd":
                                sourceFile = "NPPCMDSRC";
                                crtCmd = "CRTCMD CMD(QGPL/" + member + ") PGM(QGPL/" + member + ") SRCFILE(QTEMP/NPPCMDSRC) SRCMBR(" + member + ") REPLACE(*YES)";
                                break;
                            default:
                                continue;
                        }

                        cmd[i++] = "SEND " + f + " /home/" + IBMi.getConfig("username") + "/" + file;
                        cmd[i++] = "QUOTE RCMD CPYFRMSTMF FROMSTMF('/home/" + IBMi.getConfig("username") + "/" + file + "') "
                                 +                       "TOMBR('/QSYS.LIB/QTEMP.LIB/" + sourceFile + ".FILE/" + member + ".MBR')";
                        cmd[i++] = "QUOTE RCMD RMVLNK OBJLNK('/home/" + IBMi.getConfig("username") + '/' + file + "')";
                        cmd[i++] = "QUOTE RCMD " + crtCmd;
                    }
                    // cmd[i++] = "QUOTE RCMD DSPJOBLOG"; // For Debug on remote 

                    IBMi.runCommands(cmd); // Send -> Copy -> Cleanup -> Compile

                    // Cleanup local
                    foreach (string f in sourceFiles)
                    {
                        File.Delete(f);
                    }
                } catch (Exception e) {
                    IBMiUtilities.Log(e.ToString());
                }
#if DEBUG
                IBMiUtilities.Log("Thread installRemoteLib completed.");
#endif
            });
            thread.Start();
        }

        private static List<string> generateRemoteSource()
        {
            List<string> tmpFiles = new List<string>();
            string tmp = "";

            tmp = Path.GetTempFileName();
            File.Delete(tmp); 
            tmpFiles.Add(generateNPPDspFfdPgm(tmp));

            tmp = Path.GetTempFileName();
            File.Delete(tmp);
            tmpFiles.Add(generateNPPDspFfdCmd(tmp));

            return tmpFiles;
        }

        private static string generateNPPDspFfdPgm(string path)
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

        private static string generateNPPDspFfdCmd(string path)
        {
            List<string> src = new List<string>();
            src.Add("            CMD        ALLOW(*ALL)");
            src.Add("FILE:       PARM       KWD(FILE) TYPE(*CHAR) LEN(10)");
            File.WriteAllLines(path + "-NPPDSPFFD.cmd", src.ToArray());
            return path + "-NPPDSPFFD.cmd";
        }
    }
}
