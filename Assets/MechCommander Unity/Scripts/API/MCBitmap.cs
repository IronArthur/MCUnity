

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion
namespace MechCommanderUnity.API
{
    /// <summary>
    /// Stores raw bitmap data. The binary format of the image data will depend on the method which returned the DFBitmap object.
    /// </summary>
    public class MCBitmap : IDisposable
    {
        #region Structure Variables

        /// <summary>Format byte width of indexed formats.</summary>
        const int indexedFormatWidth = 1;

        /// <summary>Format byte width of full colour formats.</summary>
        const int colorFormatWidth = 4;

        public string Name;


        /// <summary>Format of the image.</summary>
        public Formats Format;

        /// <summary>Width of the image in pixels.</summary>
        public int Width;

        /// <summary>Height of the image in pixels.</summary>
        public int Height;

        /// <summary>Stride (bytes per horizontal row) of the image.</summary>
        public int Stride;

        /// <summary>Stride (bytes per horizontal row) of the image.</summary>
        public int PivotX = 0;

        /// <summary>Stride (bytes per horizontal row) of the image.</summary>
        public int PivotY = 0;

        /// <summary>Image byte array in specified format.</summary>
        public byte[] Data;

        #endregion

        #region Structures

        /// <summary>
        /// Bitmap formats enumeration.
        /// </summary>
        public enum Formats
        {
            /// <summary>Indexed image. 1 byte per pixel. Each byte is an index into the palette colour data.</summary>
            Indexed,
            /// <summary>Colour channels in order alpha, red, green, blue. 4 bytes per pixel.</summary>
            ARGB,
            /// <summary>Colour channels in order red, green, blue, alpha. 4 bytes per pixel.</summary>
            RGBA,
        }

        /// <summary>
        /// Internal colour struct.
        /// </summary>
        //public struct MCColor
        //{
        //    public byte r;
        //    public byte g;
        //    public byte b;
        //    public byte a;

        //    public static MCColor FromRGBA(byte r, byte g, byte b, byte a = 255)
        //    {
        //        MCColor color = new MCColor()
        //        {
        //            r = r,
        //            g = g,
        //            b = b,
        //            a = a,
        //        };
        //        return color;
        //    }
        //}
        #endregion

        #region Properties

        /// <summary>
        /// Gets byte width of a single pixel (Indexed=1, other formats=4).
        /// </summary>
        public int FormatWidth
        {
            get
            {
                if (this.Format == Formats.Indexed)
                    return indexedFormatWidth;
                else
                    return colorFormatWidth;
            }
        }

        public MCSize Size
        {
            get { return new MCSize(this.Width, this.Height); }
        }

        public Vector2 Pivot
        {
            get
            {
                return new Vector2(
                    ((float)PivotX / (float)Width),
                      1f - ((float)PivotY / (float)Height));
            }
        }

        public byte[] RawData
        {
            get
            {
                byte[] rawdata = new byte[Data.Length];

                for (int y = Height - 1,i=0; y >= 0; y--,i++)
                {
                    Array.Copy(Data, y * Stride, rawdata, i*Stride, Width);
                }

                return rawdata;
            }
        }

        public void Serialize(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path)))
            {
                writer.Write(this.Width);
                writer.Write(this.Height);
                writer.Write( this.Data);
            }
            //File.WriteAllBytes(path,this.RawData);
        }

        public static MCBitmap Unserialize(string path)
        {
            MCBitmap bitmap= new MCBitmap();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                bitmap=new MCBitmap(reader.ReadInt32(),reader.ReadInt32());

                bitmap.Data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            }
            return bitmap;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates an empty DFBitmap with null data.
        /// </summary>
        public MCBitmap()
        {
            Initialise(0, 0);
        }

        /// <summary>
        /// Constructor.
        /// Creates a new DFBitmap with sized and zeroed data array.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public MCBitmap(int width, int height)
        {
            Initialise(width, height);
        }

        /// <summary>
        /// Constructor.
        /// Creates a new DFBitmap with sized data array filled to specified colour.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="color">Fill colour.</param>
        public MCBitmap(int width, int height, MCColor color)
            : this(width, height)
        {
            Fill(this, color);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise DFBitmap data to specified size.
        /// Any existing data will be discarded.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public void Initialise(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Stride = width * FormatWidth;
            this.Data = new byte[this.Height * this.Stride];

        }




        #endregion

        #region Static Public Methods

        /// <summary>
        /// Fills DFBitmap with specified values.
        /// </summary>
        /// <param name="bitmap">DFBitmap to fill.</param>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        static public void Fill(MCBitmap bitmap, byte r, byte g, byte b, byte a)
        {
            // Fill with value and alpha

            bitmap.Data.Fill(r, g, b, a);

            return;
            for (int pos = 0; pos < bitmap.Data.Length; pos += bitmap.FormatWidth)
            {
                bitmap.Data[pos] = r;
                bitmap.Data[pos + 1] = g;
                bitmap.Data[pos + 2] = b;
                bitmap.Data[pos + 3] = a;
            }
        }

        /// <summary>
        /// Fills DFBitmap with specified colour.
        /// </summary>
        /// <param name="bitmap">DFBitmap to fill.</param>
        /// <param name="color">Source colour.</param>
        static public void Fill(MCBitmap bitmap, MCColor color)
        {
            Fill(bitmap, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Sets pixel in DFBitmap. Colour formats only.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="colour">Fill colour.</param>
        static public void SetPixel(MCBitmap bitmap, int x, int y, MCColor color)
        {
            int pos = y * bitmap.Stride + x * bitmap.FormatWidth;
            if (bitmap.Format == Formats.RGBA)
            {
                bitmap.Data[pos++] = color.R;
                bitmap.Data[pos++] = color.G;
                bitmap.Data[pos++] = color.B;
                bitmap.Data[pos] = color.A;
            } else if (bitmap.Format == Formats.ARGB)
            {
                bitmap.Data[pos++] = color.A;
                bitmap.Data[pos++] = color.R;
                bitmap.Data[pos++] = color.G;
                bitmap.Data[pos] = color.B;
            }
        }

        static public void SetPixel(MCBitmap bitmap, int x, int y, byte color)
        {
            int pos = y * bitmap.Stride + x * bitmap.FormatWidth;
            if (bitmap.Format == Formats.Indexed)
            {
                bitmap.Data[pos] = color;
            }
        }

        /// <summary>
        /// Sets pixel in DFBitmap. Colour formats only.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        static public void SetPixel(MCBitmap bitmap, int x, int y, byte r, byte g, byte b, byte a)
        {
            SetPixel(bitmap, x, y, MCColor.FromRGBA(r, g, b, a));
        }

        /// <summary>
        /// Gets pixel DFColor from DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <returns>DFColor.</returns>
        static public MCColor GetPixel(MCBitmap bitmap, int x, int y)
        {
            MCColor color = new MCColor();
            int srcPos = y * bitmap.Stride + x * bitmap.FormatWidth;
            color.R = bitmap.Data[srcPos++];
            color.G = bitmap.Data[srcPos++];
            color.B = bitmap.Data[srcPos++];
            color.A = bitmap.Data[srcPos++];

            return color;
        }

        /// <summary>
        /// Creates a clone of a DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <returns>Cloned DFBitmap.</returns>
        static public MCBitmap CloneMCBitmap(MCBitmap bitmap)
        {
            // Create destination bitmap to receive normal image
            MCBitmap newBitmap = new MCBitmap();
            newBitmap.Width = bitmap.Width;
            newBitmap.Height = bitmap.Height;
            newBitmap.Stride = bitmap.Stride;
            newBitmap.Format = bitmap.Format;
            newBitmap.Data = new byte[bitmap.Data.Length];

            return newBitmap;
        }

        /// <summary>
        /// Gets a Color32 array for engine.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <returns>Color32 array.</returns>
        static public Color32[] GetColor32(MCBitmap srcBitmap, MCPalette myPalette, int alphaIndex, int border, out MCSize sizeOut)
        {
            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            Color32[] colors = new Color32[dstWidth * dstHeight];

            Color32 c = new Color32();
            int index, offset, srcRow, dstRow;
            byte[] paletteData = myPalette.PaletteBuffer;
            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                srcRow = y * srcWidth;
                dstRow = (dstHeight - 1 - border - y) * dstWidth;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    index = srcBitmap.Data[srcRow + x];
                    offset = myPalette.HeaderLength + index * 3;
                    c.r = (byte)(paletteData[offset] * 4);
                    c.g = (byte)(paletteData[offset + 1] * 4);
                    c.b = (byte)(paletteData[offset + 2] * 4);
                    c.a = (alphaIndex == index) ? (byte)0 : (byte)255;
                    colors[dstRow + border + x] = c;
                }
            }

            sizeOut = new MCSize(dstWidth, dstHeight);

            return colors;
        }

        public void Dispose()
        {
            this.Data = null;

        }


        #endregion
    }

}