using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (UpperInclude.StartsWith("/"))
                Member = HandleRPG(UpperInclude);

            //if (Include.StartsWith("#"))
            //Member = HandleC(Include);

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

                return new OpenMember("", "", lib, obj, mbr);
            }
            else
            {
                return null;
            }
        }
    }
}
