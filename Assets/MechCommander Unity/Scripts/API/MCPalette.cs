
#region Using Statements
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using MechCommanderUnity.Utility;
using UnityEngine;
#endregion

namespace MechCommanderUnity.API
{
    /// <summary>
    /// Describes a 256-colour Daggerfall palette. Supports loading .PAL and .COL files.
    ///  Palette is initialised to all 0xff0000 (red) to make it obvious when palette isn't loaded.
    /// </summary>
    public class MCPalette
    {
        #region Class Variables

        /// <summary>
        /// Length of header in bytes for supporting .COL files.
        /// </summary>
        private int headerLength = 0;

        /// <summary>
        /// Array of 256x RGB values. Includes 8-byte header for supporting .COL files.
        /// </summary>
        private byte[] paletteBuffer = new byte[776];


        /// <summary>
        /// Gets palette memory buffer.
        /// </summary>
        public byte[] PaletteBuffer
        {
            get { return paletteBuffer; }
        }

        /// <summary>
        /// Gets palette header length;
        /// </summary>
        public int HeaderLength
        {
            get { return headerLength; }
        }


        private Dictionary<int, MCColor> SpecialColors = new Dictionary<int, MCColor>()
        {
            {   0,      new MCColor( 0   ,0   ,0   , 0)},	 // This is the marker pixel, and must be translucent (same as 255)
			{   1,      new MCColor( 030 , 30, 30, 191)},//	 ; 75 percent black
			{   2,      new MCColor(  030, 030, 030, 127 )} ,  // 50 percent black
			{   3,      new MCColor(  030, 030, 030 ,   64   )} ,  // 25 percent black
            {   4,      new MCColor(  116, 104, 054  ,  255  )} ,  // bright yellow add
            {   5,      new MCColor(  116, 056, 000 ,  255   )} ,  // bright orange add
            {   6,      new MCColor(  092, 038, 000 ,   255  )} ,  // dim orange add
            {   7,      new MCColor(  253, 132, 132 ,  255   )} ,  // Pink add (laser core)
            {   8,      new MCColor(  187, 012, 012 ,  255   )} ,  // bright red add
            {   9,      new MCColor(  079, 014, 014 ,  255   )} ,  // dim red add

            //{   96,     new MCColor(  038, 038, 038,   0	 )} ,  // white light 1
            //{   98,     new MCColor( 076, 076, 076 ,   0	 )} ,  // white light 2
            //{   100,    new MCColor( 115, 115, 115 ,   0	 )} ,  // white light 3
            //{   102,    new MCColor( 153, 153, 153,    0	 )} ,  // white light 4
            //{   104,    new MCColor( 132, 253, 132,	   255	 )} ,  // mint green add (laser core)
            //{   106,    new MCColor( 012, 187, 012	 , 255	 )} ,  // bright green add
            //{   108,    new MCColor(014, 079, 014 ,   255	 )} ,  // dim green add
            //{   110,    new MCColor( 050, 000, 000 ,   0	 )} ,  // red light 1
            //{   112,    new MCColor( 085, 000, 000 ,   0	 )} ,  // red light 2
            //{   114,    new MCColor(150, 020, 020 ,   0	 )} ,  // red light 3
            //{   116,    new MCColor( 000, 050, 000 ,   0	 )} ,  // green light 1
            //{   118,    new MCColor(000, 085, 000 ,   0	 )} ,  // green light 2
            //{   120,    new MCColor( 020 ,150, 020 ,   0	 )} ,  // green light 3
            //{   122,    new MCColor(008, 056, 096	,   0      )} ,  // ppc light 1
            //{   124,    new MCColor(024, 080 ,096 ,   0	 )} ,  // ppc light 2
            //{   126,    new MCColor( 040, 112, 152 ,   0	 )} ,  // ppc light 3
            //{   128,    new MCColor(144, 160, 176 ,   0	 )} ,  // ppc light 4

            {   246,    new MCColor(    092, 122, 185 ,   255    )} ,  // bright electric blue
            {   247,    new MCColor(    012, 039, 102 ,   255    )} ,  // dim electric blue
            {   248,    new MCColor(    255, 255, 255 ,  64  )} ,  // add 1/3 white
            {   249,    new MCColor(    255, 255, 255 ,   191    )} ,  // add 2/3 white
		    {   250,    new MCColor(    132, 132, 060 ,   127    )} ,  // 50 percent dye smoke
		    {   251,    new MCColor(    052, 024, 016 ,   127    )} ,  // 50 percent dirt
		    {   252,    new MCColor(    132, 132, 132 ,   168  )} ,  // heavy grey smoke
		    {   253,    new MCColor(    132, 132, 132 ,    84 )} ,  // light grey smoke
		    {   254,    new MCColor(    0,   0,   0 ,   127  )} ,  // Dims the destination pixel by 50% (Shadow)
		    {   255,    new MCColor(    0 ,  0 ,  0 ,   0    )} // Leaves the destination pixel at 1.0 (transparent)
        };





        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. Palette is initialised with red.
        /// </summary>
        public MCPalette()
        {
            Fill(0xff, 0, 0);
        }

        /// <summary>
        /// Load constructor (supports both .PAL and .COL files).
        /// </summary>
        /// <param name="FilePath">Absolute path to palette file.</param>
        public MCPalette(string FilePath)
        {
            if (!Load(FilePath))
                Fill(0xff, 0, 0);
        }
        
        public MCPalette(byte[] data)
        {
            if (!Load(data))
                Fill(0xff, 0, 0);
        }

        /// <summary>
        /// Loads a Daggerfall palette file (supports both .PAL and .COL files).
        /// </summary>
        /// <param name="FilePath">Absolute path to palette file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string FilePath)
        {
            FileProxy fileProxy = new FileProxy(FilePath, FileUsage.UseMemory, true);
            switch (fileProxy.Length)
            {
                case 772:
                    headerLength = 4;
                    break;
                case 768:
                    headerLength = 0;
                    break;
                case 776:
                    headerLength = 8;
                    break;
                default:
                    return false;
            }

            // Read palette
            BinaryReader reader = fileProxy.GetReader();
            if (fileProxy.Length != reader.Read(PaletteBuffer, 0, (int)fileProxy.Length))
                return false;
            return true;
        }
        
        public bool Load(byte[] data)
        {
            FileProxy fileProxy = new FileProxy(data,"");
            switch (fileProxy.Length)
            {
                case 772:
                    headerLength = 4;
                    break;
                case 768:
                    headerLength = 0;
                    break;
                case 776:
                    headerLength = 8;
                    break;
                default:
                    return false;
            }

            // Read palette
            BinaryReader reader = fileProxy.GetReader();
            if (fileProxy.Length != reader.Read(PaletteBuffer, 0, (int)fileProxy.Length))
                return false;
            return true;
        }

        /// <summary>
        /// Read palette information from a binary reader.
        ///  Palette must be a 768-byte PalFile structure (256x 24-bit RGB values).
        /// </summary>
        /// <param name="Reader">Source reader positioned at start of palette data.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Read(ref BinaryReader Reader)
        {
            // Read palette bytes
            if (768 != Reader.Read(PaletteBuffer, 8, 768))
                return false;

            // Set header length
            headerLength = 8;

            return true;
        }

        /// <summary>
        /// Fills entire palette with specified RGB value.
        /// </summary>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        public void Fill(byte R, byte G, byte B)
        {
            int offset = HeaderLength;
            for (int i = 0; i < 256; i++)
            {
                PaletteBuffer[offset++] = R;
                PaletteBuffer[offset++] = G;
                PaletteBuffer[offset++] = B;
            }
        }

        /// <summary>
        /// Fills entire palette with grayscale values.
        /// </summary>
        public void MakeGrayscale()
        {
            int offset = HeaderLength;
            for (int i = 0; i < 256; i++)
            {
                PaletteBuffer[offset++] = (byte)i;
                PaletteBuffer[offset++] = (byte)i;
                PaletteBuffer[offset++] = (byte)i;
            }
        }


        /// <summary>
        /// Gets colour at specified index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>DFColor object.</returns>
        public MCColor Get(int Index, bool SpecialColor = false)
        {
            if (SpecialColor && SpecialColors.ContainsKey(Index))
            {
                return SpecialColors[Index];
            } else
            {
                int offset = HeaderLength + Index * 3;
                MCColor col = new MCColor((byte)(PaletteBuffer[offset] * 4), (byte)(PaletteBuffer[offset + 1] * 4), (byte)(PaletteBuffer[offset + 2] * 4));

                return col;
            }

        }

        /// <summary>
        /// Gets red colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Red value byte.</returns>
        public byte GetRed(int Index)
        {
            int offset = HeaderLength + Index * 3;
            return PaletteBuffer[offset];
        }

        /// <summary>
        /// Gets green colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Green value byte.</returns>
        public byte GetGreen(int Index)
        {
            int offset = HeaderLength + Index * 3;
            return PaletteBuffer[offset + 1];
        }

        /// <summary>
        /// Gets blue colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Blue value byte.</returns>
        public byte GetBlue(int Index)
        {
            int offset = HeaderLength + Index * 3;
            return PaletteBuffer[offset + 2];
        }

        /// <summary>
        /// Sets index to specified RGB values.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        public void Set(int Index, byte R, byte G, byte B)
        {
            int offset = HeaderLength + Index * 3;
            PaletteBuffer[offset] = R;
            PaletteBuffer[offset + 1] = G;
            PaletteBuffer[offset + 2] = B;
        }

        /// <summary>
        /// Finds index with specified RGB values.
        /// </summary>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        /// <returns>Index of found RGB value.</returns>
        public int Find(byte R, byte G, byte B)
        {
            int offset = HeaderLength;
            for (int i = 0; i < 256; i++)
            {
                // Check for match
                if (PaletteBuffer[offset] == R && PaletteBuffer[offset + 1] == G && PaletteBuffer[offset + 2] == B)
                    return i;

                // Increment offset
                offset += 3;
            }

            return -1;
        }

        public MCColor[] ExportPalette()
        {
            MCColor[] exportPalette = new MCColor[256];

            for (int i = 0; i < 256; i++)
            {
                exportPalette[i] = Get(i);
            }

            return exportPalette;
        }

        public Texture2D ExportPaletteTexture()
        {
            using (var bmpT = new MCBitmap(256, 1))
            {
                for (int y = 0; y < 256; y++)
                {
                    MCBitmap.SetPixel(bmpT, y, 0, (byte)y);
                }
                return ImageProcessing.MakeTexture2D(bmpT, this);
            }

                
        }
        #endregion
    }


}
