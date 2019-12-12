using UnityEngine;
using UnityEditor;
using MechCommanderUnity.API;
using System;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class FireType : ObjectType
    {
        #region Class Variables
        public float damageLevel;
        public int soundEffectId;
        public int lightObjectId;

        public int startLoopFrame;
        public int endLoopFrame;
        public int numLoops;

        public float maxExtentRadius;        //How Good am I at setting off other fires
        public float timeToMaxExtent;        //How long before I grow to MaxExtent size

        public int totalFireShapes;

        //public float* fireOffsetX;
        //public float* fireOffsetY;
        //public float* fireDelay;

        //public long* fireRandomOffsetX;
        //public long* fireRandomOffsetY;
        //public long* fireRandomDelay;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public FireType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.FIRE_TYPE;
            objectClass = ObjectClass.FIRE;

            damageLevel = 0f;
            soundEffectId = -1;

            maxExtentRadius = 0f;
            timeToMaxExtent = 0f;

            totalFireShapes = 1;

            //fireOffsetX = NULL;
            //fireOffsetY = NULL;
            //fireDelay = NULL;

            //fireRandomOffsetX = NULL;
            //fireRandomOffsetY = NULL;
            //fireRandomDelay = NULL;

            if (!objFitFile.SeekSection("FireData"))
                return;

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            if (!objFitFile.GetInt("SoundEffectId", out soundEffectId))
                return;

            if (!objFitFile.GetInt("LightObjectId", out lightObjectId))
                lightObjectId = -1;

            if (!objFitFile.GetInt("startLoopFrame", out startLoopFrame))
                return;
            if (!objFitFile.GetInt("numLoops", out numLoops))
                return;
            if (!objFitFile.GetInt("endLoopFrame", out endLoopFrame))
                return;

            if (!objFitFile.GetFloat("maxExtentRadius", out maxExtentRadius))
                maxExtentRadius = 0f;
            if (!objFitFile.GetFloat("TimeToMaxExtent", out timeToMaxExtent))
                timeToMaxExtent = 20f;

            if (!objFitFile.GetInt("TotalFireShapes", out totalFireShapes))
                totalFireShapes = 1;

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