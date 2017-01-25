namespace IBMiCmd
{
    class lineError
    {
        private int _sev;
        private string _line = "";
        private string _data = "";

        public lineError(int sev, string line, string data)
        {
            _sev = sev;
            _line = line;
            _data = data;
        }

        public int getSev()
        {
            return _sev;
        }

        public string getLine()
        {
            return _line;
        }

        public string getData()
        {
            return _data;
        }
    }
}