using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using NppPluginNET;
using IBMiCmd.IBMiTools;

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

        public static string GetLine(int line)
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
        
        public static void SetLine(string value)
        {
            //Hopefully is still selected?
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_REPLACESEL, 0, value);
        }
        
        public static string[] GetOpenFiles()
        {
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETNBOPENFILES, 0, 0);
            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETOPENFILENAMES, cStrArray.NativePointer, nbFile) != IntPtr.Zero)
                    return cStrArray.ManagedStringsUnicode.ToArray();
                else
                    return null;
            }
        }

        public static void DisplayErrors(string Lib, string Obj)
        {
            ErrorHandle.getErrors(Lib, Obj);

            if (Main.ErrorWindow != null)
            {
                Main.ErrorWindow.publishErrors();
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, Main.ErrorWindow.Handle);
            }
        }
        
        public static void SwitchToFile(string name, int line, int col)
        {
            int pos = 0;
            IntPtr curScintilla = PluginBase.nppData._nppHandle;
            Win32.SendMessage(curScintilla, NppMsg.NPPM_SWITCHTOFILE, 0, name);

            curScintilla = PluginBase.GetCurrentScintilla();
            Win32.SendMessage(curScintilla, SciMsg.SCI_ENSUREVISIBLE, line, 0);
            if (line >= 0)
            {
                pos = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, line, 0);
                pos += col;
                Win32.SendMessage(curScintilla, SciMsg.SCI_GOTOPOS, pos, 0);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GRABFOCUS, 0, 0);
            }
        }

        public static void HandleTrigger(SCNotification Notification)
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            OpenMember member;
            switch (Notification.nmhdr.code)
            {
                case (uint)NppMsg.NPPN_FILESAVED:
                    Win32.SendMessage(Notification.nmhdr.hwndFrom, NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
                    member = OpenMembers.GetMember(path.ToString());
                    if (member != null)
                    {
                        Thread gothread = new Thread((ThreadStart)delegate {  
                            if (member.GetSystemName() == IBMi.GetConfig("system"))
                            {
                                bool UploadResult = IBMiUtilities.UploadMember(member.GetLocalFile(), member.GetLibrary(), member.GetObject(), member.GetMember());
                                if (UploadResult == false)
                                {
                                    System.Windows.Forms.MessageBox.Show("Failed to upload to " + member.GetMember() + " on " + member.GetSystemName() + ".");
                                }
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Unable to upload to " + member.GetMember() + ". You must be connected to " + member.GetSystemName() + " in order to save this file.");
                            }
                            if (Main.CommandWindow != null) Main.CommandWindow.loadNewOutput();
                        });
                        gothread.Start();
                    }
                    break;

                case (uint)NppMsg.NPPN_FILEBEFORECLOSE:
                    Win32.SendMessage(Notification.nmhdr.hwndFrom, NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
                    if (OpenMembers.Contains(path.ToString()))
                    {
                        OpenMembers.RemoveMember(path.ToString());
                        File.Delete(path.ToString());
                    }
                    break;
            }
        }
    }
}
