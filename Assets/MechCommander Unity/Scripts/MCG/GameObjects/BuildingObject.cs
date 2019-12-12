using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.ObjectTypes;
using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG.GameObjectTypes
{
    public class BuildingObject : MCGameObject
    {
        #region Class Variables
        byte teamId;
        byte baseTileId;
        //SensorSystemPtr sensorSystem;
        byte commanderId;                                   //If capturable, who last captured it...
        //GameObjectWatchID refitBuddyWID;
        //DWORD parentId;
        //GameObjectWatchID parent;
        byte listID;
        float captureTime;
        float scoreTime;

        //PerimeterAlarms 		
        bool moverInProximity;
        float proximityTimer;
        long updatedTurn;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public void Init(ObjectType objType, AppearanceTypeManager appearanceTypeManager) 
        {
            base.Init(objType);
            //-------------------------------------------
            // Initialize the Building Appearance here.

            //setExists(true);
            //setFlag(OBJECT_FLAG_JUSTCREATED, true);

            captureTime = -1000.0f;

            var buildingAppearanceType = appearanceTypeManager.GetAppearanceType(objType.Appearance);

            appearance = new BuildingAppearance(buildingAppearanceType, this);

            objectClass = ObjectClass.BUILDING;
            //setFlag(OBJECT_FLAG_DAMAGED, true);

            BuildingType type = (BuildingType)objType;
            if (type.extentRadius > 0.0) { }
                //setTangible(true);

            tonnage = type.baseTonnage;
            explDamage = type.explDmg;
            explRadius = type.explRad;

            curCV = maxCV = (short)type.startBR;

            //setFlag(OBJECT_FLAG_CANREFIT, type->canRefit);
            //setFlag(OBJECT_FLAG_MECHBAY, type->mechBay);

            //setTeamId(type.teamId, true);

            //if (type.sensorRange > -1.0 && getTeam())
            //    setSensorData(getTeam(), type->sensorRange, true);

            //setFlag(OBJECT_FLAG_CAPTURABLE, false);
            if (type.capturable)
                //setFlag(OBJECT_FLAG_CAPTURABLE, true);

            //setRefitBuddy(0);

            if (type.DamageLevel == 0.0f)
            {
                //-------------------------------------------------------
                // We are already destroyed. Used for extraction markers.
                //setTangible(false);
                //setStatus(OBJECT_STATUS_DESTROYED);
                //setFlag(OBJECT_FLAG_DAMAGED, true);
            }
        }
        #endregion

        #region Public Functions
        public Appearance.AppearanceData AppearanceData
        {
            get
            {
                if (appearance.type == null)
                {
                    Debug.Log("Error appearance type null in buildingobject");
                   // return null;
                }

                return ((BuildingAppearance)appearance).SpriteData;
            }
        }
        #endregion

    }
}
