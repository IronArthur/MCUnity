using MechCommanderUnity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class BuildingType : ObjectType
    {
        #region Class Variables
        public float damageLevel;
        public float sensorRange;
        public float baseTonnage;
        public float explDmg;
        public float explRad;
        public int buildingTypeName;
        //EString buildingName; //magic 19032011
        public string buildingName; //magic 09092011
        public int buildingDescriptionID;
        public int startBR;
        public int numMarines;
        public int resourcePoints;
        public bool marksImpassableWhenDestroyed;

        public int basePixelOffsetX;
        public int basePixelOffsetY;
        public int collisionOffsetX;
        public int collisionOffsetY;

        public int lowTemplate;
        public int highTemplate;

        public bool canRefit;
        public bool mechBay;                           // otherwise it's a vehicle bay.
        public bool blocksLineOfFire;
        public bool capturable;
        public bool powerSource;
        public float perimeterAlarmRange;
        public float perimeterAlarmTimer;
        public float lookoutTowerRange;

        public int activityEffectId;

        #endregion



        #region Class Structures


        #endregion

        #region Constructors

        public BuildingType() : base()
        {
            objectTypeClass = (int)ObjectTypeClass.BUILDING_TYPE; //any reason to record TREEBUILDING_TYPE?
            objectClass = ObjectClass.BUILDING;
            damageLevel = 0f;
            sensorRange = -1f;
            teamId = -1;
            explDmg = explRad = 0f;
            baseTonnage = 0f;
            buildingTypeName = 0;
            buildingDescriptionID = -1;
            startBR = 0;
            numMarines = 0;
            canRefit = false;
            mechBay = false;
            blocksLineOfFire = false;

            lowTemplate = 0;
            highTemplate = 0;

        }

        public BuildingType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.BUILDING_TYPE; //any reason to record TREEBUILDING_TYPE?
            objectClass = ObjectClass.BUILDING;
            damageLevel = 0f;
            sensorRange = -1f;
            teamId = -1;
            explDmg = explRad = 0f;
            baseTonnage = 0f;
            buildingTypeName = 0;
            buildingDescriptionID = -1;
            startBR = 0;
            numMarines = 0;
            canRefit = false;
            mechBay = false;
            blocksLineOfFire = false;

            lowTemplate = 0;
            highTemplate = 0;

            if (!objFitFile.SeekSection("TreeData")
                || !objFitFile.SeekSection("BuildingData"))
            {
                return;
            }

            //objFitFile.SeekSection("BuildingData");

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            objFitFile.GetBool("CanRefit", out canRefit);
            if (canRefit)
                objFitFile.GetBool("MechBay", out mechBay);

            objFitFile.GetBool("BlocksLineOfFire", out blocksLineOfFire);

            objFitFile.GetInt("LowTemplate", out lowTemplate);
            objFitFile.GetInt("HighTemplate", out highTemplate);

            if (!objFitFile.GetFloat("ExplosionRadius", out explRad))
                explRad = 0f;// if this fails, explosion radius is not set and no splash damage.

            if (!objFitFile.GetFloat("ExplosionDamage", out explDmg))
                explDmg = 0f; // if this fails, explosion damage is not set and no splash damage.

            if (!objFitFile.GetFloat("Tonnage", out baseTonnage))
                baseTonnage = 20f;

            if (!objFitFile.GetInt("BattleRating", out startBR))
                startBR = 20;

            if (!objFitFile.GetInt("NumMarines", out numMarines))
                numMarines = 0;

            float realExtent = 0f;
            if (!objFitFile.GetFloat("ExtentRadius", out realExtent))
                realExtent = -1f;

            //----------------------------
            // Init sensor-related data...

            if (!objFitFile.GetInt("TeamID", out teamId))
                teamId = -1;

            if (!objFitFile.GetFloat("SensorRange", out sensorRange))
                sensorRange = -1f;

            int buildingNameInt = 30163;
            if (!objFitFile.GetInt("BuildingName", out buildingNameInt))
                buildingNameInt = 30163;
            buildingName = buildingNameInt.ToString();


            if (!objFitFile.GetInt("BattleRating", out resourcePoints))
                resourcePoints = -1;

            if (!objFitFile.GetInt("BasePixelOffsetX", out basePixelOffsetX))
                basePixelOffsetX = 0;
            if (!objFitFile.GetInt("BasePixelOffsetY", out basePixelOffsetY))
                basePixelOffsetY = 0;
            if (!objFitFile.GetInt("CollisionOffsetX", out collisionOffsetX))
                collisionOffsetX = 0;
            if (!objFitFile.GetInt("CollisionOffsetY", out collisionOffsetY))
                collisionOffsetY = 0;

            objFitFile.GetInt("HighTemplate", out highTemplate);
            objFitFile.GetInt("LowTemplate", out lowTemplate);

          //  impassability = (highTemplate << 32) | lowTemplate;


            //-------------------------------------------------------
            // Initialize the base object Type from the current file.

            extentRadius = realExtent;



        }

        #endregion

        #region Public Functions

        public float DamageLevel
        {
            get { return (damageLevel); }

        }

        public override int Appearance
        {
            get { return (int)appearName; }
        }

        public int HighAppearance
        {
            get { return Appearance & (int)highTemplate; }
        }

        public int LowAppearance
        {
            get { return Appearance & (int)lowTemplate; }
        }


        #endregion

    }
}
