using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.API
{
   

    public class MapElvFile
    {
        #region Class Variables

        PakFile elvPakFile;

        int BlocksMapSide, VerticesBlockSide;

        MCTile[] Vertice;
        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        List<int> LstTileIndex;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
         /// <summary>
        /// Default constructor.
        /// </summary>
        public MapElvFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MapElvFile(string filePath, int BlocksMapSide, int VerticesBlockSide)
        {
            this.BlocksMapSide = BlocksMapSide;
            this.VerticesBlockSide = VerticesBlockSide;
            Vertice = new MCTile[VerticesBlockSide*VerticesBlockSide*BlocksMapSide*BlocksMapSide];

            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MapElvFile(byte[] data, int BlocksMapSide, int VerticesBlockSide)
        {
            this.BlocksMapSide = BlocksMapSide;
            this.VerticesBlockSide = VerticesBlockSide;
            Vertice = new MCTile[VerticesBlockSide*VerticesBlockSide*BlocksMapSide*BlocksMapSide];
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MCTile
        {
            public byte Elevation;
            public byte Mask;
            public byte Byte2;
            public byte Byte3;
            public ushort Terrain;
            public short OverlayTile;
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
            elvPakFile = new PakFile(filePath);

            // Exit if this file already loaded
            //if (managedFile.FilePath == filePath)
            //    return true;

            //// Load file
            //if (!managedFile.Load(filePath, usage, readOnly))
            //    return false;

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
            elvPakFile = new PakFile(data);

            //// Load file
            //if (!managedFile.Load(data, ""))
            //    return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        public MCTile[] GetVertices()
        {

             return Vertice;
           
        }

        public List<int> GetDifferentTileIds()
        {

            LstTileIndex = Vertice.Select(x => (int)x.Terrain).Distinct().ToList(); // = prefile.GetLstTileIndex(); 

            var lstOVerlayTileIndex = Vertice.Select(x => (int)x.OverlayTile).Distinct().ToList();

            LstTileIndex.AddRange(lstOVerlayTileIndex);

            LstTileIndex.Sort();

            return LstTileIndex;
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
                for (int i = 0; i < elvPakFile.PakInnerFileCount; i++)
                {
                    var elvdata = elvPakFile.GetFileInner(i);

                    for (int x = 0; x < (VerticesBlockSide*VerticesBlockSide); x++)
                    {
                        //  var tile = elvdata.Read<MCTile>();
                        var tile = new MCTile();
                        tile.Elevation = elvdata[(x * 8) + 0];
                        tile.Mask = elvdata[(x * 8) + 1];
                        tile.Byte2 = elvdata[(x * 8) + 2];
                        tile.Byte3 = elvdata[(x * 8) + 3];
                        tile.Terrain = BitConverter.ToUInt16(elvdata, (x * 8) + 4);
                        tile.OverlayTile = BitConverter.ToInt16(elvdata, (x * 8) + 6);


                        Vertice[(i * VerticesBlockSide * VerticesBlockSide) + x] = tile;
                    }

                    //if (!ReadMapBlock(elvdata))
                    //    return false;
                }
                

            }
            catch (Exception e)
            {
               MechCommanderUnity.LogMessage(e.Message + " "+e.InnerException);
                return false;
            }

            return true;
        }

        private bool ReadMapBlock(byte[] MapBlock){

            ReadVerticesBlock(MapBlock.Take(VerticesBlockSide).ToArray());
            return true;
        }

        private bool ReadVerticesBlock(byte[] VerticeBlock)
        {
            var size = VerticeBlock.Length / VerticesBlockSide;

            for (int i = 0; i < VerticesBlockSide; i++)
            {
                var tile = VerticeBlock.Read<MCTile>();
                //var tile = new MCTile();
                //tile.Heigth = (VerticeBlock[(i * 8) + 0]);
                //tile.algo = (VerticeBlock[(i * 8) + 1]);
                //tile.otro = (VerticeBlock[(i * 8) + 2]);
                //tile.otra = (VerticeBlock[(i * 8) + 3]);
                //tile.TileId = BitConverter.ToUInt16(VerticeBlock, (i * 8) + 4);
                //tile.OverlayTileId = BitConverter.ToInt16(VerticeBlock, (i * 8) + 6);

                Vertice[i] = tile;
            }

           

            
            return true;
        }
        #endregion
    }
}
