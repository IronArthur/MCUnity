using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.API
{
    class MapDatFile
    {
        #region Class Variables


        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        int height;
        int width;
        int planet;

        List<MCDatObj> Objs;

        List<byte[]> auxlst;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapDatFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MapDatFile(string filePath)
        {
            Objs = new List<MCDatObj>();
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MapDatFile(byte[] data)
        {
            Objs = new List<MCDatObj>();
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures

        struct MCDatObj
        {
            int id1;
            int id2;
            public MCDatObj(int id1, int id2)
            {
                this.id1 = id1;
                this.id2 = id2;
            }
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


        [Flags]
        public enum MapCell : ulong
        {
            None = 0,
            First = 1 << 0,
            Second = 1 << 1,
            Third = 1 << 2,
            Fourth = 1 << 3,
            MAPCELL_TERRAIN_MASK = 0x0000000F
        }

        public string ToFullString()
        {
            string result = "";

            var MAPCELL_TERRAIN_SHIFT = 0;
            ulong MAPCELL_TERRAIN_MASK = 0xF;

            var MAPCELL_OVERLAY_SHIFT = 4;
            ulong MAPCELL_OVERLAY_MASK = 0x00000030;

            var MAPCELL_MOVER_SHIFT = 6;
            ulong MAPCELL_MOVER_MASK = 0x00000040;

            var MAPCELL_HEIGHT_SHIFT = 18;
            ulong MAPCELL_HEIGHT_MASK = 0x003C0000;

            var MAPCELL_PASSABLE_SHIFT = 10;
            ulong MAPCELL_PASSABLE_MASK = 0x00004000;

            var MAPCELL_FOREST_SHIFT = 28;
            ulong MAPCELL_FOREST_MASK = 0x10000000;

            var MAPCELL_WALL_SHIFT = 24;
            ulong MAPCELL_WALL_MASK = 0x01000000;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var arr = this.auxlst[(x * width) + y];

                    var a = BitConverter.ToUInt32(arr, 0);
                    //var b = BitConverter.ToUInt32(arr, 4);

                    // var a = BitConverter.ToUInt64(arr, 0);

                    if ((a & MAPCELL_TERRAIN_MASK).ToString() != "0")
                    {
                        var ab = 2;
                    }

                    MapCell z = (MapCell)a;

                    if ((z & MapCell.MAPCELL_TERRAIN_MASK).ToString() == (a & MAPCELL_TERRAIN_MASK).ToString())
                    {
                        var st = 0;
                    }

                    //  result += ((a & MAPCELL_TERRAIN_MASK) >> MAPCELL_TERRAIN_SHIFT).ToString("D2") + "|";


                    result += ((a & 0x1F0000) >> MAPCELL_WALL_SHIFT).ToString("D2") + "|";

                    // result += ((a & MAPCELL_WALL_MASK) >> MAPCELL_WALL_SHIFT).ToString("D2") + "|";
                    // result += ((a & MAPCELL_HEIGHT_MASK) >> MAPCELL_HEIGHT_SHIFT).ToString("D2") + "|";
                    // 
                    // result += ((a & MAPCELL_OVERLAY_MASK) >> MAPCELL_OVERLAY_SHIFT).ToString("D2") + "|";
                    //   result += ((a & MAPCELL_MOVER_MASK) >> MAPCELL_MOVER_SHIFT).ToString("D2") + "|";

                    // result += ((a & MAPCELL_FOREST_MASK)).ToString("D2") + "|";
                    //  result += ((a & MAPCELL_PASSABLE_MASK)).ToString("D2") + "|";
                    // result += a + "-" + b + "|";
                    // result += a+"-"+b+"|";

                }

                result += "\r\n";
            }

            return result;
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

                height = Reader.ReadInt32();
                width = Reader.ReadInt32();


                planet = Reader.ReadInt32();

                auxlst = new List<byte[]>();

                for (int i = 0; i < width; i++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        //var id1 = Reader.ReadInt32();
                        //var id2 = Reader.ReadInt32();

                        //Objs.Add(new MCDatObj(id1, id2));

                        auxlst.Add(Reader.ReadBytes(8));
                    }
                }


            } catch (Exception e)
            {
                MechCommanderUnity.LogMessage(e.Message);
                return false;
            }

            return true;
        }




        #endregion
    }
}
