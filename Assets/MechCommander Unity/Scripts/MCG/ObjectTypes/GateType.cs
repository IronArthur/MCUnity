using UnityEngine;
using System.Collections;
using MechCommanderUnity.API;
using System;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class GateType : BuildingType
    {

        #region Class Variables
        public float damageLevel;

        public int blownEffectId;
        public int normalEffectId;
        public int damageEffectId;

        float baseTonnage;

        int basePixelOffsetX;
        int basePixelOffsetY;

        float explDmg;
        float explRad;

        float openRadius;

        float littleExtent;

        int gateTypeName;

        bool blocksLineOfFire;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public GateType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.GATE_TYPE;
            objectClass = ObjectClass.GATE;

            damageLevel = 0;

            blownEffectId = -1;
            normalEffectId = -1;
            damageEffectId = -1;

            explDmg = explRad = 0f;
            baseTonnage = 0f;

            gateTypeName = 0;


            if (!objFitFile.SeekSection("GateData"))
                return;

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            if (!objFitFile.GetInt("BlownEffectId", out blownEffectId))
                blownEffectId = -1;

            if (!objFitFile.GetInt("NormalEffectId", out normalEffectId))
                normalEffectId = -1;

            if (!objFitFile.GetInt("DamageEffectId", out damageEffectId))
                damageEffectId = -1;


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
        #endregion
    }
}