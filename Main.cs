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

namespace IBMiCmd
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "IBMiCmd";
        static string iniFilePath = null;
        public static commandEntry commandWindow = null;
        public static errorListing errorWindow = null;
		public static libraryList liblWindow = null;
        public static cmdBindings bindsWindow = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIcon = null;
        #endregion

        #region " StartUp/CleanUp "
        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);

            IBMi.loadConfig(iniFilePath + PluginName + ".cfg");
            
            PluginBase.SetCommand(0, "About IBMiCmd", myMenuFunction, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "IBM i Remote System Setup", remoteSetup);
            PluginBase.SetCommand(2, "IBM i Command Entry", commandDialog);
            PluginBase.SetCommand(3, "IBM i Error Listing", errorDialog);
            PluginBase.SetCommand(4, "IBM i Command Bindings", bindsDialog);

            PluginBase.SetCommand(6, "IBM i RPG Conversion", launchConversion, new ShortcutKey(true, false, false, Keys.F4));
            PluginBase.SetCommand(7, "IBM i Relic Build", launchRBLD, new ShortcutKey(true, false, false, Keys.F5));

			// Set Library list config
			PluginBase.SetCommand(8, "IBM i LIBL", liblDialog, new ShortcutKey(true, false, false, Keys.F6));
		}
        
        internal static void SetToolBarIcon()
        {
            
        }
        internal static void PluginCleanUp()
        {
            
        }
        #endregion

        #region " Menu functions "
        internal static void myMenuFunction()
        {
            MessageBox.Show("IBMiCmds, created by WorksOfBarry.");
        }

        internal static void launchRBLD()
        {
            DialogResult outp = MessageBox.Show("Confirm build of '" + IBMi.getConfig("relicdir") + "' into " + IBMi.getConfig("reliclib") + "?", "Relic Build", MessageBoxButtons.YesNo);

            if (outp == DialogResult.Yes)
            {
                Thread gothread = new Thread((ThreadStart)delegate {
                    string filetemp = Path.GetTempFileName();
                    string buildDir = IBMi.getConfig("relicdir");
                    if (!buildDir.EndsWith("/"))
                    {
                        buildDir += '/';
                    }

                    IBMi.addOutput("Starting build of '" + IBMi.getConfig("relicdir") + "' into " + IBMi.getConfig("reliclib"));
                    if (Main.commandWindow != null) Main.commandWindow.loadNewCommands();
                    IBMi.runCommands(new string[] {
                        "QUOTE RCMD CD '" + IBMi.getConfig("relicdir") + "'",
                        "QUOTE RCMD RBLD " + IBMi.getConfig("reliclib"),
                        "ASCII",
                        "RECV " + buildDir + "RELICBLD.log \"" + filetemp + "\""
                    });
                    IBMi.addOutput("");
                    foreach(string line in File.ReadAllLines(filetemp))
                    {
                        IBMi.addOutput("> " + line);
                    }
                    IBMi.addOutput("");
                    IBMi.addOutput("End of build.");
                    if (Main.commandWindow != null) Main.commandWindow.loadNewCommands();
                });
                gothread.Start();
            }
        }

        internal static void launchConversion()
        {
            rpgForm.curFileLine = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETCURRENTLINE, 0, 0);
            new rpgForm().ShowDialog();
        }
        
        internal static void remoteSetup()
        {
            new userSettings().ShowDialog();
        }

        internal static void liblDialog()
        {
            new libraryList().ShowDialog();
        }

        internal static void commandDialog()
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

        internal static void errorDialog()
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

        internal static void bindsDialog()
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