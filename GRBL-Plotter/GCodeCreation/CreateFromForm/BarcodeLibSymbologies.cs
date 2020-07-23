/* 
 * ***************************************************
 *                 Barcode Library                   *
 *                                                   *
 *             Written by: Brad Barnhill             *
 *                   Date: 09-21-2007                *
 *                                                   *
 *  This library was designed to give developers an  *
 *  easy class to use when they need to generate     *
 *  barcode images from a string of data.            *
 * ***************************************************
 */



using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GRBL_Plotter.BarcodeCreation
{


    abstract class BarcodeCommon
    {
        protected string Raw_Data = "";
        protected List<string> _Errors = new List<string>();

        public string RawData
        {
            get { return this.Raw_Data; }
        }

        public List<string> Errors
        {
            get { return this._Errors; }
        }

        public void Error(string ErrorMessage)
        {
            this._Errors.Add(ErrorMessage);
			MessageBox.Show(ErrorMessage);
      //      MyException(ErrorMessage);
        }

        internal static bool CheckNumericOnly(string Data)
        {
            return Regex.IsMatch(Data, @"^\d+$", RegexOptions.Compiled);
        }
    }//BarcodeVariables abstract class


    /// <summary>
    ///  Blank encoding template
    ///  Written by: Brad Barnhill
    /// </summary>
    class Blank : BarcodeCommon, IBarcode
    {

        #region IBarcode Members

        public string Encoded_Value
        {   get { return ""; }
            //get { throw new NotImplementedException(); }
        }

        #endregion
    }

    /// <summary>
    ///  Codabar encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Codabar : BarcodeCommon, IBarcode
    {
        private System.Collections.Hashtable Codabar_Code = new System.Collections.Hashtable(); //is initialized by init_Codabar()

        public Codabar(string input)
        {
            Raw_Data = input;
        }//Codabar

        /// <summary>
        /// Encode the raw data using the Codabar algorithm.
        /// </summary>
        private string Encode_Codabar()
        {
            if (Raw_Data.Length < 2) Error("ECODABAR-1: Data format invalid. (Invalid length)");

            //check first char to make sure its a start/stop char
            switch (Raw_Data[0].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default:
                    Error("ECODABAR-2: Data format invalid. (Invalid START character)");
                    break;
            }//switch

            //check the ending char to make sure its a start/stop char
            switch (Raw_Data[Raw_Data.Trim().Length - 1].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default:
                    Error("ECODABAR-3: Data format invalid. (Invalid STOP character)");
                    break;
            }//switch

            //populate the hashtable to begin the process
            this.init_Codabar();

            //replace non-numeric VALID chars with empty strings before checking for all numerics
            string temp = Raw_Data;

            foreach (char c in Codabar_Code.Keys)
            {
                if (!CheckNumericOnly(c.ToString()))
                {
                    temp = temp.Replace(c, '1');
                }//if
            }//if

            //now that all the valid non-numeric chars have been replaced with a number check if all numeric exist
            if (!CheckNumericOnly(temp))
                Error("ECODABAR-4: Data contains invalid  characters.");

            string result = "";

            foreach (char c in Raw_Data)
            {
                result += Codabar_Code[c].ToString();
                result += "0"; //inter-character space
            }//foreach

            //remove the extra 0 at the end of the result
            result = result.Remove(result.Length - 1);

            //clears the hashtable so it no longer takes up memory
            this.Codabar_Code.Clear();

            //change the Raw_Data to strip out the start stop chars for label purposes
            Raw_Data = Raw_Data.Trim().Substring(1, RawData.Trim().Length - 2);

            return result;
        }//Encode_Codabar
        private void init_Codabar()
        {
            Codabar_Code.Clear();
            Codabar_Code.Add('0', "101010011");//"101001101101");
            Codabar_Code.Add('1', "101011001");//"110100101011");
            Codabar_Code.Add('2', "101001011");//"101100101011");
            Codabar_Code.Add('3', "110010101");//"110110010101");
            Codabar_Code.Add('4', "101101001");//"101001101011");
            Codabar_Code.Add('5', "110101001");//"110100110101");
            Codabar_Code.Add('6', "100101011");//"101100110101");
            Codabar_Code.Add('7', "100101101");//"101001011011");
            Codabar_Code.Add('8', "100110101");//"110100101101");
            Codabar_Code.Add('9', "110100101");//"101100101101");
            Codabar_Code.Add('-', "101001101");//"110101001011");
            Codabar_Code.Add('$', "101100101");//"101101001011");
            Codabar_Code.Add(':', "1101011011");//"110110100101");
            Codabar_Code.Add('/', "1101101011");//"101011001011");
            Codabar_Code.Add('.', "1101101101");//"110101100101");
            Codabar_Code.Add('+', "101100110011");//"101101100101");
            Codabar_Code.Add('A', "1011001001");//"110110100101");
            Codabar_Code.Add('B', "1010010011");//"101011001011");
            Codabar_Code.Add('C', "1001001011");//"110101100101");
            Codabar_Code.Add('D', "1010011001");//"101101100101");
            Codabar_Code.Add('a', "1011001001");//"110110100101");
            Codabar_Code.Add('b', "1010010011");//"101011001011");
            Codabar_Code.Add('c', "1001001011");//"110101100101");
            Codabar_Code.Add('d', "1010011001");//"101101100101");
        }//init_Codeabar

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Codabar(); }
        }

        #endregion

    }//class

    /// <summary>
    ///  UPC-A encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class UPCA : BarcodeCommon, IBarcode
    {
        private string[] UPC_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] UPC_CodeB = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
        private string _Country_Assigning_Manufacturer_Code = "N/A";
        private Hashtable CountryCodes = new Hashtable(); //is initialized by init_CountryCodes()

        public UPCA(string input)
        {
            Raw_Data = input;
        }
        /// <summary>
        /// Encode the raw data using the UPC-A algorithm.
        /// </summary>
        private string Encode_UPCA()
        {
            //check length of input
            if (Raw_Data.Length != 11 && Raw_Data.Length != 12)
                Error("EUPCA-1: Data length invalid. (Length must be 11 or 12)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPCA-2: Numeric Data Only");

            CheckDigit();

            string result = "101"; //start with guard bars

            //first number
            result += UPC_CodeA[Int32.Parse(Raw_Data[0].ToString())];

            //second (group) of numbers
            int pos = 0;
            while (pos < 5)
            {
                result += UPC_CodeA[Int32.Parse(Raw_Data[pos + 1].ToString())];
                pos++;
            }//while

            //add divider bars
            result += "01010";

            //third (group) of numbers
            pos = 0;
            while (pos < 5)
            {
                result += UPC_CodeB[Int32.Parse(Raw_Data[(pos++) + 6].ToString())];
            }//while

            //forth
            result += UPC_CodeB[Int32.Parse(Raw_Data[Raw_Data.Length - 1].ToString())];

            //add ending guard bars
            result += "101";

            //get the manufacturer assigning country
            this.init_CountryCodes();
            string twodigitCode = "0" + Raw_Data.Substring(0, 1);
            try
            {
                _Country_Assigning_Manufacturer_Code = CountryCodes[twodigitCode].ToString();
            }//try
            catch
            {
                Error("EUPCA-3: Country assigning manufacturer code not found.");
            }//catch
            finally { CountryCodes.Clear(); }

            return result;
        }//Encode_UPCA
        private void init_CountryCodes()
        {
            CountryCodes.Clear();
            CountryCodes.Add("00", "US / CANADA");
            CountryCodes.Add("01", "US / CANADA");
            CountryCodes.Add("02", "US / CANADA");
            CountryCodes.Add("03", "US / CANADA");
            CountryCodes.Add("04", "US / CANADA");
            CountryCodes.Add("05", "US / CANADA");
            CountryCodes.Add("06", "US / CANADA");
            CountryCodes.Add("07", "US / CANADA");
            CountryCodes.Add("08", "US / CANADA");
            CountryCodes.Add("09", "US / CANADA");
            CountryCodes.Add("10", "US / CANADA");
            CountryCodes.Add("11", "US / CANADA");
            CountryCodes.Add("12", "US / CANADA");
            CountryCodes.Add("13", "US / CANADA");

            CountryCodes.Add("20", "IN STORE");
            CountryCodes.Add("21", "IN STORE");
            CountryCodes.Add("22", "IN STORE");
            CountryCodes.Add("23", "IN STORE");
            CountryCodes.Add("24", "IN STORE");
            CountryCodes.Add("25", "IN STORE");
            CountryCodes.Add("26", "IN STORE");
            CountryCodes.Add("27", "IN STORE");
            CountryCodes.Add("28", "IN STORE");
            CountryCodes.Add("29", "IN STORE");

            CountryCodes.Add("30", "FRANCE");
            CountryCodes.Add("31", "FRANCE");
            CountryCodes.Add("32", "FRANCE");
            CountryCodes.Add("33", "FRANCE");
            CountryCodes.Add("34", "FRANCE");
            CountryCodes.Add("35", "FRANCE");
            CountryCodes.Add("36", "FRANCE");
            CountryCodes.Add("37", "FRANCE");

            CountryCodes.Add("40", "GERMANY");
            CountryCodes.Add("41", "GERMANY");
            CountryCodes.Add("42", "GERMANY");
            CountryCodes.Add("43", "GERMANY");
            CountryCodes.Add("44", "GERMANY");

            CountryCodes.Add("45", "JAPAN");
            CountryCodes.Add("46", "RUSSIAN FEDERATION");
            CountryCodes.Add("49", "JAPAN (JAN-13)");

            CountryCodes.Add("50", "UNITED KINGDOM");
            CountryCodes.Add("54", "BELGIUM / LUXEMBOURG");
            CountryCodes.Add("57", "DENMARK");

            CountryCodes.Add("64", "FINLAND");

            CountryCodes.Add("70", "NORWAY");
            CountryCodes.Add("73", "SWEDEN");
            CountryCodes.Add("76", "SWITZERLAND");

            CountryCodes.Add("80", "ITALY");
            CountryCodes.Add("81", "ITALY");
            CountryCodes.Add("82", "ITALY");
            CountryCodes.Add("83", "ITALY");
            CountryCodes.Add("84", "SPAIN");
            CountryCodes.Add("87", "NETHERLANDS");

            CountryCodes.Add("90", "AUSTRIA");
            CountryCodes.Add("91", "AUSTRIA");
            CountryCodes.Add("93", "AUSTRALIA");
            CountryCodes.Add("94", "NEW ZEALAND");
            CountryCodes.Add("99", "COUPONS");

            CountryCodes.Add("471", "TAIWAN");
            CountryCodes.Add("474", "ESTONIA");
            CountryCodes.Add("475", "LATVIA");
            CountryCodes.Add("477", "LITHUANIA");
            CountryCodes.Add("479", "SRI LANKA");
            CountryCodes.Add("480", "PHILIPPINES");
            CountryCodes.Add("482", "UKRAINE");
            CountryCodes.Add("484", "MOLDOVA");
            CountryCodes.Add("485", "ARMENIA");
            CountryCodes.Add("486", "GEORGIA");
            CountryCodes.Add("487", "KAZAKHSTAN");
            CountryCodes.Add("489", "HONG KONG");

            CountryCodes.Add("520", "GREECE");
            CountryCodes.Add("528", "LEBANON");
            CountryCodes.Add("529", "CYPRUS");
            CountryCodes.Add("531", "MACEDONIA");
            CountryCodes.Add("535", "MALTA");
            CountryCodes.Add("539", "IRELAND");
            CountryCodes.Add("560", "PORTUGAL");
            CountryCodes.Add("569", "ICELAND");
            CountryCodes.Add("590", "POLAND");
            CountryCodes.Add("594", "ROMANIA");
            CountryCodes.Add("599", "HUNGARY");

            CountryCodes.Add("600", "SOUTH AFRICA");
            CountryCodes.Add("601", "SOUTH AFRICA");
            CountryCodes.Add("609", "MAURITIUS");
            CountryCodes.Add("611", "MOROCCO");
            CountryCodes.Add("613", "ALGERIA");
            CountryCodes.Add("619", "TUNISIA");
            CountryCodes.Add("622", "EGYPT");
            CountryCodes.Add("625", "JORDAN");
            CountryCodes.Add("626", "IRAN");
            CountryCodes.Add("690", "CHINA");
            CountryCodes.Add("691", "CHINA");
            CountryCodes.Add("692", "CHINA");

            CountryCodes.Add("729", "ISRAEL");
            CountryCodes.Add("740", "GUATEMALA");
            CountryCodes.Add("741", "EL SALVADOR");
            CountryCodes.Add("742", "HONDURAS");
            CountryCodes.Add("743", "NICARAGUA");
            CountryCodes.Add("744", "COSTA RICA");
            CountryCodes.Add("746", "DOMINICAN REPUBLIC");
            CountryCodes.Add("750", "MEXICO");
            CountryCodes.Add("759", "VENEZUELA");
            CountryCodes.Add("770", "COLOMBIA");
            CountryCodes.Add("773", "URUGUAY");
            CountryCodes.Add("775", "PERU");
            CountryCodes.Add("777", "BOLIVIA");
            CountryCodes.Add("779", "ARGENTINA");
            CountryCodes.Add("780", "CHILE");
            CountryCodes.Add("784", "PARAGUAY");
            CountryCodes.Add("785", "PERU");
            CountryCodes.Add("786", "ECUADOR");
            CountryCodes.Add("789", "BRAZIL");

            CountryCodes.Add("850", "CUBA");
            CountryCodes.Add("858", "SLOVAKIA");
            CountryCodes.Add("859", "CZECH REPUBLIC");
            CountryCodes.Add("860", "YUGLOSLAVIA");
            CountryCodes.Add("869", "TURKEY");
            CountryCodes.Add("880", "SOUTH KOREA");
            CountryCodes.Add("885", "THAILAND");
            CountryCodes.Add("888", "SINGAPORE");
            CountryCodes.Add("890", "INDIA");
            CountryCodes.Add("893", "VIETNAM");
            CountryCodes.Add("899", "INDONESIA");

            CountryCodes.Add("955", "MALAYSIA");
            CountryCodes.Add("977", "INTERNATIONAL STANDARD SERIAL NUMBER FOR PERIODICALS (ISSN)");
            CountryCodes.Add("978", "INTERNATIONAL STANDARD BOOK NUMBERING (ISBN)");
            CountryCodes.Add("979", "INTERNATIONAL STANDARD MUSIC NUMBER (ISMN)");
            CountryCodes.Add("980", "REFUND RECEIPTS");
            CountryCodes.Add("981", "COMMON CURRENCY COUPONS");
            CountryCodes.Add("982", "COMMON CURRENCY COUPONS");
        }//init_CountryCodes
        private void CheckDigit()
        {
            try
            {
                string RawDataHolder = Raw_Data.Substring(0, 11);

                //calculate check digit
                int even = 0;
                int odd = 0;

                for (int i = 0; i < RawDataHolder.Length; i++)
                {
                    if (i % 2 == 0)
                        odd += Int32.Parse(RawDataHolder.Substring(i, 1)) * 3;
                    else
                        even += Int32.Parse(RawDataHolder.Substring(i, 1));
                }//for

                int total = even + odd;
                int cs = total % 10;
                cs = 10 - cs;
                if (cs == 10)
                    cs = 0;

                Raw_Data = RawDataHolder + cs.ToString()[0];
            }//try
            catch
            {
                Error("EUPCA-4: Error calculating check digit.");
            }//catch
        }//CheckDigit

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return this.Encode_UPCA(); }
        }

        #endregion
    }

    /// <summary>
    ///  EAN-13 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class EAN13 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] EAN_CodeC = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
        private string[] EAN_Pattern = { "aaaaaa", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };
        private Hashtable CountryCodes = new Hashtable(); //is initialized by init_CountryCodes()
        private string _Country_Assigning_Manufacturer_Code = "N/A";

        public EAN13(string input)
        {
            Raw_Data = input;

            CheckDigit();
        }
        /// <summary>
        /// Encode the raw data using the EAN-13 algorithm. (Can include the checksum already.  If it doesnt exist in the data then it will calculate it for you.  Accepted data lengths are 12 + 1 checksum or just the 12 data digits)
        /// </summary>
        private string Encode_EAN13()
        {
            //check length of input
            if (Raw_Data.Length < 12 || Raw_Data.Length > 13)
            { Error("EEAN13-1: Data length invalid. (Length must be 12 or 13)"); return ""; }

            if (!CheckNumericOnly(Raw_Data))
            {    Error("EEAN13-2: Numeric Data Only"); return ""; }

            string patterncode = EAN_Pattern[Int32.Parse(Raw_Data[0].ToString())];
            string result = "101";

            //first
            //result += EAN_CodeA[Int32.Parse(RawData[0].ToString())];

            //second
            int pos = 0;
            while (pos < 6)
            {
                if (patterncode[pos] == 'a')
                    result += EAN_CodeA[Int32.Parse(Raw_Data[pos + 1].ToString())];
                if (patterncode[pos] == 'b')
                    result += EAN_CodeB[Int32.Parse(Raw_Data[pos + 1].ToString())];
                pos++;
            }//while


            //add divider bars
            result += "01010";

            //get the third
            pos = 1;
            while (pos <= 5)
            {
                result += EAN_CodeC[Int32.Parse(Raw_Data[(pos++) + 6].ToString())];
            }//while

            //checksum digit
            int cs = Int32.Parse(Raw_Data[Raw_Data.Length - 1].ToString());

            //add checksum
            result += EAN_CodeC[cs];

            //add ending bars
            result += "101";

            //get the manufacturer assigning country
            Init_CountryCodes();
            _Country_Assigning_Manufacturer_Code = "N/A";
            string twodigitCode = Raw_Data.Substring(0, 2);
            string threedigitCode = Raw_Data.Substring(0, 3);
            try
            {
                _Country_Assigning_Manufacturer_Code = CountryCodes[threedigitCode].ToString();
            }//try
            catch
            {
                try
                {
                    _Country_Assigning_Manufacturer_Code = CountryCodes[twodigitCode].ToString();
                }//try
                catch
                {
                    Error("EEAN13-3: Country assigning manufacturer code not found.");
                }//catch 
            }//catch
            finally { CountryCodes.Clear(); }

            return result;
        }//Encode_EAN13

        private void Create_CountryCodeRange(int startingNumber, int endingNumber, string countryDescription)
        {
            for (int i = startingNumber; i <= endingNumber; i++)
            {
                CountryCodes.Add(i.ToString("00"), countryDescription);
            }   // for
        }   // create_CountryCodeRange

        private void Init_CountryCodes()
        {
            CountryCodes.Clear();

            // Source: https://en.wikipedia.org/wiki/List_of_GS1_country_codes
            Create_CountryCodeRange(0, 19, "US / CANADA");
            Create_CountryCodeRange(20, 29, "IN STORE");
            Create_CountryCodeRange(30, 39, "US DRUGS");
            Create_CountryCodeRange(40, 49, "Used to issue restricted circulation numbers within a geographic region (MO defined)");
            Create_CountryCodeRange(50, 59, "GS1 US reserved for future use");
            Create_CountryCodeRange(60, 99, "US / CANADA");
            Create_CountryCodeRange(100, 139, "UNITED STATES");
            Create_CountryCodeRange(200, 299, "Used to issue GS1 restricted circulation number within a geographic region (MO defined)");
            Create_CountryCodeRange(300, 379, "FRANCE AND MONACO");

            Create_CountryCodeRange(380, 380, "BULGARIA");
            Create_CountryCodeRange(383, 383, "SLOVENIA");
            Create_CountryCodeRange(385, 385, "CROATIA");
            Create_CountryCodeRange(387, 387, "BOSNIA AND HERZEGOVINA");
            Create_CountryCodeRange(389, 389, "MONTENEGRO");
            Create_CountryCodeRange(400, 440, "GERMANY");
            Create_CountryCodeRange(450, 459, "JAPAN");
            Create_CountryCodeRange(460, 469, "RUSSIA");
            Create_CountryCodeRange(470, 470, "KYRGYZSTAN");
            Create_CountryCodeRange(471, 471, "TAIWAN");
            Create_CountryCodeRange(474, 474, "ESTONIA");
            Create_CountryCodeRange(475, 475, "LATVIA");
            Create_CountryCodeRange(476, 476, "AZERBAIJAN");
            Create_CountryCodeRange(477, 477, "LITHUANIA");
            Create_CountryCodeRange(478, 478, "UZBEKISTAN");
            Create_CountryCodeRange(479, 479, "SRI LANKA");
            Create_CountryCodeRange(480, 480, "PHILIPPINES");
            Create_CountryCodeRange(481, 481, "BELARUS");
            Create_CountryCodeRange(482, 482, "UKRAINE");
            Create_CountryCodeRange(483, 483, "TURKMENISTAN");
            Create_CountryCodeRange(484, 484, "MOLDOVA");
            Create_CountryCodeRange(485, 485, "ARMENIA");
            Create_CountryCodeRange(486, 486, "GEORGIA");
            Create_CountryCodeRange(487, 487, "KAZAKHSTAN");
            Create_CountryCodeRange(488, 488, "TAJIKISTAN");
            Create_CountryCodeRange(489, 489, "HONG KONG");
            Create_CountryCodeRange(490, 499, "JAPAN");
            Create_CountryCodeRange(500, 509, "UNITED KINGDOM");
            Create_CountryCodeRange(520, 521, "GREECE");
            Create_CountryCodeRange(528, 528, "LEBANON");
            Create_CountryCodeRange(529, 529, "CYPRUS");
            Create_CountryCodeRange(530, 530, "ALBANIA");
            Create_CountryCodeRange(531, 531, "MACEDONIA");
            Create_CountryCodeRange(535, 535, "MALTA");
            Create_CountryCodeRange(539, 539, "REPUBLIC OF IRELAND");
            Create_CountryCodeRange(540, 549, "BELGIUM AND LUXEMBOURG");
            Create_CountryCodeRange(560, 560, "PORTUGAL");
            Create_CountryCodeRange(569, 569, "ICELAND");
            Create_CountryCodeRange(570, 579, "DENMARK, FAROE ISLANDS AND GREENLAND");
            Create_CountryCodeRange(590, 590, "POLAND");
            Create_CountryCodeRange(594, 594, "ROMANIA");
            Create_CountryCodeRange(599, 599, "HUNGARY");
            Create_CountryCodeRange(600, 601, "SOUTH AFRICA");
            Create_CountryCodeRange(603, 603, "GHANA");
            Create_CountryCodeRange(604, 604, "SENEGAL");
            Create_CountryCodeRange(608, 608, "BAHRAIN");
            Create_CountryCodeRange(609, 609, "MAURITIUS");
            Create_CountryCodeRange(611, 611, "MOROCCO");
            Create_CountryCodeRange(613, 613, "ALGERIA");
            Create_CountryCodeRange(615, 615, "NIGERIA");
            Create_CountryCodeRange(616, 616, "KENYA");
            Create_CountryCodeRange(618, 618, "IVORY COAST");
            Create_CountryCodeRange(619, 619, "TUNISIA");
            Create_CountryCodeRange(620, 620, "TANZANIA");
            Create_CountryCodeRange(621, 621, "SYRIA");
            Create_CountryCodeRange(622, 622, "EGYPT");
            Create_CountryCodeRange(623, 623, "BRUNEI");
            Create_CountryCodeRange(624, 624, "LIBYA");
            Create_CountryCodeRange(625, 625, "JORDAN");
            Create_CountryCodeRange(626, 626, "IRAN");
            Create_CountryCodeRange(627, 627, "KUWAIT");
            Create_CountryCodeRange(628, 628, "SAUDI ARABIA");
            Create_CountryCodeRange(629, 629, "UNITED ARAB EMIRATES");
            Create_CountryCodeRange(640, 649, "FINLAND");
            Create_CountryCodeRange(690, 699, "CHINA");
            Create_CountryCodeRange(700, 709, "NORWAY");
            Create_CountryCodeRange(729, 729, "ISRAEL");
            Create_CountryCodeRange(730, 739, "SWEDEN");
            Create_CountryCodeRange(740, 740, "GUATEMALA");
            Create_CountryCodeRange(741, 741, "EL SALVADOR");
            Create_CountryCodeRange(742, 742, "HONDURAS");
            Create_CountryCodeRange(743, 743, "NICARAGUA");
            Create_CountryCodeRange(744, 744, "COSTA RICA");
            Create_CountryCodeRange(745, 745, "PANAMA");
            Create_CountryCodeRange(746, 746, "DOMINICAN REPUBLIC");
            Create_CountryCodeRange(750, 750, "MEXICO");
            Create_CountryCodeRange(754, 755, "CANADA");
            Create_CountryCodeRange(759, 759, "VENEZUELA");
            Create_CountryCodeRange(760, 769, "SWITZERLAND AND LIECHTENSTEIN");
            Create_CountryCodeRange(770, 771, "COLOMBIA");
            Create_CountryCodeRange(773, 773, "URUGUAY");
            Create_CountryCodeRange(775, 775, "PERU");
            Create_CountryCodeRange(777, 777, "BOLIVIA");
            Create_CountryCodeRange(778, 779, "ARGENTINA");
            Create_CountryCodeRange(780, 780, "CHILE");
            Create_CountryCodeRange(784, 784, "PARAGUAY");
            Create_CountryCodeRange(786, 786, "ECUADOR");
            Create_CountryCodeRange(789, 790, "BRAZIL");
            Create_CountryCodeRange(800, 839, "ITALY, SAN MARINO AND VATICAN CITY");
            Create_CountryCodeRange(840, 849, "SPAIN AND ANDORRA");
            Create_CountryCodeRange(850, 850, "CUBA");
            Create_CountryCodeRange(858, 858, "SLOVAKIA");
            Create_CountryCodeRange(859, 859, "CZECH REPUBLIC");
            Create_CountryCodeRange(860, 860, "SERBIA");
            Create_CountryCodeRange(865, 865, "MONGOLIA");
            Create_CountryCodeRange(867, 867, "NORTH KOREA");
            Create_CountryCodeRange(868, 869, "TURKEY");
            Create_CountryCodeRange(870, 879, "NETHERLANDS");
            Create_CountryCodeRange(880, 880, "SOUTH KOREA");
            Create_CountryCodeRange(884, 884, "CAMBODIA");
            Create_CountryCodeRange(885, 885, "THAILAND");
            Create_CountryCodeRange(888, 888, "SINGAPORE");
            Create_CountryCodeRange(890, 890, "INDIA");
            Create_CountryCodeRange(893, 893, "VIETNAM");
            Create_CountryCodeRange(896, 896, "PAKISTAN");
            Create_CountryCodeRange(899, 899, "INDONESIA");
            Create_CountryCodeRange(900, 919, "AUSTRIA");
            Create_CountryCodeRange(930, 939, "AUSTRALIA");
            Create_CountryCodeRange(940, 949, "NEW ZEALAND");
            Create_CountryCodeRange(950, 950, "GS1 GLOBAL OFFICE SPECIAL APPLICATIONS");
            Create_CountryCodeRange(951, 951, "EPC GLOBAL SPECIAL APPLICATIONS");
            Create_CountryCodeRange(955, 955, "MALAYSIA");
            Create_CountryCodeRange(958, 958, "MACAU");
            Create_CountryCodeRange(960, 961, "GS1 UK OFFICE: GTIN-8 ALLOCATIONS");
            Create_CountryCodeRange(962, 969, "GS1 GLOBAL OFFICE: GTIN-8 ALLOCATIONS");
            Create_CountryCodeRange(977, 977, "SERIAL PUBLICATIONS (ISSN)");
            Create_CountryCodeRange(978, 979, "BOOKLAND (ISBN) – 979-0 USED FOR SHEET MUSIC (ISMN-13, REPLACES DEPRECATED ISMN M- NUMBERS)");
            Create_CountryCodeRange(980, 980, "REFUND RECEIPTS");
            Create_CountryCodeRange(981, 984, "GS1 COUPON IDENTIFICATION FOR COMMON CURRENCY AREAS");
            Create_CountryCodeRange(990, 999, "GS1 COUPON IDENTIFICATION");
        }//init_CountryCodes
        private void CheckDigit()
        {
            try
            {
                string RawDataHolder = Raw_Data.Substring(0, 12);

                int even = 0;
                int odd = 0;

                for (int i = 0; i < RawDataHolder.Length; i++)
                {
                    if (i % 2 == 0)
                        odd += Int32.Parse(RawDataHolder.Substring(i, 1));
                    else
                        even += Int32.Parse(RawDataHolder.Substring(i, 1)) * 3;
                }//for

                int total = even + odd;
                int cs = total % 10;
                cs = 10 - cs;
                if (cs == 10)
                    cs = 0;

                Raw_Data = RawDataHolder + cs.ToString()[0];
            }//try
            catch
            {
                Error("EEAN13-4: Error calculating check digit.");
            }//catch
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return this.Encode_EAN13(); }
        }

        #endregion
    }

    /// <summary>
    ///  Interleaved 2 of 5 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Interleaved2of5 : BarcodeCommon, IBarcode
    {
        private readonly string[] I25_Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };
        private readonly TYPE Encoded_Type = TYPE.UNSPECIFIED;

        public Interleaved2of5(string input, TYPE EncodedType)
        {
            Encoded_Type = EncodedType;
            Raw_Data = input;
        }
        /// <summary>
        /// Encode the raw data using the Interleaved 2 of 5 algorithm.
        /// </summary>
        private string Encode_Interleaved2of5()
        {
            //check length of input (only even if no checkdigit, else with check digit odd)
            if (Raw_Data.Length % 2 != (Encoded_Type == TYPE.Interleaved2of5_Mod10 ? 1 : 0))
                Error("EI25-1: Data length invalid.");

            if (!CheckNumericOnly(Raw_Data))
                Error("EI25-2: Numeric Data Only");

            string result = "1010";
            string data = Raw_Data + (Encoded_Type == TYPE.Interleaved2of5_Mod10 ? CalculateMod10CheckDigit().ToString() : "");

            for (int i = 0; i < data.Length; i += 2)
            {
                bool bars = true;
                string patternbars = I25_Code[Int32.Parse(data[i].ToString())];
                string patternspaces = I25_Code[Int32.Parse(data[i + 1].ToString())];
                string patternmixed = "";

                //interleave
                while (patternbars.Length > 0)
                {
                    patternmixed += patternbars[0].ToString() + patternspaces[0].ToString();
                    patternbars = patternbars.Substring(1);
                    patternspaces = patternspaces.Substring(1);
                }//while

                foreach (char c1 in patternmixed)
                {
                    if (bars)
                    {
                        if (c1 == 'N')
                            result += "1";
                        else
                            result += "11";
                    }//if
                    else
                    {
                        if (c1 == 'N')
                            result += "0";
                        else
                            result += "00";
                    }//else
                    bars = !bars;
                }//foreach
            }//foreach

            //add ending bars
            result += "1101";
            return result;
        }//Encode_Interleaved2of5

        private int CalculateMod10CheckDigit()
        {
            int sum = 0;
            bool even = true;
            for (int i = Raw_Data.Length - 1; i >= 0; --i)
            {
                sum += Raw_Data[i] * (even ? 3 : 1);
                even = !even;
            }

            return sum % 10;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return this.Encode_Interleaved2of5(); }
        }

        #endregion
    }

    /// <summary>
    ///  Standard 2 of 5 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Standard2of5 : BarcodeCommon, IBarcode
    {
        private readonly string[] S25_Code = { "10101110111010", "11101010101110", "10111010101110", "11101110101010", "10101110101110", "11101011101010", "10111011101010", "10101011101110", "11101010111010", "10111010111010" };
        private readonly TYPE Encoded_Type = TYPE.UNSPECIFIED;

        public Standard2of5(string input, TYPE EncodedType)
        {
            Raw_Data = input;
            Encoded_Type = EncodedType;
        }//Standard2of5

        /// <summary>
        /// Encode the raw data using the Standard 2 of 5 algorithm.
        /// </summary>
        private string Encode_Standard2of5()
        {
            if (!CheckNumericOnly(Raw_Data))
                Error("ES25-1: Numeric Data Only");

            string result = "11011010";

            foreach (char c in Raw_Data)
            {
                result += S25_Code[Int32.Parse(c.ToString())];
            }//foreach

            result += Encoded_Type == TYPE.Standard2of5_Mod10 ? S25_Code[CalculateMod10CheckDigit()] : "";

            //add ending bars
            result += "1101011";
            return result;
        }//Encode_Standard2of5

        private int CalculateMod10CheckDigit()
        {
            int sum = 0;
            bool even = true;
            for (int i = Raw_Data.Length - 1; i >= 0; --i)
            {
                sum += Raw_Data[i] * (even ? 3 : 1);
                even = !even;
            }

            return sum % 10;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Standard2of5(); }
        }

        #endregion
    }

    /// <summary>
    ///  Code 39 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Code39 : BarcodeCommon, IBarcode
    {
        private System.Collections.Hashtable C39_Code = new System.Collections.Hashtable(); //is initialized by init_Code39()
        private System.Collections.Hashtable ExtC39_Translation = new System.Collections.Hashtable();
        private bool _AllowExtended = false;
        private bool _EnableChecksum = false;

        /// <summary>
        /// Encodes with Code39.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code39(string input)
        {
            Raw_Data = input;
        }//Code39

        /// <summary>
        /// Encodes with Code39.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        /// <param name="AllowExtended">Allow Extended Code 39 (Full Ascii mode).</param>
        public Code39(string input, bool AllowExtended)
        {
            Raw_Data = input;
            _AllowExtended = AllowExtended;
        }

        /// <summary>
        /// Encodes with Code39.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        /// <param name="AllowExtended">Allow Extended Code 39 (Full Ascii mode).</param>
        /// <param name="EnableChecksum">Whether to calculate the Mod 43 checksum and encode it into the barcode</param>
        public Code39(string input, bool AllowExtended, bool EnableChecksum)
        {
            Raw_Data = input;
            _AllowExtended = AllowExtended;
            _EnableChecksum = EnableChecksum;
        }

        /// <summary>
        /// Encode the raw data using the Code 39 algorithm.
        /// </summary>
        private string Encode_Code39()
        {
            this.init_Code39();
            this.init_ExtendedCode39();

            string strNoAstr = Raw_Data.Replace("*", "");
            string strFormattedData = "*" + strNoAstr + (_EnableChecksum ? getChecksumChar(strNoAstr).ToString() : String.Empty) + "*";

            if (_AllowExtended)
                InsertExtendedCharsIfNeeded(ref strFormattedData);

            string result = "";
            //foreach (char c in this.FormattedData)
            foreach (char c in strFormattedData)
            {
                try
                {
                    result += C39_Code[c].ToString();
                    result += "0";//whitespace
                }//try
                catch
                {
                    if (_AllowExtended)
                        Error("EC39-1: Invalid data.");
                    else
                        Error("EC39-1: Invalid data. (Try using Extended Code39)");
                }//catch
            }//foreach

            result = result.Substring(0, result.Length - 1);

            //clear the hashtable so it no longer takes up memory
            this.C39_Code.Clear();

            return result;
        }//Encode_Code39
        private void init_Code39()
        {
            C39_Code.Clear();
            C39_Code.Add('0', "101001101101");
            C39_Code.Add('1', "110100101011");
            C39_Code.Add('2', "101100101011");
            C39_Code.Add('3', "110110010101");
            C39_Code.Add('4', "101001101011");
            C39_Code.Add('5', "110100110101");
            C39_Code.Add('6', "101100110101");
            C39_Code.Add('7', "101001011011");
            C39_Code.Add('8', "110100101101");
            C39_Code.Add('9', "101100101101");
            C39_Code.Add('A', "110101001011");
            C39_Code.Add('B', "101101001011");
            C39_Code.Add('C', "110110100101");
            C39_Code.Add('D', "101011001011");
            C39_Code.Add('E', "110101100101");
            C39_Code.Add('F', "101101100101");
            C39_Code.Add('G', "101010011011");
            C39_Code.Add('H', "110101001101");
            C39_Code.Add('I', "101101001101");
            C39_Code.Add('J', "101011001101");
            C39_Code.Add('K', "110101010011");
            C39_Code.Add('L', "101101010011");
            C39_Code.Add('M', "110110101001");
            C39_Code.Add('N', "101011010011");
            C39_Code.Add('O', "110101101001");
            C39_Code.Add('P', "101101101001");
            C39_Code.Add('Q', "101010110011");
            C39_Code.Add('R', "110101011001");
            C39_Code.Add('S', "101101011001");
            C39_Code.Add('T', "101011011001");
            C39_Code.Add('U', "110010101011");
            C39_Code.Add('V', "100110101011");
            C39_Code.Add('W', "110011010101");
            C39_Code.Add('X', "100101101011");
            C39_Code.Add('Y', "110010110101");
            C39_Code.Add('Z', "100110110101");
            C39_Code.Add('-', "100101011011");
            C39_Code.Add('.', "110010101101");
            C39_Code.Add(' ', "100110101101");
            C39_Code.Add('$', "100100100101");
            C39_Code.Add('/', "100100101001");
            C39_Code.Add('+', "100101001001");
            C39_Code.Add('%', "101001001001");
            C39_Code.Add('*', "100101101101");
        }//init_Code39
        private void init_ExtendedCode39()
        {
            ExtC39_Translation.Clear();
            ExtC39_Translation.Add(Convert.ToChar(0).ToString(), "%U");
            ExtC39_Translation.Add(Convert.ToChar(1).ToString(), "$A");
            ExtC39_Translation.Add(Convert.ToChar(2).ToString(), "$B");
            ExtC39_Translation.Add(Convert.ToChar(3).ToString(), "$C");
            ExtC39_Translation.Add(Convert.ToChar(4).ToString(), "$D");
            ExtC39_Translation.Add(Convert.ToChar(5).ToString(), "$E");
            ExtC39_Translation.Add(Convert.ToChar(6).ToString(), "$F");
            ExtC39_Translation.Add(Convert.ToChar(7).ToString(), "$G");
            ExtC39_Translation.Add(Convert.ToChar(8).ToString(), "$H");
            ExtC39_Translation.Add(Convert.ToChar(9).ToString(), "$I");
            ExtC39_Translation.Add(Convert.ToChar(10).ToString(), "$J");
            ExtC39_Translation.Add(Convert.ToChar(11).ToString(), "$K");
            ExtC39_Translation.Add(Convert.ToChar(12).ToString(), "$L");
            ExtC39_Translation.Add(Convert.ToChar(13).ToString(), "$M");
            ExtC39_Translation.Add(Convert.ToChar(14).ToString(), "$N");
            ExtC39_Translation.Add(Convert.ToChar(15).ToString(), "$O");
            ExtC39_Translation.Add(Convert.ToChar(16).ToString(), "$P");
            ExtC39_Translation.Add(Convert.ToChar(17).ToString(), "$Q");
            ExtC39_Translation.Add(Convert.ToChar(18).ToString(), "$R");
            ExtC39_Translation.Add(Convert.ToChar(19).ToString(), "$S");
            ExtC39_Translation.Add(Convert.ToChar(20).ToString(), "$T");
            ExtC39_Translation.Add(Convert.ToChar(21).ToString(), "$U");
            ExtC39_Translation.Add(Convert.ToChar(22).ToString(), "$V");
            ExtC39_Translation.Add(Convert.ToChar(23).ToString(), "$W");
            ExtC39_Translation.Add(Convert.ToChar(24).ToString(), "$X");
            ExtC39_Translation.Add(Convert.ToChar(25).ToString(), "$Y");
            ExtC39_Translation.Add(Convert.ToChar(26).ToString(), "$Z");
            ExtC39_Translation.Add(Convert.ToChar(27).ToString(), "%A");
            ExtC39_Translation.Add(Convert.ToChar(28).ToString(), "%B");
            ExtC39_Translation.Add(Convert.ToChar(29).ToString(), "%C");
            ExtC39_Translation.Add(Convert.ToChar(30).ToString(), "%D");
            ExtC39_Translation.Add(Convert.ToChar(31).ToString(), "%E");
            ExtC39_Translation.Add("!", "/A");
            ExtC39_Translation.Add("\"", "/B");
            ExtC39_Translation.Add("#", "/C");
            ExtC39_Translation.Add("$", "/D");
            ExtC39_Translation.Add("%", "/E");
            ExtC39_Translation.Add("&", "/F");
            ExtC39_Translation.Add("'", "/G");
            ExtC39_Translation.Add("(", "/H");
            ExtC39_Translation.Add(")", "/I");
            ExtC39_Translation.Add("*", "/J");
            ExtC39_Translation.Add("+", "/K");
            ExtC39_Translation.Add(",", "/L");
            ExtC39_Translation.Add("/", "/O");
            ExtC39_Translation.Add(":", "/Z");
            ExtC39_Translation.Add(";", "%F");
            ExtC39_Translation.Add("<", "%G");
            ExtC39_Translation.Add("=", "%H");
            ExtC39_Translation.Add(">", "%I");
            ExtC39_Translation.Add("?", "%J");
            ExtC39_Translation.Add("[", "%K");
            ExtC39_Translation.Add("\\", "%L");
            ExtC39_Translation.Add("]", "%M");
            ExtC39_Translation.Add("^", "%N");
            ExtC39_Translation.Add("_", "%O");
            ExtC39_Translation.Add("{", "%P");
            ExtC39_Translation.Add("|", "%Q");
            ExtC39_Translation.Add("}", "%R");
            ExtC39_Translation.Add("~", "%S");
            ExtC39_Translation.Add("`", "%W");
            ExtC39_Translation.Add("@", "%V");
            ExtC39_Translation.Add("a", "+A");
            ExtC39_Translation.Add("b", "+B");
            ExtC39_Translation.Add("c", "+C");
            ExtC39_Translation.Add("d", "+D");
            ExtC39_Translation.Add("e", "+E");
            ExtC39_Translation.Add("f", "+F");
            ExtC39_Translation.Add("g", "+G");
            ExtC39_Translation.Add("h", "+H");
            ExtC39_Translation.Add("i", "+I");
            ExtC39_Translation.Add("j", "+J");
            ExtC39_Translation.Add("k", "+K");
            ExtC39_Translation.Add("l", "+L");
            ExtC39_Translation.Add("m", "+M");
            ExtC39_Translation.Add("n", "+N");
            ExtC39_Translation.Add("o", "+O");
            ExtC39_Translation.Add("p", "+P");
            ExtC39_Translation.Add("q", "+Q");
            ExtC39_Translation.Add("r", "+R");
            ExtC39_Translation.Add("s", "+S");
            ExtC39_Translation.Add("t", "+T");
            ExtC39_Translation.Add("u", "+U");
            ExtC39_Translation.Add("v", "+V");
            ExtC39_Translation.Add("w", "+W");
            ExtC39_Translation.Add("x", "+X");
            ExtC39_Translation.Add("y", "+Y");
            ExtC39_Translation.Add("z", "+Z");
            ExtC39_Translation.Add(Convert.ToChar(127).ToString(), "%T"); //also %X, %Y, %Z 
        }
        private void InsertExtendedCharsIfNeeded(ref string FormattedData)
        {
            string output = "";
            foreach (char c in FormattedData)
            {
                try
                {
                    string s = C39_Code[c].ToString();
                    output += c;
                }//try
                catch
                {
                    //insert extended substitution
                    object oTrans = ExtC39_Translation[c.ToString()];
                    output += oTrans.ToString();
                }//catch
            }//foreach

            FormattedData = output;
        }
        private char getChecksumChar(string strNoAstr)
        {
            //checksum
            string Code39_Charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
            InsertExtendedCharsIfNeeded(ref strNoAstr);
            int sum = 0;

            //Calculate the checksum
            for (int i = 0; i < strNoAstr.Length; ++i)
            {
                sum = sum + Code39_Charset.IndexOf(strNoAstr[i].ToString());
            }

            //return the checksum char
            return Code39_Charset[sum % 43];
        }
        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code39(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  Postnet encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Postnet : BarcodeCommon, IBarcode
    {
        private string[] POSTNET_Code = { "11000", "00011", "00101", "00110", "01001", "01010", "01100", "10001", "10010", "10100" };

        public Postnet(string input)
        {
            Raw_Data = input;
        }//Postnet

        /// <summary>
        /// Encode the raw data using the PostNet algorithm.
        /// </summary>
        private string Encode_Postnet()
        {
            //remove dashes if present
            Raw_Data = Raw_Data.Replace("-", "");

            switch (Raw_Data.Length)
            {
                case 5:
                case 6:
                case 9:
                case 11: break;
                default:
                    Error("EPOSTNET-2: Invalid data length. (5, 6, 9, or 11 digits only)");
                    break;
            }//switch

            //Note: 0 = half bar and 1 = full bar
            //initialize the result with the starting bar
            string result = "1";
            int checkdigitsum = 0;

            foreach (char c in Raw_Data)
            {
                try
                {
                    int index = Convert.ToInt32(c.ToString());
                    result += POSTNET_Code[index];
                    checkdigitsum += index;
                }//try
                catch (Exception ex)
                {
                    Error("EPOSTNET-2: Invalid data. (Numeric only) --> " + ex.Message);
                }//catch
            }//foreach

            //calculate and add check digit
            int temp = checkdigitsum % 10;
            int checkdigit = 10 - (temp == 0 ? 10 : temp);

            result += POSTNET_Code[checkdigit];

            //ending bar
            result += "1";

            return result;
        }//Encode_PostNet

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Postnet(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  ISBN encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            Raw_Data = input;
        }
        /// <summary>
        /// Encode the raw data using the Bookland/ISBN algorithm.
        /// </summary>
        private string Encode_ISBN_Bookland()
        {
            if (!CheckNumericOnly(Raw_Data))
                Error("EBOOKLANDISBN-1: Numeric Data Only");

            string type = "UNKNOWN";
            if (Raw_Data.Length == 10 || Raw_Data.Length == 9)
            {
                if (Raw_Data.Length == 10) Raw_Data = Raw_Data.Remove(9, 1);
                Raw_Data = "978" + Raw_Data;
                type = "ISBN";
            }//if
            else if (Raw_Data.Length == 12 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-NOCHECKDIGIT";
            }//else if
            else if (Raw_Data.Length == 13 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-CHECKDIGIT";
                Raw_Data = Raw_Data.Remove(12, 1);
            }//else if

            //check to see if its an unknown type
            if (type == "UNKNOWN") Error("EBOOKLANDISBN-2: Invalid input.  Must start with 978 and be length must be 9, 10, 12, 13 characters.");

            EAN13 ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }//Encode_ISBN_Bookland

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_ISBN_Bookland(); }
        }

        #endregion
    }

    /// <summary>
    ///  JAN-13 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class JAN13 : BarcodeCommon, IBarcode
    {
        public JAN13(string input)
        {
            Raw_Data = input;
        }
        /// <summary>
        /// Encode the raw data using the JAN-13 algorithm.
        /// </summary>
        private string Encode_JAN13()
        {
            if (!Raw_Data.StartsWith("49")) Error("EJAN13-1: Invalid Country Code for JAN13 (49 required)");
            if (!CheckNumericOnly(Raw_Data))
                Error("EJAN13-2: Numeric Data Only");

            EAN13 ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }//Encode_JAN13

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_JAN13(); }
        }

        #endregion
    }

    /// <summary>
    ///  UPC Supplement-2 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class UPCSupplement2 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] UPC_SUPP_2 = { "aa", "ab", "ba", "bb" };

        public UPCSupplement2(string input)
        {
            Raw_Data = input;
        }

        /// <summary>
        /// Encode the raw data using the UPC Supplemental 2-digit algorithm.
        /// </summary>
        private string Encode_UPCSupplemental_2()
        {
            if (Raw_Data.Length != 2) Error("EUPC-SUP2-1: Invalid data length. (Length = 2 required)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPC-SUP2-2: Numeric Data Only");

            string pattern = "";

            try
            {
                pattern = this.UPC_SUPP_2[Int32.Parse(Raw_Data.Trim()) % 4];
            }//try
            catch { Error("EUPC-SUP2-3: Invalid Data. (Numeric only)"); }

            string result = "1011";

            int pos = 0;
            foreach (char c in pattern)
            {
                if (c == 'a')
                {
                    //encode using odd parity
                    result += EAN_CodeA[Int32.Parse(Raw_Data[pos].ToString())];
                }//if
                else if (c == 'b')
                {
                    //encode using even parity
                    result += EAN_CodeB[Int32.Parse(Raw_Data[pos].ToString())];
                }//else if

                if (pos++ == 0) result += "01"; //Inter-character separator
            }//foreach
            return result;
        }//Encode_UPSSupplemental_2

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_2(); }
        }

        #endregion
    }//class

    class MSI : BarcodeCommon, IBarcode
    {
        /// <summary>
        ///  MSI encoding
        ///  Written by: Brad Barnhill
        /// </summary>
        private string[] MSI_Code = { "100100100100", "100100100110", "100100110100", "100100110110", "100110100100", "100110100110", "100110110100", "100110110110", "110100100100", "110100100110" };
        private TYPE Encoded_Type = TYPE.UNSPECIFIED;

        public MSI(string input, TYPE EncodedType)
        {
            Encoded_Type = EncodedType;
            Raw_Data = input;
        }//MSI

        /// <summary>
        /// Encode the raw data using the MSI algorithm.
        /// </summary>
        private string Encode_MSI()
        {
            //check for non-numeric chars
            if (!CheckNumericOnly(Raw_Data))
                Error("EMSI-1: Numeric Data Only");

            string PreEncoded = Raw_Data;

            //get checksum
            if (Encoded_Type == TYPE.MSI_Mod10 || Encoded_Type == TYPE.MSI_2Mod10)
            {
                string odds = "";
                string evens = "";
                for (int i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i].ToString() + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1].ToString() + evens;
                }//for

                //multiply odds by 2
                odds = Convert.ToString((Int32.Parse(odds) * 2));

                int evensum = 0;
                int oddsum = 0;
                foreach (char c in evens)
                    evensum += Int32.Parse(c.ToString());
                foreach (char c in odds)
                    oddsum += Int32.Parse(c.ToString());
                int mod = (oddsum + evensum) % 10;
                int checksum = mod == 0 ? 0 : 10 - mod;
                PreEncoded += checksum.ToString();
            }//if

            if (Encoded_Type == TYPE.MSI_Mod11 || Encoded_Type == TYPE.MSI_Mod11_Mod10)
            {
                int sum = 0;
                int weight = 2;
                for (int i = PreEncoded.Length - 1; i >= 0; i--)
                {
                    if (weight > 7) weight = 2;
                    sum += Int32.Parse(PreEncoded[i].ToString()) * weight++;
                }//foreach
                int mod = sum % 11;
                int checksum = mod == 0 ? 0 : 11 - mod;

                PreEncoded += checksum.ToString();
            }//else

            if (Encoded_Type == TYPE.MSI_2Mod10 || Encoded_Type == TYPE.MSI_Mod11_Mod10)
            {
                //get second check digit if 2 mod 10 was selected or Mod11/Mod10
                string odds = "";
                string evens = "";
                for (int i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i].ToString() + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1].ToString() + evens;
                }//for

                //multiply odds by 2
                odds = Convert.ToString((Int32.Parse(odds) * 2));

                int evensum = 0;
                int oddsum = 0;
                foreach (char c in evens)
                    evensum += Int32.Parse(c.ToString());
                foreach (char c in odds)
                    oddsum += Int32.Parse(c.ToString());
                int checksum = 10 - ((oddsum + evensum) % 10);
                PreEncoded += checksum.ToString();
            }//if

            string result = "110";
            foreach (char c in PreEncoded)
            {
                result += MSI_Code[Int32.Parse(c.ToString())];
            }//foreach

            //add stop character
            result += "1001";

            return result;
        }//Encode_MSI

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_MSI(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  UPC Supplement-5 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class UPCSupplement5 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] UPC_SUPP_5 = { "bbaaa", "babaa", "baaba", "baaab", "abbaa", "aabba", "aaabb", "ababa", "abaab", "aabab" };

        public UPCSupplement5(string input)
        {
            Raw_Data = input;
        }

        /// <summary>
        /// Encode the raw data using the UPC Supplemental 5-digit algorithm.
        /// </summary>
        private string Encode_UPCSupplemental_5()
        {
            if (Raw_Data.Length != 5) Error("EUPC-SUP5-1: Invalid data length. (Length = 5 required)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPCA-2: Numeric Data Only");

            //calculate the checksum digit
            int even = 0;
            int odd = 0;

            //odd
            for (int i = 0; i <= 4; i += 2)
            {
                odd += Int32.Parse(Raw_Data.Substring(i, 1)) * 3;
            }//for

            //even
            for (int i = 1; i < 4; i += 2)
            {
                even += Int32.Parse(Raw_Data.Substring(i, 1)) * 9;
            }//for

            int total = even + odd;
            int cs = total % 10;

            string pattern = UPC_SUPP_5[cs];

            string result = "";

            int pos = 0;
            foreach (char c in pattern)
            {
                //Inter-character separator
                if (pos == 0) result += "1011";
                else result += "01";

                if (c == 'a')
                {
                    //encode using odd parity
                    result += EAN_CodeA[Int32.Parse(Raw_Data[pos].ToString())];
                }//if
                else if (c == 'b')
                {
                    //encode using even parity
                    result += EAN_CodeB[Int32.Parse(Raw_Data[pos].ToString())];
                }//else if  
                pos++;
            }//foreach
            return result;
        }//Encode_UPCSupplemental_5

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_5(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  UPC-E encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class UPCE : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] EAN_Pattern = { "aaaaaa", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };
        private string[] UPCE_Code_0 = { "bbbaaa", "bbabaa", "bbaaba", "bbaaab", "babbaa", "baabba", "baaabb", "bababa", "babaab", "baabab" };
        private string[] UPCE_Code_1 = { "aaabbb", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };

        /// <summary>
        /// Encodes a UPC-E symbol.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public UPCE(string input)
        {
            Raw_Data = input;
        }//UPCE
        /// <summary>
        /// Encode the raw data using the UPC-E algorithm.
        /// </summary>
        private string Encode_UPCE()
        {
            if (Raw_Data.Length != 6 && Raw_Data.Length != 8 && Raw_Data.Length != 12)
                Error("EUPCE-1: Invalid data length. (8 or 12 numbers only)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPCE-2: Numeric only.");

            //check for a valid number system
            int NumberSystem = Int32.Parse(Raw_Data[0].ToString());
            if (NumberSystem != 0 && NumberSystem != 1)
                Error("EUPCE-3: Invalid Number System (only 0 & 1 are valid)");

            int CheckDigit = Int32.Parse(Raw_Data[Raw_Data.Length - 1].ToString());

            //Convert to UPC-E from UPC-A if necessary
            if (Raw_Data.Length == 12)
            {
                string UPCECode = "";

                //break apart into components
                string Manufacturer = Raw_Data.Substring(1, 5);
                string ProductCode = Raw_Data.Substring(6, 5);

                if (Manufacturer.EndsWith("000") || Manufacturer.EndsWith("100") || Manufacturer.EndsWith("200") && Int32.Parse(ProductCode) <= 999)
                {
                    //rule 1
                    UPCECode += Manufacturer.Substring(0, 2); //first two of manufacturer
                    UPCECode += ProductCode.Substring(2, 3); //last three of product
                    UPCECode += Manufacturer[2].ToString(); //third of manufacturer
                }//if
                else if (Manufacturer.EndsWith("00") && Int32.Parse(ProductCode) <= 99)
                {
                    //rule 2
                    UPCECode += Manufacturer.Substring(0, 3); //first three of manufacturer
                    UPCECode += ProductCode.Substring(3, 2); //last two of product
                    UPCECode += "3"; //number 3
                }//else if
                else if (Manufacturer.EndsWith("0") && Int32.Parse(ProductCode) <= 9)
                {
                    //rule 3
                    UPCECode += Manufacturer.Substring(0, 4); //first four of manufacturer
                    UPCECode += ProductCode[4]; //last digit of product
                    UPCECode += "4"; //number 4
                }//else if
                else if (!Manufacturer.EndsWith("0") && Int32.Parse(ProductCode) <= 9 && Int32.Parse(ProductCode) >= 5)
                {
                    //rule 4
                    UPCECode += Manufacturer; //manufacturer
                    UPCECode += ProductCode[4]; //last digit of product
                }//else if
                else
                    Error("EUPCE-4: Illegal UPC-A entered for conversion.  Unable to convert.");

                Raw_Data = UPCECode;
            }//if

            //get encoding pattern 
            string pattern = "";

            if (NumberSystem == 0) pattern = UPCE_Code_0[CheckDigit];
            else pattern = UPCE_Code_1[CheckDigit];

            //encode the data
            string result = "101";

            int pos = 0;
            foreach (char c in pattern)
            {
                int i = Int32.Parse(Raw_Data[++pos].ToString());
                if (c == 'a')
                {
                    result += EAN_CodeA[i];
                }//if
                else if (c == 'b')
                {
                    result += EAN_CodeB[i];
                }//else if
            }//foreach

            //guard bars
            result += "01010";

            //end bars
            result += "1";

            return result;
        }//Encode_UPCE

        #region IBarcode Members
        public string Encoded_Value
        {
            get { return Encode_UPCE(); }
        }
        #endregion





    }

    /// <summary>
    ///  EAN-8 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class EAN8 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeC = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };

        public EAN8(string input)
        {
            Raw_Data = input;

            //check numeric only
            if (!CheckNumericOnly(Raw_Data)) Error("EEAN8-2: Numeric only.");

            CheckDigit();
        }
        /// <summary>
        /// Encode the raw data using the EAN-8 algorithm.
        /// </summary>
        private string Encode_EAN8()
        {
            //check length
            if (Raw_Data.Length != 8 && Raw_Data.Length != 7) Error("EEAN8-1: Invalid data length. (7 or 8 numbers only)");

            //encode the data
            string result = "101";

            //first half (Encoded using left hand / odd parity)
            for (int i = 0; i < Raw_Data.Length / 2; i++)
            {
                result += EAN_CodeA[Int32.Parse(Raw_Data[i].ToString())];
            }//for

            //center guard bars
            result += "01010";

            //second half (Encoded using right hand / even parity)
            for (int i = Raw_Data.Length / 2; i < Raw_Data.Length; i++)
            {
                result += EAN_CodeC[Int32.Parse(Raw_Data[i].ToString())];
            }//for

            result += "101";

            return result;
        }//Encode_EAN8

        private void CheckDigit()
        {
            //calculate the checksum digit if necessary
            if (Raw_Data.Length == 7)
            {
                //calculate the checksum digit
                int even = 0;
                int odd = 0;

                //odd
                for (int i = 0; i <= 6; i += 2)
                {
                    odd += Int32.Parse(Raw_Data.Substring(i, 1)) * 3;
                }//for

                //even
                for (int i = 1; i <= 5; i += 2)
                {
                    even += Int32.Parse(Raw_Data.Substring(i, 1));
                }//for

                int total = even + odd;
                int checksum = total % 10;
                checksum = 10 - checksum;
                if (checksum == 10)
                    checksum = 0;

                //add the checksum to the end of the 
                Raw_Data += checksum.ToString();
            }//if
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_EAN8(); }
        }

        #endregion
    }

    /// <summary>
    ///  Code 11 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Code11 : BarcodeCommon, IBarcode
    {
        private string[] C11_Code = { "101011", "1101011", "1001011", "1100101", "1011011", "1101101", "1001101", "1010011", "1101001", "110101", "101101", "1011001" };

        public Code11(string input)
        {
            Raw_Data = input;
        }//Code11
        /// <summary>
        /// Encode the raw data using the Code 11 algorithm.
        /// </summary>
        private string Encode_Code11()
        {
            if (!CheckNumericOnly(Raw_Data.Replace("-", "")))
                Error("EC11-1: Numeric data and '-' Only");

            //calculate the checksums
            int weight = 1;
            int CTotal = 0;
            string Data_To_Encode_with_Checksums = Raw_Data;

            //figure the C checksum
            for (int i = Raw_Data.Length - 1; i >= 0; i--)
            {
                //C checksum weights go 1-10
                if (weight == 10) weight = 1;

                if (Raw_Data[i] != '-')
                    CTotal += Int32.Parse(Raw_Data[i].ToString()) * weight++;
                else
                    CTotal += 10 * weight++;
            }//for
            int checksumC = CTotal % 11;

            Data_To_Encode_with_Checksums += checksumC.ToString();

            //K checksums are recommended on any message length greater than or equal to 10
            if (Raw_Data.Length >= 10)
            {
                weight = 1;
                int KTotal = 0;

                //calculate K checksum
                for (int i = Data_To_Encode_with_Checksums.Length - 1; i >= 0; i--)
                {
                    //K checksum weights go 1-9
                    if (weight == 9) weight = 1;

                    if (Data_To_Encode_with_Checksums[i] != '-')
                        KTotal += Int32.Parse(Data_To_Encode_with_Checksums[i].ToString()) * weight++;
                    else
                        KTotal += 10 * weight++;
                }//for
                int checksumK = KTotal % 11;
                Data_To_Encode_with_Checksums += checksumK.ToString();
            }//if

            //encode data
            string space = "0";
            string result = C11_Code[11] + space; //start-stop char + interchar space

            foreach (char c in Data_To_Encode_with_Checksums)
            {
                int index = (c == '-' ? 10 : Int32.Parse(c.ToString()));
                result += C11_Code[index];

                //inter-character space
                result += space;
            }//foreach

            //stop bars
            result += C11_Code[11];

            return result;
        }//Encode_Code11 

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code11(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  Code 128 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Code128 : BarcodeCommon, IBarcode
    {
        public static char FNC1 = Convert.ToChar(200);
        public static char FNC2 = Convert.ToChar(201);
        public static char FNC3 = Convert.ToChar(202);
        public static char FNC4 = Convert.ToChar(203);

        public enum TYPES : int { DYNAMIC, A, B, C };
        private DataTable C128_Code = new DataTable("C128");
        private List<string> _FormattedData = new List<string>();
        private List<string> _EncodedData = new List<string>();
        private DataRow StartCharacter = null;
        private TYPES type = TYPES.DYNAMIC;

        /// <summary>
        /// Encodes data in Code128 format.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code128(string input)
        {
            Raw_Data = input;
        }//Code128

        /// <summary>
        /// Encodes data in Code128 format.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        /// <param name="type">Type of encoding to lock to. (Code 128A, Code 128B, Code 128C)</param>
        public Code128(string input, TYPES type)
        {
            this.type = type;
            Raw_Data = input;
        }//Code128

        private string Encode_Code128()
        {
            //initialize datastructure to hold encoding information
            this.init_Code128();

            return GetEncoding();
        }//Encode_Code128
        private void init_Code128()
        {
            //set the table to case sensitive since there are upper and lower case values
            this.C128_Code.CaseSensitive = true;

            //set up columns
            this.C128_Code.Columns.Add("Value", typeof(string));
            this.C128_Code.Columns.Add("A", typeof(string));
            this.C128_Code.Columns.Add("B", typeof(string));
            this.C128_Code.Columns.Add("C", typeof(string));
            this.C128_Code.Columns.Add("Encoding", typeof(string));

            //populate data
            this.C128_Code.Rows.Add(new object[] { "0", " ", " ", "00", "11011001100" });
            this.C128_Code.Rows.Add(new object[] { "1", "!", "!", "01", "11001101100" });
            this.C128_Code.Rows.Add(new object[] { "2", "\"", "\"", "02", "11001100110" });
            this.C128_Code.Rows.Add(new object[] { "3", "#", "#", "03", "10010011000" });
            this.C128_Code.Rows.Add(new object[] { "4", "$", "$", "04", "10010001100" });
            this.C128_Code.Rows.Add(new object[] { "5", "%", "%", "05", "10001001100" });
            this.C128_Code.Rows.Add(new object[] { "6", "&", "&", "06", "10011001000" });
            this.C128_Code.Rows.Add(new object[] { "7", "'", "'", "07", "10011000100" });
            this.C128_Code.Rows.Add(new object[] { "8", "(", "(", "08", "10001100100" });
            this.C128_Code.Rows.Add(new object[] { "9", ")", ")", "09", "11001001000" });
            this.C128_Code.Rows.Add(new object[] { "10", "*", "*", "10", "11001000100" });
            this.C128_Code.Rows.Add(new object[] { "11", "+", "+", "11", "11000100100" });
            this.C128_Code.Rows.Add(new object[] { "12", ",", ",", "12", "10110011100" });
            this.C128_Code.Rows.Add(new object[] { "13", "-", "-", "13", "10011011100" });
            this.C128_Code.Rows.Add(new object[] { "14", ".", ".", "14", "10011001110" });
            this.C128_Code.Rows.Add(new object[] { "15", "/", "/", "15", "10111001100" });
            this.C128_Code.Rows.Add(new object[] { "16", "0", "0", "16", "10011101100" });
            this.C128_Code.Rows.Add(new object[] { "17", "1", "1", "17", "10011100110" });
            this.C128_Code.Rows.Add(new object[] { "18", "2", "2", "18", "11001110010" });
            this.C128_Code.Rows.Add(new object[] { "19", "3", "3", "19", "11001011100" });
            this.C128_Code.Rows.Add(new object[] { "20", "4", "4", "20", "11001001110" });
            this.C128_Code.Rows.Add(new object[] { "21", "5", "5", "21", "11011100100" });
            this.C128_Code.Rows.Add(new object[] { "22", "6", "6", "22", "11001110100" });
            this.C128_Code.Rows.Add(new object[] { "23", "7", "7", "23", "11101101110" });
            this.C128_Code.Rows.Add(new object[] { "24", "8", "8", "24", "11101001100" });
            this.C128_Code.Rows.Add(new object[] { "25", "9", "9", "25", "11100101100" });
            this.C128_Code.Rows.Add(new object[] { "26", ":", ":", "26", "11100100110" });
            this.C128_Code.Rows.Add(new object[] { "27", ";", ";", "27", "11101100100" });
            this.C128_Code.Rows.Add(new object[] { "28", "<", "<", "28", "11100110100" });
            this.C128_Code.Rows.Add(new object[] { "29", "=", "=", "29", "11100110010" });
            this.C128_Code.Rows.Add(new object[] { "30", ">", ">", "30", "11011011000" });
            this.C128_Code.Rows.Add(new object[] { "31", "?", "?", "31", "11011000110" });
            this.C128_Code.Rows.Add(new object[] { "32", "@", "@", "32", "11000110110" });
            this.C128_Code.Rows.Add(new object[] { "33", "A", "A", "33", "10100011000" });
            this.C128_Code.Rows.Add(new object[] { "34", "B", "B", "34", "10001011000" });
            this.C128_Code.Rows.Add(new object[] { "35", "C", "C", "35", "10001000110" });
            this.C128_Code.Rows.Add(new object[] { "36", "D", "D", "36", "10110001000" });
            this.C128_Code.Rows.Add(new object[] { "37", "E", "E", "37", "10001101000" });
            this.C128_Code.Rows.Add(new object[] { "38", "F", "F", "38", "10001100010" });
            this.C128_Code.Rows.Add(new object[] { "39", "G", "G", "39", "11010001000" });
            this.C128_Code.Rows.Add(new object[] { "40", "H", "H", "40", "11000101000" });
            this.C128_Code.Rows.Add(new object[] { "41", "I", "I", "41", "11000100010" });
            this.C128_Code.Rows.Add(new object[] { "42", "J", "J", "42", "10110111000" });
            this.C128_Code.Rows.Add(new object[] { "43", "K", "K", "43", "10110001110" });
            this.C128_Code.Rows.Add(new object[] { "44", "L", "L", "44", "10001101110" });
            this.C128_Code.Rows.Add(new object[] { "45", "M", "M", "45", "10111011000" });
            this.C128_Code.Rows.Add(new object[] { "46", "N", "N", "46", "10111000110" });
            this.C128_Code.Rows.Add(new object[] { "47", "O", "O", "47", "10001110110" });
            this.C128_Code.Rows.Add(new object[] { "48", "P", "P", "48", "11101110110" });
            this.C128_Code.Rows.Add(new object[] { "49", "Q", "Q", "49", "11010001110" });
            this.C128_Code.Rows.Add(new object[] { "50", "R", "R", "50", "11000101110" });
            this.C128_Code.Rows.Add(new object[] { "51", "S", "S", "51", "11011101000" });
            this.C128_Code.Rows.Add(new object[] { "52", "T", "T", "52", "11011100010" });
            this.C128_Code.Rows.Add(new object[] { "53", "U", "U", "53", "11011101110" });
            this.C128_Code.Rows.Add(new object[] { "54", "V", "V", "54", "11101011000" });
            this.C128_Code.Rows.Add(new object[] { "55", "W", "W", "55", "11101000110" });
            this.C128_Code.Rows.Add(new object[] { "56", "X", "X", "56", "11100010110" });
            this.C128_Code.Rows.Add(new object[] { "57", "Y", "Y", "57", "11101101000" });
            this.C128_Code.Rows.Add(new object[] { "58", "Z", "Z", "58", "11101100010" });
            this.C128_Code.Rows.Add(new object[] { "59", "[", "[", "59", "11100011010" });
            this.C128_Code.Rows.Add(new object[] { "60", @"\", @"\", "60", "11101111010" });
            this.C128_Code.Rows.Add(new object[] { "61", "]", "]", "61", "11001000010" });
            this.C128_Code.Rows.Add(new object[] { "62", "^", "^", "62", "11110001010" });
            this.C128_Code.Rows.Add(new object[] { "63", "_", "_", "63", "10100110000" });
            this.C128_Code.Rows.Add(new object[] { "64", "\0", "`", "64", "10100001100" });
            this.C128_Code.Rows.Add(new object[] { "65", Convert.ToChar(1).ToString(), "a", "65", "10010110000" });
            this.C128_Code.Rows.Add(new object[] { "66", Convert.ToChar(2).ToString(), "b", "66", "10010000110" });
            this.C128_Code.Rows.Add(new object[] { "67", Convert.ToChar(3).ToString(), "c", "67", "10000101100" });
            this.C128_Code.Rows.Add(new object[] { "68", Convert.ToChar(4).ToString(), "d", "68", "10000100110" });
            this.C128_Code.Rows.Add(new object[] { "69", Convert.ToChar(5).ToString(), "e", "69", "10110010000" });
            this.C128_Code.Rows.Add(new object[] { "70", Convert.ToChar(6).ToString(), "f", "70", "10110000100" });
            this.C128_Code.Rows.Add(new object[] { "71", Convert.ToChar(7).ToString(), "g", "71", "10011010000" });
            this.C128_Code.Rows.Add(new object[] { "72", Convert.ToChar(8).ToString(), "h", "72", "10011000010" });
            this.C128_Code.Rows.Add(new object[] { "73", Convert.ToChar(9).ToString(), "i", "73", "10000110100" });
            this.C128_Code.Rows.Add(new object[] { "74", Convert.ToChar(10).ToString(), "j", "74", "10000110010" });
            this.C128_Code.Rows.Add(new object[] { "75", Convert.ToChar(11).ToString(), "k", "75", "11000010010" });
            this.C128_Code.Rows.Add(new object[] { "76", Convert.ToChar(12).ToString(), "l", "76", "11001010000" });
            this.C128_Code.Rows.Add(new object[] { "77", Convert.ToChar(13).ToString(), "m", "77", "11110111010" });
            this.C128_Code.Rows.Add(new object[] { "78", Convert.ToChar(14).ToString(), "n", "78", "11000010100" });
            this.C128_Code.Rows.Add(new object[] { "79", Convert.ToChar(15).ToString(), "o", "79", "10001111010" });
            this.C128_Code.Rows.Add(new object[] { "80", Convert.ToChar(16).ToString(), "p", "80", "10100111100" });
            this.C128_Code.Rows.Add(new object[] { "81", Convert.ToChar(17).ToString(), "q", "81", "10010111100" });
            this.C128_Code.Rows.Add(new object[] { "82", Convert.ToChar(18).ToString(), "r", "82", "10010011110" });
            this.C128_Code.Rows.Add(new object[] { "83", Convert.ToChar(19).ToString(), "s", "83", "10111100100" });
            this.C128_Code.Rows.Add(new object[] { "84", Convert.ToChar(20).ToString(), "t", "84", "10011110100" });
            this.C128_Code.Rows.Add(new object[] { "85", Convert.ToChar(21).ToString(), "u", "85", "10011110010" });
            this.C128_Code.Rows.Add(new object[] { "86", Convert.ToChar(22).ToString(), "v", "86", "11110100100" });
            this.C128_Code.Rows.Add(new object[] { "87", Convert.ToChar(23).ToString(), "w", "87", "11110010100" });
            this.C128_Code.Rows.Add(new object[] { "88", Convert.ToChar(24).ToString(), "x", "88", "11110010010" });
            this.C128_Code.Rows.Add(new object[] { "89", Convert.ToChar(25).ToString(), "y", "89", "11011011110" });
            this.C128_Code.Rows.Add(new object[] { "90", Convert.ToChar(26).ToString(), "z", "90", "11011110110" });
            this.C128_Code.Rows.Add(new object[] { "91", Convert.ToChar(27).ToString(), "{", "91", "11110110110" });
            this.C128_Code.Rows.Add(new object[] { "92", Convert.ToChar(28).ToString(), "|", "92", "10101111000" });
            this.C128_Code.Rows.Add(new object[] { "93", Convert.ToChar(29).ToString(), "}", "93", "10100011110" });
            this.C128_Code.Rows.Add(new object[] { "94", Convert.ToChar(30).ToString(), "~", "94", "10001011110" });

            this.C128_Code.Rows.Add(new object[] { "95", Convert.ToChar(31).ToString(), Convert.ToChar(127).ToString(), "95", "10111101000" });
            this.C128_Code.Rows.Add(new object[] { "96", FNC3, FNC3, "96", "10111100010" });
            this.C128_Code.Rows.Add(new object[] { "97", FNC2, FNC2, "97", "11110101000" });
            this.C128_Code.Rows.Add(new object[] { "98", "SHIFT", "SHIFT", "98", "11110100010" });
            this.C128_Code.Rows.Add(new object[] { "99", "CODE_C", "CODE_C", "99", "10111011110" });
            this.C128_Code.Rows.Add(new object[] { "100", "CODE_B", FNC4, "CODE_B", "10111101110" });
            this.C128_Code.Rows.Add(new object[] { "101", FNC4, "CODE_A", "CODE_A", "11101011110" });
            this.C128_Code.Rows.Add(new object[] { "102", FNC1, FNC1, FNC1, "11110101110" });
            this.C128_Code.Rows.Add(new object[] { "103", "START_A", "START_A", "START_A", "11010000100" });
            this.C128_Code.Rows.Add(new object[] { "104", "START_B", "START_B", "START_B", "11010010000" });
            this.C128_Code.Rows.Add(new object[] { "105", "START_C", "START_C", "START_C", "11010011100" });
            this.C128_Code.Rows.Add(new object[] { "", "STOP", "STOP", "STOP", "11000111010" });
        }//init_Code128
        private List<DataRow> FindStartorCodeCharacter(string s, ref int col)
        {
            List<DataRow> rows = new List<DataRow>();

            //if two chars are numbers (or FNC1) then START_C or CODE_C
            if (s.Length > 1 && (Char.IsNumber(s[0]) || s[0] == FNC1) && (Char.IsNumber(s[1]) || s[1] == FNC1))
            {
                if (StartCharacter == null)
                {
                    StartCharacter = this.C128_Code.Select("A = 'START_C'")[0];
                    rows.Add(StartCharacter);
                }//if
                else
                    rows.Add(this.C128_Code.Select("A = 'CODE_C'")[0]);

                col = 1;
            }//if
            else
            {
                bool AFound = false;
                bool BFound = false;
                foreach (DataRow row in this.C128_Code.Rows)
                {
                    try
                    {
                        if (!AFound && s == row["A"].ToString())
                        {
                            AFound = true;
                            col = 2;

                            if (StartCharacter == null)
                            {
                                StartCharacter = this.C128_Code.Select("A = 'START_A'")[0];
                                rows.Add(StartCharacter);
                            }//if
                            else
                            {
                                rows.Add(this.C128_Code.Select("B = 'CODE_A'")[0]);//first column is FNC4 so use B
                            }//else
                        }//if
                        else if (!BFound && s == row["B"].ToString())
                        {
                            BFound = true;
                            col = 1;

                            if (StartCharacter == null)
                            {
                                StartCharacter = this.C128_Code.Select("A = 'START_B'")[0];
                                rows.Add(StartCharacter);
                            }//if
                            else
                                rows.Add(this.C128_Code.Select("A = 'CODE_B'")[0]);
                        }//else
                        else if (AFound && BFound)
                            break;
                    }//try
                    catch (Exception ex)
                    {
                        Error("EC128-1: " + ex.Message);
                    }//catch
                }//foreach                

                if (rows.Count <= 0)
                    Error("EC128-2: Could not determine start character.");
            }//else

            return rows;
        }
        private string CalculateCheckDigit()
        {
            string currentStartChar = _FormattedData[0];
            uint CheckSum = 0;

            for (uint i = 0; i < _FormattedData.Count; i++)
            {
                //replace apostrophes with double apostrophes for escape chars
                string s = _FormattedData[(int)i].Replace("'", "''");

                //try to find value in the A column
                DataRow[] rows = this.C128_Code.Select("A = '" + s + "'");

                //try to find value in the B column
                if (rows.Length <= 0)
                    rows = this.C128_Code.Select("B = '" + s + "'");

                //try to find value in the C column
                if (rows.Length <= 0)
                    rows = this.C128_Code.Select("C = '" + s + "'");

                uint value = UInt32.Parse(rows[0]["Value"].ToString());
                uint addition = value * ((i == 0) ? 1 : i);
                CheckSum += addition;
            }//for

            uint Remainder = (CheckSum % 103);
            DataRow[] RetRows = this.C128_Code.Select("Value = '" + Remainder.ToString() + "'");
            return RetRows[0]["Encoding"].ToString();
        }
        private void BreakUpDataForEncoding()
        {
            string temp = "";
            string tempRawData = Raw_Data;

            //breaking the raw data up for code A and code B will mess up the encoding
            switch (this.type)
            {
                case TYPES.A:
                case TYPES.B:
                    {
                        foreach (char c in Raw_Data)
                            _FormattedData.Add(c.ToString());
                        return;
                    }

                case TYPES.C:
                    {
                        int indexOfFirstNumeric = -1;
                        int numericCount = 0;
                        for (int x = 0; x < RawData.Length; x++)
                        {
                            Char c = RawData[x];
                            if (Char.IsNumber(c))
                            {
                                numericCount++;
                                if (indexOfFirstNumeric == -1)
                                {
                                    indexOfFirstNumeric = x;
                                }
                            }
                            else if (c != FNC1)
                            {
                                Error("EC128-6: Only numeric values can be encoded with C128-C (Invalid char at position " + x + ").");
                            }
                        }

                        //CODE C: adds a 0 to the front of the Raw_Data if the length is not divisible by 2
                        if (numericCount % 2 == 1)
                            tempRawData = tempRawData.Insert(indexOfFirstNumeric, "0");
                        break;
                    }
            }

            foreach (char c in tempRawData)
            {
                if (Char.IsNumber(c))
                {
                    if (temp == "")
                    {
                        temp += c;
                    }//if
                    else
                    {
                        temp += c;
                        _FormattedData.Add(temp);
                        temp = "";
                    }//else
                }//if
                else
                {
                    if (temp != "")
                    {
                        _FormattedData.Add(temp);
                        temp = "";
                    }//if
                    _FormattedData.Add(c.ToString());
                }//else
            }//foreach

            //if something is still in temp go ahead and push it onto the queue
            if (temp != "")
            {
                _FormattedData.Add(temp);
                temp = "";
            }//if
        }
        private void InsertStartandCodeCharacters()
        {
            DataRow CurrentCodeSet = null;
            string CurrentCodeString = "";

            if (this.type != TYPES.DYNAMIC)
            {
                switch (this.type)
                {
                    case TYPES.A:
                        _FormattedData.Insert(0, "START_A");
                        break;
                    case TYPES.B:
                        _FormattedData.Insert(0, "START_B");
                        break;
                    case TYPES.C:
                        _FormattedData.Insert(0, "START_C");
                        break;
                    default:
                        Error("EC128-4: Unknown start type in fixed type encoding.");
                        break;
                }
            }//if
            else
            {
                try
                {
                    for (int i = 0; i < (_FormattedData.Count); i++)
                    {
                        int col = 0;
                        List<DataRow> tempStartChars = FindStartorCodeCharacter(_FormattedData[i], ref col);

                        //check all the start characters and see if we need to stay with the same codeset or if a change of sets is required
                        bool sameCodeSet = false;
                        foreach (DataRow row in tempStartChars)
                        {
                            if (row["A"].ToString().EndsWith(CurrentCodeString) || row["B"].ToString().EndsWith(CurrentCodeString) || row["C"].ToString().EndsWith(CurrentCodeString))
                            {
                                sameCodeSet = true;
                                break;
                            }//if
                        }//foreach

                        //only insert a new code char if starting a new codeset
                        //if (CurrentCodeString == "" || !tempStartChars[0][col].ToString().EndsWith(CurrentCodeString)) /* Removed because of bug */

                        if (CurrentCodeString == "" || !sameCodeSet)
                        {
                            CurrentCodeSet = tempStartChars[0];

                            bool error = true;
                            while (error)
                            {
                                try
                                {
                                    CurrentCodeString = CurrentCodeSet[col].ToString().Split(new char[] { '_' })[1];
                                    error = false;
                                }//try
                                catch
                                {
                                    error = true;

                                    if (col++ > CurrentCodeSet.ItemArray.Length)
                                        Error("No start character found in CurrentCodeSet.");
                                }//catch
                            }//while

                            _FormattedData.Insert(i++, CurrentCodeSet[col].ToString());
                        }//if

                    }//for
                }//try
                catch (Exception ex)
                {
                    Error("EC128-3: Could not insert start and code characters.\n Message: " + ex.Message);
                }//catch
            }//else
        }
        private string GetEncoding()
        {
            //break up data for encoding
            BreakUpDataForEncoding();

            //insert the start characters
            InsertStartandCodeCharacters();

            string CheckDigit = CalculateCheckDigit();

            string Encoded_Data = "";
            foreach (string s in _FormattedData)
            {
                //handle exception with apostrophes in select statements
                string s1 = s.Replace("'", "''");
                DataRow[] E_Row;

                //select encoding only for type selected
                switch (this.type)
                {
                    case TYPES.A:
                        E_Row = this.C128_Code.Select("A = '" + s1 + "'");
                        break;
                    case TYPES.B:
                        E_Row = this.C128_Code.Select("B = '" + s1 + "'");
                        break;
                    case TYPES.C:
                        E_Row = this.C128_Code.Select("C = '" + s1 + "'");
                        break;
                    case TYPES.DYNAMIC:
                        E_Row = this.C128_Code.Select("A = '" + s1 + "'");

                        if (E_Row.Length <= 0)
                        {
                            E_Row = this.C128_Code.Select("B = '" + s1 + "'");

                            if (E_Row.Length <= 0)
                            {
                                E_Row = this.C128_Code.Select("C = '" + s1 + "'");
                            }//if
                        }//if
                        break;
                    default:
                        E_Row = null;
                        break;
                }//switch              

                if (E_Row == null || E_Row.Length <= 0)
                    Error("EC128-5: Could not find encoding of a value( " + s1 + " ) in C128 type " + this.type.ToString());

                Encoded_Data += E_Row[0]["Encoding"].ToString();
                _EncodedData.Add(E_Row[0]["Encoding"].ToString());
            }//foreach

            //add the check digit
            Encoded_Data += CalculateCheckDigit();
            _EncodedData.Add(CalculateCheckDigit());

            //add the stop character
            Encoded_Data += this.C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString();
            _EncodedData.Add(this.C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString());

            //add the termination bars
            Encoded_Data += "11";
            _EncodedData.Add("11");

            return Encoded_Data;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code128(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  ITF-14 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class ITF14 : BarcodeCommon, IBarcode
    {
        private string[] ITF14_Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };

        public ITF14(string input)
        {
            Raw_Data = input;

            CheckDigit();
        }
        /// <summary>
        /// Encode the raw data using the ITF-14 algorithm.
        /// </summary>
        private string Encode_ITF14()
        {
            //check length of input
            if (Raw_Data.Length > 14 || Raw_Data.Length < 13)
                Error("EITF14-1: Data length invalid. (Length must be 13 or 14)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EITF14-2: Numeric data only.");

            string result = "1010";

            for (int i = 0; i < Raw_Data.Length; i += 2)
            {
                bool bars = true;
                string patternbars = ITF14_Code[Int32.Parse(Raw_Data[i].ToString())];
                string patternspaces = ITF14_Code[Int32.Parse(Raw_Data[i + 1].ToString())];
                string patternmixed = "";

                //interleave
                while (patternbars.Length > 0)
                {
                    patternmixed += patternbars[0].ToString() + patternspaces[0].ToString();
                    patternbars = patternbars.Substring(1);
                    patternspaces = patternspaces.Substring(1);
                }//while

                foreach (char c1 in patternmixed)
                {
                    if (bars)
                    {
                        if (c1 == 'N')
                            result += "1";
                        else
                            result += "11";
                    }//if
                    else
                    {
                        if (c1 == 'N')
                            result += "0";
                        else
                            result += "00";
                    }//else
                    bars = !bars;
                }//foreach
            }//foreach

            //add ending bars
            result += "1101";
            return result;
        }//Encode_ITF14
        private void CheckDigit()
        {
            //calculate and include checksum if it is necessary
            if (Raw_Data.Length == 13)
            {
                int total = 0;

                for (int i = 0; i <= Raw_Data.Length - 1; i++)
                {
                    int temp = Int32.Parse(Raw_Data.Substring(i, 1));
                    total += temp * ((i == 0 || i % 2 == 0) ? 3 : 1);
                }//for

                int cs = total % 10;
                cs = 10 - cs;
                if (cs == 10)
                    cs = 0;

                this.Raw_Data += cs.ToString();
            }//if
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return this.Encode_ITF14(); }
        }

        #endregion
    }

    /// <summary>
    ///  Code 93 encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Code93 : BarcodeCommon, IBarcode
    {
        private System.Data.DataTable C93_Code = new System.Data.DataTable("C93_Code");

        /// <summary>
        /// Encodes with Code93.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code93(string input)
        {
            Raw_Data = input;
        }//Code93

        /// <summary>
        /// Encode the raw data using the Code 93 algorithm.
        /// </summary>
        private string Encode_Code93()
        {
            this.init_Code93();

            string FormattedData = Add_CheckDigits(Raw_Data);

            string result = C93_Code.Select("Character = '*'")[0]["Encoding"].ToString();
            foreach (char c in FormattedData)
            {
                try
                {
                    result += C93_Code.Select("Character = '" + c.ToString() + "'")[0]["Encoding"].ToString();
                }//try
                catch
                {
                    Error("EC93-1: Invalid data.");
                }//catch
            }//foreach

            result += C93_Code.Select("Character = '*'")[0]["Encoding"].ToString();

            //termination bar
            result += "1";

            //clear the hashtable so it no longer takes up memory
            this.C93_Code.Clear();

            return result;
        }//Encode_Code93
        private void init_Code93()
        {
            C93_Code.Rows.Clear();
            C93_Code.Columns.Clear();
            C93_Code.Columns.Add("Value");
            C93_Code.Columns.Add("Character");
            C93_Code.Columns.Add("Encoding");
            C93_Code.Rows.Add(new object[] { "0", "0", "100010100" });
            C93_Code.Rows.Add(new object[] { "1", "1", "101001000" });
            C93_Code.Rows.Add(new object[] { "2", "2", "101000100" });
            C93_Code.Rows.Add(new object[] { "3", "3", "101000010" });
            C93_Code.Rows.Add(new object[] { "4", "4", "100101000" });
            C93_Code.Rows.Add(new object[] { "5", "5", "100100100" });
            C93_Code.Rows.Add(new object[] { "6", "6", "100100010" });
            C93_Code.Rows.Add(new object[] { "7", "7", "101010000" });
            C93_Code.Rows.Add(new object[] { "8", "8", "100010010" });
            C93_Code.Rows.Add(new object[] { "9", "9", "100001010" });
            C93_Code.Rows.Add(new object[] { "10", "A", "110101000" });
            C93_Code.Rows.Add(new object[] { "11", "B", "110100100" });
            C93_Code.Rows.Add(new object[] { "12", "C", "110100010" });
            C93_Code.Rows.Add(new object[] { "13", "D", "110010100" });
            C93_Code.Rows.Add(new object[] { "14", "E", "110010010" });
            C93_Code.Rows.Add(new object[] { "15", "F", "110001010" });
            C93_Code.Rows.Add(new object[] { "16", "G", "101101000" });
            C93_Code.Rows.Add(new object[] { "17", "H", "101100100" });
            C93_Code.Rows.Add(new object[] { "18", "I", "101100010" });
            C93_Code.Rows.Add(new object[] { "19", "J", "100110100" });
            C93_Code.Rows.Add(new object[] { "20", "K", "100011010" });
            C93_Code.Rows.Add(new object[] { "21", "L", "101011000" });
            C93_Code.Rows.Add(new object[] { "22", "M", "101001100" });
            C93_Code.Rows.Add(new object[] { "23", "N", "101000110" });
            C93_Code.Rows.Add(new object[] { "24", "O", "100101100" });
            C93_Code.Rows.Add(new object[] { "25", "P", "100010110" });
            C93_Code.Rows.Add(new object[] { "26", "Q", "110110100" });
            C93_Code.Rows.Add(new object[] { "27", "R", "110110010" });
            C93_Code.Rows.Add(new object[] { "28", "S", "110101100" });
            C93_Code.Rows.Add(new object[] { "29", "T", "110100110" });
            C93_Code.Rows.Add(new object[] { "30", "U", "110010110" });
            C93_Code.Rows.Add(new object[] { "31", "V", "110011010" });
            C93_Code.Rows.Add(new object[] { "32", "W", "101101100" });
            C93_Code.Rows.Add(new object[] { "33", "X", "101100110" });
            C93_Code.Rows.Add(new object[] { "34", "Y", "100110110" });
            C93_Code.Rows.Add(new object[] { "35", "Z", "100111010" });
            C93_Code.Rows.Add(new object[] { "36", "-", "100101110" });
            C93_Code.Rows.Add(new object[] { "37", ".", "111010100" });
            C93_Code.Rows.Add(new object[] { "38", " ", "111010010" });
            C93_Code.Rows.Add(new object[] { "39", "$", "111001010" });
            C93_Code.Rows.Add(new object[] { "40", "/", "101101110" });
            C93_Code.Rows.Add(new object[] { "41", "+", "101110110" });
            C93_Code.Rows.Add(new object[] { "42", "%", "110101110" });
            C93_Code.Rows.Add(new object[] { "43", "(", "100100110" });//dont know what character actually goes here
            C93_Code.Rows.Add(new object[] { "44", ")", "111011010" });//dont know what character actually goes here
            C93_Code.Rows.Add(new object[] { "45", "#", "111010110" });//dont know what character actually goes here
            C93_Code.Rows.Add(new object[] { "46", "@", "100110010" });//dont know what character actually goes here
            C93_Code.Rows.Add(new object[] { "-", "*", "101011110" });
        }//init_Code93
        private string Add_CheckDigits(string input)
        {
            //populate the C weights
            int[] aryCWeights = new int[input.Length];
            int curweight = 1;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (curweight > 20)
                    curweight = 1;
                aryCWeights[i] = curweight;
                curweight++;
            }//for

            //populate the K weights
            int[] aryKWeights = new int[input.Length + 1];
            curweight = 1;
            for (int i = input.Length; i >= 0; i--)
            {
                if (curweight > 15)
                    curweight = 1;
                aryKWeights[i] = curweight;
                curweight++;
            }//for

            //calculate C checksum
            int SUM = 0;
            for (int i = 0; i < input.Length; i++)
            {
                SUM += aryCWeights[i] * Int32.Parse(C93_Code.Select("Character = '" + input[i].ToString() + "'")[0]["Value"].ToString());
            }//for
            int ChecksumValue = SUM % 47;

            input += C93_Code.Select("Value = '" + ChecksumValue.ToString() + "'")[0]["Character"].ToString();

            //calculate K checksum
            SUM = 0;
            for (int i = 0; i < input.Length; i++)
            {
                SUM += aryKWeights[i] * Int32.Parse(C93_Code.Select("Character = '" + input[i].ToString() + "'")[0]["Value"].ToString());
            }//for
            ChecksumValue = SUM % 47;

            input += C93_Code.Select("Value = '" + ChecksumValue.ToString() + "'")[0]["Character"].ToString();

            return input;
        }//Calculate_CheckDigits

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code93(); }
        }

        #endregion
    }//class

    /// <summary>
    ///  Telepen encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Telepen : BarcodeCommon, IBarcode
    {
        private static Hashtable Telepen_Code = new Hashtable();
        private enum StartStopCode : int { START1, STOP1, START2, STOP2, START3, STOP3 };
        private StartStopCode StartCode = StartStopCode.START1;
        private StartStopCode StopCode = StartStopCode.STOP1;
        private int SwitchModeIndex = 0;
        private int iCheckSum = 0;

        /// <summary>
        /// Encodes data using the Telepen algorithm.
        /// </summary>
        /// <param name="input"></param>
        public Telepen(string input)
        {
            Raw_Data = input;
        }

        /// <summary>
        /// Encode the raw data using the Telepen algorithm.
        /// </summary>
        private string Encode_Telepen()
        {
            //only init if needed
            if (Telepen_Code.Count == 0)
                Init_Telepen();

            iCheckSum = 0;
            string result = "";

            SetEncodingSequence();

            //include the Start sequence pattern
            result = Telepen_Code[StartCode].ToString();

            switch (StartCode)
            {
                //numeric --> ascii
                case StartStopCode.START2:
                    EncodeNumeric(RawData.Substring(0, SwitchModeIndex), ref result);

                    if (SwitchModeIndex < RawData.Length)
                    {
                        EncodeSwitchMode(ref result);
                        EncodeASCII(RawData.Substring(SwitchModeIndex), ref result);
                    }//if
                    break;
                //ascii --> numeric
                case StartStopCode.START3:
                    EncodeASCII(RawData.Substring(0, SwitchModeIndex), ref result);
                    EncodeSwitchMode(ref result);
                    EncodeNumeric(RawData.Substring(SwitchModeIndex), ref result);
                    break;
                //full ascii
                default:
                    EncodeASCII(RawData, ref result);
                    break;
            }//switch

            //checksum
            result += Telepen_Code[Calculate_Checksum(iCheckSum)];

            //stop character
            result += Telepen_Code[StopCode];

            return result;
        }//Encode_Telepen

        private void EncodeASCII(string input, ref string output)
        {
            try
            {
                foreach (char c in input)
                {
                    output += Telepen_Code[c];
                    iCheckSum += Convert.ToInt32(c);
                }//foreach
            }//try
            catch
            {
                Error("ETELEPEN-1: Invalid data when encoding ASCII");
            }//catch
        }
        private void EncodeNumeric(string input, ref string output)
        {
            try
            {
                if ((input.Length % 2) > 0)
                    Error("ETELEPEN-3: Numeric encoding attempted on odd number of characters");

                for (int i = 0; i < input.Length; i += 2)
                {
                    output += Telepen_Code[Convert.ToChar(Int32.Parse(input.Substring(i, 2)) + 27)];
                    iCheckSum += Int32.Parse(input.Substring(i, 2)) + 27;
                }//for
            }//try
            catch
            {
                Error("ETELEPEN-2: Numeric encoding failed");
            }//catch
        }
        private void EncodeSwitchMode(ref string output)
        {
            //ASCII code DLE is used to switch modes
            iCheckSum += 16;
            output += Telepen_Code[Convert.ToChar(16)];
        }

        private char Calculate_Checksum(int iCheckSum)
        {
            return Convert.ToChar(127 - (iCheckSum % 127));
        }//Calculate_Checksum(string)

        private void SetEncodingSequence()
        {
            //reset to full ascii
            StartCode = StartStopCode.START1;
            StopCode = StartStopCode.STOP1;
            SwitchModeIndex = Raw_Data.Length;

            //starting number of 'numbers'
            int StartNumerics = 0;
            foreach (char c in Raw_Data)
            {
                if (Char.IsNumber(c))
                    StartNumerics++;
                else
                    break;
            }//foreach

            if (StartNumerics == Raw_Data.Length)
            {
                //Numeric only mode due to only numbers being present
                StartCode = StartStopCode.START2;
                StopCode = StartStopCode.STOP2;

                if ((Raw_Data.Length % 2) > 0)
                    SwitchModeIndex = RawData.Length - 1;
            }//if
            else
            {
                //ending number of numbers
                int EndNumerics = 0;
                for (int i = Raw_Data.Length - 1; i >= 0; i--)
                {
                    if (Char.IsNumber(Raw_Data[i]))
                        EndNumerics++;
                    else
                        break;
                }//for

                if (StartNumerics >= 4 || EndNumerics >= 4)
                {
                    //hybrid mode will be used
                    if (StartNumerics > EndNumerics)
                    {
                        //start in numeric switching to ascii
                        StartCode = StartStopCode.START2;
                        StopCode = StartStopCode.STOP2;
                        SwitchModeIndex = (StartNumerics % 2) == 1 ? StartNumerics - 1 : StartNumerics;
                    }//if
                    else
                    {
                        //start in ascii switching to numeric
                        StartCode = StartStopCode.START3;
                        StopCode = StartStopCode.STOP3;
                        SwitchModeIndex = (EndNumerics % 2) == 1 ? Raw_Data.Length - EndNumerics + 1 : Raw_Data.Length - EndNumerics;
                    }//else
                }//if
            }//else
        }//SetEncodingSequence

        private void Init_Telepen()
        {
            Telepen_Code.Add(Convert.ToChar(0), "1110111011101110");
            Telepen_Code.Add(Convert.ToChar(1), "1011101110111010");
            Telepen_Code.Add(Convert.ToChar(2), "1110001110111010");
            Telepen_Code.Add(Convert.ToChar(3), "1010111011101110");
            Telepen_Code.Add(Convert.ToChar(4), "1110101110111010");
            Telepen_Code.Add(Convert.ToChar(5), "1011100011101110");
            Telepen_Code.Add(Convert.ToChar(6), "1000100011101110");
            Telepen_Code.Add(Convert.ToChar(7), "1010101110111010");
            Telepen_Code.Add(Convert.ToChar(8), "1110111000111010");
            Telepen_Code.Add(Convert.ToChar(9), "1011101011101110");
            Telepen_Code.Add(Convert.ToChar(10), "1110001011101110");
            Telepen_Code.Add(Convert.ToChar(11), "1010111000111010");
            Telepen_Code.Add(Convert.ToChar(12), "1110101011101110");
            Telepen_Code.Add(Convert.ToChar(13), "1010001000111010");
            Telepen_Code.Add(Convert.ToChar(14), "1000101000111010");
            Telepen_Code.Add(Convert.ToChar(15), "1010101011101110");
            Telepen_Code.Add(Convert.ToChar(16), "1110111010111010");
            Telepen_Code.Add(Convert.ToChar(17), "1011101110001110");
            Telepen_Code.Add(Convert.ToChar(18), "1110001110001110");
            Telepen_Code.Add(Convert.ToChar(19), "1010111010111010");
            Telepen_Code.Add(Convert.ToChar(20), "1110101110001110");
            Telepen_Code.Add(Convert.ToChar(21), "1011100010111010");
            Telepen_Code.Add(Convert.ToChar(22), "1000100010111010");
            Telepen_Code.Add(Convert.ToChar(23), "1010101110001110");
            Telepen_Code.Add(Convert.ToChar(24), "1110100010001110");
            Telepen_Code.Add(Convert.ToChar(25), "1011101010111010");
            Telepen_Code.Add(Convert.ToChar(26), "1110001010111010");
            Telepen_Code.Add(Convert.ToChar(27), "1010100010001110");
            Telepen_Code.Add(Convert.ToChar(28), "1110101010111010");
            Telepen_Code.Add(Convert.ToChar(29), "1010001010001110");
            Telepen_Code.Add(Convert.ToChar(30), "1000101010001110");
            Telepen_Code.Add(Convert.ToChar(31), "1010101010111010");
            Telepen_Code.Add(' ', "1110111011100010");
            Telepen_Code.Add('!', "1011101110101110");
            Telepen_Code.Add('"', "1110001110101110");
            Telepen_Code.Add('#', "1010111011100010");
            Telepen_Code.Add('$', "1110101110101110");
            Telepen_Code.Add('%', "1011100011100010");
            Telepen_Code.Add('&', "1000100011100010");
            Telepen_Code.Add('\'', "1010101110101110");
            Telepen_Code.Add('(', "1110111000101110");
            Telepen_Code.Add(')', "1011101011100010");
            Telepen_Code.Add('*', "1110001011100010");
            Telepen_Code.Add('+', "1010111000101110");
            Telepen_Code.Add(',', "1110101011100010");
            Telepen_Code.Add('-', "1010001000101110");
            Telepen_Code.Add('.', "1000101000101110");
            Telepen_Code.Add('/', "1010101011100010");
            Telepen_Code.Add('0', "1110111010101110");
            Telepen_Code.Add('1', "1011101000100010");
            Telepen_Code.Add('2', "1110001000100010");
            Telepen_Code.Add('3', "1010111010101110");
            Telepen_Code.Add('4', "1110101000100010");
            Telepen_Code.Add('5', "1011100010101110");
            Telepen_Code.Add('6', "1000100010101110");
            Telepen_Code.Add('7', "1010101000100010");
            Telepen_Code.Add('8', "1110100010100010");
            Telepen_Code.Add('9', "1011101010101110");
            Telepen_Code.Add(':', "1110001010101110");
            Telepen_Code.Add(';', "1010100010100010");
            Telepen_Code.Add('<', "1110101010101110");
            Telepen_Code.Add('=', "1010001010100010");
            Telepen_Code.Add('>', "1000101010100010");
            Telepen_Code.Add('?', "1010101010101110");
            Telepen_Code.Add('@', "1110111011101010");
            Telepen_Code.Add('A', "1011101110111000");
            Telepen_Code.Add('B', "1110001110111000");
            Telepen_Code.Add('C', "1010111011101010");
            Telepen_Code.Add('D', "1110101110111000");
            Telepen_Code.Add('E', "1011100011101010");
            Telepen_Code.Add('F', "1000100011101010");
            Telepen_Code.Add('G', "1010101110111000");
            Telepen_Code.Add('H', "1110111000111000");
            Telepen_Code.Add('I', "1011101011101010");
            Telepen_Code.Add('J', "1110001011101010");
            Telepen_Code.Add('K', "1010111000111000");
            Telepen_Code.Add('L', "1110101011101010");
            Telepen_Code.Add('M', "1010001000111000");
            Telepen_Code.Add('N', "1000101000111000");
            Telepen_Code.Add('O', "1010101011101010");
            Telepen_Code.Add('P', "1110111010111000");
            Telepen_Code.Add('Q', "1011101110001010");
            Telepen_Code.Add('R', "1110001110001010");
            Telepen_Code.Add('S', "1010111010111000");
            Telepen_Code.Add('T', "1110101110001010");
            Telepen_Code.Add('U', "1011100010111000");
            Telepen_Code.Add('V', "1000100010111000");
            Telepen_Code.Add('W', "1010101110001010");
            Telepen_Code.Add('X', "1110100010001010");
            Telepen_Code.Add('Y', "1011101010111000");
            Telepen_Code.Add('Z', "1110001010111000");
            Telepen_Code.Add('[', "1010100010001010");
            Telepen_Code.Add('\\', "1110101010111000");
            Telepen_Code.Add(']', "1010001010001010");
            Telepen_Code.Add('^', "1000101010001010");
            Telepen_Code.Add('_', "1010101010111000");
            Telepen_Code.Add('`', "1110111010001000");
            Telepen_Code.Add('a', "1011101110101010");
            Telepen_Code.Add('b', "1110001110101010");
            Telepen_Code.Add('c', "1010111010001000");
            Telepen_Code.Add('d', "1110101110101010");
            Telepen_Code.Add('e', "1011100010001000");
            Telepen_Code.Add('f', "1000100010001000");
            Telepen_Code.Add('g', "1010101110101010");
            Telepen_Code.Add('h', "1110111000101010");
            Telepen_Code.Add('i', "1011101010001000");
            Telepen_Code.Add('j', "1110001010001000");
            Telepen_Code.Add('k', "1010111000101010");
            Telepen_Code.Add('l', "1110101010001000");
            Telepen_Code.Add('m', "1010001000101010");
            Telepen_Code.Add('n', "1000101000101010");
            Telepen_Code.Add('o', "1010101010001000");
            Telepen_Code.Add('p', "1110111010101010");
            Telepen_Code.Add('q', "1011101000101000");
            Telepen_Code.Add('r', "1110001000101000");
            Telepen_Code.Add('s', "1010111010101010");
            Telepen_Code.Add('t', "1110101000101000");
            Telepen_Code.Add('u', "1011100010101010");
            Telepen_Code.Add('v', "1000100010101010");
            Telepen_Code.Add('w', "1010101000101000");
            Telepen_Code.Add('x', "1110100010101000");
            Telepen_Code.Add('y', "1011101010101010");
            Telepen_Code.Add('z', "1110001010101010");
            Telepen_Code.Add('{', "1010100010101000");
            Telepen_Code.Add('|', "1110101010101010");
            Telepen_Code.Add('}', "1010001010101000");
            Telepen_Code.Add('~', "1000101010101000");
            Telepen_Code.Add(Convert.ToChar(127), "1010101010101010");
            Telepen_Code.Add(StartStopCode.START1, "1010101010111000");
            Telepen_Code.Add(StartStopCode.STOP1, "1110001010101010");
            Telepen_Code.Add(StartStopCode.START2, "1010101011101000");
            Telepen_Code.Add(StartStopCode.STOP2, "1110100010101010");
            Telepen_Code.Add(StartStopCode.START3, "1010101110101000");
            Telepen_Code.Add(StartStopCode.STOP3, "1110101000101010");
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Telepen(); }
        }

        #endregion
    }

    /// <summary>
    ///  FIM encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class FIM : BarcodeCommon, IBarcode
    {
        private string[] FIM_Codes = { "110010011", "101101101", "110101011", "111010111" };
        public enum FIMTypes { FIM_A = 0, FIM_B, FIM_C, FIM_D };

        public FIM(string input)
        {
            input = input.Trim();

            switch (input)
            {
                case "A":
                case "a":
                    Raw_Data = FIM_Codes[(int)FIMTypes.FIM_A];
                    break;
                case "B":
                case "b":
                    Raw_Data = FIM_Codes[(int)FIMTypes.FIM_B];
                    break;
                case "C":
                case "c":
                    Raw_Data = FIM_Codes[(int)FIMTypes.FIM_C];
                    break;
                case "D":
                case "d":
                    Raw_Data = FIM_Codes[(int)FIMTypes.FIM_D];
                    break;
                default:
                    Error("EFIM-1: Could not determine encoding type. (Only pass in A, B, C, or D)");
                    break;
            }//switch
        }

        public string Encode_FIM()
        {
            string Encoded = "";
            foreach (char c in RawData)
            {
                Encoded += c + "0";
            }//foreach

            Encoded = Encoded.Substring(0, Encoded.Length - 1);

            return Encoded;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_FIM(); }
        }

        #endregion
    }

    /// <summary>
    ///  Pharmacode encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Pharmacode : BarcodeCommon, IBarcode
    {
        string _thinBar = "1";
        string _gap = "00";
        string _thickBar = "111";

        /// <summary>
        /// Encodes with Pharmacode.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Pharmacode(string input)
        {
            Raw_Data = input;

            if (!CheckNumericOnly(Raw_Data))
            {
                Error("EPHARM-1: Data contains invalid  characters (non-numeric).");
            }//if
            else if (Raw_Data.Length > 6)
            {
                Error("EPHARM-2: Data too long (invalid data input length).");
            }//if
        }

        /// <summary>
        /// Encode the raw data using the Pharmacode algorithm.
        /// </summary>
        private string Encode_Pharmacode()
        {
            int num;

            if (!Int32.TryParse(Raw_Data, out num))
            {
                Error("EPHARM-3: Input is unparseable.");
				return "";
            }
            else if (num < 3 || num > 131070)
            {
                Error("EPHARM-4: Data contains invalid  characters (invalid numeric range).");
				return "";
            }//if

            string result = String.Empty;
            do
            {
                if ((num & 1) == 0)
                {
                    result = _thickBar + result;
                    num = (num - 2) / 2;
                }
                else
                {
                    result = _thinBar + result;
                    num = (num - 1) / 2;
                }

                if (num != 0)
                {
                    result = _gap + result;
                }
            } while (num != 0);

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Pharmacode(); }
        }

        #endregion
    }

    /*
    class Labels
    {
        /// <summary>
        /// Draws Label for ITF-14 barcodes
        /// </summary>
        /// <param name="img">Image representation of the barcode without the labels</param>
        /// <returns>Image representation of the barcode with labels applied</returns>
        public static Image Label_ITF14(Barcode Barcode, Bitmap img)
        {
            try
            {
                Font font = Barcode.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //color a white box at the bottom of the barcode to hold the string of data
                    using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                    {
                        g.FillRectangle(backBrush, new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));
                    }

                    //draw datastring under the barcode image
                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;

                    using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                    {
                        g.DrawString(Barcode.AlternateLabel == null ? Barcode.RawData : Barcode.AlternateLabel, font, foreBrush, (float)(img.Width / 2), img.Height - font.Height + 1, f);
                    }

                    using (Pen pen = new Pen(Barcode.ForeColor, (float)img.Height / 16))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        g.DrawLine(pen, new Point(0, img.Height - font.Height - 2), new Point(img.Width, img.Height - font.Height - 2));//bottom
                    }

                    g.Save();
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                MyException("ELABEL_ITF14-1: " + ex.Message);
            }//catch
        }

        /// <summary>
        /// Draws Label for Generic barcodes
        /// </summary>
        /// <param name="img">Image representation of the barcode without the labels</param>
        /// <returns>Image representation of the barcode with labels applied</returns>
        public static Image Label_Generic(Barcode Barcode, Bitmap img)
        {
            try
            {
                Font font = Barcode.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Near;
                    f.LineAlignment = StringAlignment.Near;
                    int LabelX = 0;
                    int LabelY = 0;

                    switch (Barcode.LabelPosition)
                    {
                        case LabelPositions.BOTTOMCENTER:
                            LabelX = img.Width / 2;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.BOTTOMLEFT:
                            LabelX = 0;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.BOTTOMRIGHT:
                            LabelX = img.Width;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Far;
                            break;
                        case LabelPositions.TOPCENTER:
                            LabelX = img.Width / 2;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.TOPLEFT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.TOPRIGHT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Far;
                            break;
                    }//switch

                    //color a background color box at the bottom of the barcode to hold the string of data
                    using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                    {
                        g.FillRectangle(backBrush, new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));
                    }

                    //draw datastring under the barcode image
                    using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                    {
                        g.DrawString(Barcode.AlternateLabel == null ? Barcode.RawData : Barcode.AlternateLabel, font, foreBrush, new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height), f);
                    }

                    g.Save();
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                MyException("ELABEL_GENERIC-1: " + ex.Message);
            }//catch
        }//Label_Generic


        /// <summary>
        /// Draws Label for EAN-13 barcodes
        /// </summary>
        /// <param name="img">Image representation of the barcode without the labels</param>
        /// <returns>Image representation of the barcode with labels applied</returns>
        public static Image Label_EAN13(Barcode Barcode, Bitmap img)
        {
            try
            {
                int iBarWidth = Barcode.Width / Barcode.EncodedValue.Length;
                string defTxt = Barcode.RawData;

                using (Font labFont = new Font("Arial", getFontsize(Barcode.Width - Barcode.Width % Barcode.EncodedValue.Length, img.Height, defTxt), FontStyle.Regular))
                {
                    int shiftAdjustment;
                    switch (Barcode.Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length) / 2;
                            break;
                    }//switch

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawImage(img, (float)0, (float)0);

                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        StringFormat f = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Near
                        };
                        int LabelY = 0;

                        //Default alignment for EAN13
                        LabelY = img.Height - labFont.Height;
                        f.Alignment = StringAlignment.Near;

                        float w1 = iBarWidth * 4; //Width of first block
                        float w2 = iBarWidth * 42; //Width of second block
                        float w3 = iBarWidth * 42; //Width of third block

                        float s1 = shiftAdjustment - iBarWidth;
                        float s2 = s1 + (iBarWidth * 4); //Start position of block 2
                        float s3 = s2 + w2 + (iBarWidth * 5); //Start position of block 3

                        //Draw the background rectangles for each block
                        using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                        {
                            g.FillRectangle(backBrush, new RectangleF(s2, (float)LabelY, w2, (float)labFont.Height));
                            g.FillRectangle(backBrush, new RectangleF(s3, (float)LabelY, w3, (float)labFont.Height));

                        }

                        //draw datastring under the barcode image
                        using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                        {
                            using (Font smallFont = new Font(labFont.FontFamily, labFont.SizeInPoints * 0.5f, labFont.Style))
                            {
                                g.DrawString(defTxt.Substring(0, 1), smallFont, foreBrush, new RectangleF(s1, (float)img.Height - (float)(smallFont.Height * 0.9), (float)img.Width, (float)labFont.Height), f);
                            }
                            g.DrawString(defTxt.Substring(1, 6), labFont, foreBrush, new RectangleF(s2, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                            g.DrawString(defTxt.Substring(7), labFont, foreBrush, new RectangleF(s3 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                        }

                        g.Save();
                    }
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                MyException("ELABEL_EAN13-1: " + ex.Message);
            }//catch
        }//Label_EAN13

        /// <summary>
        /// Draws Label for UPC-A barcodes
        /// </summary>
        /// <param name="img">Image representation of the barcode without the labels</param>
        /// <returns>Image representation of the barcode with labels applied</returns>
        public static Image Label_UPCA(Barcode Barcode, Bitmap img)
        {
            try
            {
                int iBarWidth = (int)(Barcode.Width / Barcode.EncodedValue.Length);
                int halfBarWidth = (int)(iBarWidth * 0.5);
                string defTxt = Barcode.RawData;

                using (Font labFont = new Font("Arial", getFontsize((int)((Barcode.Width - Barcode.Width % Barcode.EncodedValue.Length) * 0.9f), img.Height, defTxt), FontStyle.Regular))
                {
                    int shiftAdjustment;
                    switch (Barcode.Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length) / 2;
                            break;
                    }//switch

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawImage(img, (float)0, (float)0);

                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        StringFormat f = new StringFormat();
                        f.Alignment = StringAlignment.Near;
                        f.LineAlignment = StringAlignment.Near;
                        int LabelY = 0;

                        //Default alignment for UPCA
                        LabelY = img.Height - labFont.Height;
                        f.Alignment = StringAlignment.Near;

                        float w1 = iBarWidth * 4; //Width of first block
                        float w2 = iBarWidth * 34; //Width of second block
                        float w3 = iBarWidth * 34; //Width of third block

                        float s1 = shiftAdjustment - iBarWidth;
                        float s2 = s1 + (iBarWidth * 12); //Start position of block 2
                        float s3 = s2 + w2 + (iBarWidth * 5); //Start position of block 3
                        float s4 = s3 + w3 + (iBarWidth * 8) - halfBarWidth;

                        //Draw the background rectangles for each block
                        using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                        {
                            g.FillRectangle(backBrush, new RectangleF(s2, (float)LabelY, w2, (float)labFont.Height));
                            g.FillRectangle(backBrush, new RectangleF(s3, (float)LabelY, w3, (float)labFont.Height));
                        }

                        //draw data string under the barcode image
                        using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                        {
                            using (Font smallFont = new Font(labFont.FontFamily, labFont.SizeInPoints * 0.5f, labFont.Style))
                            {
                                g.DrawString(defTxt.Substring(0, 1), smallFont, foreBrush, new RectangleF(s1, (float)img.Height - smallFont.Height, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(1, 5), labFont, foreBrush, new RectangleF(s2 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(6, 5), labFont, foreBrush, new RectangleF(s3 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(11), smallFont, foreBrush, new RectangleF(s4, (float)img.Height - smallFont.Height, (float)img.Width, (float)labFont.Height), f);
                            }
                        }

                        g.Save();
                    }
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                MyException("ELABEL_UPCA-1: " + ex.Message);
            }//catch
        }//Label_UPCA

        public static int getFontsize(int wid, int hgt, string lbl)
        {
            //Returns the optimal font size for the specified dimensions
            int fontSize = 10;

            if (lbl.Length > 0)
            {
                Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.

                // Make a Graphics object to measure the text.
                using (Graphics gr = Graphics.FromImage(fakeImage))
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        using (Font test_font = new Font("Arial", i))
                        {
                            // See how much space the text would
                            // need, specifying a maximum width.
                            SizeF text_size = gr.MeasureString(lbl, test_font);
                            if ((text_size.Width > wid) || (text_size.Height > hgt))
                            {
                                fontSize = i - 1;
                                break;
                            }
                        }
                    }
                }


            };

            return fontSize;
        }
    }

    */
}
