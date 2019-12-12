// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#endregion

namespace MechCommanderUnity.API
{
    /// <summary>
    /// Connects to PAK
    /// </summary>
    public class PakFile
    {
        #region Class Variables

        /// <summary>Abstracts PAK file to a managed disk or memory stream.</summary>
        private FileProxy managedFile = new FileProxy();

        private MCPalette myPalette;
        /// <summary>Extracted PAK file buffer.</summary>
        private Byte[] pakExtractedBuffer;// = new Byte[pakBufferLengthValue];

        private int[] DataOffsets;

        private int[] DataSizes;

        private int[] SkipFlags;

        private int[] CompressedFlags;

        private int NumFiles;


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets extracted PAK data.
        /// </summary>
        public Byte[] Buffer
        {
            get { return pakExtractedBuffer; }
            set { pakExtractedBuffer = value; }
        }



        public int PakInnerFileCount
        {
            get { return NumFiles; }
        }

        public MCPalette Palette
        {
            get { return myPalette; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PakFile()
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to PAK file.</param>
        public PakFile(string filePath, string palettePath)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            myPalette = new MCPalette(palettePath);

            Load(filePath);
        }
        
        public PakFile(string filePath, byte[] paletteData)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            myPalette = new MCPalette(paletteData);

            Load(filePath);
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to PAK file.</param>
        public PakFile(string filePath)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            Load(filePath);
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="data">Byte data of Pak file</param>
        public PakFile(byte[] data, string palettePath)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            myPalette = new MCPalette(palettePath);

            Load(data);
        }
        
        public PakFile(byte[] data, byte[] paletteData)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            myPalette = new MCPalette(paletteData);

            Load(data);
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="data">Byte data of Pak file</param>
        public PakFile(byte[] data)
        {
            DataOffsets = new int[0];
            DataSizes = new int[0];
            SkipFlags = new int[0];
            CompressedFlags = new int[0];
            Load(data);
        }



        #endregion

        #region Public Methods

        /// <summary>
        /// Load PAK file.
        /// </summary>
        /// <param name="filePath">Absolute path to PAK file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            //if (!filePath.EndsWith(".PAK"))
            //    return false;

            // Load file
            if (!managedFile.Load(filePath, FileUsage.UseMemory, true))
                return false;

            // Read file
            if (!Initialize())
                return false;

            // Managed file is no longer needed
            //managedFile.Close();


            return true;
        }

        /// <summary>
        /// Load PAK file.
        /// </summary>
        /// <param name="data">Byte data of Pak file</param>
        /// <returns>True if successful, otherwise false.</returns> 
        public bool Load(byte[] data)
        {

            // Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Initialize())
                return false;

            return true;
        }



        public byte[] GetFileInner(int index)
        {
            if (NumFiles <= index)
                return null;

            BinaryReader DataReader = managedFile.GetReader();

            pakExtractedBuffer = null;
            if (SkipFlags[index] != 1)
            {
                DataReader.BaseStream.Position = (long)DataOffsets[index];
                if (CompressedFlags[index] == 1)
                {
                    long length3 = (long)DataReader.ReadInt32();
                    byte[] compressedBuffer = DataReader.ReadBytes(DataSizes[index] - 4);

                    pakExtractedBuffer = new byte[length3];
                    LZFuncs.LZDecomp(out pakExtractedBuffer, compressedBuffer, (uint)length3, (uint)compressedBuffer.Length);
                } else
                {
                    pakExtractedBuffer = DataReader.ReadBytes(DataSizes[index]);
                }
            }


            return pakExtractedBuffer;

        }

        public int GetFirstValidIndex()
        {
            for (int i = 0; i < SkipFlags.Count(); i++)
            {
                if (SkipFlags[i] == 0)
                {
                    return i;
                }
            }
            MechCommanderUnity.LogMessage("no Valid First Index");
            return -1;
        }


        public void Save( string saveLocation)
        {
            List<byte[]> files = new List<byte[]>();

            for (int i = 0; i < NumFiles; i++)
            {
                var file = GetFileInner(i);
                if (file == null)
                    file = new byte[0];
                files.Add(file);
            }

            int count = files.Count;
            int FirstOffset = count * 4 + 8;
            int[] Ofssets = new int[count];
            int num2 = FirstOffset;
            int index = 0;
            foreach (var s in files)
            {
                Ofssets[index] = num2;
                if (s.Length == 0)
                {
                    long EmptyOffset = (long)Ofssets[index] | 3758096384L;
                    Ofssets[index] = (int)EmptyOffset;
                }
                num2 += s.Length;
                ++index;
            }
            List<byte> list2 = new List<byte>();
            byte[] numArray2 = new byte[Ofssets.Length * 4];
            System.Buffer.BlockCopy((Array)Ofssets, 0, (Array)numArray2, 0, numArray2.Length);
            list2.AddRange((IEnumerable<byte>)BitConverter.GetBytes(FirstOffset));
            list2.AddRange((IEnumerable<byte>)numArray2);
            foreach (byte[] numArray3 in files)
                list2.AddRange((IEnumerable<byte>)numArray3);
            uint num4 = 0U;
            foreach (byte num3 in list2)
                num4 += (uint)num3;
            BinaryWriter binaryWriter = new BinaryWriter((Stream)new FileStream(saveLocation, FileMode.Create));
            binaryWriter.Write(4277009102);
            binaryWriter.Write(list2.ToArray());
            binaryWriter.Close();

        }

        #endregion

        #region Private Methods

        private bool Initialize()
        {
            // Expand each row of PAK file into buffer
            BinaryReader offsetReader = managedFile.GetReader(0);


            var firmaPak = offsetReader.ReadUInt32();

            if (firmaPak != 4277009102)
                return false;

            Int32 FirstOffset = offsetReader.ReadInt32();

            NumFiles = (FirstOffset - 8) / 4;

            DataOffsets = new int[NumFiles + 1];
            DataSizes = new int[NumFiles];
            SkipFlags = new int[NumFiles];
            CompressedFlags = new int[NumFiles];

            for (int index = 0; (long)index < NumFiles; ++index)
                DataOffsets[index] = offsetReader.ReadInt32();
            DataOffsets[NumFiles] = Convert.ToInt32(managedFile.Length);

            for (int index = 0; (long)index < NumFiles; ++index)
            {
                if (((long)DataOffsets[index] & 3758096384L) == 3758096384L)
                {
                    DataOffsets[index] &= 536870911;
                    SkipFlags[index] = 1;
                } else
                {
                    SkipFlags[index] = 0;
                    if ((DataOffsets[index] & 1073741824) == 1073741824)
                    {
                        DataOffsets[index] &= -1073741825;
                        CompressedFlags[index] = 1;
                    } else
                        CompressedFlags[index] = 0;
                }
            }
            for (int index = 0; (long)index < NumFiles; ++index)
                DataSizes[index] = SkipFlags[index] == 1 ? 0 : DataOffsets[index + 1] - DataOffsets[index];

            return true;
        }


       
        #endregion
    }
}
