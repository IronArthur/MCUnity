using MechCommanderUnity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace MechCommanderUnity.MCG
{
    public class MapBlockManager
    {
        #region Class Variables
        MapElvFile.MCTile[] blocks;

        int VerticesBlockSide;
        #endregion



        #region Class Structures


        #endregion

        #region Constructors

        public MapBlockManager(string terrainFileName, int BlocksMapSide, int VerticesBlockSide)
        {
            string ElvFileName = System.IO.Path.ChangeExtension(terrainFileName, "elv");
            this.VerticesBlockSide = VerticesBlockSide;
            MapElvFile terrainElvFile = new MapElvFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] {"terrain", ElvFileName })), BlocksMapSide, VerticesBlockSide);
            blocks=terrainElvFile.GetVertices();

        }

        #endregion

        #region Public Functions

        public MapElvFile.MCTile[] Tiles
        {
            get { return blocks; }
        }

        public long getTerrain(long block, long vertex)
        {
            long index = (block * this.VerticesBlockSide * this.VerticesBlockSide) + vertex;

            return blocks[index].Terrain;
        }


        public long getOverlayTile(long block, long vertex)
        {
            long index = (block * this.VerticesBlockSide * this.VerticesBlockSide) + vertex;

            return blocks[index].OverlayTile;
        }


        public float terrainElevation(long block, long vertex)
        {
            long index = (block * this.VerticesBlockSide * this.VerticesBlockSide) + vertex;

            return blocks[index].Elevation;
        }
        #endregion
    }
}
