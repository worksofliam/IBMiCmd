namespace IBMiCmd
{
    class lineError
    {
        private int _sev;
        private int _line;
        private string _data = "";
        private string _errcode;

        public lineError(int sev, int line, string data, string errcode)
        {
            _sev = sev;
            _line = line;
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