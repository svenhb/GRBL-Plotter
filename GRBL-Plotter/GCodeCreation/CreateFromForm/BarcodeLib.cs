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

/*
 * 2020-06-22 removed unneeded functions
 * 2020-06-22 replaced Exceptions by MyException
 * 2020-12-16 Line 1278 'repair' dispose function
 * 2022-01-17 add try/catch
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

//#pragma warning disable CA1303
//#pragma warning disable CA1707
//#pragma warning disable CA1709
//#pragma warning disable CA1717

namespace GrblPlotter.BarcodeCreation
{
    #region Enums
    public enum TYPE : int { UNSPECIFIED, UPCA, UPCE, UPC_SUPPLEMENTAL_2DIGIT, UPC_SUPPLEMENTAL_5DIGIT, EAN13, EAN8, Interleaved2of5, Interleaved2of5_Mod10, Standard2of5, Standard2of5_Mod10, Industrial2of5, Industrial2of5_Mod10, CODE39, CODE39Extended, CODE39_Mod43, Codabar, PostNet, BOOKLAND, ISBN, JAN13, MSI_Mod10, MSI_2Mod10, MSI_Mod11, MSI_Mod11_Mod10, Modified_Plessey, CODE11, USD8, UCC12, UCC13, LOGMARS, CODE128, CODE128A, CODE128B, CODE128C, ITF14, CODE93, TELEPEN, FIM, PHARMACODE };
    public enum SaveTypes : int { JPG, BMP, PNG, GIF, TIFF, UNSPECIFIED };
    public enum AlignmentPositions : int { CENTER, LEFT, RIGHT };
    public enum LabelPositions : int { TOPLEFT, TOPCENTER, TOPRIGHT, BOTTOMLEFT, BOTTOMCENTER, BOTTOMRIGHT };
    #endregion


    interface IBarcode
    {
        string Encoded_Value
        {
            get;
        }//Encoded_Value

        string RawData
        {
            get;
        }//Raw_Data

        List<string> Errors
        {
            get;
        }//Errors

    }//interface

    /// <summary>
    /// Generates a barcode image of a specified symbology from a string of data.
    /// </summary>
    public class Barcode : IDisposable
    {
        #region Variables
        private IBarcode ibarcode = new Blank();
        private string Raw_Data = "";
        private string Encoded_Value = "";
        //      private string _Country_Assigning_Manufacturer_Code = "N/A";
        private TYPE Encoded_Type = TYPE.UNSPECIFIED;
        private Image _Encoded_Image = null;
        private Color _ForeColor = Color.Black;
        private Color _BackColor = Color.White;
        private int _Width = 300;
        private int _Height = 150;
        private string _XML = "";
        private ImageFormat _ImageFormat = ImageFormat.Jpeg;
        private Font _LabelFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private LabelPositions _LabelPosition = LabelPositions.BOTTOMCENTER;
        private RotateFlipType _RotateFlipType = RotateFlipType.RotateNoneFlipNone;
        //     private bool _StandardizeLabel = true;
        private bool gotError = false;
        #endregion

        private void MyException(string text)
        {
            MessageBox.Show(text);
            gotError = true;
        }
        public bool GetError
        {
            get { return gotError; }
        }

        #region Constructors
        /// <summary>
        /// Default constructor.  Does not populate the raw data.  MUST be done via the RawData property before encoding.
        /// </summary>
        public Barcode()
        {
            //constructor
        }//Barcode
        #endregion

        #region Properties
        /// <summary>
        /// Gets the encoded value.
        /// </summary>
        public string EncodedValue
        {
            get { return Encoded_Value; }
        }//EncodedValue
        /// <summary>
        /// Gets or sets the Encoded Type (ex. UPC-A, EAN-13 ... etc)
        /// </summary>
        public TYPE EncodedType
        {
            set { Encoded_Type = value; }
            get { return Encoded_Type; }
        }//EncodedType
        /// <summary>
        /// Gets the Image of the generated barcode.
        /// </summary>
        public Image EncodedImage
        {
            get
            {
                return _Encoded_Image;
            }
        }//EncodedImage
        /// <summary>
        /// Gets or sets the color of the bars. (Default is black)
        /// </summary>
        public Color ForeColor
        {
            get { return this._ForeColor; }
            set { this._ForeColor = value; }
        }//ForeColor
        /// <summary>
        /// Gets or sets the background color. (Default is white)
        /// </summary>
        public Color BackColor
        {
            get { return this._BackColor; }
            set { this._BackColor = value; }
        }//BackColor
        /// <summary>
        /// Gets or sets the label font. (Default is Microsoft Sans Serif, 10pt, Bold)
        /// </summary>
        public Font LabelFont
        {
            get { return this._LabelFont; }
            set { this._LabelFont = value; }
        }//LabelFont
        /// <summary>
        /// Gets or sets the location of the label in relation to the barcode. (BOTTOMCENTER is default)
        /// </summary>
        public LabelPositions LabelPosition
        {
            get { return _LabelPosition; }
            set { _LabelPosition = value; }
        }//LabelPosition
        /// <summary>
        /// Gets or sets the degree in which to rotate/flip the image.(No action is default)
        /// </summary>
        public RotateFlipType RotateFlipType
        {
            get { return _RotateFlipType; }
            set { _RotateFlipType = value; }
        }//RotatePosition
        /// <summary>
        /// Gets or sets the width of the image to be drawn. (Default is 300 pixels)
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// Gets or sets the height of the image to be drawn. (Default is 150 pixels)
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        ///   If non-null, sets the width of a bar. <see cref="Width"/> is ignored and calculated automatically.
        /// </summary>
        public int? BarWidth { get; set; }
        /// <summary>
        ///   If non-null, <see cref="Height"/> is ignored and set to <see cref="Width"/> divided by this value rounded down.
        /// </summary>
        /// <remarks><para>
        ///   As longer barcodes may be more difficult to align a scanner gun with,
        ///   growing the height based on the width automatically allows the gun to be rotated the
        ///   same amount regardless of how wide the barcode is. A recommended value is 2.
        ///   </para><para>
        ///   This value is applied to <see cref="Height"/> after after <see cref="Width"/> has been
        ///   calculated. So it is safe to use in conjunction with <see cref="BarWidth"/>.
        /// </para></remarks>
        public double? AspectRatio { get; set; }
        /// <summary>
        /// Gets or sets whether a label should be drawn below the image. (Default is false)
        /// </summary>
        public bool IncludeLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the amount of time in milliseconds that it took to encode and draw the barcode.
        /// </summary>
        public double EncodingTime
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the XML representation of the Barcode data and image.
        /// </summary>
        public string XML
        {
            get { return _XML; }
        }
        /// <summary>
        /// Gets or sets the image format to use when encoding and returning images. (Jpeg is default)
        /// </summary>
        public ImageFormat ImageFormat
        {
            get { return _ImageFormat; }
            set { _ImageFormat = value; }
        }
        /// <summary>
        /// Gets or sets the alignment of the barcode inside the image. (Not for Postnet or ITF-14)
        /// </summary>
        public AlignmentPositions Alignment
        {
            get;
            set;
        }//Alignment
        #endregion

        #region General Encode
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="ValueToEncode">Raw data to encode.</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string ValueToEncode, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, ValueToEncode);
        }//Encode(TYPE, string, int, int)
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="ValueToEncode">Raw data to encode.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <param name="Width">Width of the resulting barcode.(pixels)</param>
        /// <param name="Height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string ValueToEncode, Color ForeColor, Color BackColor, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            return Encode(iType, ValueToEncode, ForeColor, BackColor);
        }//Encode(TYPE, string, Color, Color, int, int)
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="ValueToEncode">Raw data to encode.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string ValueToEncode, Color ForeColor, Color BackColor)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
            return Encode(iType, ValueToEncode);
        }//(Image)Encode(Type, string, Color, Color)
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        /// <param name="ValueToEncode">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
        public Image Encode(TYPE iType, string ValueToEncode)
        {
            Raw_Data = ValueToEncode;
            return Encode(iType);
        }//(Image)Encode(TYPE, string)
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="iType">Type of encoding to use.</param>
        internal Image Encode(TYPE iType)
        {
            Encoded_Type = iType;
            return Encode();
        }//Encode()
        /// <summary>
        /// Encodes the raw data into a barcode image.
        /// </summary>
        internal Image Encode()
        {
            ibarcode.Errors.Clear();
            gotError = false;

            DateTime dtStartTime = DateTime.Now;

            GenerateBarcode("");

            this.Encoded_Value = ibarcode.Encoded_Value;
            this.Raw_Data = ibarcode.RawData;

            if (!string.IsNullOrEmpty(this.Encoded_Value))
            {
                _Encoded_Image = (Image)Generate_Image();
                //            else
                //                return null;

                this.EncodedImage.RotateFlip(this.RotateFlipType);
            }
            this.EncodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            //      _XML = GetXML();

            return EncodedImage;
        }//Encode

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.
        /// </summary>
        /// <returns>
        /// Returns a string containing the binary value of the barcode. 
        /// This also sets the internal values used within the class.
        /// </returns>
        /// <param name="raw_data" >Optional raw_data parameter to for quick barcode generation</param>
        public string GenerateBarcode(string raw_data)
        {
            if (!string.IsNullOrEmpty(raw_data))
            {
                Raw_Data = raw_data;
            }

            //make sure there is something to encode
            if (string.IsNullOrEmpty(Raw_Data.Trim()))
            { MyException("EENCODE-1: Input data not allowed to be blank."); return ""; }

            if (this.EncodedType == TYPE.UNSPECIFIED)
            { MyException("EENCODE-2: Symbology type not allowed to be unspecified."); return ""; }

            this.Encoded_Value = "";
            //      this._Country_Assigning_Manufacturer_Code = "N/A";


            switch (this.Encoded_Type)
            {
                case TYPE.UCC12:
                case TYPE.UPCA: //Encode_UPCA();
                    ibarcode = new UPCA(Raw_Data);
                    break;
                case TYPE.UCC13:
                case TYPE.EAN13: //Encode_EAN13();
                    ibarcode = new EAN13(Raw_Data);
                    break;
                case TYPE.Interleaved2of5_Mod10:
                case TYPE.Interleaved2of5: //Encode_Interleaved2of5();
                    ibarcode = new Interleaved2of5(Raw_Data, Encoded_Type);
                    break;
                case TYPE.Industrial2of5_Mod10:
                case TYPE.Industrial2of5:
                case TYPE.Standard2of5_Mod10:
                case TYPE.Standard2of5: //Encode_Standard2of5();
                    ibarcode = new Standard2of5(Raw_Data, Encoded_Type);
                    break;
                case TYPE.LOGMARS:
                case TYPE.CODE39: //Encode_Code39();
                    ibarcode = new Code39(Raw_Data);
                    break;
                case TYPE.CODE39Extended:
                    ibarcode = new Code39(Raw_Data, true);
                    break;
                case TYPE.CODE39_Mod43:
                    ibarcode = new Code39(Raw_Data, false, true);
                    break;
                case TYPE.Codabar: //Encode_Codabar();
                    ibarcode = new Codabar(Raw_Data);
                    break;
                case TYPE.PostNet: //Encode_PostNet();
                    ibarcode = new Postnet(Raw_Data);
                    break;
                case TYPE.ISBN:
                case TYPE.BOOKLAND: //Encode_ISBN_Bookland();
                    ibarcode = new ISBN(Raw_Data);
                    break;
                case TYPE.JAN13: //Encode_JAN13();
                    ibarcode = new JAN13(Raw_Data);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_2DIGIT: //Encode_UPCSupplemental_2();
                    ibarcode = new UPCSupplement2(Raw_Data);
                    break;
                case TYPE.MSI_Mod10:
                case TYPE.MSI_2Mod10:
                case TYPE.MSI_Mod11:
                case TYPE.MSI_Mod11_Mod10:
                case TYPE.Modified_Plessey: //Encode_MSI();
                    ibarcode = new MSI(Raw_Data, Encoded_Type);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_5DIGIT: //Encode_UPCSupplemental_5();
                    ibarcode = new UPCSupplement5(Raw_Data);
                    break;
                case TYPE.UPCE: //Encode_UPCE();
                    ibarcode = new UPCE(Raw_Data);
                    break;
                case TYPE.EAN8: //Encode_EAN8();
                    ibarcode = new EAN8(Raw_Data);
                    break;
                case TYPE.USD8:
                case TYPE.CODE11: //Encode_Code11();
                    ibarcode = new Code11(Raw_Data);
                    break;
                case TYPE.CODE128: //Encode_Code128();
                    ibarcode = new Code128(Raw_Data);
                    break;
                case TYPE.CODE128A:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.A);
                    break;
                case TYPE.CODE128B:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.B);
                    break;
                case TYPE.CODE128C:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.C);
                    break;
                case TYPE.ITF14:
                    ibarcode = new ITF14(Raw_Data);
                    break;
                case TYPE.CODE93:
                    ibarcode = new Code93(Raw_Data);
                    break;
                case TYPE.TELEPEN:
                    ibarcode = new Telepen(Raw_Data);
                    break;
                case TYPE.FIM:
                    ibarcode = new FIM(Raw_Data);
                    break;
                case TYPE.PHARMACODE:
                    ibarcode = new Pharmacode(Raw_Data);
                    break;

                default: MyException("EENCODE-2: Unsupported encoding type specified."); return "";
            }//switch

            return this.Encoded_Value;

        }
        #endregion

        #region Image Functions
        /// <summary>
        /// Gets a bitmap representation of the encoded data.
        /// </summary>
        /// <returns>Bitmap of encoded value.</returns>
        private Bitmap Generate_Image()
        {
            Bitmap bitmap = null;
            if (string.IsNullOrEmpty(Encoded_Value)) { MyException("EGENERATE_IMAGE-1: Must be encoded first."); return bitmap; }

            DateTime dtStartTime = DateTime.Now;

            switch (this.Encoded_Type)
            {
                case TYPE.ITF14:
                    {
                        // Automatically calculate the Width if applicable. Quite confusing with this
                        // barcode type, and it seems this method overestimates the minimum width. But
                        // at least it�s deterministic and doesn�t produce too small of a value.
                        if (BarWidth.HasValue)
                        {
                            // Width = (BarWidth * EncodedValue.Length) + bearerwidth + iquietzone
                            // Width = (BarWidth * EncodedValue.Length) + 2*Width/12.05 + 2*Width/20
                            // Width - 2*Width/12.05 - 2*Width/20 = BarWidth * EncodedValue.Length
                            // Width = (BarWidth * EncodedValue.Length)/(1 - 2/12.05 - 2/20)
                            // Width = (BarWidth * EncodedValue.Length)/((241 - 40 - 24.1)/241)
                            // Width = BarWidth * EncodedValue.Length / 176.9 * 241
                            // Rounding error? + 1
                            Width = (int)(241 / 176.9 * Encoded_Value.Length * BarWidth.Value + 1);
                        }
                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        if (IncludeLabel)
                        {
                            ILHeight -= this.LabelFont.Height;
                        }

                        bitmap = new Bitmap(Width, Height);

                        int bearerwidth = (int)((bitmap.Width) / 12.05);
                        int iquietzone = Convert.ToInt32(bitmap.Width * 0.05);
                        int iBarWidth = (bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) / Encoded_Value.Length;
                        int shiftAdjustment = ((bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) % Encoded_Value.Length) / 2;

                        if (iBarWidth <= 0 || iquietzone <= 0)
                        { MyException("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)"); return null; }

                        //draw image
                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            //fill background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen pen = new Pen(ForeColor, iBarWidth))
                            {
                                pen.Alignment = PenAlignment.Right;

                                while (pos < Encoded_Value.Length)
                                {
                                    //draw the appropriate color line vertically
                                    if (Encoded_Value[pos] == '1')
                                        g.DrawLine(pen, new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height));

                                    pos++;
                                }//while

                                //bearer bars
                                pen.Width = (float)ILHeight / 8;
                                pen.Color = ForeColor;
                                pen.Alignment = PenAlignment.Center;
                                g.DrawLine(pen, new Point(0, 0), new Point(bitmap.Width, 0));//top
                                g.DrawLine(pen, new Point(0, ILHeight), new Point(bitmap.Width, ILHeight));//bottom
                                g.DrawLine(pen, new Point(0, 0), new Point(0, ILHeight));//left
                                g.DrawLine(pen, new Point(bitmap.Width, 0), new Point(bitmap.Width, ILHeight));//right
                            }//using
                        }//using

                        //         if (IncludeLabel)
                        //              Labels.Label_ITF14(this, bitmap);
                        bitmap.Dispose();
                        break;
                    }//case
                case TYPE.UPCA:
                    {
                        // Automatically calculate Width if applicable.
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        // Automatically calculate Height if applicable.
                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        int shiftAdjustment;
                        int iBarWidth = Width / Encoded_Value.Length;

                        //set alignment
                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }//switch


                        bitmap = new Bitmap(Width, Height);
                        int iBarWidthModifier = 1;
                        if (iBarWidth <= 0)
                        { MyException("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)"); return null; }

                        //draw image
                        int pos = 0;
                        int halfBarWidth = (int)(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            //clears the image and colors the entire background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (Encoded_Value[pos] == '1')
                                        {
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }

                                        pos++;
                                    }//while
                                }//using
                            }//using
                        }//using

                        break;
                    }//case
                case TYPE.EAN13:
                    {
                        // Automatically calculate Width if applicable.
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        // Automatically calculate Height if applicable.
                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        int shiftAdjustment;

                        //set alignment
                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }//switch

                        bitmap = new Bitmap(Width, Height);
                        int iBarWidth = Width / Encoded_Value.Length;
                        int iBarWidthModifier = 1;
                        if (iBarWidth <= 0)
                        { MyException("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)"); return null; }

                        //draw image
                        int pos = 0;
                        int halfBarWidth = (int)(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            //clears the image and colors the entire background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (Encoded_Value[pos] == '1')
                                        {
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }

                                        pos++;
                                    }//while
                                }//using
                            }//using
                        }//using
                        if (IncludeLabel)
                        {
                        }
                        //                bitmap.Dispose();
                        break;
                    }//case
                default:
                    {
                        // Automatically calculate Width if applicable.
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        // Automatically calculate Height if applicable.
                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        if (IncludeLabel)
                        {
                            // Shift drawing down if top label.
                            if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                                topLabelAdjustment = this.LabelFont.Height;

                            ILHeight -= this.LabelFont.Height;
                        }


                        bitmap = new Bitmap(Width, Height);
                        int iBarWidth = Width / Encoded_Value.Length;
                        int shiftAdjustment;
                        int iBarWidthModifier = 1;

                        if (this.Encoded_Type == TYPE.PostNet)
                            iBarWidthModifier = 2;

                        //set alignment
                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }//switch

                        if (iBarWidth <= 0)
                        { MyException("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)"); return null; }

                        //draw image
                        int pos = 0;
                        int halfBarWidth = (int)Math.Round(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            //clears the image and colors the entire background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (this.Encoded_Type == TYPE.PostNet)
                                        {
                                            //draw half bars in postnet
                                            if (Encoded_Value[pos] == '0')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, (ILHeight / 2) + topLabelAdjustment));
                                            else
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment));
                                        }//if
                                        else
                                        {
                                            if (Encoded_Value[pos] == '1')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }
                                        pos++;
                                    }//while
                                }//using
                            }//using
                        }//using
                        if (IncludeLabel)
                        {
                            //         Labels.Label_Generic(this, bitmap);
                        }//if
                         //             bitmap.Dispose();
                        break;
                    }//switch
            }//switch

            _Encoded_Image = (Image)bitmap;

            this.EncodingTime += ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return bitmap;
        }//Generate_Image

        #endregion

        #region XML Methods
        #endregion

        #region Static Encode Methods

        #region IDisposable Support
        // To detect redundant calls
        private bool _disposed = false;
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            { return; }
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _LabelFont.Dispose();

                LabelFont?.Dispose();
                LabelFont = null;

                _Encoded_Image?.Dispose();
                _Encoded_Image = null;

                _XML = null;
                Raw_Data = null;
                Encoded_Value = null;
                //        _Country_Assigning_Manufacturer_Code = null;
                _ImageFormat = null;
            }
            _disposed = true;
        }
        #endregion

        #endregion
    }//Barcode Class
}
