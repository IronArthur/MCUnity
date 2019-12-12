// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
using System.IO;
using MechCommanderUnity.Utility;
using System.Collections.Generic;
#endregion

namespace MechCommanderUnity.API
{
    /// <summary>
    /// Connects to a *.shp file to enumerate and extract image data. 
    /// </summary>
    public class MCTileFileOverlay
    {
        #region Class Variables

        /// <summary>
        /// File header.
        /// </summary>
        private TileFileHeaderOverlay header;

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        /// <summary>
        /// The List image data for this SHP.
        /// </summary>
        private MCBitmap[] imgRecord;

        private MCBitmap bmp;

        /// <summary>
        /// Palette for building image data
        /// </summary>
        protected MCPalette myPalette;// = new MCPalette();

        #endregion

        #region Class Structures

        internal struct TileFileHeaderOverlay
        {

            public Int16 NumFiles;

            public Int16 Height;
            public Int16 Width;

            public Int16 HotSpotX;
            public Int16 HotSpotY;


            //public int FrameCount;
            //public long DataPosition;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MCTileFileOverlay()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MCTileFileOverlay(string filePath)
        {
            imgRecord = new MCBitmap[0];
            bmp = new MCBitmap();
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MCTileFileOverlay(byte[] data)
        {
            imgRecord = new MCBitmap[0];
            bmp = new MCBitmap();
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties

        public int ImgLength
        {
            get { return imgRecord.Length; }

        }



        #endregion

        #region Public Methods

        /// <summary>
        /// Loads an IMG file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        /// <summary>
        /// Loads an IMG file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(byte[] data, FileUsage usage, bool readOnly)
        {
            // managedFile = new FileProxy(data, "");

            /*// Exit if this file already loaded
             if (managedFile.FilePath == filePath)
                 return true;*/

            // Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }


        public MCBitmap GetBitMap(int index = 0)
        {
            if (index < imgRecord.Length)
            {
                return imgRecord[index];
            }
            else
            {
                return new MCBitmap();
            }
        }

        public MCBitmap[] GetBitMaps()
        {
            return imgRecord;
        }


        #endregion

        #region Private Methods



        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool Read()
        {
            try
            {
                // Step through file
                BinaryReader Reader = managedFile.GetReader();

                if (!ReadHeader(ref Reader))
                    return false;

                header.NumFiles = (Int16)Reader.ReadInt32();
                var DataOffsets = new int[header.NumFiles + 1];

                for (int row = 0; row < header.NumFiles; row++)
                {
                    var offset = Reader.ReadInt32();
                    Reader.ReadInt32();// 00
                    DataOffsets[row] = offset;

                }

                DataOffsets[header.NumFiles] = (int)Reader.BaseStream.Length;

                imgRecord = new MCBitmap[header.NumFiles];

                for (int rowN = 0; rowN < header.NumFiles; rowN++)
                {
                    // Reader.BaseStream.Position = DataOffsets[row];
                    if (!ReadTileHeader(ref Reader))
                        return false;

                    var DataOffsetsInner = new int[header.Height + 2];

                    bmp = new MCBitmap(header.Width, header.Height);
                    bmp.PivotX = header.HotSpotX;
                    bmp.PivotY = header.HotSpotY;
                    // var firstPosition=Reader.ReadInt32();;
                    for (int row = 0; row < header.Height + 2; row++)
                    {
                        var offset = Reader.ReadInt16();
                        DataOffsetsInner[row] = offset;

                    }

                    for (int row = 0; row < (header.Height); row++)
                    {
                        var index = Reader.ReadByte();
                        var tiledata = Reader.ReadBytes(DataOffsetsInner[row + 1] - DataOffsetsInner[row] - 1);

                        AddTileData(tiledata, index, row, header.Width);

                    }

                    imgRecord[rowN] = bmp;


                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads file header.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private bool ReadTileHeader(ref BinaryReader reader)
        {
            var inicio = reader.ReadInt32();
            if (inicio != 1212239428)
            {
                return false;
            }

            header.HotSpotX = reader.ReadInt16();
            header.HotSpotY = reader.ReadInt16();


            // Read SHP header data
            header.Height = reader.ReadInt16();
            header.Width = reader.ReadInt16();

            return true;
        }

        /// <summary>
        /// Reads file header.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private bool ReadHeader(ref BinaryReader reader)
        {
            // Start header
            reader.BaseStream.Position = 0;

            var inicio = reader.ReadInt32();
            if (inicio != 1212239428)
            {
                return false;
            }

            // header.NumFiles = (Int16)reader.ReadInt32();

            return true;


        }

        /// <summary>
        /// Reads image data.
        /// </summary>
        private bool AddTileData2(byte[] TileData, int InitialIndex, int line)
        {
            var lineoffset = (line * (header.Width));

            //MechCommanderUnity.LogMessage("Linea: "+line +" ,off: "+lineoffset + " Start: " + InitialIndex);
            for (int i = 0; i < TileData.Length; i++)
            {
                var offset = InitialIndex + i;

                //if (offset < 0)
                //    offset = 0;
                //if (offset > header.Width)
                //    offset = header.Width;
                if (offset >= header.Width)
                    continue;
                //if (TileData[i] == 255)
                //    continue;

                bmp.Data[lineoffset + (offset)] = TileData[i];
            }

            return true;

        }

        /// <summary>
        /// Reads image data.
        /// </summary>
        private bool AddTileData(byte[] TileData, int InitialIndex, int line, int MaxWidth)
        {
            var lineoffset = (line * (header.Width));
            int i = 0;
            var offset = InitialIndex;

            while (i < TileData.Length && offset < MaxWidth)
            {

                if (TileData[i] == 255)//Especial order
                {
                    i++;
                }

                if (TileData[i] > 128) //Length for the next color bytes -128
                {
                    var LengthToRead = TileData[i] - 128;
                    i++;
                    for (int j = 0; j < LengthToRead; j++)
                    {
                        bmp.Data[lineoffset + (offset)] = TileData[i];
                        i++; offset++;
                    }

                    if (offset >= MaxWidth)//End line
                        return true;

                }

                if (TileData[i] < 128) // Jump 
                {
                    offset += TileData[i];
                    i++;
                }


                if (offset >= MaxWidth)// End line
                    return true;

            }

            return true;

        }



        #endregion
    }
}
