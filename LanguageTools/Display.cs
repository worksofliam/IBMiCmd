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
                    y = buildString(chars, 38, 3).Trim();
                    x = buildString(chars, 41, 3).Trim();
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
                                        CurrentField.fieldType = FieldInfo.FieldType.Input;
                                        break;
                                    case "B":
                                        CurrentField.fieldType = FieldInfo.FieldType.Both;
                                        break;
                                    case "H":
                                        CurrentField.fieldType = FieldInfo.FieldType.Hidden;
                                        break;
                                    case " ":
                                    case "O":
                                        CurrentField.fieldType = FieldInfo.FieldType.Output;
                                        break;
                                }

                                CurrentField.Decimals = 0;
                                switch (type.ToUpper())
                                {
                                    case "D":
                                        CurrentField.dataType = FieldInfo.DataType.Decimal;
                                        if (dec != "") CurrentField.Decimals = Convert.ToInt32(dec);
                                        break;
                                    default:
                                        CurrentField.dataType = FieldInfo.DataType.Char;
                                        break;
                                }
                                HandleKeywords(keywords);
                            }
                            else
                            {
                                HandleKeywords(keywords);
                                if (CurrentField != null)
                                {
                                    if (CurrentField.Name == null)
                                    {
                                        textcounter++;
                                        CurrentField.Name = "TEXT" + textcounter.ToString();
                                        if (CurrentField.Value == null) CurrentField.Value = "";
                                        CurrentField.Length = CurrentField.Value.Length;
                                        CurrentField.fieldType = FieldInfo.FieldType.Const;
                                    }
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

    public class DisplayGenerate
    {
        private List<string> Output;
        
        public void Generate(Dictionary<string, RecordInfo> Formats)
        {
            Output = new List<string>();

            foreach(string Format in Formats.Keys)
            {
                GenerateFormatHeader(Formats[Format]);
                foreach (FieldInfo Field in Formats[Format].Fields)
                {
                    GenerateField(Field);
                }
            }
        }

        public string[] GetOutput()
        {
            return Output.ToArray();
        }

        private void GenerateFormatHeader(RecordInfo Format)
        {
            string ind = "";

            Output.Add("     A          R " + Format.Name.PadRight(10));

            for (var i = 0; i < Format.FunctionKeys.Length; i++)
            {
                if (Format.FunctionKeys[i])
                {
                    ind = (i + 1).ToString().PadLeft(2, '0');
                    Output.Add("     A                                      CA" + ind + "(" + ind + ")");
                }
            }

            if (Format.Pageup)
                Output.Add("     A                                      ROLLUP(66)");
            if (Format.Pagedown)
                Output.Add("     A                                      ROLLDOWN(44)");
        }

        private void GenerateField(FieldInfo Field)
        {
            string Definition;

            if (Field.fieldType != FieldInfo.FieldType.Const)
            {
                Definition = "     A            " + Field.Name.PadRight(10) + " " + Field.Length.ToString().Trim().PadRight(5);
                switch (Field.dataType)
                {
                    case FieldInfo.DataType.Decimal:
                        Definition += "D";
                        if (Field.Decimals == 0)
                        {
                            Definition += "  ";
                        }
                        else
                        {
                            Definition += Field.Decimals.ToString().Trim().PadLeft(2);
                        }
                        break;
                    case FieldInfo.DataType.Char:
                        Definition += "".PadLeft(3);
                        break;
                }

                switch (Field.fieldType)
                {
                    case FieldInfo.FieldType.Both:
                        Definition += 'B';
                        break;
                    case FieldInfo.FieldType.Hidden:
                        Definition += 'H';
                        break;
                    case FieldInfo.FieldType.Input:
                        Definition += 'I';
                        break;
                    case FieldInfo.FieldType.Output:
                        Definition += 'O';
                        break;
                }
            }
            else
            {
                Definition = "     A                                ";
            }

            if (Field.fieldType != FieldInfo.FieldType.Hidden)
            {
                Definition += Field.Position.Y.ToString().Trim().PadLeft(3);
                Definition += Field.Position.X.ToString().Trim().PadLeft(3);
            }
            else
            {
                Definition += "".PadRight(6);
            }

            if (Field.Value != "")
                Definition += "'" + Field.Value + "'";

            Output.Add(Definition); Definition = "";
            
            if (Field.Colour != "Green")
                Output.Add("     A                                      " + FieldInfo.ColourToDspf(Field.Colour));
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
        public DataType dataType;
        public FieldType fieldType;
        public int Length, Decimals;
        public string Colour = "Green";
        public Point Position;

        public enum DataType
        {
            Char,
            Decimal
        }

        public enum FieldType
        {
            Input,
            Output,
            Both,
            Const,
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

        public static string ColourToDspf(String Colour)
        {
            switch (Colour.ToUpper())
            {
                case "GREEN":
                    return "COLOR(GRN)";
                case "YELLOW":
                    return "COLOR(YLW)";
                case "BLUE":
                    return "COLOR(BLU)";
                case "RED":
                    return "COLOR(RED)";
                case "WHITE":
                    return "COLOR(WHT)";
                case "TURQUOISE":
                    return "COLOR(TRQ)";
                case "PINK":
                    return "COLOR(PNK)";

                default:
                    return "COLOR(GRN)";
            }
        }
    }
}
