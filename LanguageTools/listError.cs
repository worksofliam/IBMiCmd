namespace IBMiCmd.LanguageTools
{
    class lineError
    {
        private int _sev;
        private int _line;
        private int _col;
        private string _data = "";
        private string _errcode;

        public lineError(int sev, int line, int col, string data, string errcode)
        {
            _sev = sev;
            _line = line;
            _col = col;
            _data = data;
            _errcode = errcode;
        }

        public int getSev()
        {
            return _sev;
        }

        public int getLine()
        {
            return _line;
        }

        public int getColumn()
        {
            return _col;
        }

        public string getData()
        {
            return _data;
        }

        public string getCode()
        {
            return _errcode;
        }
    }
}