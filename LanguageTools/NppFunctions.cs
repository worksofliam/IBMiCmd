using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NppPluginNET;

namespace IBMiCmd.LanguageTools
{
    class NppFunctions
    {
        public static void OpenFile(string Path, Boolean ReadOnly)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DOOPEN, 0, Path);
            if (ReadOnly) Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETREADONLY, 1, 0);
        }

        public static int GetLineNumber()
        {
            return (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETCURRENTLINE, 0, 0);
        }

        public static string getLine(int line)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, line, 0);
            StringBuilder sb = new StringBuilder(lineLength);
            Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, line, sb);

            line = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, line, 0);
            lineLength--;
            Win32.SendMessage(curScintilla, SciMsg.SCI_SETSELECTION, line, line + lineLength);

            return sb.ToString().Substring(0, lineLength);
        }


        public static void setLine(string value)
        {
            //Hopefully is still selected?
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_REPLACESEL, 0, value);
        }
    }
}
