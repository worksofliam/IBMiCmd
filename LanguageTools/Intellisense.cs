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
        private readonly static Dictionary<string, string[]> Values = new Dictionary<string, string[]>()
        {
            { "RPG", Resources.RPG.Split('\n') },
            { "CL", Resources.CL.Split('\n') }
        };
        
        public static string[] ParseLine()
        {
            string[] Keysout = null;
            string currentFile = NppFunctions.GetCurrentFileName().ToLower();
            string currentPiece = NppFunctions.GetLine(NppFunctions.GetLineNumber()).Trim();

            if (currentFile.Contains("RPG"))
            {
                if (currentPiece.EndsWith(";"))
                {
                    return null;
                }
                else if (currentPiece.Length >= 1)
                {
                    string[] pieces = currentPiece.Split(' ');
                    currentPiece = pieces[pieces.Length - 1];
                    currentPiece = currentPiece.ToUpper();
                    Keysout = Array.FindAll(Values["RPG"], c => c.StartsWith(currentPiece));
                }
            }
            else if (currentFile.Contains("CL"))
            {
                if (currentPiece.Length >= 4)
                {
                    string[] pieces = currentPiece.Split(' ');
                    if (pieces.Length == 1)
                    {
                        currentPiece = pieces[pieces.Length - 1];
                        currentPiece = currentPiece.ToUpper();
                        Keysout = Array.FindAll(Values["CL"], c => c.StartsWith(currentPiece));
                    }
                }
            }

            return Keysout;
        }
    }
}
