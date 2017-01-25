using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace IBMiCmd
{
    class ErrorHandle
    {
        private static string _name = "";
        private static string[] _Lines;
        private static List<lineError> _Errors;

        public static void getErrors(string lib, string obj)
        {
            string filetemp = Path.GetTempFileName();

            List<string> commands = new List<string>();

            lib = lib.Trim().ToUpper();
            obj = obj.Trim().ToUpper();

            commands.Add("ASCII");
            commands.Add("cd /QSYS.lib");
            commands.Add("recv " + lib + ".lib/EVFEVENT.file/" + obj + ".mbr " + filetemp);
            IBMi.runCommands(commands.ToArray());

            ErrorHandle.doName(lib + '/' + obj);
            ErrorHandle.setLines(File.ReadAllLines(filetemp));
        }

        public static string doName(string newName = "")
        {
            if (newName != "") _name = newName;

            return _name;
        }

        public static void setLines(string[] data)
        {
            _Lines = data;
            wrkErrors();
        }

        public static void wrkErrors()
        {
            _Errors = new List<lineError>();
            string err;
            int sev;

            foreach (string line in _Lines)
            {
                if (line == null) continue;
                err = line.PadRight(150);
                if (err.Substring(0, 10).TrimEnd() == "ERROR")
                {
                    sev = int.Parse(err.Substring(58, 2));
                    _Errors.Add(new lineError(sev, err.Substring(37, 6), err.Substring(65)));
                }
            }
        }

        public static lineError[] getErrors()
        {
            return _Errors.ToArray();
        }
    }
}
