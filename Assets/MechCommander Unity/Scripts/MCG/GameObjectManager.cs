using MechCommanderUnity.API;
using MechCommanderUnity.MCG.GameObjectTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MechCommander_Unity.Scripts.MCG.GameObjects;
using UnityEngine;

//using System.Threading.Tasks;

namespace MechCommanderUnity.MCG
{
    public class GameObjectManager : MonoBehaviour
    {
        #region Class Variables

        public static GameObjectManager Instance;

        // static private GameObjectManagerHelper messengerHelper = (new GameObject("GameObjectManagerHelper")).AddComponent<GameObjectManagerHelper>();

        public GameObject TerrainObjectPrefab;
        public GameObject BuildingObjectPrefab;
        public GameObject TurretObjectPrefab;
        public GameObject GateObjectPrefab;


        Texture2D PalTexture;

        ObjectTypeManager objTypeManager;
        AppearanceTypeManager appearanceTypeManager;

        public Terrain terrain;


        //BattleMechPtr* mechs;
        //GroundVehiclePtr* vehicles;
        //ElementalPtr* elementals;
        public List<GameObject> test = new List<GameObject>();

        public List<GameObject> terrainObjects = new List<GameObject>();
        public List<GameObject> buildings = new List<GameObject>();
        //WeaponBoltPtr* weapons;
        //LightPtr* lights;
        //CarnagePtr* carnage;
        //ArtilleryPtr* artillery;
        public List<GameObject> turrets = new List<GameObject>();//TurretPtr* turrets;
        //GatePtr* gates;


        #endregion



        #region Class Structures


        #endregion

        #region Constructors

        private void Awake()
        {
            if (Instance != null)
                throw new UnityException("[Mission] Can have only one instance per scene.");
            Instance = this;


            objTypeManager = new ObjectTypeManager("object2");

            appearanceTypeManager = new AppearanceTypeManager("sprites");
        }



        #endregion

        #region Public Functions


        internal IEnumerator LoadTerrainObjects(List<MapObjFile.MCObj> objDataBlock,Func<IEnumerator> OnFinish)
        {
            if (PalTexture == null)
                PalTexture = MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette.ExportPaletteTexture();

            yield return null;
            int baseSortingIndex = 0;
            
            foreach (var objData in objDataBlock)
            {
                var objtype = objTypeManager.Get(objData.ObjId);

                var realDmg = objData.damage & 0x0f;

                MCGameObject obj;

                switch (objtype.objectClass)
                {
                    case ObjectClass.TERRAINOBJECT:
                    case ObjectClass.TREE:

                        var terrainGo = Instantiate(TerrainObjectPrefab);
                        obj = terrainGo.AddComponent<TerrainObject>();
                        ((TerrainObject)obj).Init(objtype, appearanceTypeManager);

                        var terr = obj.GetComponentInChildren<TerrainObjectUnity>();
                        terr.ResetComponent();
                        terr.PalTexture = this.PalTexture;
                        terr.baseObject = obj;
                        terr.baseSortingIndex = baseSortingIndex;
                        
                        this.InitObjectInfo(obj,objData,objtype,terrainGo);
                        
                        //if (realDmg>0)
                        //((TerrainObjectPtr)obj)->setDamage(((TerrainObjectTypePtr)objType)->getDamageLevel());

                        //objList[curCollidableHandle] = obj;
                        //obj->setHandle(curCollidableHandle++);

                        //obj->setExists(true);
                        //appearanceTypeManager.GetAppearanceType(objtype.Appearance);

                        terrainObjects.Add(terrainGo);
                        break;
                    case ObjectClass.TURRET:
//                        MechCommanderUnity.LogMessage("Creating Turret!!");

                        var turretGo = Instantiate(TurretObjectPrefab);
                        obj = turretGo.AddComponent<TurretObject>();
                        ((TurretObject)obj).Init(objtype, appearanceTypeManager);
                        
                        var turr = obj.GetComponentInChildren<TurretObjectUnity>();
                        turr.ResetComponent();
                        turr.PalTexture = this.PalTexture;
                        turr.baseObject = obj;
                        turr.baseSortingIndex = baseSortingIndex;
                        
                        this.InitObjectInfo(obj,objData,objtype,turretGo);
                        
                        //obj = getTurret(curTurretIndex++);
                        //if (obj)
                        //{
                        //    ((TurretPtr)obj)->init(true, objType);
                        //    if (realDmg)
                        //        obj->setDamage(((TurretTypePtr)objType)->getDamageLevel());

                        //    objList[curCollidableHandle] = obj;
                        //    obj->setHandle(curCollidableHandle++);
                        //    obj->setExists(true);
                        //    obj->setParentId(objData->parentId);
                        //    obj->setTeamId(objData->teamId, true);
                        //}

                        turrets.Add(turretGo);

                        break;
                    case ObjectClass.BUILDING:
                    case ObjectClass.TREEBUILDING:
                        var buildGo = Instantiate(BuildingObjectPrefab);
                        obj = buildGo.AddComponent<BuildingObject>();
                        ((BuildingObject)obj).Init(objtype, appearanceTypeManager);
                        
                        var bld = obj.GetComponentInChildren<BuildingObjectUnity>();
                        bld.ResetComponent();
                        bld.PalTexture = this.PalTexture;
                        bld.baseObject = obj;
                        bld.baseSortingIndex = baseSortingIndex;

                        this.InitObjectInfo(obj,objData,objtype,buildGo);
                        //appearanceTypeManager.GetAppearanceType(objtype.Appearance);

                        //if (realDmg>0)
                        //    obj->setDamage(((BuildingTypePtr)objType)->getDamageLevel());
                        //((BuildingPtr)obj)->baseTileId = (objData->damage >> 4);
                        //if (obj->isSpecialBuilding())
                        //{
                        //    objList[curCollidableHandle] = obj;
                        //    obj->setHandle(curCollidableHandle++);
                        //} else
                        //{
                        //    objList[curNonCollidableHandle] = obj;
                        //    obj->setHandle(curNonCollidableHandle++);
                        //}

                        //obj->setExists(true);
                        //obj->setParentId(objData->parentId);
                        //obj->setTeamId(objData->teamId, true);

                        buildings.Add(buildGo);

                        break;
                    case ObjectClass.GATE:
//                        MechCommanderUnity.LogMessage("Creating Gate!!");
                        var gateGo = Instantiate(GateObjectPrefab);
                        obj = gateGo.AddComponent<GateObject>();
                        ((GateObject)obj).Init(objtype, appearanceTypeManager);
                        
                        var gate = obj.GetComponentInChildren<GateObjectUnity>();
                        gate.ResetComponent();
                        gate.PalTexture = this.PalTexture;
                        gate.baseObject = obj;
                        gate.baseSortingIndex = baseSortingIndex;
                        
                        this.InitObjectInfo(obj,objData,objtype,gateGo);

                        buildings.Add(gateGo);
                        break;
                    default:
                        Debug.Log("Loading Unknow TypeOBj: " + objtype.objectClass +"  ID: "+objData.ObjId);
                        break;

                }

                baseSortingIndex++;
            }

            yield return OnFinish();
//            Debug.Log("Finished LoadTerraingOBjects");
        }

        public void AddObject(int objTypeNum)
        {
            if (PalTexture == null)
                PalTexture = MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette.ExportPaletteTexture();

            MapObjFile.MCObj objData = new MapObjFile.MCObj()
            {
                Block = 0,
                damage = 0,
                ObjId = (short)objTypeNum,
                pixelOffsetX = 0,
                pixelOffsetY = 0,
                Vertex = 0
            };


            var objtype = objTypeManager.Get(objData.ObjId);

            var realDmg = objData.damage & 0x0f;

            MCGameObject obj;

            switch (objtype.objectClass)
            {
                case ObjectClass.TERRAINOBJECT:
                case ObjectClass.TREE:
                    var terrainGo = new GameObject();
                    obj = terrainGo.AddComponent<TerrainObject>();
                    ((TerrainObject)obj).Init(objtype, appearanceTypeManager);

                    //obj = new TerrainObject(objtype, appearanceTypeManager);

                    obj.block = objData.Block;
                    obj.vertex = objData.Vertex;
                    obj.OffsetPixelX = objData.pixelOffsetX;
                    obj.OffsetPixelY = objData.pixelOffsetY;
                    obj.position = terrain.MapPositionToWorldCoords(obj.block, obj.vertex, obj.OffsetPixelX, obj.OffsetPixelY);

                    //if (realDmg>0)
                    //((TerrainObjectPtr)obj)->setDamage(((TerrainObjectTypePtr)objType)->getDamageLevel());

                    //objList[curCollidableHandle] = obj;
                    //obj->setHandle(curCollidableHandle++);

                    //obj->setExists(true);
                    //appearanceTypeManager.GetAppearanceType(objtype.Appearance);

                    terrainObjects.Add(terrainGo);
                    break;
                case ObjectClass.TURRET:
                    MechCommanderUnity.LogMessage("Creating Turret!!");
                    var turretGo = new GameObject();
                    obj = turretGo.AddComponent<TurretObject>();
                    ((TurretObject)obj).Init(objtype, appearanceTypeManager);
                    // obj = new TurretObject(objtype, appearanceTypeManager);
                    obj.block = objData.Block;
                    obj.vertex = objData.Vertex;
                    obj.OffsetPixelX = objData.pixelOffsetX;
                    obj.OffsetPixelY = objData.pixelOffsetY;
                    obj.position = terrain.MapPositionToWorldCoords(obj.block, obj.vertex, obj.OffsetPixelX, obj.OffsetPixelY);

                    //obj = getTurret(curTurretIndex++);
                    //if (obj)
                    //{
                    //    ((TurretPtr)obj)->init(true, objType);
                    //    if (realDmg)
                    //        obj->setDamage(((TurretTypePtr)objType)->getDamageLevel());

                    //    objList[curCollidableHandle] = obj;
                    //    obj->setHandle(curCollidableHandle++);
                    //    obj->setExists(true);
                    //    obj->setParentId(objData->parentId);
                    //    obj->setTeamId(objData->teamId, true);
                    //}

                    turrets.Add(turretGo);

                    break;
                case ObjectClass.BUILDING:
                case ObjectClass.TREEBUILDING:
                    var buildGo = new GameObject();
                    obj = buildGo.AddComponent<BuildingObject>();
                    ((BuildingObject)obj).Init(objtype, appearanceTypeManager);
                    //obj = new BuildingObject(objtype, appearanceTypeManager);
                    obj.block = objData.Block;
                    obj.vertex = objData.Vertex;
                    obj.OffsetPixelX = objData.pixelOffsetX;
                    obj.OffsetPixelY = objData.pixelOffsetY;
                    obj.position = terrain.MapPositionToWorldCoords(obj.block, obj.vertex, obj.OffsetPixelX, obj.OffsetPixelY);

                    //appearanceTypeManager.GetAppearanceType(objtype.Appearance);

                    //if (realDmg>0)
                    //    obj->setDamage(((BuildingTypePtr)objType)->getDamageLevel());
                    //((BuildingPtr)obj)->baseTileId = (objData->damage >> 4);
                    //if (obj->isSpecialBuilding())
                    //{
                    //    objList[curCollidableHandle] = obj;
                    //    obj->setHandle(curCollidableHandle++);
                    //} else
                    //{
                    //    objList[curNonCollidableHandle] = obj;
                    //    obj->setHandle(curNonCollidableHandle++);
                    //}

                    //obj->setExists(true);
                    //obj->setParentId(objData->parentId);
                    //obj->setTeamId(objData->teamId, true);

                    buildings.Add(buildGo);

                    break;
                default:
                    Debug.Log("Loading Unknow TypeOBj: " + objtype.objectClass);
                    break;

            }
        }

        internal static void Cleanup()
        {

        }

        private void InitObjectInfo(MCGameObject obj,MapObjFile.MCObj objData, ObjectType objtype, GameObject go)
        {
            obj.partId = objData.ObjId;
            obj.appearanceId = objtype.AppearanceName;
            obj.block = objData.Block;
            obj.vertex = objData.Vertex;
            obj.OffsetPixelX = objData.pixelOffsetX;
            obj.OffsetPixelY = objData.pixelOffsetY;
            obj.position = terrain.MapPositionToWorldCoords(obj.block, obj.vertex, obj.OffsetPixelX, obj.OffsetPixelY);
            go.name = string.Format("Obj-{0} [OBJ={1} SPR={2}]", obj.Name, obj.partId, obj.appearanceId);
            go.transform.position = obj.position;
        }

        #endregion
    }

    public sealed class GameObjectManagerHelper : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //Clean up eventTable every time a new level loads.
        public void SceneLoaded(int unused)
        {
            GameObjectManager.Cleanup();
        }
    }
}
