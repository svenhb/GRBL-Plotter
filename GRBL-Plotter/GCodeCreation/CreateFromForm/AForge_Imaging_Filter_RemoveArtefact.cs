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
    using System.Linq;
    /// <summary>
    /// Artefact filter.
    /// </summary>
    /// 
    /// <remarks><para>Removes single pixel from grafics with just a few colors</para>
    /// 
    /// <para>Each pixel of the original source image is replaced with the median of neighboring pixel
    /// values. The median is calculated by first sorting all the pixel values from the surrounding
    /// neighborhood into numerical order and then replacing the pixel being considered with the
    /// middle pixel value.</para>
    /// 
    /// <para>The filter accepts 24/32 bpp images
    /// color images for processing.</para>
    /// 
    /// </remarks>
    /// 
    public class RemoveArtefact : BaseUsingCopyPartialFilter
    {
        private int size = 5;   
        private int count = 3;

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
        /// Processing square size for the median filter, [3, 25].
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <b>3</b>.</para>
        /// 
        /// <para><note>The value should be odd.</note></para>
        /// </remarks>
        /// 
        public int Size
        {
            get { return size; }
            set { size = Math.Max(3, Math.Min(25, value | 1)); }
        }
        public int Count
        {
            get { return count; }
            set { count = Math.Max(1, Math.Min(5, value | 1)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Median"/> class.
        /// </summary>
        public RemoveArtefact()
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Median"/> class.
        /// </summary>
        /// 
        /// <param name="size">Processing square size.</param>
        /// 
        public RemoveArtefact(int size) : this()
        {   Size = size;
        }
        public RemoveArtefact(int size, int count) : this()
        {
            Size = size;
            Count = count;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            int pixelSize = Image.GetPixelFormatSize(source.PixelFormat) / 8;

            // processing start and stop X,Y positions
            int startX = rect.Left;
            int startY = rect.Top;
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;

            int srcStride = source.Stride;
            int dstStride = destination.Stride;
            int srcOffset = srcStride - rect.Width * pixelSize;
            int dstOffset = dstStride - rect.Width * pixelSize;

            // loop and array indexes
            int i, j, t;
            // processing square's radius
            int radius = size >> 1;

            byte cr, cg, cb;
            Dictionary<Color, int> myColors = new Dictionary<Color, int>();
            Color ocolor,pcolor;

            byte* src = (byte*)source.ImageData.ToPointer();
            byte* dst = (byte*)destination.ImageData.ToPointer();
            byte* p;

            // allign pointers to the first pixel to process
            src += (startY * srcStride + startX * pixelSize);
            dst += (startY * dstStride + startX * pixelSize);

            // do the processing job
            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
            /*    for (int y = startY; y < stopY; y++)
                {
                    for (int x = startX; x < stopX; x++, src++, dst++)
                    {
                        c = 0;
                        for (i = -radius; i <= radius; i++)
                        {
                            t = y + i;
                            if (t < startY)
                                continue;
                            if (t >= stopY)
                                break;
                            for (j = -radius; j <= radius; j++)
                            {
                                t = x + j;
                                if (t < startX)
                                    continue;
                                if (t < stopX)
                                {
                                    g[c++] = src[i * srcStride + j];
                                }
                            }
                        }
                        // get the median
                        *dst = g[c >> 1];
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }*/
            }
            else
            {   // RGB image
                for (int y = startY; y < stopY; y++)                                            // for each line
                {   for (int x = startX; x < stopX; x++, src += pixelSize, dst += pixelSize)    // for each pixel
                    {   myColors.Clear();
                        ocolor = Color.White;
                        for (i = -radius; i <= radius; i++)     // for each kernel row
                        {   t = y + i;
                            if (t < startY)     // skip row
                                continue;
                            if (t >= stopY)     // break
                                break;
                            for (j = -radius; j <= radius; j++) // for each kernel column
                            {   t = x + j;
                                if (t < startX) // skip column
                                    continue;
                                if (t < stopX)
                                {   p = &src[i * srcStride + j * pixelSize];

                                    cr = p[RGB.R];
                                    cg = p[RGB.G];
                                    cb = p[RGB.B];
                                    pcolor = Color.FromArgb(cr,cg,cb);
                                    if (myColors.ContainsKey(pcolor))
                                        myColors[pcolor]++;             // count color
                                    else
                                        myColors.Add(pcolor, 1);        // count color 1st time

                                    if ((i == 0) && (j == 0))
                                        ocolor = pcolor;
                                }
                            }
                        }

                        if (myColors[ocolor] <= count)   // replace single pixel by most used color in kernel
                        {   var top1 = myColors.OrderByDescending(pair => pair.Value).Take(1);
                            ocolor = top1.ToArray()[0].Key;
                        }
                        dst[RGB.R] = ocolor.R;
                        dst[RGB.G] = ocolor.G;
                        dst[RGB.B] = ocolor.B;
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}