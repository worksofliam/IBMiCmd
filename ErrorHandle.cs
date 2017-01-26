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
            commands.Add("recv " + lib + ".lib/EVFEVENT.file/" + obj + ".mbr \"" + filetemp + "\"");

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
            int linenum, sqldiff;

            List<expRange> exps = new List<expRange>();
            string[] pieces;
            string curtype;

            foreach (string line in _Lines)
            {
                if (line == null) continue;
                err = line.PadRight(150);
                curtype = err.Substring(0, 10).TrimEnd();
                switch(curtype)
                {
                    case "EXPANSION":
                        pieces = err.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        exps.Add(new expRange(int.Parse(pieces[6]), int.Parse(pieces[7])));

                        break;
                    case "ERROR":
                        sev = int.Parse(err.Substring(58, 2));
                        linenum = int.Parse(err.Substring(37, 6));
                        sqldiff = 0;

                        if (sev >= 10)
                        {
                            foreach (expRange range in exps)
                            {
                                if (range.inRange(linenum))
                                {
                                    sqldiff += range.getVal();
                                }
                            }
                        }

                        if (sqldiff > 0)
                        {
                            linenum -= sqldiff;
                        }

                        _Errors.Add(new lineError(sev, linenum, err.Substring(65), err.Substring(48, 7)));
                        break;
                }
            }
        }

        public static lineError[] getErrors()
        {
            return _Errors.ToArray();
        }
    }

    class expRange
    {
        public int _low;
        public int _high;

        public expRange(int low, int high)
        {
            _low = low;
            _high = high;
        }

        public bool inRange(int num)
        {
            return (num >= _high);
        }

        public int getVal()
        {
            return (_high - _low) + 1;
        }
    }
}
