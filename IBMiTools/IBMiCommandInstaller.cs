using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NppPluginNET;
using IBMiCmd.Properties;

namespace IBMiCmd.IBMiTools
{
    public class IBMiCommandInstaller
    {

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

                if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
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
        public static List<string> GenerateRemoteSource()
        {
            List<string> tmpFiles = new List<string>
            {
                GenerateCtlOptCpy(Path.GetTempFileName()),
                GenerateDspFfdPgm(Path.GetTempFileName()),
                GenerateDspFfdCmd(Path.GetTempFileName()),
                GenerateRtvCmdPgm(Path.GetTempFileName()),
                GenerateRtvCmdCmd(Path.GetTempFileName())
            };
            return tmpFiles;
        }

        private static string GenerateCtlOptCpy(string path)
        {
            File.Delete(path);
            path = $"{path}.CPY";
            File.WriteAllText(path, Resources.CTLOPTCPY);
            return path;
        }

        private static string GenerateDspFfdPgm(string path)
        {
            File.Delete(path);
            path = $"{path}.CLP";
            File.WriteAllText(path, Resources.DSPFFDCLP);
            return path;
        }

        private static string GenerateDspFfdCmd(string path)
        {
            File.Delete(path);
            path = $"{path}.CMD";
            File.WriteAllText(path, Resources.DSPFFDCMD);
            return path;
        }

        private static string GenerateRtvCmdPgm(string path)
        {
            File.Delete(path);
            path = $"{path}.RPG";
            File.WriteAllText(path, Resources.RTVCMDRPG);
            return path;
        }

        private static string GenerateRtvCmdCmd(string path)
        {
            File.Delete(path);
            path = $"{path}.CMD";
            File.WriteAllText(path, Resources.RTVCMDCMD);
            return path;
        }
    }
}
