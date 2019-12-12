using MechCommanderUnity.API;
using MechCommanderUnity.MCG.ObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//using System.Threading.Tasks;

namespace MechCommanderUnity.MCG
{
    
    class ObjectTypeManager
    {
        #region Class Variables
        PakFile objPakFile;
        Dictionary<int, ObjectType> ObjectTypeList;

        //--------------------------------------------------------
        // Following is done to maintain compatibility with MC1...
        int bridgeTypeHandle;
        int forestTypeHandle;
        int wallHeavyTypeHandle;
        int wallMediumTypeHandle;
        int wallLightTypeHandle;


        #endregion



        #region Class Structures


        #endregion

        #region Constructors

        public ObjectTypeManager(string objFileName)
        {

            string ObjFileName = System.IO.Path.ChangeExtension(objFileName, "pak");

            this.objPakFile = new PakFile(MCGExtensions.PathCombine(new string[] { MechCommanderUnity.Instance.MCGPath, "objects", ObjFileName }));

            ObjectTypeList = new Dictionary<int, ObjectType>();

        }

        #endregion

        #region Public Functions

        public ObjectType Load(int objTypeNum, bool noCacheOut = true, bool forceLoad = false)
        {
            if (ObjectTypeList.ContainsKey(objTypeNum))
            {
                return ObjectTypeList[objTypeNum];
            }

            int objectTypeNum = -1;

            var objFitData = objPakFile.GetFileInner(objTypeNum);
            if (objFitData == null)
            {
                return null;
            }

            var objFitFile = new FITFile(objFitData);

            objFitFile.SeekSection("ObjectClass");

            objFitFile.GetInt("ObjectTypeNum", out objectTypeNum);

            ObjectType objType= new ObjectType(objectTypeNum);

            switch ((ObjectTypeClass)objectTypeNum)
            {
                case ObjectTypeClass.CRAPPY_OBJECT:
                    //----------------------------------------------
                    // In theory we can't ever get here.
                    // Because we did our jobs correctly!!

                    throw new Exception();
                    break;
                case ObjectTypeClass.BATTLEMECH_TYPE:
                    {
                        //objType = new ObjectType(objFitFile);
                        //objType.init(objFitFile);
                        //objType = new BattleMechType;
                        //objType->setObjTypeNum(objTypeNum);
                        ////if ((objType->init(objectFile,objectFile->getPacketSize()) != NO_ERR) && (objType->init(tmp1) != NO_ERR))
                        //if ((objType->init(objectFile, objectFile->getPacketSize(), tmp1) != NO_ERR))// && (objType->init(tmp1) != NO_ERR))
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Mech type ");
                    }
                    break;
                case ObjectTypeClass.VEHICLE_TYPE:
                    {
                        //objType = new GroundVehicleType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if ((objType->init(objectFile, objectFile->getPacketSize(), tmp1) != NO_ERR))// && (objType->init(tmp1,3) != NO_ERR))
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Vehicle type ");
                    }
                    break;
                case ObjectTypeClass.TREEBUILDING_TYPE:
                case ObjectTypeClass.BUILDING_TYPE:
                    {
                        objType = new BuildingType(objFitFile);
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize(), tmp1) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Building type ");
                    }
                    break;
                case ObjectTypeClass.TREE_TYPE:
                    {
                        objType = new TerrainObjectType(objFitFile);
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init TerrainObject:Tree type ");
                    }
                    break;
                case ObjectTypeClass.TERRAINOBJECT_TYPE:
                    {
                        objType = new TerrainObjectType(objFitFile);
                        //objType->setObjTypeNum(objTypeNum);
                        //if (isMiscTerrObj)
                        //    ((TerrainObjectTypePtr)objType)->initMiscTerrObj(objTypeNum);
                        //else if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init TerrainObject type ");
                    }
                    break;
                case ObjectTypeClass.WEAPONBOLT_TYPE:
                    {
                        //objType = new WeaponBoltType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init WeaponBolt type ");
                    }
                    break;

                case ObjectTypeClass.TURRET_TYPE:
                    {
                        objType = new TurretType(objFitFile);
                        //objType = new TurretType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize(), tmp1) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Turret type ");
                    }
                    break;

                case ObjectTypeClass.EXPLOSION_TYPE:
                    {
                        objType = new ExplosionType(objFitFile);
                        //objType = new ExplosionType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Explosion type ");
                    }
                    break;

                case ObjectTypeClass.FIRE_TYPE:
                    {
                        objType = new FireType(objFitFile);
                        //objType = new FireType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Fire type ");
                    }
                    break;

                case ObjectTypeClass.GATE_TYPE:
                    {
                        objType = new GateType(objFitFile);
                        //objType = new GateType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize(), tmp1) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Gate type ");
                    }
                    break;

                case ObjectTypeClass.ARTILLERY_TYPE:
                {
                    
                }
                    break;
                case ObjectTypeClass.MINE_TYPE:
                    {
                        objType = new ObjectType(objFitFile);
                        objType.objectClass = ObjectClass.MINE;
                        //objType = new ArtilleryType;
                        //objType->setObjTypeNum(objTypeNum);
                        //if (objType->init(objectFile, objectFile->getPacketSize()) != NO_ERR)
                        //    Fatal(objectTypeNum, " ObjectTypeManager.load: unable to init Artillery type ");
                    }
                    break;

                default:
                    return null;
            }


            if (noCacheOut)
            {
                objType.NoCacheOut();
                if (objType.ExplosionObject > 0)
                    Load((int)objType.ExplosionObject, false);
            }

            ObjectTypeList.Add(objTypeNum,objType);

            return (objType);
        }

        public ObjectType Get(int objTypeNum, bool loadIt = true)
        {
            if ((objTypeNum < 0))
                throw new Exception(" ObjectTypeManager.find: bad objTypeNum " + objTypeNum);


            if (!ObjectTypeList.ContainsKey(objTypeNum))
            {
                if (loadIt)
                {
                    if (Load(objTypeNum, true, true) == null)
                    {
                        throw new Exception(objTypeNum + " ObjectTypeManager.get: unable to load object type ");
                    }
                }
            }

            return ObjectTypeList[objTypeNum];

        }

        #endregion
    }
}
