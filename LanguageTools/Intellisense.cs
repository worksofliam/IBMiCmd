using IBMiCmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IBMiCmd.Properties;
using System.Text;
using System.Threading;
using IBMiCmd.Forms;
using System.Windows.Forms;

namespace IBMiCmd.LanguageTools
{
    class Intellisense
    {
        public static ListViewItem[] FileItems = null;
        public static void ScanFile()
        {
            string currentFile = NppFunctions.GetCurrentFileName().ToUpper();

            if (currentFile.Contains("RPG"))
                FileItems = RPGParser.ScanFile(currentFile).ToArray();
            else
                FileItems = null;
            
            if (Main.CommandWindow != null) Main.CommandWindow.loadNewOutput();
        }

        private readonly static Dictionary<string, ListViewItem[]> Values = new Dictionary<string, ListViewItem[]>()
        {
            { "RPG", GetPieces(Resources.RPG) },
            { "CL", GetPieces(Resources.CL) }
        };

        private static ListViewItem[] GetPieces(string Resource)
        {
            List<ListViewItem> Items = new List<ListViewItem>();
            string[] Pieces;
            
            foreach (String Item in Resource.Split('\n'))
            {
                Pieces = Item.Split('|');
                Items.Add(new ListViewItem(Pieces[0], Convert.ToInt32(Pieces[1])));
            }

            return Items.ToArray();
        }
        
        public static ListViewItem[] ParseLine()
        {
            List<ListViewItem> Keysout = new List<ListViewItem>(); ;
            string currentFile = NppFunctions.GetCurrentFileName().ToUpper();
            string currentPiece = NppFunctions.GetLine(NppFunctions.GetLineNumber()).Trim();
            int currentLine = NppFunctions.GetLineNumber();
            string DataStructureName = "";
            int number;
            string[] pieces;

            pieces = currentPiece.Split(new char[] { ' ', ':', '(', ')', '.', '=' });
            if (pieces.Length == 0) return null;

            currentPiece = pieces[pieces.Length - 1];
            currentPiece = currentPiece.ToUpper();

            if (currentFile.Contains("RPG"))
            {
                if (currentPiece.EndsWith(";"))
                {
                    return null;
                }
                else if (currentPiece.Length >= 2)
                {
                    Keysout.AddRange(Array.FindAll(Values["RPG"], c => c.Text.StartsWith(currentPiece)));

                    if (FileItems != null)
                        Keysout.AddRange(Array.FindAll(FileItems, c => c.Text.ToUpper().StartsWith(currentPiece)));
                }
                else if (pieces.Length >= 2)
                {
                    if (FileItems != null)
                    {
                        if (pieces.Length >= 3)
                        {
                            if (int.TryParse(pieces[pieces.Length - 3], out number))
                                DataStructureName = pieces[pieces.Length - 4];
                            else
                                DataStructureName = pieces[pieces.Length - 2];
                        }
                        else
                            DataStructureName = pieces[pieces.Length - 2];

                        DataStructureName = DataStructureName.ToUpper();
                        if (DataStructureName != "")
                            Keysout.AddRange(Array.FindAll(FileItems, c => c.Tag.ToString() == DataStructureName));
                    }
                }
            }
            else if (currentFile.Contains("CL"))
            {
                if (currentPiece.Length >= 4)
                {
                    if (pieces.Length == 1)
                    {
                        Keysout.AddRange(Array.FindAll(Values["CL"], c => c.Text.StartsWith(currentPiece)));
                    }
                }
            }

            if (Keysout.Count >= 1)
                Main.IntelliSenseWindow.SetKey(currentPiece);

            if (Keysout == null)
                return null;
            else
                return Keysout.ToArray();
        }
    }
}
