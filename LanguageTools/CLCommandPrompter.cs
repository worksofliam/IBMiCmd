using System;
using System.Text;
using IBMiCmd.IBMiTools;
using NppPluginNET;

namespace IBMiCmd.LanguageTools
{
    class CLCommandPrompter
    {
        /// <summary>
        /// Uses information about position of the cursor and cached CL commands
        /// Provides a window where the user can enter parameters for a CL command
        /// </summary>
        internal static void PromptCommand()
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();

            int curLine = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETCURRENTLINE, 0, 0);
            int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, curLine, 0);

            if (lineLength <= 0) return;

            StringBuilder sb = new StringBuilder(lineLength);
            Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, curLine, sb);

            int cursorPosition = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_GETCURRENTPOS, 0, 0);
            int lineOffsetPosition = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, curLine, 0);
            int linePosition = cursorPosition - lineOffsetPosition;
            if (linePosition == 0) return;

            StringBuilder fileExtension = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETEXTPART, Win32.MAX_PATH, fileExtension);

            string lookupString = CLParser.GetCommandAtColumn(sb.ToString(), linePosition);
            //switch (fileExtension.ToString())
            //{
            //    case "rpgle":
            //    case "sqlrpgle":
            //        lookupString = RPGParser.GetVariableAtColumn(sb.ToString(), linePosition, out MatchType typeOfMatch);
            //        break;
            //    case "clp":
            //    case "clle":
            //        lookupString = CLParser.GetCommandAtColumn(sb.ToString(), linePosition);
            //        break;
            //    default:
            //        // TODO: ?
            //        break;
            //}

            if (lookupString == "") return;

            GetCommandHelp(lookupString.ToUpper());
        }

        private static void GetCommandHelp(string c)
        {
            bool cacheHit = false;
            CLCommand command = new CLCommand("");
            foreach (CLCommand cmd in CLParser.CLCommands)
            {
                if (cmd.Equals(c))
                {
                    cacheHit = true;
                    command = cmd;
                    break;
                }
            }

            if (cacheHit)
            {
                //DisplayCommandHelp(command);
            }
            else
            {
                //DisplayLoadingScreen();
                FetchCommandDefinition(c);
                //DisplayCommandHelp(CLParser.CLCommands[CLParser.CLCommands.Count - 1]);
            }
        }

        private static void DisplayLoadingScreen()
        {
            throw new NotImplementedException();
        }

        private static void DisplayCommandHelp(CLCommand command)
        {
            throw new NotImplementedException();
        }

        internal static void FetchCommandDefinition(string command)
        {
            // Receive command description via IICRTVCMD
            IBMi.RunCommands(IBMiCommandRender.RenderCommandDescriptionCollection(command));

            try
            {
                CLParser.LoadCMD(command);
            }
            catch (Exception e)
            {
                IBMiUtilities.Log(e.ToString()); // TODO: Show error?
            }
        }
    }
}
