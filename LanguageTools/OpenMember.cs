using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBMiCmd.LanguageTools
{
    class OpenMembers
    {
        private static Dictionary<string, OpenMember> _Members = new Dictionary<string, OpenMember>();
        
        public static string[] GetLocals()
        {
            return _Members.Keys.ToArray();
        }

        public static void AddMember(string System, string Local, string Lib, string Obj, string Mbr, Boolean isEditable = true)
        {
            if (Contains(Local))
                RemoveMember(Local);

            _Members.Add(Local, new OpenMember(System, Local, Lib, Obj, Mbr, isEditable));
        }

        public static void RemoveMember(String Local)
        {
            if (Contains(Local))
            {
                _Members.Remove(Local);
            }
        }

        public static Boolean Contains(String Local)
        {
            return _Members.ContainsKey(Local);
        }

        public static OpenMember GetMember(String Local)
        {
            if (Contains(Local))
            {
                return _Members[Local];
            } 
            else
            {
                return null;
            }
        }
    }

    class OpenMember
    {
        private string _Sys;
        private string _Local;
        private string _Lib;
        private string _Obj;
        private string _Mbr;
        private Boolean _isEditable;

        public OpenMember(string System, string Local, string Lib, string Obj, string Mbr, Boolean isEditable)
        {
            this._Sys = System;
            this._Local = Local;
            this._Lib = Lib;
            this._Obj = Obj;
            this._Mbr = Mbr;
            this._isEditable = isEditable;
        }

        public string GetSystemName()
        {
            return this._Sys;
        }

        public string GetLocalFile()
        {
            return this._Local;
        }

        public string GetLibrary()
        {
            return this._Lib;
        }

        public string GetObject()
        {
            return this._Obj;
        }

        public string GetMember()
        {
            return this._Mbr;
        }

        public bool IsEditable()
        {
            return this._isEditable;
        }
    }
}
