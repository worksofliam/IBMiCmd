using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IBMiCmd.LanguageTools
{
    public class DisplayParse
    {
        private Dictionary<string, RecordInfo> Formats;
        private RecordInfo CurrentRecord;

        private List<FieldInfo> CurrentFields;
        private FieldInfo CurrentField;

        public DisplayParse()
        {
            Formats = new Dictionary<string, RecordInfo>();
        }

        public void ParseFile(string Location)
        {
            ParseLines(File.ReadAllLines(Location));
        }

        public void ParseLines(string[] Lines)
        {
            int textcounter = 0;
            char[] chars;
            string name, len, type, dec, inout, x, y, keywords, Line = "";

            //https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_73/rzakc/rzakcmstpsnent.htm
            try
            {
                foreach (string TrueLine in Lines)
                {
                    Line = TrueLine.PadRight(80);
                    chars = Line.ToCharArray();
                    name = buildString(chars, 18, 10).Trim();
                    len = buildString(chars, 29, 5).Trim();
                    type = chars[34].ToString().ToUpper();
                    dec = buildString(chars, 35, 2).Trim();
                    inout = chars[37].ToString().ToUpper();
                    y = buildString(chars, 39, 2).Trim();
                    x = buildString(chars, 42, 2).Trim();
                    keywords = Line.Substring(44).Trim();

                    switch (chars[16])
                    {
                        case 'R':
                            if (CurrentField != null) CurrentFields.Add(CurrentField);
                            if (CurrentFields != null) CurrentRecord.Fields = CurrentFields.ToArray();
                            if (CurrentRecord != null) Formats.Add(CurrentRecord.Name, CurrentRecord);

                            CurrentRecord = new RecordInfo(name);
                            CurrentFields = new List<FieldInfo>();
                            CurrentField = null;
                            HandleKeywords(keywords);
                            break;
                        case ' ':
                            if ((x != "" && y != "") || inout == "H")
                            {
                                if (CurrentField != null)
                                    CurrentFields.Add(CurrentField);

                                if (inout == "H")
                                {
                                    x = "0"; y = "0";
                                }

                                CurrentField = new FieldInfo();
                                CurrentField.Position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                            }
                            if (name != "")
                            {
                                CurrentField.Name = name;
                                CurrentField.Value = "";
                                CurrentField.Length = Convert.ToInt32(len);
                                switch (inout)
                                {
                                    case "I":
                                        CurrentField.Type = FieldInfo.TextType.Input;
                                        break;
                                    case "B":
                                        CurrentField.Type = FieldInfo.TextType.Both;
                                        break;
                                    case "H":
                                        CurrentField.Type = FieldInfo.TextType.Hidden;
                                        break;
                                    case " ":
                                    case "O":
                                        CurrentField.Type = FieldInfo.TextType.Output;
                                        break;
                                }
                                HandleKeywords(keywords);
                            }
                            else
                            {
                                HandleKeywords(keywords);
                                if (CurrentField != null)
                                {
                                    textcounter++;
                                    CurrentField.Name = "TEXT" + textcounter.ToString();
                                    CurrentField.Length = CurrentField.Value.Length;
                                    CurrentField.Type = FieldInfo.TextType.Text;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Line error " + Line);
            }

            if (CurrentField != null) CurrentFields.Add(CurrentField);
            if (CurrentFields != null) CurrentRecord.Fields = CurrentFields.ToArray();
            if (CurrentRecord != null) Formats.Add(CurrentRecord.Name, CurrentRecord);

        }

        public Dictionary<string, RecordInfo> GetRecordFormats()
        {
            return Formats;
        }

        private string buildString(char[] array, int from, int len)
        {
            string outp = "";
            for (var i = from; i < from+len; i++)
            {
                outp += array[i];
            }
            return outp;
        }

        private void HandleKeywords(string Keywords)
        {
            if (Keywords.StartsWith("'") && Keywords.EndsWith("'"))
            {
                CurrentField.Value = Keywords.Trim('\'');
                return;
            }
            if (Keywords.Contains("(") && Keywords.EndsWith(")"))
            {
                int midIndex = Keywords.IndexOf('(');
                string option = Keywords.Substring(0, midIndex).ToUpper();
                string value = Keywords.Substring(midIndex + 1);
                value = value.Substring(0, value.Length - 1);

                switch (option.ToUpper())
                {
                    case "COLOR":
                        CurrentField.Colour = FieldInfo.DspfToColour(value);
                        break;
                    case "ROLLUP":
                        CurrentRecord.Pageup = true;
                        break;
                    case "ROLLDOWN":
                        CurrentRecord.Pageup = false;
                        break;
                }

                if (option.StartsWith("CA"))
                {
                    CurrentRecord.FunctionKeys[Convert.ToInt32(value)-1] = true;
                }
            }
        }
    }

    public class RecordInfo
    {
        public string Name;
        public FieldInfo[] Fields;
        public Boolean[] FunctionKeys;
        public Boolean Pageup;
        public Boolean Pagedown;

        public RecordInfo(String name)
        {
            Name = name;
            Fields = new FieldInfo[0];
            FunctionKeys = new Boolean[24];

            for (int i = 0; i < 24; i++)
            {
                FunctionKeys[i] = false;
            }

            Pageup = false;
            Pagedown = false;
        }
    }

    public class FieldInfo
    {
        public string Name;
        public string Value;
        public TextType Type;
        public int Length;
        public string Colour = "Green";
        public Point Position;

        public enum TextType
        {
            Input,
            Output,
            Both,
            Text,
            Hidden
        }

        public static Color TextToColor(string Colour)
        {
            switch (Colour.ToUpper())
            {
                case "GREEN":
                    return Color.Lime;
                case "YELLOW":
                    return Color.Yellow;
                case "BLUE":
                    return Color.LightBlue;
                case "RED":
                    return Color.Red;
                case "WHITE":
                    return Color.White;
                case "TURQUOISE":
                    return Color.Turquoise;
                case "PINK":
                    return Color.Pink;

                default:
                    return Color.Green;
            }
        }

        public static string DspfToColour(String COLOR)
        {
            switch (COLOR.ToUpper())
            {
                case "RED":
                    return "Red";
                case "BLU":
                    return "Blue";
                case "WHT":
                    return "White";
                case "GRN":
                    return "Green";
                case "TRQ":
                    return "Turquoise";
                case "YLW":
                    return "Yellow";
                case "PNK":
                    return "Pink";
                default:
                    return "Green";
            }
        }
    }
}
