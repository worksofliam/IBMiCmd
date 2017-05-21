using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBMiCmd
{
    public class PackedNumeric
    {
        private double value = 0;
       // string stringRepresentation = "";

        public PackedNumeric(int size) {

        }

        public string generateFromString() {
            byte[] buffer = new byte[new System.IO.FileInfo("C:\\Users\\Mathias\\AppData\\Local\\Temp\\tmp4DBA.tmp").Length];
            buffer = File.ReadAllBytes("C:\\Users\\Mathias\\AppData\\Local\\Temp\\tmp4DBA.tmp");            

            // 
            byte[] ccsidSequence = new byte[3];
            ccsidSequence[0] = buffer[492];
            ccsidSequence[1] = buffer[493];
            ccsidSequence[2] = buffer[494];

            char[] ccsidDigits = new char[6];
            int i = 0;
            foreach (byte b in ccsidSequence)
            {
                ccsidDigits[i++] = (char)(b & 0x0F);
                ccsidDigits[i++] = (char)(b >> 4);
            }

            return ccsidDigits.ToString();
        }

        public double getValue()
        {
            return value;
        }
    }
}
