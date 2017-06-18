using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.LanguageTools
{
    class Include
    {
        public static OpenMember HandleInclude(string Include)
        {
            string UpperInclude = "";
            OpenMember Member = null;
            Include = Include.Trim();
            UpperInclude = Include.ToUpper();

            try
            {
                if (UpperInclude.StartsWith("/"))
                    Member = HandleRPG(UpperInclude);

                //if (Include.StartsWith("#"))
                //Member = HandleC(Include);
            }
            catch
            {
                Member = null;
            }

            return Member;
        }

        private static OpenMember HandleRPG(string Copy)
        {
            if (Copy.StartsWith("/COPY") || Copy.StartsWith("/INCLUDE"))
            {
                string lib, obj, mbr;
                string[] data = Copy.Split(' ');
                string[] pieces;

                if (data.Length != 2) return null;
                data[1] = data[1].Trim();

                pieces = data[1].Split(',');
                if (pieces.Length == 2)
                {
                    mbr = pieces[1];
                    obj = pieces[0];
                    pieces = pieces[0].Split('/');
                    if (pieces.Length == 2)
                    {
                        obj = pieces[1];
                        lib = pieces[0];
                    }
                    else
                    {
                        lib = "*CURLIB";
                    }
                }
                else
                {
                    mbr = data[1];
                    obj = "QRPGLESRC";
                    lib = "*CURLIB";
                }
                
                if (lib == "*CURLIB") lib = IBMi.GetConfig("curlib");

                if (IBMiUtilities.IsValidQSYSObjectName(lib) && IBMiUtilities.IsValidQSYSObjectName(obj) && IBMiUtilities.IsValidQSYSObjectName(mbr)) {
                    return new OpenMember("", "", lib, obj, mbr);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
