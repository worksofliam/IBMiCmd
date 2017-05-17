using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;
using IBMiCmd.Forms;
using System.Threading;
using System.Collections.Generic;

namespace IBMiCmd
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "IBMiCmd";
        internal const string PluginDescription = "IBMiCmd v1.3.1.0 github.com/WorksOfBarry/IBMiCmd";

        public static commandEntry commandWindow { get; set; }
        public static errorListing errorWindow { get; set; }
        public static libraryList liblWindow { get; set; }
        public static cmdBindings bindsWindow { get; set; }

        private static string iniFilePath = null;
        private static int idMyDlg = -1;
        private static Bitmap tbBmp = Properties.Resources.star;
        private static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        private static Icon tbIcon = null;  
        #endregion

        #region " StartUp/CleanUp "
        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = $"{ sbIniFilePath.ToString() }/{ PluginName }/";
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);

            IBMi.loadConfig(iniFilePath + PluginName);

            IBMiUtilities.CreateLog(iniFilePath + PluginName);

            PluginBase.SetCommand(0, "About IBMiCmd", About, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "IBM i Remote System Setup", RemoteSetup);
            PluginBase.SetCommand(2, "IBM i Command Entry", CommandDialog);
            PluginBase.SetCommand(3, "IBM i Error Listing", ErrorDialog);
            PluginBase.SetCommand(4, "IBM i Command Bindings", BindsDialog);

            PluginBase.SetCommand(5, "IBM i RPG Conversion", LaunchConversion, new ShortcutKey(true, false, false, Keys.F4));
            PluginBase.SetCommand(6, "IBM i Relic Build", LaunchRBLD, new ShortcutKey(true, false, false, Keys.F5));

            // Parse RPG Source and retrieve contect information from remote system
            PluginBase.SetCommand(7, "IBM i Refresh Definitions", BuildSourceContext, new ShortcutKey(true, false, false, Keys.F6));

            // TODO: Implement SCI API calls to provide suggestions for current line + cursor position based on source context
            PluginBase.SetCommand(8, "IBM i Auto Complete", AutoComplete, new ShortcutKey(true, false, false, Keys.Space));

            // Set Library list config
            PluginBase.SetCommand(9, "IBM i Library List", LiblDialog, new ShortcutKey(true, false, false, Keys.F7));

            // Get Record format info for all EXTNAME data strctures in current source
            PluginBase.SetCommand(10, "IBM i Remote Install Plugin Server", RemoteInstall);
        }
        
        internal static void SetToolBarIcon()
        {
            
        }
        internal static void PluginCleanUp()
        {
            
        }
        #endregion

        #region " Menu functions "
        internal static void About()
        {
            MessageBox.Show($"IBMiCmd, created by WorksOfBarry. { Environment.NewLine} github.com/WorksOfBarry/IBMiCmd");
        }

        internal static void LaunchRBLD()
        {
            DialogResult outp = MessageBox.Show("Confirm build of '" + IBMi.getConfig("relicdir") + "' into " + IBMi.getConfig("reliclib") + "?", "Relic Build", MessageBoxButtons.YesNo);
            if (outp == DialogResult.Yes)
            {
                IBMiNPPInstaller.RebuildRelic();
            }
        }

        internal static void LaunchConversion()
        {
            rpgForm.curFileLine = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETCURRENTLINE, 0, 0);
            new rpgForm().ShowDialog();
        }
        
        internal static void RemoteSetup()
        {
            new userSettings().ShowDialog();
        }

        internal static void RemoteInstall()
        {
            new installRemote().ShowDialog();
            Main.commandWindow.loadNewCommands();
        }

        internal static void LiblDialog()
        {
            new libraryList().ShowDialog();
        }

        internal static void BuildSourceContext()
        {
            RPGParser.LaunchFFDCollection();
        }

        internal static void AutoComplete() {
            RPGAutoCompleter.ProvideSuggestions(RPGParser.dataStructures);
        }

        internal static void CommandDialog()
        {
            if (commandWindow == null)
            {
                commandWindow = new commandEntry();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.Blue);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = commandWindow.Handle;
                _nppTbData.pszName = "Command Entry";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, commandWindow.Handle);
            }
        }

        internal static void ErrorDialog()
        {
            if (errorWindow == null)
            {
                errorWindow = new errorListing();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.Orange);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = errorWindow.Handle;
                _nppTbData.pszName = "Error Listing";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, errorWindow.Handle);
            }
        }

        internal static void BindsDialog()
        {
            if (bindsWindow == null)
            {
                bindsWindow = new cmdBindings();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.Green);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = bindsWindow.Handle;
                _nppTbData.pszName = "Command Bindings";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);

                bindsWindow.cmdBindings_Load();
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, bindsWindow.Handle);
            }
        }
        #endregion
    }
}