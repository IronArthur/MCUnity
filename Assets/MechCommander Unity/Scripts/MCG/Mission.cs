using MechCommanderUnity.API;
using MechCommanderUnity.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class Mission : MonoBehaviour
    {
        #region Class Variables

        public static Mission Instance;

        public bool LoadTerrainObjects = true;
        public bool LoadBuildingObjects = true;
        public bool LoadTurretsObjects = true;

        GameObject MapGO;


        bool GeneratingTerrain = false;


        FITFile missionFile;

        int operationId;       // aka operation id
        int missionId;         // aka mission id

        public string MissionName;
        string missionFileName;
        string missionScriptName;

        public MissionLoadType loadType = MissionLoadType.MISSION_LOAD_SP_QUICKSTART;
        // ABLModulePtr missionBrain;
        // ABLParamPtr missionParams;
        // SymTableNodePtr missionBrainCallback;

        public Terrain Terrain { get; private set; }

        public GameObjectManager ObjectManager;

        int numParts;
        // PartPtr parts;

        bool active;

        static double missionTerminationTime;


        public static bool terminationCounterStarted;
        public static ulong terminationResult;

        public int numObjectives;
        // public ObjectivePtr objectives;
        public float m_timeLimit;
        //CObjectives						missionObjectives;
        //int						numPrimaryObjectives;
        public int duration;
        public bool warning1;
        public bool warning2;

        public float actualTime;
        public float runningTime;

        public int numSmallStrikes;
        public int numLargeStrikes;
        public int numSensorStrikes;
        public int numCameraStrikes;

        public int missionTuneNum;

        public int missionScriptHandle;
        // public ABLParam* missionBrainParams;

        // public  MissionInterfaceManagerPtr missionInterface;

        public Vector2 dropZone;

        public int theSkyNumber;

        public static bool statisticsInitialized;

        #endregion



        #region Class Structures



        #endregion

        #region Constructors

        protected virtual void Awake()
        {
            if (Instance != null)
                throw new UnityException("[Mission] Can have only one instance per scene.");
            Instance = this;

            
        }

        protected void Start()
        {
            if (MechCommanderUnity.Instance.IsReady)
                this.Init(MissionName, loadType);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public void Init()
        {
            missionFile = null;

            operationId = missionId = -1;

            //missionBrain = NULL;
            // missionParams = NULL;
            // missionBrainCallback = NULL;

            numParts = 0;
            // parts = null;

            numObjectives = 0;
            // objectives = null;

            duration = 0;

            active = false;

            numSmallStrikes = 0;
            numLargeStrikes = 0;
            numSensorStrikes = 0;
            numCameraStrikes = 0;

            missionTuneNum = 0;

            missionScriptHandle = -1;

            //  missionInterface = null;

            //missionFileName = "";

            // missionBrainParams = null;
        }

        public void Init(string missionName, MissionLoadType loadType)
        //public Mission(string missionName, MissionLoadType loadType)
        {
            this.Init();
            float loadProgress = 0f;

            MechCommanderUnity.LogMessage("Loading Mission: " + missionName);

            var auxStr = "";


            //LoadType mission SP (quick or logistic) and MP 
            //define team relations

            loadProgress = 4.0f;

            //--------------------------
            // Load Game System stuff...
            //Define General Game Settings

            var generalFitFile = new FITFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] { "missions", "gamesys.fit" })));
            if (generalFitFile.SectionNumber == 0)
            {
                MechCommanderUnity.LogMessage("Error Loading General FitFile :" + MCGExtensions.PathCombine(new string[] { MechCommanderUnity.Instance.MCGPath, "missions", "gamesys.fit" }));
                return;
            }

            //----------------------------------------------------------------------
            // Now that we have some base values, load the master component table...



            //---------------------------------------
            // Read in difficulty here if it exits.
            //InitDifficultySettings(gameSystemFile);


            //---------------------------------------
            // Read Mission File
            missionFileName = Path.ChangeExtension(missionName, "fit");

            //Read Mission Fit File
            missionFile = new FITFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] { "missions", missionFileName })));
            if (missionFile.SectionNumber == 0)
            {
                MechCommanderUnity.LogMessage("Error Loading Mission FitFile :" + MCGExtensions.PathCombine(new string[] { MechCommanderUnity.Instance.MCGPath, "missions", missionFileName }));
            }
            //-----------------------------------------
            // Begin Setting up Teams and Commanders...
            loadProgress = 10.0f;

            //NULL ANY TEAMS & COMMANDERS

            //------------------------------------------------------------
            // First, let's see how many teams and commanders there are...
            long maxTeamID = -1;
            long maxCommanderID = -1;

            if (loadType == MissionLoadType.MISSION_LOAD_MP_LOGISTICS)
            {
                //MP TEAMS & COMMANDERS

            } else
            {

                missionFile.SeekSection("Parts");
                int numParts;

                missionFile.GetInt("NumParts", out numParts);

                if (numParts > 0)
                {
                    for (int i = 1; i < (long)numParts + 1; i++)
                    {
                        //SP TEAMS & COMMANDERS



                        //------------------------------------------------------------------
                        // Find the object to load
                        missionFile.SeekSection("Part" + i);

                        int teamID = -1;
                        missionFile.GetInt("TeamId", out teamID);

                        int commanderID = -1;

                        if (missionFile.GetInt("CommanderId", out commanderID))
                        {
                            int cID;
                            missionFile.GetInt("CommanderId", out cID);
                            commanderID = (sbyte)cID;
                        }

                        //if (MPlayer && dropZoneList)
                        //{
                        //    //-------------------------------------------------------------
                        //    // Since dropZoneList is not NULL, we know this was not started
                        //    // from the command-line...
                        //    long origCommanderID = commanderID;
                        //    commanderID = commandersToLoad[origCommanderID][0];
                        //    teamID = commandersToLoad[origCommanderID][1];
                        //}

                        if (commanderID > maxCommanderID)
                            maxCommanderID = commanderID;
                        if (teamID > maxTeamID)
                            maxTeamID = teamID;
                    }

                }

            }

            missionFile.SeekSection("PaletteSystem");
            missionFile.GetString("PaletteSystem", out string paletteName);

            ContentReader.PaletteXpansion = paletteName == "palettex";
            
            //----------------------------------------------
            // Now, init the teams and commanders we need...

            //INIT TEAMS & COMMANDERS

            //-----------------------------
            // Init Trigger Area Manager...

            //-----------------------------------
            // Setup the Sensor System Manager...

            //INIT SENSOR MANAGER

            //INITIAL DROPZONE FROM missionFIle

            missionFile.SeekSection("DropZone0");
            missionFile.GetFloat("PositionX", out dropZone.x);
            missionFile.GetFloat("PositionY", out dropZone.y);

            //-----------------------------------------------------------------
            // Load the names of the scenario tunes.
            missionFile.SeekSection("Music");
            missionFile.GetInt("scenarioTuneNum", out missionTuneNum);
            //auxStr = "";
            //missionFile.getKeyValueBySection("Music", "scenarioTuneNum", out auxStr);
            //missionTuneNum=int.Parse(auxStr);

            //INIT CRATER MANAGER!!?¿?
            //result = craterManager->init(1000,20479,"feet");

//            MechCommanderUnity.LogMessage("Starting ObjectManager");
            //-----------------------------------------------------------------
            // Start the object system next.	
            ObjectManager = GameObjectManager.Instance;
//            MechCommanderUnity.LogMessage("Finished ObjectManager");


            //-----------------------------------------------------------------
            // Start the collision detection system. -- Doesn't need objects?

            //------------------------------------------------------------
            // Start the Terrain System

            missionFile.SeekSection("TerrainSystem");
            string terrainFileName;
            missionFile.GetString("TerrainFileName", out terrainFileName);//System.IO.Path.ChangeExtension(missionName, "fit");
            //Read Terrain Fit File
            //INIT TERRAIN FILE
//            MechCommanderUnity.LogMessage("Starting Terrain");
            Terrain = new Terrain(terrainFileName);
            var Map = new GameObject();
            this.MapGO = Map;
//            MechCommanderUnity.LogMessage("Finished Terrain");

            loadProgress = 15.0f;

            //INIT TERRAIN GENERATION
//            MechCommanderUnity.LogMessage("Starting TerrainTiles");
            StartCoroutine(Terrain.InitTerrainTiles(GenerateMapIsoMeshes));
//            MechCommanderUnity.LogMessage("Finished TerrainTiles");
            //long terrainInitResult = land->init(&pakFile, 0, GameVisibleVertices, loadProgress, 20.0 );


            loadProgress = 35.0f;


            // land->load(missionFile);

            loadProgress = 36.0f;


            //----------------------------------------------------
            // Start GameMap for Movement System


            // PathManager = new MovePathManager;

            loadProgress = 40.0f;


            //----------------------
            // Load ABL Libraries...


            //---------------------------
            // Load the mission script...
            //-----------------------------------------------------------------
            // We now read in the mission Script File Name


            loadProgress = 41.0f;

            //-------------------------------------------
            // Load all MechWarriors for this mission...

            //Get Num Warrior from missionFile
            //foreach warrior 
            //-------------------------
            // Find the warrior to load
            //--------------------------------------
            // Load the mechwarrior into the mech...
            //----------------------------
            // Read in the Brain module...
            //------------------------------------------------------------
            // For now, all mplayer brains are pbrain. Need to change when
            // we allow ai brains in mplayer...

            //---------------------------------------------------------------
            // Load the brain parameter file and load 'em for each warrior...


            loadProgress = 43.0f;

            //-----------------------------------------------------------------
            // All systems are GO if we reach this point.  Now we need to
            // parse the scenario file for the Objects we need for this scenario
            // We then create each object and place it in the world at the 
            // position we read in with the frame we read in.



            //--------------------------------------------------------------------------------
            // IMPORTANT NOTE: mission parts should always start with Part 1.
            // Part 0 is reserved as a "NULL" id for routines that reference the mission
            // parts. AI routines, Brain keywords, etc. use PART ID 0 as an "object not found"
            // error code. DO NOT USE PART 0!!!!!!! Start with Part 1...


            loadProgress = 43.5f;

            //READ SPECIAL PARTS & LOAD THEM

            loadProgress = 48.5f;
            //--------------------------------------------------------------------------
            // Now that the parts data has been loaded, let's prep the ObjectManager for
            // the real things. First, count the number of objects we need...
            long numMechs = 0;
            long numVehicles = 0;

            //foreach part load in Objectmanager calculate total nummechs & vehicles

            loadProgress = 55.0f;

            //pakFile.seekPacket(1);
            //ObjectManager->countTerrainObjects(&pakFile, (numMechs + MAX_TEAMS * MAX_REINFORCEMENTS_PER_TEAM) + (numVehicles + MAX_TEAMS * MAX_REINFORCEMENTS_PER_TEAM)/* + ObjectManager->maxElementals*/ + 1);
            loadProgress = 58.0f;
            //ObjectManager->setNumObjects(numMechs, numVehicles, 0, -1, -1, -1, 100, 50, 0, 130, -1);

            //-------------------------
            // Load the mech objects...


            loadProgress = 68.0f;

            //  ObjectManager->loadTerrainObjects(&pakFile, loadProgress, 30);
            ObjectManager.terrain = Terrain;
            StartCoroutine(ObjectManager.LoadTerrainObjects(Terrain.ObjBlock.objDataBlock,GenerateMapObj));
            //Terrain.InitTerrainObjects();

            loadProgress = 98.0f;

            //----------------------------------------------
            // Read in the Mission Time Limit.


            //----------------------------------------------
            // Read in the Objectives.  Safe to have none.

            //----------------------------
            // Read in Commander Groups...

            //-----------------------------------------------------------------------
            // Now that the parts are loaded, let's build the roster for each team...

            //---------------------------------------------------------------------------------
            // If we're not playing multiplayer, make sure all home commander movers have their
            // localMoverId set to 0, so the iface can at least check if a mover is player
            // controlled...

            //-----------------------------------------------------
            // This tracks time since scenario started in seconds.

            //----------------------------
            // Create and load the Weather

            //---------------------------------------------------------------
            // Start the Camera and Lists


            //----------------------------------------------------------------------------
            // Start the Mission GUI


            //--------------------------------------------------
            // Close all walls and open gates and landbridges...

//            MechCommanderUnity.LogMessage(loadProgress.ToString());


        }

        #endregion


        #region Private Methods

        IEnumerator WaitMinimumSeconds(int time)
        {
            Debug.Log("Wait " +time);
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            this.GenerateMapIsoMeshes();
            st.Stop();

            if (st.ElapsedMilliseconds < time*1000)
            {
                yield return new WaitForSecondsRealtime((time*1000 - st.ElapsedMilliseconds) / 1000);    //Wait at least 5secs
            }

            this.GenerateMapObj();
        }

        internal IEnumerator GenerateMapIsoMeshes()
        {
//            Debug.Log("GenerateMapIsoMeshes");
            var tilePrefab = Resources.Load("TileIsoMesh");
            var OvertilePrefab = Resources.Load("OverlayTileIso");

            var TerrainObj = this.Terrain;

            int BlocksMapSide = (int)TerrainObj.blocksMapSide;
            int VerticesBlockSide = (int)TerrainObj.verticesBlockSide;

            float MetersPerVertex = TerrainObj.MetersPerVertex;
            float MetersPerElevLevel = TerrainObj.MetersPerElevLevel;
            string MapName = TerrainObj.terrainName;

            var MainTexture = TerrainObj.TerrainTiles.MainText;
            var MainOVTexture = TerrainObj.TerrainTiles.MainOVText;
            var PalTexture = TerrainObj.TerrainTiles.PalText;

            var DictSpriteInfo = TerrainObj.TerrainTiles.DictTileInfo;
            var DictSpriteOVInfo = TerrainObj.TerrainTiles.DictTileOVInfo;

            var LstTiles = TerrainObj.MapBlock.Tiles;

            float TileWidth = 2.28f;

            var HalfTileWidth = TileWidth / 2;
            var HalfTileHeigth = (float)MetersPerVertex / 200;
            var TileHeigth = HalfTileHeigth * 2;

            var Map = this.MapGO;
            Map.name = MapName;

            Map.layer = LayerMask.NameToLayer("Terrain");

            var grid = Map.AddComponent<MapGrid>();
            Map.AddComponent<Pathfinding>();

            grid.displayGridGizmos = true;
            grid.gridWorldSize = new Vector2(BlocksMapSide * VerticesBlockSide, BlocksMapSide * VerticesBlockSide);
            grid.gridWorldBlocks = new Vector2(BlocksMapSide, BlocksMapSide);
            grid.gridWorldVertexPerBlock = new Vector2(VerticesBlockSide, VerticesBlockSide);
            grid.tileWidth = TileWidth;
            grid.tileHeigth = (float)MetersPerVertex / 100f;
            grid.tileZ = (MetersPerElevLevel / 100f);
            //grid.TileMap = LstTiles;

            // grid.nodeRadius = TileWidth;


            var Terrain = new GameObject();
            Terrain.name = "TerrainTiles-" + MapName;
            Terrain.transform.parent = Map.transform;

            var TerrainOV = new GameObject();
            TerrainOV.name = "TerrainTilesOverlay-" + MapName;
            TerrainOV.transform.parent = Map.transform;


            var index = 0;
            
            Shader shader = Shader.Find("MechCommanderUnity/PaletteSwapLookup");
            Material material = new Material(shader);
            material.mainTexture = MainTexture;
            material.SetTexture("_PaletteTex", PalTexture);
            
            Material materialOV = new Material(shader);
            materialOV.mainTexture = MainOVTexture;
            materialOV.SetTexture("_PaletteTex", PalTexture);
            

            for (int y = 0; y < BlocksMapSide; y++)
            {
                for (int x = 0; x < BlocksMapSide; x++)
                {
                    int BlockNumber = (y * BlocksMapSide) + (x);
                    GameObject block = new GameObject("Block" + (BlockNumber).ToString());
                    block.transform.parent = Terrain.transform;

                    MeshRenderer renderer = block.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = block.AddComponent<MeshFilter>();
                    
                    var info = DictSpriteInfo[LstTiles[index].Terrain.ToString()];
                    
                    renderer.sortingLayerName = "Terrain";
                    renderer.sortingOrder = 0;
                    renderer.sharedMaterial = material;

                    // Create mesh
                    Mesh mesh = new Mesh();
                    mesh.name = string.Format("TileBlockMesh");

                    List<Vector3> LstVertex = new List<Vector3>();
                    List<Vector3> LstNormals = new List<Vector3>();
                    List<int> LstTriangles = new List<int>();

                    List<Vector2> LstUVs = new List<Vector2>();



                    for (int j = 0; j < VerticesBlockSide; j++) //BlocksMapSide * 
                    {

                        for (int i = 0; i < VerticesBlockSide; i++) //BlocksMapSide *
                        {
                            int VertexNumber = j * VerticesBlockSide + i;
                            
                            if (!DictSpriteInfo.ContainsKey(LstTiles[index].Terrain.ToString()))
                            {
                                Debug.LogError("No existe index: " + LstTiles[index].Terrain.ToString());
                            }
                            info = DictSpriteInfo[LstTiles[index].Terrain.ToString()];

                            var rect = info.rect;

                            float width = rect.width / 100;
                            float height = rect.height / 100;

                            float halfX = width / 2f;
                            float halfY = height / 2f;

                            float halfX2 = HalfTileWidth;
                            float halfY2 = HalfTileHeigth;

                            Vector3 BasePosition = new Vector3((x * VerticesBlockSide) + i,
                            (y * VerticesBlockSide) + j,
                            LstTiles[index].Elevation);

                            Vector3 IsoPosition = TerrainObj.MapPositionToWorldCoords(BlockNumber,
                                VertexNumber);
                            
                            Vector3[] vertices = new Vector3[4];

                            vertices[0] = new Vector3(IsoPosition.x - halfX, IsoPosition.y, IsoPosition.z); //top-left
                            vertices[1] = new Vector3(IsoPosition.x + halfX, IsoPosition.y, IsoPosition.z); //top-right
                            vertices[2] = new Vector3(IsoPosition.x - halfX, IsoPosition.y - height, IsoPosition.z); //bottom-left
                            vertices[3] = new Vector3(IsoPosition.x + halfX, IsoPosition.y - height, IsoPosition.z); //bottom-right

                            var idxTriangles = LstVertex.Count;
                            // Indices
                            int[] triangles = new int[6]
                                {
                                    idxTriangles, idxTriangles+1, idxTriangles+2,
                                    idxTriangles+3, idxTriangles+2, idxTriangles+1,
                                };

                            // Normals
                            Vector3 normal = Vector3.Normalize(Vector3.up + Vector3.forward);
                            Vector3[] normals = new Vector3[4];
                            normals[0] = normal;
                            normals[1] = normal;
                            normals[2] = normal;
                            normals[3] = normal;

                            Vector2[] uvs = new Vector2[4];

                            uvs[0] = new Vector2(rect.x / MainTexture.width, rect.yMax / MainTexture.height);
                            uvs[1] = new Vector2(rect.xMax / MainTexture.width, rect.yMax / MainTexture.height);
                            uvs[2] = new Vector2(rect.x / MainTexture.width, rect.y / MainTexture.height);
                            uvs[3] = new Vector2(rect.xMax / MainTexture.width, rect.y / MainTexture.height);

                            LstVertex.AddRange(vertices);
                            LstTriangles.AddRange(triangles);
                            LstNormals.AddRange(normals);
                            LstUVs.AddRange(uvs);


                            //tile.transform.parent = row.transform;

                            var ovTileIndex = LstTiles[index].OverlayTile;

                            if (ovTileIndex != 41)// && DictIndexSprites.ContainsKey(LstTiles[index].OverlayTileId)
                            {
                                GameObject overtile = new GameObject("OV-" + ovTileIndex);

                                MeshRenderer rendererOV = overtile.AddComponent<MeshRenderer>();
                                MeshFilter meshFilterOV = overtile.AddComponent<MeshFilter>();

                                Vector3 BasePositionOV = new Vector3((x * VerticesBlockSide) + i,
                                  (y * VerticesBlockSide) + j,
                                  LstTiles[index].Elevation);

                                if (DictSpriteOVInfo.ContainsKey(ovTileIndex.ToString()))
                                {
                                    var infoOV = DictSpriteOVInfo[ovTileIndex.ToString()];
                                    
                                    var rectOV = infoOV.rect;

                                    rendererOV.sortingLayerName = "Overlays";
                                    rendererOV.sortingOrder = (int)(BasePositionOV.x + BasePositionOV.y);
                                    rendererOV.sharedMaterial = materialOV;

                                    // Create mesh
                                    Mesh meshOV = new Mesh();
                                    meshOV.name = string.Format("TileOVMesh");

                                    Vector3[] verticesOV = new Vector3[4];


                                    var halfXOV1 = (rectOV.width / 100) * infoOV.pivot.x;
                                    var halfXOV2 = (rectOV.width / 100) * (1 - infoOV.pivot.x);
                                    var heightOV1 = (rectOV.height / 100) * (1 - infoOV.pivot.y);
                                    var heightOV2 = (rectOV.height / 100) * infoOV.pivot.y;

                                    verticesOV[0] = new Vector3(-halfXOV1, heightOV1, 0); //top-left
                                    verticesOV[1] = new Vector3(halfXOV2, heightOV1, 0); //top-right
                                    verticesOV[2] = new Vector3(-halfXOV1, -heightOV2, 0); //bottom-left
                                    verticesOV[3] = new Vector3(halfXOV2, -heightOV2, 0); //bottom-right

                                    // Indices
                                    int[] trianglesOV = new int[6]
                                        {
                                        0, 1, 2,
                                        3, 2, 1,
                                        };

                                    // Normals


                                    Vector2[] uvsOV = new Vector2[4];

                                    // Debug.Log(rect);
                                    uvsOV[0] = new Vector2(rectOV.x / MainOVTexture.width, rectOV.yMax / MainOVTexture.height);
                                    uvsOV[1] = new Vector2(rectOV.xMax / MainOVTexture.width, rectOV.yMax / MainOVTexture.height);
                                    uvsOV[2] = new Vector2(rectOV.x / MainOVTexture.width, rectOV.y / MainOVTexture.height);
                                    uvsOV[3] = new Vector2(rectOV.xMax / MainOVTexture.width, rectOV.y / MainOVTexture.height);

                                    meshOV.vertices = verticesOV;
                                    meshOV.triangles = trianglesOV;
                                    meshOV.normals = normals;
                                    meshOV.uv = uvsOV;
                                    // Assign mesh
                                    meshFilterOV.sharedMesh = meshOV;
                                    meshFilterOV.sharedMesh.uv = uvsOV;

                                    Vector3 IsoPositionOV = Vector3.zero;
                                    
                                    IsoPositionOV.x = (BasePositionOV.x - BasePositionOV.y) * halfX2;
                                    IsoPositionOV.y = ((BasePositionOV.x + BasePositionOV.y) * -halfY2) + ((BasePositionOV.z) * (MetersPerElevLevel / 100));

                                    IsoPositionOV.z = 0;// (BasePosition.z * ((float)MetersPerElevLevel / 100));


                                    Vector3 IsoPositionOV2 = TerrainObj.MapPositionToWorldCoords(BlockNumber,
                                        VertexNumber);
                                    
                                    
                                    
                                    overtile.transform.position = IsoPositionOV;

                                    overtile.transform.parent = TerrainOV.transform;
                                }
                            }
                            index++;
                        }
                    }

                    mesh.vertices = LstVertex.ToArray();
                    mesh.triangles = LstTriangles.ToArray();
                    mesh.normals = LstNormals.ToArray();
                    mesh.uv = LstUVs.ToArray();
                    // Assign mesh
                    meshFilter.sharedMesh = mesh;
                    meshFilter.sharedMesh.uv = LstUVs.ToArray();


                    yield return new WaitForFixedUpdate();
                }
            }
        }


        private IEnumerator GenerateMapObj()
        {
            yield return null;
            
//            Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Init Painting MapObjects");

            string MapName = this.Terrain.terrainName;
            var MapObjects = this.Terrain.ObjBlock.objDataBlock;

            var MapGameObject = new GameObject("MapObjects");

            if (this.MapGO != null)
            {
                MapGameObject.transform.parent = this.MapGO.transform;
            }

            var objMGR = this.ObjectManager;

            var PalTexture = MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette.ExportPaletteTexture();

            if (LoadTerrainObjects)
            {
                var MapGameObjectTerrain = new GameObject("MapObjects-TerrainObj");
                MapGameObjectTerrain.transform.parent = MapGameObject.transform;
                
                foreach (var terrObj in objMGR.terrainObjects)
                {
                    terrObj.transform.parent = MapGameObjectTerrain.transform;
                }

                yield return new WaitForFixedUpdate();
//                Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Finish Painting TerrainObj");
            }


            if (LoadBuildingObjects)
            {
//                Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Init Painting Buildings");

                var MapGameObjectBuildings = new GameObject("MapObjects-Building");
                MapGameObjectBuildings.transform.parent = MapGameObject.transform;
                foreach (var bld in objMGR.buildings)
                {
                    bld.transform.parent = MapGameObjectBuildings.transform;
                }

//                Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Finish Painting MapObjects");

                yield return new WaitForFixedUpdate();
            }

            if (LoadTurretsObjects)
            {
//                Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Init Painting Turrets");
                var MapGameObjectTurrets = new GameObject("MapObjects-Turrets");
                MapGameObjectTurrets.transform.parent = MapGameObject.transform;
                foreach (var tur in objMGR.turrets)
                {
                   // GameObjectHelper.CreateMapObj(tur, MapGameObjectTurrets.transform, PalTexture);
                    tur.transform.parent = MapGameObjectTurrets.transform;
                }
                foreach (var tur in objMGR.test)
                {
                    tur.transform.parent = MapGameObjectTurrets.transform;
                }


//                Debug.Log(System.DateTime.Now.ToLongTimeString() + " : Finish Painting Turrets");
                yield return new WaitForFixedUpdate();
            }

        }
        #endregion
    }
}
