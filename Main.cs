using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using IBMiCmd.Forms;
using IBMiCmd.IBMiTools;
using IBMiCmd.LanguageTools;
using NppPluginNET;
using System.Threading;

namespace IBMiCmd
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "IBMiCmd";
        internal const string PluginDescription = "IBMiCmd v1.3.2.0 github.com/WorksOfBarry/IBMiCmd";

        public static commandEntry CommandWindow { get; set; }
        public static errorListing ErrorWindow { get; set; }
        public static libraryList LiblWindow { get; set; }
        public static cmdBindings BindsWindow { get; set; }
        public static selectMember MemberListWindow { get; set; }

        public static string ConfigDirectory { get; set; }
        public static string SystemsDirectory { get; set; }
        public static string FileCacheDirectory { get; set; }

        private static int idMyDlg = -1;
        private static Bitmap tbBmp = Properties.Resources.star;
        private static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        private static Icon tbIcon = null;
        #endregion

        #region " StartUp/CleanUp "
        internal static void CommandMenuInit()
        {
            StringBuilder pluginsConfigDir = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, pluginsConfigDir);
            ConfigDirectory = $"{ pluginsConfigDir.ToString() }\\{ PluginName }\\";
            SystemsDirectory = $"{ConfigDirectory}systems\\";
            FileCacheDirectory = $"{ConfigDirectory}cache\\";

            if (!Directory.Exists(ConfigDirectory)) Directory.CreateDirectory(ConfigDirectory);
            if (!Directory.Exists(SystemsDirectory)) Directory.CreateDirectory(SystemsDirectory);
            if (!Directory.Exists(FileCacheDirectory)) Directory.CreateDirectory(FileCacheDirectory);

            IBMiUtilities.CreateLog(ConfigDirectory + PluginName);
            RPGParser.LoadFileCache();
            CLParser.LoadFileCache();

            if (File.Exists(ConfigDirectory + "dftcfg"))
            {
                Config.SwitchToConfig(File.ReadAllText(ConfigDirectory + "dftcfg"));
            }
            else
            {
                LoadConfigSelect();
                if (Config.GetConfigs().Length == 0) return;
                if (IBMi._ConfigFile == "" || IBMi._ConfigFile == null)
                {
                    string UseConfig = Config.GetConfigs()[0];
                    MessageBox.Show("No config selected. Defaulted to " + UseConfig + ".");
                    Config.SwitchToConfig(UseConfig);
                }
            }
            //if (IBMi.GetConfig("localDefintionsInstalled") == "false")
            //{
            //    IBMiNPPInstaller.InstallLocalDefinitions();
            //}

            bool ExperimentalFeatures = (IBMi.GetConfig("experimental") == "true");
            int ItemOrder = 0;

            PluginBase.SetCommand(ItemOrder++, "About IBMiCmd", About, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(ItemOrder++, "-SEPARATOR-", null);
            PluginBase.SetCommand(ItemOrder++, "Select Remote System", LoadConfigSelect);
            PluginBase.SetCommand(ItemOrder++, "Remote System Setup", RemoteSetup);
            PluginBase.SetCommand(ItemOrder++, "Library List", LiblDialog, new ShortcutKey(true, false, false, Keys.F7));
            if (ExperimentalFeatures) PluginBase.SetCommand(ItemOrder++, "Remote Install Plugin Server", RemoteInstall);
            PluginBase.SetCommand(ItemOrder++, "-SEPARATOR-", null);
            PluginBase.SetCommand(ItemOrder++, "Command Entry", CommandDialog);
            PluginBase.SetCommand(ItemOrder++, "Error Listing", ErrorDialog);
            PluginBase.SetCommand(ItemOrder++, "Command Bindings", BindsDialog);
            PluginBase.SetCommand(ItemOrder++, "-SEPARATOR-", null);
            PluginBase.SetCommand(ItemOrder++, "Member Listing", MemberListing);
            PluginBase.SetCommand(ItemOrder++, "Open Source Member", OpenMember, new ShortcutKey(true, false, true, Keys.A));
            //PluginBase.SetCommand(ItemOrder++, "Upload Source Member", UploadMember, new ShortcutKey(true, false, true, Keys.X));
            PluginBase.SetCommand(ItemOrder++, "Open Include/Copy", OpenInclude, new ShortcutKey(true, false, false, Keys.F12));
            PluginBase.SetCommand(ItemOrder++, "-SEPARATOR-", null);
            PluginBase.SetCommand(ItemOrder++, "Format CL file", ManageCL, new ShortcutKey(true, false, false, Keys.F4));
            PluginBase.SetCommand(ItemOrder++, "RPG Line Conversion", LaunchConversion, new ShortcutKey(true, false, false, Keys.F5));
            PluginBase.SetCommand(ItemOrder++, "RPG File Conversion", LaunchFileConversion, new ShortcutKey(true, false, false, Keys.F6));
            PluginBase.SetCommand(ItemOrder++, "-SEPARATOR-", null);
            PluginBase.SetCommand(ItemOrder++, "Display File Editor", DisplayEdit);
            if (ExperimentalFeatures) PluginBase.SetCommand(ItemOrder++, "Refresh Extname Definitions", BuildSourceContext);
            if (ExperimentalFeatures) PluginBase.SetCommand(ItemOrder++, "Extname Content Assist", AutoComplete, new ShortcutKey(false, true, false, Keys.Space));
            if (ExperimentalFeatures) PluginBase.SetCommand(ItemOrder++, "Prompt CL Command", PromptCommand, new ShortcutKey(true, false, false, Keys.F4));
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
            MessageBox.Show($"IBMiCmd, created by Works Of Barry. { Environment.NewLine} github.com/WorksOfBarry/IBMiCmd");
        }

        internal static void DisplayEdit()
        {
            dspfEdit Editor = new dspfEdit();
            string path = NppFunctions.GetCurrentFileName();
            if (path.Trim() != "")
            {
                DisplayParse parser = new LanguageTools.DisplayParse();
                parser.ParseFile(path);
                Editor = new dspfEdit(parser.GetRecordFormats(), path);
            }

            Editor.ShowDialog();
        }

        internal static void ManageCL()
        {
            CLFile.CorrectLines(NppFunctions.GetCurrentFileName(), 80);
            NppFunctions.RefreshWindow(NppFunctions.GetCurrentFileName());
        }

        internal static void LoadConfigSelect()
        {
            new selectConfig().ShowDialog();
        }

        internal static void OpenInclude()
        {
            Thread gothread = new Thread((ThreadStart)delegate { 
                string LineNum = NppFunctions.GetLine(NppFunctions.GetLineNumber());
                OpenMember Member = Include.HandleInclude(LineNum);

                if (Member != null)
                {
                    string FileLoc = "";
                
                    FileLoc = IBMiUtilities.DownloadMember(Member.GetLibrary(), Member.GetObject(), Member.GetMember());

                    if (FileLoc != "")
                    {
                        NppFunctions.OpenFile(FileLoc, true);
                    }
                    else
                    {
                        MessageBox.Show("Unable to download member " + Member.GetLibrary() + "/" + Member.GetObject() + "." + Member.GetMember() + ". Please check it exists and that you have access to the remote system.");
                    }
                }
                else
                {
                    MessageBox.Show("Unable to parse out member.");
                }
            });
            gothread.Start();
        }

        internal static void MemberListing()
        {
            if (MemberListWindow == null)
            {
                MemberListWindow = new selectMember();

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
                _nppTbData.hClient = MemberListWindow.Handle;
                _nppTbData.pszName = "Member Listing";
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
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, MemberListWindow.Handle);
            }
        }

        internal static void OpenMember()
        {
            new openMember().ShowDialog();
        }

        internal static void UploadMember()
        {
            new uploadMember().ShowDialog();
        }

        internal static void LaunchConversion()
        {
            new rpgForm().ShowDialog();
        }

        internal static void LaunchFileConversion()
        {
            new rpgFileConvert().ShowDialog();

        }

        internal static void RemoteSetup()
        {
            new userSettings().ShowDialog();
        }

        internal static void RemoteInstall()
        {
            new installRemote().ShowDialog();
        }

        internal static void LiblDialog()
        {
            new libraryList().ShowDialog();
        }

        internal static void BuildSourceContext()
        {
            RPGParser.LaunchFFDCollection();
        }

        internal static void AutoComplete()
        {
            RPGAutoCompleter.ProvideSuggestions(RPGParser.dataStructures);
        }

        internal static void PromptCommand()
        {
            CLCommandPrompter.PromptCommand();
        }

        internal static void CommandDialog()
        {
            if (CommandWindow == null)
            {
                CommandWindow = new commandEntry();

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
                _nppTbData.hClient = CommandWindow.Handle;
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
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, CommandWindow.Handle);
            }
        }

        internal static void ErrorDialog()
        {
            if (ErrorWindow == null)
            {
                ErrorWindow = new errorListing();

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
                _nppTbData.hClient = ErrorWindow.Handle;
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
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, ErrorWindow.Handle);
            }
        }

        internal static void BindsDialog()
        {
            if (BindsWindow == null)
            {
                BindsWindow = new cmdBindings();

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
                _nppTbData.hClient = BindsWindow.Handle;
                _nppTbData.pszName = "Command Bindings";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);

                BindsWindow.cmdBindings_Load();
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, BindsWindow.Handle);
            }
        }
        #endregion
    }
}