using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IBMiCmd
{
    class IBMiUtilities
    {
        /// <summary>
        /// Returns true if input string is a valid IBM i QSYS Object Name
        /// </summary>
        /// <param name="s">Object name to be verified</param>
        /// <returns>True if valid s is a valid object name</returns>
        public static bool isValidQSYSObjectName(string s)
        {
            return Regex.Match(s, "^[a-zA-Z]\\w{0,9}$").Success;
        }
    }
}
