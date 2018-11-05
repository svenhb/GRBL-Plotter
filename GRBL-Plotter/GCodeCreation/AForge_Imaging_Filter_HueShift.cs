// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright ? AForge.NET, 2005-2011
// contacts@aforgenet.com
//
// Add on 2018 by Sven Hasemann

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Hue shift, made from Hue add.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and updates
    /// pixels' hue values shifting it by specified value (luminance and
    /// saturation are kept unchanged). The result of the filter looks like pop art.</para>
    ///
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// </remarks>
    /// 
    public class HueShift : BaseInPlacePartialFilter
    {
        private int hue = 0;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Hue value to set, [0, 359].
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <b>0</b>.</para></remarks>
        /// 
        public int Hue
        {
            get { return hue; }
            set { hue = Math.Max(0, Math.Min(359, value)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HueModifier"/> class.
        /// </summary>
        /// 
        public HueShift()
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HueModifier"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Hue value to set.</param>
        /// 
        public HueShift(int hue) : this( )
        {
            this.hue = hue;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            if (hue == 0) return;
            int pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;

            int startX = rect.Left;
            int startY = rect.Top;
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;
            int offset = image.Stride - rect.Width * pixelSize;

            RGB rgb = new RGB();
            HSL hsl = new HSL();

            int tmpHue = 0;
            // do the job
            byte* ptr = (byte*)image.ImageData.ToPointer();

            // allign pointer to the first pixel to process
            ptr += (startY * image.Stride + startX * pixelSize);

            // for each row
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, ptr += pixelSize)
                {
                    rgb.Red = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue = ptr[RGB.B];

                    // convert to HSL
                    AForge.Imaging.HSL.FromRGB(rgb, hsl);

                    // modify hue value
                    tmpHue = hsl.Hue + hue;
                    if (tmpHue > 360) tmpHue -= 360;
                    hsl.Hue = tmpHue;

                    // convert back to RGB
                    AForge.Imaging.HSL.ToRGB(hsl, rgb);

                    ptr[RGB.R] = rgb.Red;
                    ptr[RGB.G] = rgb.Green;
                    ptr[RGB.B] = rgb.Blue;
                }
                ptr += offset;
            }
        }
    }
}