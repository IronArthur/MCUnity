
#region Using Statements
using System;
using System.Text;
using System.IO;
using System.Linq;

#endregion

namespace MechCommanderUnity.API
{
    /// <summary>
    /// Connects to a *.shp file to enumerate and extract image data. 
    /// </summary>
    public class ShpFile : IDisposable
    {
        #region Class Variables

        /// <summary>
        /// File header.
        /// </summary>
        private ShpFileHeader header;

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        /// <summary>
        /// The List image data for this SHP.
        /// </summary>
        private MCBitmap[] imgRecord;

        private int Numfiles;
        private int XStart = 1000, YStart = 1000, XEnd = -1000, YEnd = -1000;

        #endregion

        #region Class Structures

        internal struct ShpFileHeader
        {
            public Int16 Type;
            public Int16 NumFiles;

            public Int16 Height;
            public Int16 Width;

            public Int16 XCenter;
            public Int16 YCenter;

            public Int16 XStart;
            public Int16 YStart;

            public Int16 XEnd;
            public Int16 YEnd;

            public int FrameCount;
            public long DataPosition;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShpFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public ShpFile(string filePath)
        {
            imgRecord = new MCBitmap[0];
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public ShpFile(byte[] data)
        {
            imgRecord = new MCBitmap[0];
            Load(data, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public ShpFile(string filePath, FileUsage usage, bool readOnly)
        {
            imgRecord = new MCBitmap[0];

            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public ShpFile(byte[] data, FileUsage usage, bool readOnly)
        {
            imgRecord = new MCBitmap[0];
            Load(data, usage, readOnly);
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

        public MCBitmap GetBitMap(int index)
        {
            if (index < imgRecord.Length)
            {
                return imgRecord[index];
            } else
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
            //try
            //{
            // Step through file
            BinaryReader Reader = managedFile.GetReader();

            if (!ReadHeader(Reader))
                return false;

            Numfiles = Reader.ReadInt32();

            var DataOffsets = new int[Numfiles];

            for (int row = 0; row < Numfiles; row++)
            {
                var offset = Reader.ReadInt32();
                Reader.BaseStream.Position += 4;
                DataOffsets[row] = offset;
            }
            // DataOffsets[Numfiles] = (int)Reader.BaseStream.Length;

            imgRecord = new MCBitmap[Numfiles];

            this.ReadMinHeightWidth(Reader, DataOffsets);

            for (int row = 0; row < Numfiles; row++)
            {
                if (DataOffsets[row] == 0)
                {
                    InitEmptyBmp(row);
                    continue;
                }

                Reader.BaseStream.Position = DataOffsets[row];
                if (!ReadShpHeader(Reader))
                    return false;

                var bmp = new MCBitmap();

                if (!ReadShpData(Reader, out bmp))
                {
                    InitEmptyBmp(row);
                    UnityEngine.Debug.Log("Error Reader Header");
                    continue;
                }


                bmp.Name = row.ToString();

                imgRecord[row] = bmp;

            }

            //} catch (Exception e)
            //{
            //    UnityEngine.Debug.Log(e.Message);
            //    managedFile.Close();
            //    return false;
            //}

            managedFile.Close();

            return true;
        }

        /// <summary>
        /// Reads file header.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private bool ReadShpHeader(BinaryReader reader)
        {
            try
            {
                header.Height = reader.ReadInt16();
                header.Width = reader.ReadInt16();


                header.YCenter = reader.ReadInt16();
                header.XCenter = reader.ReadInt16();

                header.XStart = (Int16)reader.ReadInt32();
                header.YStart = (Int16)reader.ReadInt32();

                header.XEnd = (Int16)reader.ReadInt32();
                header.YEnd = (Int16)reader.ReadInt32();
            } catch (Exception)
            {

                return false;
            }
            // Read SHP header data


            return true;
        }

        /// <summary>
        /// Reads file header.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private bool ReadHeader(BinaryReader reader)
        {
            // Start header
            reader.BaseStream.Position = 0;
            if (reader.ReadInt32() != 808529457)
            {
                reader.BaseStream.Position = 6;
                if (reader.ReadInt32() != 808529457)
                {
                    return false;
                }
            }

            return true;


        }

        /// <summary>
        /// Reads image data.
        /// </summary>
        private bool ReadShpData(BinaryReader Reader, out MCBitmap bmp)
        {
            bmp = new MCBitmap(XEnd - XStart, YEnd - YStart);

            bmp.PivotX = Math.Abs(XStart);
            bmp.PivotY = Math.Abs(YStart);

            var BmpData = bmp.Data;

            byte ch;
            int b, r, i, l, lf;

            var linestart = XStart < 0 ? XStart * -1 + header.XStart : XStart + header.XStart;
            var x = linestart;
            if (YStart > 0 && header.YStart > 0)
            {
                l = Math.Abs(bmp.PivotY - header.YStart);
            } else
            {
                l = Math.Abs(bmp.PivotY + (header.YStart > 0 ? header.YStart * -1 : header.YStart));
            }

            lf = l + (header.YEnd - header.YStart);

            if (l < 0) l = 0;

            while (l <= lf)
            {

                ch = Reader.ReadByte();
                r = ch % 2;
                b = (int)(ch / 2f);

                if (b == 0 && r == 1) // a skip over
                {
                    ch = Reader.ReadByte();
                    x += ch;
                } else if (b == 0)   // end of line
                {
                    l++;
                    x = linestart;
                } else if (r == 0) // a run of bytes
                {
                    ch = Reader.ReadByte();// ShpData[buffindex]; buffindex++; // the color #
                    for (i = 0; i < b; ++i)
                    {
                        BmpData[(l * bmp.Width) + x] = ch;
                        x++;
                    }
                } else // b!0 and r==1 ... read the next b bytes as color #'s
                {
                    var ShpData = Reader.ReadBytes(b);
                    Array.Copy(ShpData, 0, BmpData, (l * bmp.Width) + x, b);
                    x += b;
                }
            }
            bmp.Data = BmpData;

            return true;
        }


        private bool ReadMinHeightWidth(BinaryReader Reader, int[] DataOffsets)
        {

            for (int i = 0; i < DataOffsets.Length; i++)
            {
                if (DataOffsets[i] == 0)
                    continue;
                if (DataOffsets[i] + 8 > Reader.BaseStream.Length)
                    continue;

                Reader.BaseStream.Position = DataOffsets[i] + 8;
                var xst = (short)Reader.ReadInt32();
                if (xst < XStart)
                    XStart = xst;

                var yst = (short)Reader.ReadInt32();
                if (yst < YStart)
                    YStart = yst;

                var xen = (short)Reader.ReadInt32();
                if (xen > XEnd)
                    XEnd = xen;

                var yen = (short)Reader.ReadInt32();
                if (yen > YEnd)
                    YEnd = yen;

            }

            XStart--;
            YStart--;
            XEnd++;
            YEnd++;
            //UnityEngine.Debug.Log(string.Format("Min Size: {0},{1},{2},{3}", XStart, YStart, XEnd, YEnd));

            return true;
        }

        private void InitEmptyBmp(int row)
        {
            var bmpEmpty = new MCBitmap(XEnd - XStart, YEnd - YStart);

            bmpEmpty.PivotX = XStart < 0 ? XStart * -1 : XStart;
            bmpEmpty.PivotY = YStart < 0 ? YStart * -1 : YStart;

            bmpEmpty.Data.Fill((byte)0);
            bmpEmpty.Name = row.ToString();

            imgRecord[row] = bmpEmpty;
        }

        #endregion

        #region Private Methods
        public void Dispose()
        {
            for (int i = 0; i < imgRecord.Length; i++)
            {
                if (imgRecord[i] != null)
                    imgRecord[i].Dispose();
            }
            managedFile.Close();
        }
        #endregion
    }
}
