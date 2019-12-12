using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.API
{
    class MapGmmFile
    {
        #region Class Variables

        PakFile objPakFile;

        List<MCAreaObj> Objs;
        List<MCDoorInfoObj> ObjsDoorInfo;
        List<MCAreaInfoObj> ObjsAreaInfo;
        List<MCGlobalMapArea> ObjsGlobalMapArea;



        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapGmmFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MapGmmFile(string filePath)
        {

            Objs = new List<MCAreaObj>();
            ObjsDoorInfo = new List<MCDoorInfoObj>();
            ObjsAreaInfo = new List<MCAreaInfoObj>();
            ObjsGlobalMapArea = new List<MCGlobalMapArea>();

            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MapGmmFile(byte[] data)
        {
            Objs = new List<MCAreaObj>();
            ObjsDoorInfo = new List<MCDoorInfoObj>();
            ObjsAreaInfo = new List<MCAreaInfoObj>();
            ObjsGlobalMapArea = new List<MCGlobalMapArea>();

            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures


        


        public struct MCGlobalMapArea
        {
            // Map layout data
            short sectorR;
            short sectorC;
            //_DoorInfo				doors;
            AreaType type;
            int numDoors;
            int open;
            int teamID;
            int ownerWID;

            int offMap;

            short[] cellsCovered;

            public MCGlobalMapArea(short sectorR, short sectorC, AreaType type,int numDoors,int open,int teamID,int ownerWID,int offMap, short[] cellsCovered)
            {
                this.sectorR = sectorR;
                this.sectorC = sectorC;
                this.type = type;
                this.numDoors = numDoors;
                this.open = open;
                this.teamID = teamID;
                this.ownerWID = ownerWID;
                this.offMap = offMap;
                this.cellsCovered = cellsCovered;


            }
        }

        public struct MCAreaObj
        {
            int id1;
            int id2;
            public MCAreaObj(int id1, int id2)
            {
                this.id1 = id1;
                this.id2 = id2;
            }
        }

        public struct MCDoorInfoObj
        {
            short DoorIndex;
            byte DoorSide;

            public MCDoorInfoObj(short DoorIndex, byte DoorSide)
            {
                this.DoorIndex = DoorIndex;
                this.DoorSide = DoorSide;


            }
        }

        public struct MCAreaInfoObj
        {
            int id1;
            int id2;
            int id3;

            public MCAreaInfoObj(int id1, int id2, int id3)
            {
                this.id1 = id1;
                this.id2 = id2;
                this.id3 = id3;

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

            //// Load file
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

            //// Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
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
                BinaryReader Reader = managedFile.GetReader();

                int Firma = Reader.ReadInt32();

                if (Firma != 140649)
                {
                    return false;
                }
                Reader.ReadInt32();//vacio1
                Reader.ReadInt32();//vacio2

                int width = Reader.ReadInt32();
                int heigth = Reader.ReadInt32();

                int sectorDim = Reader.ReadInt32();
                int sectorHeight = Reader.ReadInt32();
                int sectorWidth = Reader.ReadInt32();

                int numAreas = Reader.ReadInt32();
                int numDoors = Reader.ReadInt32();
                int numDoorInfos = Reader.ReadInt32();
                int numDoorLinks = Reader.ReadInt32();

                int LargeArea = numAreas > 256 ? 2 : 1;

                var AreaMap = new sbyte[LargeArea * width * heigth]; //Reader.ReadBytes(LargeArea * width * heigth);

                for (int i = 0; i < (LargeArea * width * heigth) / 4; i++)
                {
                    AreaMap[i] = Reader.ReadSByte();
                    // var id1 = Reader.ReadInt16();
                    // var id2 = Reader.ReadInt32();

                    // Objs.Add(new MCAreaObj(id1));
                }

                for (int i = 0; i < numDoorInfos; i++)
                {
                    var DoorIndex = Reader.ReadInt16();
                    var DoorSide = Reader.ReadByte();


                    ObjsDoorInfo.Add(new MCDoorInfoObj(DoorIndex, DoorSide));
                }

                for (int i = 0; i < numAreas; i++)
                {

                }


                var st = 2;

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
