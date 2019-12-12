using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.API
{
    class MCTileFile
    {
        #region Class Variables


        /// <summary>
        /// File header.
        /// </summary>
        private TileFileHeader header;

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        /// <summary>
        /// The List image data for this SHP.
        /// </summary>
        private MCBitmap bmp;

        /// <summary>
        /// Palette for building image data
        /// </summary>
        protected MCPalette myPalette;// = new MCPalette();

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MCTileFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MCTileFile(string filePath)
        {
            bmp = new MCBitmap();
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MCTileFile(byte[] data)
        {
            bmp = new MCBitmap();
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures

        internal struct TileFileHeader
        {
            public byte tileHotSpotX;
            public byte tileHotSpotY;
            public byte Height;
            public byte Width;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a Daggerfall palette that will be used for building images.
        /// </summary>
        /// <param name="filePath">Absolute path to Daggerfall palette.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadPalette(string filePath)
        {
            return myPalette.Load(filePath);
        }

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

        public MCBitmap GetBitMap()
        {

            return bmp;

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


                var DataOffsets = new int[header.Height + 2];

                // var firstPosition=Reader.ReadInt32();;
                for (int row = 0; row < header.Height + 2; row++)
                {
                    var offset = Reader.ReadInt32();
                    DataOffsets[row] = offset;

                }




                bmp = new MCBitmap(header.Width, header.Height + 1);

                bmp.PivotX = header.tileHotSpotX;
                bmp.PivotY = header.tileHotSpotY;

                //Reader.BaseStream.Position = DataOffsets[0];
                for (int row = 0; row < header.Height + 1; row++)
                {
                    var index = Reader.ReadByte();
                    var length = DataOffsets[row + 1] - DataOffsets[row];
                    var tiledata = Reader.ReadBytes(length > 0 ? length - 1 : 0);

                    if (!AddTileData(tiledata, index, row))
                        return false;

                }


            } catch (Exception e)
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
        private bool ReadHeader(ref BinaryReader reader)
        {
            // Start header
            reader.BaseStream.Position = 0;

            header.tileHotSpotX = reader.ReadByte();
            header.tileHotSpotY = reader.ReadByte();

            header.Height = reader.ReadByte();
            header.Width = reader.ReadByte();



            return true;


        }

        private bool AddTileData(byte[] TileData, int InitialIndex, int line)
        {


            for (int i = 0; i < TileData.Length; i++)
            {
                try { 
                    bmp.Data[(line * (header.Width)) + (InitialIndex + i)] = TileData[i];
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;

        }

        #endregion
    }
}
