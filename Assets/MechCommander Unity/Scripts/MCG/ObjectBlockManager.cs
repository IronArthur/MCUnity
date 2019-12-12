using MechCommanderUnity.API;
using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class ObjectBlockManager
    {
        #region Class Variables
        public List<MapObjFile.MCObj> objDataBlock;

        List<MCBitmap> ListBitmaps = new List<MCBitmap>();

        int VerticesBlockSide;
        #endregion



        #region Class Structures


        #endregion

        #region Constructors

        public ObjectBlockManager(string terrainFileName)
        {
            string ObjFileName = System.IO.Path.ChangeExtension(terrainFileName, "obj");

            MapObjFile terrainObjFile = new MapObjFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] { "terrain", ObjFileName })));

            objDataBlock = terrainObjFile.GetLstObjs();

        }



        #endregion

        #region Public Functions
        public void Init()
        {
            MechCommanderUnity mcgUnity = MechCommanderUnity.Instance;

        }
        #endregion
    }
}
