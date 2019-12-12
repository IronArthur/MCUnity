using MechCommanderUnity.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
//using System.Threading.Tasks;

namespace MechCommanderUnity.MCG
{
    public class Terrain
    {
        #region Class Variables
        FITFile missionFile;

        int numberVertices;
        int numberQuads;
        // VertexPtr vertexList;
        // TerrainQuadPtr quadList;

        //For editor
        public int userMin;
        public int userMax;
        public int baseTerrain;
        public int fractalThreshold;
        public int fractalNoise;

        public int halfVerticesMapSide;        //Half of the below value.
        public int realVerticesMapSide;        //Number of vertices on each side of map.

        public int verticesBlockSide;            //Always 20.
        public int blocksMapSide;              //Calced from above and 
        public float worldUnitsMapSide;         //Total world units map is across.
        public float oneOverWorldUnitsMapSide;  //Inverse of the above.

        public int visibleVerticesPerSide;     //How many should I process to be sure I got all I could see.


        public float MetersPerElevLevel;
        public float MetersPerVertex;

        public float worldUnitsPerVertex;     //How many world Units between each vertex.  128.0f in current universe.
        public float worldUnitsPerCell;           //How many world units between cells.  42.66666667f ALWAYS!!!!
        public float halfWorldUnitsPerCell;       //Above divided by two.
        public float metersPerCell;               //Number of meters per cell.  8.53333333f ALWAYS!!
        public float oneOverWorldUnitsPerVertex;  //Above numbers inverse.
        public float oneOverWorldUnitsPerCell;
        public float oneOverMetersPerCell;
        public float oneOverVerticesBlockSide;
        public float worldUnitsBlockSide;     //Total world units each block of 20 vertices is.  2560.0f in current universe.

        //    public Stuff::Vector3D mapTopLeft3d;                //Where does the terrain start.

        //   public MapDataPtr mapData;                  //Pointer to class that manages terrain mesh data.
        //   public TerrainTexturesPtr terrainTextures;          //Pointer to class that manages terrain textures.
        //   public TerrainColorMapPtr terrainTextures2;         //Pointer to class that manages the NEW color map terrain texture.
        //public UserHeapPtr						terrainHeap;				//Heap used for terrain. //magic 01102011X3 disabled

        //		public ByteFlag							*VisibleBits;				//What can currently be seen

        public string terrainName;               //Name of terrain data file.
        public string colorMapName;              //Name of colormap, if different from terrainName.

        public float oneOverWorldUnitsPerElevationLevel;

        public float waterElevation;                //Actual height of water in world units.
        public float frameAngle;                    //Used to animate the waves
        public float frameCos;
        public float frameCosAlpha;
        //public DWORD alphaMiddle;               //Used to alpha the water into the shore.
        //public DWORD alphaEdge;
        //public DWORD alphaDeep;
        public float waterFreq;                 //Used to animate waves.
        public float waterAmplitude;

        public int numObjBlocks;               //Stores terrain object info.
        public int numObjVertices;               //
        //public ObjBlockInfo* objBlockInfo;              //Dynamically allocate this please!!

        //public bool* objVertexActive;           //Stores whether or not this vertices objects need to be updated

        //public float* tileRowToWorldCoord;      //Arrays used to help change from tile and cell to actual world position.
        //public float* tileColToWorldCoord;      //TILE functions will be obsolete with new system.
        //public float* cellToWorldCoord;
        //public float* cellColToWorldCoord;
        //public float* cellRowToWorldCoord;

        public bool recalcShadows;              //Should we recalc the shadow map!
        public bool recalcLight;                //Should we recalc the light data.


        public TerrainTiles TerrainTiles { get; private set; }

        public MapBlockManager MapBlock { get; private set; }

        public ObjectBlockManager ObjBlock { get; private set; }

        //public Clouds* cloudLayer;

        #endregion



        #region Class Structures


        #endregion

        #region constructors
        /// <summary>
        /// Default ructor.
        /// </summary>
        public Terrain()
        {

        }

        public Terrain(string terrainFileName)//FITFile terrainFitFile)
        {
            terrainName = terrainFileName;

            string TerrainFileName = System.IO.Path.ChangeExtension(terrainFileName, "fit");

            FITFile terrainFitFile = new FITFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] { "terrain", TerrainFileName })));

            terrainFitFile.SeekSection("TerrainData");

            terrainFitFile.GetInt("VerticesBlockSide", out this.verticesBlockSide);
            terrainFitFile.GetInt("BlocksMapSide", out this.blocksMapSide);

            terrainFitFile.GetFloat("MetersPerElevLevel", out this.MetersPerElevLevel);
            terrainFitFile.GetFloat("MetersPerVertex", out this.MetersPerVertex);

            terrainFitFile.GetInt("VisibleVerticesPerSide", out this.visibleVerticesPerSide);

            terrainFitFile.SeekSection("TileData");

            string TerrainTileFile;
            terrainFitFile.GetString("TerrainTileFile", out TerrainTileFile);

            this.realVerticesMapSide = this.verticesBlockSide * this.blocksMapSide;
            this.halfVerticesMapSide = this.realVerticesMapSide / 2;

            this.worldUnitsMapSide = this.realVerticesMapSide * MetersPerVertex;

            this.numObjBlocks = this.blocksMapSide * this.blocksMapSide;
            this.numObjVertices = this.verticesBlockSide * this.verticesBlockSide;

            //Init TerrainTiles
            TerrainTiles = new TerrainTiles(terrainFileName, TerrainTileFile, (int)this.blocksMapSide, (int)this.verticesBlockSide);

            //Init MapBlockManager
            MapBlock = new MapBlockManager(terrainFileName, (int)this.blocksMapSide, (int)this.verticesBlockSide);

            //Init VertexManager

            //Init TerrainTileManager

            //Init ObjectManager
            ObjBlock = new ObjectBlockManager(terrainFileName);

            //Init TacticalMap




        }

        public IEnumerator InitTerrainTiles(Func<IEnumerator> OnFinish)
        {
            yield return TerrainTiles.Init(OnFinish);
        }

        public Vector3 MapPositionToWorldCoords(int block, int vertex, int pixelOffsetX=0,int pixelOffsetY=0)
        {

            float halfX2 = 2.28f / 2f;
            float halfY2 = 1.28f / 2f;


            var x = block % blocksMapSide;
            var y = block / blocksMapSide;

            var i = vertex % verticesBlockSide;
            var j = vertex / verticesBlockSide;

            Vector3 BasePosition = new Vector3((x * verticesBlockSide) + i,
                      (y * verticesBlockSide) + j ,
                      MapBlock.terrainElevation(block,vertex));

            Vector3 IsoPosition = Vector3.zero;


            IsoPosition.x = ((BasePosition.x - BasePosition.y) * halfX2) + (pixelOffsetX / 100f);// 
            IsoPosition.y = ((BasePosition.x + BasePosition.y) * -halfY2) + ((BasePosition.z) * (MetersPerElevLevel / 100f)) - (pixelOffsetY / 100f); // 

            IsoPosition.z = 0;// (BasePosition.z * ((float)MetersPerElevLevel / 100));

            return IsoPosition;
        }
        

        #endregion
    }
}

